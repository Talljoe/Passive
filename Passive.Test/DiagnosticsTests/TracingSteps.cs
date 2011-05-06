// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DiagnosticsTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Passive.Diagnostics;
    using Passive.Test.Utility;
    using TechTalk.SpecFlow;

    [Binding]
    public class TracingSteps
    {
        [Given(@"a database")]
        public void GivenADatabase()
        {
            ScenarioContext.Current.Set(new DynamicDatabase());
        }

        [When(@"I execute the query ""(.*)""")]
        public void WhenIExecuteTheQuery(string query)
        {
            Func<object> action = () => ScenarioContext.Current.Get<DynamicDatabase>().Query(query).ToList();
            ScenarioContext.Current.Set<Func<object>>(action);
        }

        [When(@"I execute ""(.*)""")]
        public void WhenIExecute(string query)
        {
            Func<object> action = () => ScenarioContext.Current.Get<DynamicDatabase>().Execute(query);
            ScenarioContext.Current.Set<Func<object>>(action);
        }

        [When(@"I execute")]
        public void WhenIExecute(Table data)
        {
            var commands = data.Rows.Select(row => new DynamicCommand {Sql = row[0]});
            Func<object> action = () => ScenarioContext.Current.Get<DynamicDatabase>().Execute(commands);
            ScenarioContext.Current.Set<Func<object>>(action);
        }

        [When(@"I execute the scalar ""(.*)""")]
        public void WhenIExecuteTheScalar(string query)
        {
            Func<object> action = () => ScenarioContext.Current.Get<DynamicDatabase>().Scalar(query);
            ScenarioContext.Current.Set<Func<object>>(action);
        }

        [When(@"I fetch the query ""(.*)""")]
        public void WhenIFetchTheQuery(string query)
        {
            Func<object> action = () => ScenarioContext.Current.Get<DynamicDatabase>().Fetch(query);
            ScenarioContext.Current.Set<Func<object>>(action);
        }

        [Then(@"I should get a BeginQuery event with the value ""(.*)""")]
        public void ThenIShouldGetABeginQueryEventWithTheValue(string query)
        {
            ThenIShouldGetABeginQueryEventWithTheValues(new[] {query});
        }

        [Then(@"I should get a BeginQuery event with the values")]
        public void ThenIShouldGetABeginQueryEventWithTheValues(Table data)
        {
            ThenIShouldGetABeginQueryEventWithTheValues(data.Rows.Select(row => row[0]));
        }

        private static void ThenIShouldGetABeginQueryEventWithTheValues(IEnumerable<string> expected)
        {
            var action = ScenarioContext.Current.Get<Func<object>>();
            var actual = new List<string>();
            EventHandler<QueryTraceEventArgs> handler = (sender, e) => actual.Add(e.Sql);
            using (
                EventHelper.SetEventTemporarily(() => QueryTrace.QueryBegin += handler,
                                         () => QueryTrace.QueryBegin -= handler))
            {
                action();
            }

            actual.Should().Equal(expected);
        }

        [Then(@"I should get an EndQuery event with the value ""(.*)""")]
        public void ThenIShouldGetAnEndQueryEventWithTheValue(string query)
        {
            ThenIShouldGetAnEndQueryEventWithTheValues(new[] {query});
        }

        [Then(@"I should get an EndQuery event with the values")]
        public void ThenIShouldGetAnEndQueryEventWithTheValues(Table data)
        {
            ThenIShouldGetAnEndQueryEventWithTheValues(data.Rows.Select(row => row[0]));
        }

        private static void ThenIShouldGetAnEndQueryEventWithTheValues(IEnumerable<string> expected)
        {
            var action = ScenarioContext.Current.Get<Func<object>>();
            var actual = new List<string>();
            EventHandler<QueryTraceEventArgs> handler = (sender, e) => actual.Add(e.Sql);
            using (
                EventHelper.SetEventTemporarily(() => QueryTrace.QueryBegin += handler,
                                         () => QueryTrace.QueryBegin -= handler))
            {
                action();
            }

            actual.Should().Equal(expected);
        }
    }
}