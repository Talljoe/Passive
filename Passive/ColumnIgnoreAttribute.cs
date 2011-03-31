// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnIgnoreAttribute : Attribute {}
}