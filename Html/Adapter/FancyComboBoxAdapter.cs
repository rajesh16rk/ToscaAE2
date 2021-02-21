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

    // This class represents (or interacts with) the combobox itself. The class implements the IComboBoxAdapter, allowing the framework to 
    // identify this Adapter as the combobox.

    // Supported Technical describes the type of HTML element that this combobox is represented by in the actual HTML structure. In this case 
    // the combobox is represented by a div-tag. The supported technical is therefore IHtmlDivTechnical.
    [SupportedTechnical(typeof(IHtmlDivTechnical))]
    public class FancyComboBoxAdapter : AbstractHtmlDomNodeAdapter<IHtmlDivTechnical>, IComboBoxAdapter
    {
        // The class inherits common functionality from the AbstractHtmlDomNodeAdapter, so that we do not have to implement all the common 
        // methods and properties that work the same for all HtmlDomNodes. The AbstactHtmlDomNodeAdapter needs to be parametrized with
        // the technical it interacts with - IHtmlDivTechnical in this case (the same as the SupportedTechnical!).

        // This class also implements the IComboBoxAdapter, telling the framework that this Adapter works as combobox. This allows the 
        // framework to determine that this Adapter works for the according Representation (the combobox)

        // A constructor is necessary since there is no default constructor. The technical that this adapter is responsible for will be
        // passed in by the framework. Additionally there is a validator. This validator should be used to determine if the adapter fits
        // this technical, e.g. if this adapter is for a combobox that is represented by a div-tag with a specific classname and the 
        // framework passes in a ul-element then the validator should fail and thereby inform the framework that this combination will 
        // not work.
        // In other words, the validator can be used to tell if the technical is a fancy combobox or not.
        public FancyComboBoxAdapter(IHtmlDivTechnical technical,
                                         Validator validator)
            : base(technical, validator)
        {
            validator.AssertTrue(() => !string.IsNullOrWhiteSpace(technical.ClassName) && technical.ClassName.Equals("fancy-combobox"));
        }

        // DefaultName is the property, that shows up in the scan result as the label / logical name of this item - in this case the id 
        // is one HTML attribute that works as a good label.
        public override string DefaultName
        {
            get { return Technical.Id; }
        }

        // This is a custom method for this adapter. It makes it possible to open up the drop down part of the combobox, if needed. This 
        // method is public, so that it can be used from the outside.
        // This method is not actually invoked or needed and is only implemented to demonstrate how this would work in other cases.
        public void Open()
        {
            // Each Technical supports access to all kinds of methods and attributes of the actual underlying object. Very useful are the
            // children and all properties that return underlying nodes - but in order to improve performance, these attributes return 
            // IObjectListReferences. These are not actual objects but just references to final objects. In order to resolve such a list
            // the get method can be called, whith a generic parameter of the type of Technical one would like to retrieve. 
            // The generic parameter works as a filter.
            // From there all kinds of operations can be done, e.g. you can invoke the click method on a subtechnical, like it is done here:
            // The method retrieves the drop down button and clicks it.
            var htmlDivTechnical = Technical.Children.Get<IHtmlDivTechnical>().SingleOrDefault(q => q.ClassName.Equals("fancy-combobox-drop-btn"));
            if (htmlDivTechnical != null)
                htmlDivTechnical.Click();
        }
    }
}
