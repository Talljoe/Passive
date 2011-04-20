// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Passive.Test.Models;
    using TechTalk.SpecFlow;
    using FluentAssertions;

    [Binding]
    internal class AllSteps
    {
        #region Standard

        [When(@"I ask for all rows")]
        public void WhenIAskForAllRows()
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All());
        }

        #endregion
        #region Limit

        [When(@"I ask for (\d+) rows?")]
        public void WhenIAskForNRows(int rows)
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All(limit: rows));
        }

        [When(@"I ask for more rows than are in the database")]
        public void WhenIAskForMoreRowsThanAreInTheDatabase()
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All(limit: 10));
        }

        #endregion
        #region Where

        [When(@"I ask for appliances colored (.*)")]
        public void WhenIAskForAppliancesColored(string color)
        {
            var model = ScenarioContext.Current.Get<DynamicModel>();
            ScenarioContext.Current.Set(model.All(where: "[Color] = @0", args: color));
        }

        #endregion
        #region Then

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

        [Then(@"I should only get (.*?)-colored appliances")]
        public void ThenIShouldOnlyGetXColoredAppliances(string color)
        {
            var expected = ScenarioContext.Current.Get<IEnumerable<Appliance>>()
                .Where(app => app.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
            var result = ScenarioContext.Current
                .Get<IEnumerable<dynamic>>()
                .Select(CreateApplianceFromDynamic)
                .ToList();
            result.Should().BeEquivalentTo(expected);
        }
        #endregion

        private static Appliance CreateApplianceFromDynamic(dynamic d)
        {
            return new Appliance { Id = d.Id, Name = d.Name, Color = d.Color, Amps = d.Amps };
        }
    }
}