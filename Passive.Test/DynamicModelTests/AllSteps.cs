// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Passive.Test.Models;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;
    using FluentAssertions;

    [Binding]
    internal class AllSteps
    {
        [When(@"I ask for all rows")]
        public void WhenIAskForAllRows()
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All());
        }

        [When(@"I ask for (\d+) rows?")]
        public void WhenIAskForNRows(int rows)
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All(limit: rows));
        }

        [Then(@"I should get all items")]
        public void ThenIShouldGetAllItems()
        {
            var expected = ScenarioContext.Current.Get<IEnumerable<Appliance>>();
            var result = ScenarioContext.Current
                .Get<IEnumerable<dynamic>>()
                .Select(CreateApplianceFromDynamic)
                .ToList();
            result.Should().BeEquivalentTo(expected);
        }

        [Then(@"they should be a subset of all data")]
        public void ThenTheyShouldBeASubsetOfAllData()
        {
            var expected = ScenarioContext.Current.Get<IEnumerable<Appliance>>();
            var result = ScenarioContext.Current
                .Get<IEnumerable<dynamic>>()
                .Select(CreateApplianceFromDynamic)
                .ToList();
            result.Should().BeSubsetOf(expected);
        }

        [Then(@"I should only have (\d+) results?")]
        public void ThenIShouldOnlyHaveNResults(int rows)
        {
            var result = ScenarioContext.Current.Get<IEnumerable<dynamic>>().Count();
            result.Should().Be(rows, "because we asked for a subset of the data");
        }

        private static Appliance CreateApplianceFromDynamic(dynamic d)
        {
            return new Appliance { Id = d.Id, Name = d.Name, Color = d.Color, Amps = d.Amps };
        }
    }
}