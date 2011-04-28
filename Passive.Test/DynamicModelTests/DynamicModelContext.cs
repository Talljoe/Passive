// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.DynamicModelTests
{
    using System;
    using System.Collections.Generic;

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

        public void SetFunction(Func<dynamic> func)
        {
            this.function = func;
        }

        public dynamic GetResult()
        {
            return this.function();
        }

    }
}