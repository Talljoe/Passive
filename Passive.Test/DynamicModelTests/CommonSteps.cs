// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using Passive.Test.Models;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    internal class DynamicModelSteps
    {
        [Given(@"I have a model for (.*)")]
        public void GivenIHaveAModelFor(string modelName)
        {
            ScenarioContext.Current.Set(new DynamicModel(tableName: modelName));
        }

        [Given(@"a database with the following appliances")]
        public void GivenADatabaseWithTheFollowingAppliances(Table table)
        {
            ScenarioContext.Current.Set(table.CreateSet<Appliance>());
        }

    }
}