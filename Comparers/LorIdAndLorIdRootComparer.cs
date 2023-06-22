using System.Collections.Generic;
using UtilLoader21341.Models;

namespace UtilLoader21341.Comparers
{
    public class LorIdComparer : IEqualityComparer<LorId>
    {
        public bool Equals(LorId x, LorId y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.id == y.id && x.packageId == y.packageId;
        }

        public int GetHashCode(LorId obj)
        {
            unchecked
            {
                return (obj.id * 397) ^ (obj.packageId != null ? obj.packageId.GetHashCode() : 0);
            }
        }

        public bool Equals(LorId x, LorIdRoot y)
        {
            return x.id == y.Id && x.packageId == y.PackageId;
        }
    }

    public class LorIdRootComparer : IEqualityComparer<LorIdRoot>
    {
        public bool Equals(LorIdRoot x, LorIdRoot y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.PackageId == y.PackageId;
        }

        public int GetHashCode(LorIdRoot obj)
        {
            unchecked
            {
                return (obj.Id * 397) ^ (obj.PackageId != null ? obj.PackageId.GetHashCode() : 0);
            }
        }
    }
}