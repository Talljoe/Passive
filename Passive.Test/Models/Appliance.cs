// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.Models
{
    using System;
    using System.Globalization;

    internal class Appliance : IEquatable<Appliance>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Amps { get; set; }
        public bool InStock { get; set; }

        public bool Equals(Appliance other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.Id == this.Id
                && Equals(other.Name, this.Name)
                && Equals(other.Color, this.Color)
                && other.Amps == this.Amps
                && other.InStock == this.InStock;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (Appliance))
            {
                return false;
            }
            return this.Equals((Appliance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = this.Id;
                result = (result*397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                result = (result*397) ^ (this.Color != null ? this.Color.GetHashCode() : 0);
                result = (result*397) ^ this.Amps;
                result = (result*(this.InStock ? 1 : 397));
                return result;
            }
        }

        public static bool operator ==(Appliance left, Appliance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Appliance left, Appliance right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "[{0}: {1}]", Id, Name);
        }
    }
}