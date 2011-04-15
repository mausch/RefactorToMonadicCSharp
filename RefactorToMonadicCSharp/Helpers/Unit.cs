using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefactorToMonadicCSharp
{
    public class Unit: IComparable
    {
        public override bool Equals(object obj)
        {
            return obj == null || obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public int CompareTo(object obj)
        {
            return 0;
        }
    }
}
