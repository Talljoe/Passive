// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests.Async
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Passive.Async;
    using TechTalk.SpecFlow;

    [Binding]
    internal class DynamicAsyncModelSteps
    {
        private DynamicModelContext Context { get; set; }

        public DynamicAsyncModelSteps(DynamicModelContext context)
        {
            this.Context = context;
        }

        [Given(@"I have a model for (.*)")]
        [StepScope(Tag = "async")]
        public void GivenIHaveAModelFor(string modelName)
        {
            Context.Model = new DynamicAsyncModel(new SqlDynamicAsyncDatabase(), tableName: modelName);
        }

        #region Standard

        [When(@"I ask for a single row")]
        [StepScope(Tag = "async")]
        public void WhenIAskForASingleRow()
        {
            SetMethod("Single");
        }

        [When(@"I ask for page (\d+)")]
        [StepScope(Tag = "async")]
        public void WhenIAskForPage(int page)
        {
            SetMethod("Paged");
            this.Context.CurrentPage = page;
        }
        #endregion

        #region Key

        [When(@"I ask for the record with the id of (\d+)")]
        [StepScope(Tag = "async")]
        public void WhenIAskForTheRecordWithTheIdOf(int id)
        {
            this.SetMethod("single");
            Context.Key = id;
        }

        #endregion

        #region Then

        [Then(@"the query should throw an exception")]
        [StepScope(Tag = "async")]
        public void ThenTheQueryShouldThrowAnException()
        {
            Result.Invoking(l => l.ToList())
                .ShouldThrow<Exception>("because we asked for a column that did not exist");
        }

        #endregion

        public void SetMethod(string methodName)
        {
            switch (methodName.ToLowerInvariant())
            {
                case "all":
                    Context.SetFunction(this.AllFunc);
                    break;

                case "single":
                    Context.SetFunction(this.SingleFunc);
                    break;

                case "paged":
                    Context.SetFunction(this.PagedFunc);
                    break;

                default:
                    throw new ArgumentException("Not a valid method name", methodName);
            }
        }

        private dynamic AllFunc()
        {
            dynamic d = new ExpandoObject();
            d.Items = AsyncModel.AllAsync(Context.Where, Context.OrderBy, Context.Limit ?? 0, Context.Columns, this.GetArgs()).ToEnumerable();
            return d;
        }

        private DynamicAsyncModel AsyncModel
        {
            get { return ((DynamicAsyncModel) this.Context.Model); }
        }

        private dynamic PagedFunc()
        {
            var value = AsyncModel.PagedAsync(Context.Where, Context.OrderBy, Context.Columns, Context.PageSize ?? 20, Context.CurrentPage ?? 1,
                                    this.GetArgs()).Result;
            value.Items = AsyncEnumerable.ToEnumerable(value.Items);
            return value;
        }

        private dynamic SingleFunc()
        {
            dynamic d = new ExpandoObject();
            d.Items = this.DoSingle().Where<object>(x => x != null);
            return d;
        }

        private IEnumerable<dynamic> DoSingle()
        {
            var task = this.AsyncModel.SingleAsync(this.Context.Key, this.Context.Where, this.Context.Columns, this.GetArgs());
            yield return task.Result;
        }

        private object[] GetArgs()
        {
            return Context.Args.Any() ? Context.Args.ToArray() : null;
        }

        private IEnumerable<dynamic> Result
        {
            get { return (IEnumerable<dynamic>)(this.Context.GetResult().Items); }
        }
    }
}