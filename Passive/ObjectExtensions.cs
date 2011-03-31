// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.Common;
    using System.Dynamic;
    using System.Linq;

    public static class ObjectExtensions
    {
        /// <summary>
        ///   Extension method for performing an action against all elements.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        ///   Extension method for adding in a bunch of parameters
        /// </summary>
        public static void AddParams(this DbCommand cmd, IEnumerable<object> args)
        {
            (args ?? Enumerable.Empty<object>()).ForEach(item => AddParam(cmd, item));
        }

        /// <summary>
        ///   Extension for adding single parameter
        /// </summary>
        public static void AddParam(this DbCommand cmd, object item)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                if (item.GetType() == typeof (Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 4000;
                }
                else if (item.GetType() == typeof (ExpandoObject))
                {
                    p.Value = ((IDictionary<string, object>) item).Values.FirstOrDefault();
                }
                else
                {
                    p.Value = item;
                }
                if (item.GetType() == typeof (string))
                {
                    p.Size = 4000; //from DataChomp
                }
            }
            cmd.Parameters.Add(p);
        }

        /// <summary>
        ///   Turns the object into an ExpandoObject
        /// </summary>
        public static dynamic ToExpando(this object o)
        {
            if (o is ExpandoObject)
            {
                return o; //shouldn't have to... but just in case
            }
            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary
            var nv = o as NameValueCollection;
            if (nv != null)
            {
                nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ForEach(d.Add);
            }
            else
            {
                o.GetType().GetProperties().ForEach(item => d.Add(item.Name, item.GetValue(o, null)));
            }
            return result;
        }

        /// <summary>
        ///   Turns the object into a Dictionary
        /// </summary>
        public static IDictionary<string, object> ToDictionary(this object thingy)
        {
            return (thingy as IDictionary<string, object>) ?? thingy.ToExpando();
        }
    }
}