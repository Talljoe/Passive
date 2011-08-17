// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
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

        [When(@"I ask for a single row")]
        public void WhenIAskForASingleRow()
        {
            this.Context.SetMethod("Single");
        }

        [When(@"I ask for page (\d+)")]
        public void WhenIAskForPage(int page)
        {
            this.Context.SetMethod("Paged");
            this.Context.CurrentPage = page;
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

        #region Key

        [When(@"I ask for the record with the id of (\d+)")]
        public void WhenIAskForTheRecordWithTheIdOf(int id)
        {
            Context.SetMethod("single");
            Context.Key = id;
        }

        #endregion

        #region Paging

        [When(@"the page size is (\d+)")]
        public void WhenThePageSizeIs(int pageSize)
        {
            this.Context.PageSize = pageSize;
        }

        #endregion

        #region Then

        [Then(@"I should get all items")]
        public void ThenIShouldGetAllItems()
        {
            ApplianceResult.Should().BeEquivalentTo(ApplianceTableData);
        }

        [Then(@"(?:they|it) should be a subset of (?:all|the) data")]
        public void ThenTheyShouldBeASubsetOfAllData()
        {
            ApplianceResult.Should().BeEmptyOrSubsetOf(ApplianceTableData);
        }

        [Then(@"I should get no results")]
        public void ThenIShouldGetNoResults()
        {
            ApplianceResult.Should().BeEmpty();
        }

        [Then(@"I should only have (\d+) results?")]
        [Then(@"I should get (\d+) results?")]
        public void ThenIShouldOnlyHaveNResults(int rows)
        {
            ApplianceResult.Should().HaveCount(rows, "because we asked for a subset of the data");
        }

        [Then(@"I should only get appliances with more than (\d+) amps")]
        public void ThenIShouldOnlyGetApplianceWithMoreThanNAmps(int amps)
        {
            var expected = ApplianceTableData.Where(app => app.Amps > amps);
            expected.Should().BeEmptyOrSubsetOf(expected);
        }

        [Then(@"I should only get (.*?)-colored appliances")]
        public void ThenIShouldOnlyGetXColoredAppliances(string color)
        {
            var expected = ApplianceTableData
                .Where(app => app.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
            ApplianceResult.Should().BeEmptyOrSubsetOf(expected);
        }

        [Then(@"the records should be sorted by (.*)")]
        public void ThenTheRecordsShouldBeSortedBy(string column)
        {
            var expected = ApplianceTableData.OrderBy(GetPropertyLambda(column));
            ApplianceResult.Should().Equal(expected);
        }

        [Then(@"I should get appliance \#(\d+)")]
        public void ThenIShouldGetAppliance(int id)
        {
            ApplianceResult.Select(app => app.Id).Single().Should().Be(id);
        }

        [Then(@"the records should be reverse-sorted by (.*)")]
        public void ThenTheRecordsShouldBeReverseSortedBy(string column)
        {
            var expected = ApplianceTableData.OrderByDescending(GetPropertyLambda(column));
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

        [Then(@"they should have the ids ((?:\d+\s*,\s*)*\d+)?")]
        public void ThenTheyShouldHaveTheIds(string ids)
        {
            var expected = ids.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse);
            ApplianceResult.Select(a => a.Id).Should().Equal(expected);
        }

        [Then(@"(\w+) should be an integer with value (\d+)")]
        public void ThenPropertyShouldBy(string property, int value)
        {
            var dict = ((object)Context.GetResult()).ToDictionary();
            var actual = (int)dict[property];
            actual.Should().Be(value);
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

        private static Func<Appliance, object> GetPropertyLambda(string column)
        {
            var parameter = Expression.Parameter(typeof(Appliance), "appliance");
            var property = Expression.Convert(Expression.Property(parameter, column), typeof(object));
            return Expression.Lambda<Func<Appliance, object>>(property, parameter).Compile();
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
            return new Appliance { Id = d.Id, Name = d.Name, Color = d.Color, Amps = d.Amps, InStock = d.InStock };
        }
    }
}