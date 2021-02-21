using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.TestActions.Associations;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.Adapters.Attributes;
using Tricentis.Automation.Engines.Adapters.Controllers;
using Tricentis.Automation.Engines.Representations.Attributes;
using Tricentis.Automation.Engines.Technicals;
using Tricentis.Automation.Engines.Technicals.Html;

namespace FancyComboBox.Html.Adapter.Controller
{
 
    // The Controller resolves the relation between single elements that belong to each other logically. In this case the Controller
    // is derived from ListAdapterController, which describes how to get from the a list (in this case: the combobox) to the 
    // combobox items.
    // The ListAdapterController takes an Adapter as generic parameter. It's the same as the SupportedAdapter.

    // The SupportedAdapter describes the Adapter type which this controller needs as input source. It represents the starting point.
    // Therefore it must implement an IListAdapter, which it does since IComboBoxAdapter derives from it!
    [SupportedAdapter(typeof(FancyComboBoxAdapter))]
    public class FancyComboBoxAdapterController : ListAdapterController<FancyComboBoxAdapter>
    {

        // A controller will get several input parameters in its constructor: 
        //  * The actual Adapter object that it should use as a starting point, 
        //  * an ISearchQuery: this carries all the parameters of the module, which are required by the framework to find the 
        //      matching HTML elements. The ISearchQuery is very rarely used for customized Controllers.
        //  * and a validator. The validator can be used to express if the Controller is applicable for the input parameters,
        //      however it is not needed in this case because this Controller can deal with all FancyComboBoxAdapters.
        public FancyComboBoxAdapterController(FancyComboBoxAdapter contextAdapter, ISearchQuery query, Validator validator)
            : base(contextAdapter, query, validator)
        {
        }


        // Several methods have to be overridden from the ListAdapterController. The following three methods describe how to
        // get from the given adapter to its 
        //  + direct children in the HTML structure  
        //  + to all its descendants in the HTML structure
        //  + to its parent
        //  + to its logical children, the list items

        // The implementation of most of these methods is pretty straight forward in this case. The Technical associations can be used.
        // A TechnicalAssociation tells the framework to search for a property on the HTML Technical (its name is passed in as a string).
        // That property is invoked and the result is the collection of the according nodes.
        protected override IEnumerable<IAssociation> ResolveAssociation(ChildrenBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("Children");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(
            DescendantsBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("All");
        }

        protected override IEnumerable<IAssociation> ResolveAssociation(ParentBusinessAssociation businessAssociation)
        {
            yield return new TechnicalAssociation("ParentNode");
        }

        // Note that this method returns an AlgorithmicAssociation, telling the framework, that the customization will take
        // care by itself, to return the logical children. The Association's name, can be chosen at will - here: ListItems
        protected override IEnumerable<IAssociation> ResolveAssociation(ListItemsBusinessAssociation businessAssociation)
        {
            yield return new AlgorithmicAssociation("ListItems");
        }

        // Here the Associations will be resolved and the resulting Technicals will be returned
        // The actual work is done by a custom method called GetListItems (see below)
        protected override IEnumerable<ITechnical> SearchTechnicals(
            IAlgorithmicAssociation algorithmicAssociation)
        {
            return algorithmicAssociation.AlgorithmName != "ListItems" ? base.SearchTechnicals(algorithmicAssociation) : GetItems();
        }

        // This method now puts it all together: 
        //  + It resolves the ContextAdapter's Technical to a ul-tag with the according classname
        //  + Then it searches for all li-tags below the ul-tag, and returns a list of the correct type
        private IEnumerable<ITechnical> GetItems()
        {
            IHtmlElementTechnical ulItem = ContextAdapter.Technical.Children.Get<IHtmlElementTechnical>().SingleOrDefault(q => q.ClassName.Equals("fancy-combobox-items"));
            if (ulItem == null) return new List<ITechnical> { };

            return ulItem.GetElementByTagName("li").Get<IHtmlElementTechnical>();
        }
    }
}