using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Html.Generic;
using Tricentis.Automation.Engines.Adapters.Lists;
using Tricentis.Automation.Engines.Technicals.Html;

namespace FancyComboBox.Html.Adapter
{

    // This class represents (or interacts with) one of the elements of the drop down part of the combobox. All that can be done with the 
    // item, will be implemented here. The methods and properties are defined in the IListItemAdapter, allowing the framework to interact 
    // with this combobox item in the same way as any other combobox item that implements the interface.

    // Supported Technical describes the type of HTML element that this combobox item is represented by in the actual HTML structure. In 
    // this case the IHtmlElementTechnical is used, which is generic for all HTML elements.
    [SupportedTechnical(typeof(IHtmlElementTechnical))]
    public class FancyComboBoxItemAdapter : AbstractHtmlDomNodeAdapter<IHtmlElementTechnical>, IListItemAdapter
    {
        // The class inherits common functionality from the AbstractHtmlDomNodeAdapter, so that we do not have to implement all the common 
        // methods and properties that work the same for all HtmlDomNodes. The AbstactHtmlDomNodeAdapter needs to be parametrized with
        // the technical it interacts with - IHtmlElementTechnical in this case (the same as the SupportedTechnical!).

        // This class also implements the IListItemAdapter, telling the framework that this Adapter works as combobox item. This allows the 
        // framework to determine that this Adapter works for the according Representation (the combobox)

        // Each combobox carries an input element, which will be stored in this field:
        private IHtmlInputElementTechnical _htmlInputElementTechnical;

        // A constructor is necessary since there is no default constructor. The technical that this adapter is responsible for will be
        // passed in by the framework. Additionally there is a validator. This validator should be used to determine if the adapter fits
        // this technical, e.g. if this adapter is for a combobox item that that resides in a container with a certain classname and the 
        // framework passes in an element with a parent that does not contain the classname then the validator should fail and thereby 
        // inform the framework that this combination will not work.
        // In other words, the validator can be used to tell if the technical is a fancy combobox item or not.
        public FancyComboBoxItemAdapter(IHtmlElementTechnical technical, Validator validator) : base(technical, validator)
        {
            validator.AssertTrue(() => CheckTechnical(technical));
        }

        // This method checks if the passed node is eligible for this Adapter and stores the input element that is a sibling of the actual 
        // item.
        private bool CheckTechnical(IHtmlElementTechnical technical)
        {
            var parentNode = technical.ParentNode.Get<IHtmlElementTechnical>();
            if (parentNode == null) return false;
            var result = !string.IsNullOrWhiteSpace(parentNode.ClassName) && parentNode.ClassName.Contains("fancy-combobox-items");

            if (result)
            {
                _htmlInputElementTechnical =
                    parentNode.ParentNode.Get<IHtmlDivTechnical>()
                        .Children.Get<IHtmlInputElementTechnical>()
                        .SingleOrDefault();
            }
            return result;
        }

        // Is steerable is a standard property of a combobox item. Since items in this combobox will always be steerable we simply return 
        // true.
        public override bool IsSteerable
        {
            get { return true; }
        }

        // This property tells if the current item is selected or selects it.
        public bool Selected
        {
            // the item is selected exactly when it's text is shown in the combobox itself - this is why we needed
            // to store the _htmlInputElementTechnical before (see above)
            get { return _htmlInputElementTechnical.Value == Text; }
            set
            {
                // here we set the item selected - if the framework requests to set the selection state to false, 
                // there is not much we can do, so we don't
                if (!value) return;
                // if the node should be selected, we can just change the combobox value to the text of this item,
                // and fire a change event so that the value is propagated to the combobox.
                _htmlInputElementTechnical.Value = Text;
                _htmlInputElementTechnical.FireEvent("change");
            }
        }

        // DefaultName is the property, that shows up in the scan result as the label / logical name of this item - in this case the id 
        // is one HTML attribute that works as a good label.
        public override string DefaultName
        {
            get { return Text; }
        }

        // This custom property is a just a shortcut to get the inner text of the underlying Technical.
        public string Text
        {
            get { return Technical.InnerText; }
        }
    }
}