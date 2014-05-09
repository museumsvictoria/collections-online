using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Extensions
{
    public class OrdinalIgnoreCaseTupleComparer : IEqualityComparer<Tuple<string, string, string, string>>
    {
        public bool Equals(Tuple<string, string, string, string> x, Tuple<string, string, string, string> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return StringComparer.OrdinalIgnoreCase.Equals(x.Item1, y.Item1) &&
                   StringComparer.OrdinalIgnoreCase.Equals(x.Item2, y.Item2) &&
                   StringComparer.OrdinalIgnoreCase.Equals(x.Item3, y.Item3) &&
                   StringComparer.OrdinalIgnoreCase.Equals(x.Item4, y.Item4);
        }

        public int GetHashCode(Tuple<string, string, string, string> obj)
        {
            return ((obj.Item1 == null) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1)) ^
                   ((obj.Item2 == null) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item2)) ^
                   ((obj.Item3 == null) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item3)) ^
                   ((obj.Item4 == null) ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item4));
        }
    }
}
