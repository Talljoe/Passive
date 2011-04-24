// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Passive.Test.Models;
    using Passive.Test.Utility;
    using TechTalk.SpecFlow;
    using FluentAssertions;

    [Binding]
    internal class DynamicModelSteps
    {
        private DynamicModelContext Context { get; set; }

        public DynamicModelSteps(DynamicModelContext context)
        {
            this.Context = context;
        }

        [Given(@"I have a model for (.*)")]
        public void GivenIHaveAModelFor(string modelName)
        {
            Context.Model = new DynamicModel(tableName: modelName);
        }

        #region Standard

        [When(@"I ask for all rows")]
        public void WhenIAskForAllRows()
        {
            this.Context.SetMethod("All");
        }

        #endregion

        #region Limit

        [When(@"I limit the query to (\d+) rows?")]
        public void WhenILimitTheQueryTo(int rows)
        {
            Context.Limit = rows;
        }

        [When(@"I limit the query to more rows than are in the database")]
        public void WhenILimitTheQueryToMoreRowsThanAreInTheDatabase()
        {
            this.WhenILimitTheQueryTo(20);
        }

        #endregion

        #region Where

        [When(@"I only want appliances colored (.+)")]
        public void WhenIOnlyWantAppliancesColored(string color)
        {
            Context.Where = new {Color = color};
        }

        [When(@"I only want appliances with more than (\d+) amps")]
        public void WhenIAskForAppliancesWithMoreThanNAmps(int amps)
        {
            Context.Where = "Amps > @0";
            Context.Args.Add(amps);
        }

        #endregion

        #region OrderBy

        [When(@"I order rows by (.+)")]
        public void WhenIOrderRowsBy(string order)
        {
            Context.OrderBy = order;
        }

        #endregion

        #region Columns

        [When(@"I ask for the columns ""(.*?)""")]
        public void WhenIAskForTheColumns(string columns)
        {
            Context.Columns = columns;
        }

        [When(@"I ask for an invalid column")]
        public void WhenIAskForAnInvalidColumn()
        {
            this.WhenIAskForTheColumns("BadColumn");
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
            expected.Should().HaveEquivalencyTo(expected);
        }

        [Then(@"I should only get (.*?)-colored appliances")]
        public void ThenIShouldOnlyGetXColoredAppliances(string color)
        {
            var expected = ApplianceTableData
                .Where(app => app.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
            ApplianceResult.Should().HaveEquivalencyTo(expected);
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
            Result.Invoking(l => l.ToList())
                .ShouldThrow<DbException>("because we asked for a column that did not exist");
        }

        [Then(@"the records should only have the columns ""(.*?)""")]
        public void ThenTheRecordsShouldOnlyHaveTheColumns(string columns)
        {
            var expectedColumns = columns.Split(new[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            var result = Result.Select(d => ((object) d).ToDictionary());
            result.ForEach(d => d.Keys.Should().BeEquivalentTo(expectedColumns));
        }
        
        #endregion

        private IEnumerable<Appliance> ApplianceResult
        {
            get
            {
                return this.Result
                    .Select(CreateApplianceFromDynamic)
                    .ToList();
            }
        }

        private IEnumerable<dynamic> Result
        {
            get { return (IEnumerable<dynamic>)this.Context.GetResult().Items; }
        }

        private static IEnumerable<Appliance> ApplianceTableData
        {
            get { return ScenarioContext.Current.Get<IEnumerable<Appliance>>("Database"); }
        }

        private static Appliance CreateApplianceFromDynamic(dynamic d)
        {
            return new Appliance { Id = d.Id, Name = d.Name, Color = d.Color, Amps = d.Amps };
        }
    }
}