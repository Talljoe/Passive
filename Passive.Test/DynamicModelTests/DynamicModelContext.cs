// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    internal class DynamicModelContext
    {
        private List<object> args;
        private Func<dynamic> function;
        private DynamicModel model;

        public DynamicModel Model
        {
            get
            {
                if (model == null)
                {
                    throw new InvalidOperationException("No model has been set.");
                }
                return this.model;
            }
            set { this.model = value; }
        }

        public int? CurrentPage { get; set; }

        public int? PageSize { get; set; }

        public object Key { get; set; }

        public List<object> Args
        {
            get { return this.args = (this.args ?? new List<object>()); }
            set { this.args = value; }
        }

        public object Columns { get; set; }

        public int? Limit { get; set; }

        public string OrderBy { get; set; }

        public object Where { get; set; }

        public void SetMethod(string methodName)
        {
            switch (methodName.ToLowerInvariant())
            {
                case "all":
                    this.function = this.AllFunc;
                    break;

                case "single":
                    this.function = this.SingleFunc;
                    break;

                case "paged":
                    this.function = this.PagedFunc;
                    break;

                default:
                    throw new ArgumentException("Not a valid method name", methodName);
            }
        }

        public dynamic GetResult()
        {
            return this.function();
        }

        private dynamic AllFunc()
        {
            dynamic d = new ExpandoObject();
            d.Items = this.Model.All(this.Where, this.OrderBy, this.Limit ?? 0, this.Columns, this.GetArgs());
            return d;
        }

        private dynamic PagedFunc()
        {
            return this.Model.Paged(this.Where, this.OrderBy, this.Columns, this.PageSize ?? 20, this.CurrentPage ?? 1,
                                    this.GetArgs());
        }

        private dynamic SingleFunc()
        {
            dynamic d = new ExpandoObject();
            d.Items = GetSingle();
            return d;
        }

        private IEnumerable<dynamic> GetSingle()
        {
            var result = this.Model.Single(this.Key, this.Where, this.Columns, this.GetArgs());
            if (result != null)
            {
                yield return result;
            }
        }

        private object[] GetArgs()
        {
            return this.Args.Any() ? this.Args.ToArray() : null;
        }
    }
}