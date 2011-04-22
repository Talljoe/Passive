// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
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
            ScenarioContext.Current.Set(Model.All());
        }

        #endregion
        #region Limit

        [When(@"I ask for (\d+) rows?")]
        public void WhenIAskForNRows(int rows)
        {
            ScenarioContext.Current.Set(Model.All(limit: rows));
        }

        [When(@"I ask for more rows than are in the database")]
        public void WhenIAskForMoreRowsThanAreInTheDatabase()
        {
            ScenarioContext.Current.Set(Model.All(limit: 10));
        }

        #endregion
        #region Where

        [When(@"I ask for appliances colored (.+)")]
        public void WhenIAskForAppliancesColored(string color)
        {
            ScenarioContext.Current.Set(Model.All(where: new {Color = color}));
        }

        [When(@"I ask for appliances with more than (\d+) amps")]
        public void WhenIAskForAppliancesWithMoreThanNAmps(int amps)
        {
            ScenarioContext.Current.Set(Model.All(where: "Amps > @0", args: amps));
        }

        #endregion

        #region OrderBy

        [When(@"I order rows by (.+)")]
        public void WhenIOrderRowsBy(string order)
        {
            ScenarioContext.Current.Set(Model.All(orderBy: order));
        }

        #endregion

        #region Columns

        [When(@"I ask for the columns ""(.*?)""")]
        public void WhenIAskForTheColumns(string columns)
        {
            ScenarioContext.Current.Set(Model.All(columns: columns));
        }

        [When(@"I ask for an invalid column")]
        public void WhenIAskForAnInvalidColumn()
        {
            ScenarioContext.Current.Set(Model.All(columns: "BadColumn"));
        }

        #endregion

        #region Then

        [Then(@"I should get all items")]
        public void ThenIShouldGetAllItems()
        {
            ApplianceResult.Should().BeEquivalentTo(ApplianceTableData);
        }

        [Then(@"they should be a subset of all data")]
        public void ThenTheyShouldBeASubsetOfAllData()
        {
            ApplianceResult.Should().BeSubsetOf(ApplianceTableData);
        }

        [Then(@"I should only have (\d+) results?")]
        public void ThenIShouldOnlyHaveNResults(int rows)
        {
            ApplianceResult.Count().Should().Be(rows, "because we asked for a subset of the data");
        }

        [Then(@"I should only get appliances with more than (\d+) amps")]
        public void ThenIShouldOnlyGetApplianceWithMoreThanNAmps(int amps)
        {
            var expected = ApplianceTableData.Where(app => app.Amps > amps);
            ApplianceResult.Should().BeEquivalentTo(expected);
        }

        [Then(@"I should only get (.*?)-colored appliances")]
        public void ThenIShouldOnlyGetXColoredAppliances(string color)
        {
            var expected = ApplianceTableData
                .Where(app => app.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
            ApplianceResult.Should().BeEquivalentTo(expected);
        }

        [Then(@"the records should be sorted by Amps")]
        public void ThenTheRecordsShouldBeSortedByAmps()
        {
            var expected = ApplianceTableData.OrderBy(d => d.Amps);
            ApplianceResult.Should().Equal(expected);
        }

        [Then(@"the records should be reverse-sorted by Id")]
        public void ThenTheRecordsShouldBeReverseSortedById()
        {
            var expected = ApplianceTableData.OrderByDescending(d => d.Id);
            ApplianceResult.Should().Equal(expected);
        }

        [Then(@"the query should throw an exception")]
        public void ThenTheQueryShouldThrowAnException()
        {
            ScenarioContext.Current.Get<IEnumerable<dynamic>>()
                .Invoking(l => l.ToList())
                .ShouldThrow<DbException>("because we asked for a column that did not exist");
        }

        [Then(@"the records should only have the columns ""(.*?)""")]
        public void ThenTheRecordsShouldOnlyHaveTheColumns(string columns)
        {
            var expectedColumns = columns.Split(new[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            var result = ScenarioContext.Current.Get<IEnumerable<dynamic>>().Select(d => ((object) d).ToDictionary());
            result.ForEach(d => d.Keys.Should().BeEquivalentTo(expectedColumns));
        }
        #endregion

        private static IEnumerable<Appliance> ApplianceResult
        {
            get
            {
                return ScenarioContext.Current
                    .Get<IEnumerable<dynamic>>()
                    .Select(CreateApplianceFromDynamic)
                    .ToList();
            }
        }

        private static DynamicModel Model
        {
            get { return ScenarioContext.Current.Get<DynamicModel>(); }
        }

        private static IEnumerable<Appliance> ApplianceTableData
        {
            get { return ScenarioContext.Current.Get<IEnumerable<Appliance>>(); }
        }

        private static Appliance CreateApplianceFromDynamic(dynamic d)
        {
            return new Appliance { Id = d.Id, Name = d.Name, Color = d.Color, Amps = d.Amps };
        }
    }
}