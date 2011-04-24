// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using Passive.Test.Models;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    internal class CommonSteps
    {

        [Given(@"a database with the following appliances")]
        public void GivenADatabaseWithTheFollowingAppliances(Table table)
        {
            ScenarioContext.Current.Set(table.CreateSet<Appliance>(), "Database");
        }

    }
}