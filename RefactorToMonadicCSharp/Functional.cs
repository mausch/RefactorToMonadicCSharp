using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorToMonadicCSharp
{
    public static class Functional
    {
        public static Option<Version> ParseVersion(string value)
        {
            Version v;
            var ok = Version.TryParse(value, out v);
            if (ok)
                return v.ToOption();
            return Option<Version>.None;
        }

        public static Option<IVersionSpec> ParseVersionSpec(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            value = value.Trim();
            var checkLength = L.F((string val) => val.Length < 3 ? Option<Unit>.None : Option.SomeUnit);
            var minInclusive = L.F((string val) => {
                var c = val.First();
                if (c == '[')
                    return true.ToOption();
                if (c == '(')
                    return false.ToOption();
                return Option<bool>.None;
            });
            var maxInclusive = L.F((string val) => {
                var c = val.Last();
                if (c == ']')
                    return true.ToOption();
                if (c == ')')
                    return false.ToOption();
                return Option<bool>.None;
            });
            var checkParts = L.F((string[] parts) => parts.Length > 2 ? Option<Unit>.None : Option.SomeUnit);
            var minVersion = L.F((string[] parts) => {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    return Option<Version>.Some(null);
                return ParseVersion(parts[0]);
            });
            var maxVersion = L.F((string[] parts) =>  {
                var p = parts.Length == 2 ? parts[1] : parts[0];
                if (string.IsNullOrWhiteSpace(p))
                    return Option<Version>.Some(null);
                return ParseVersion(p);
            });
            
            var singleVersion = ParseVersion(value);
            if (singleVersion.HasValue)
                return ((IVersionSpec)new VersionSpec { IsMinInclusive = true, MinVersion = singleVersion.Value }).ToOption();

            if (!checkLength(value).HasValue)
                return Option<IVersionSpec>.None;
            var isMin = minInclusive(value);
            if (!isMin.HasValue)
                return Option<IVersionSpec>.None;
            var isMax = maxInclusive(value);
            if (!isMax.HasValue)
                return Option<IVersionSpec>.None;
            var svalue = value.Substring(1, value.Length - 2);
            var sparts = svalue.Split(',');
            var min = minVersion(sparts);
            if (!min.HasValue)
                return Option<IVersionSpec>.None;
            var max = maxVersion(sparts);
            if (!max.HasValue)
                return Option<IVersionSpec>.None;
            return ((IVersionSpec)new VersionSpec { IsMinInclusive = isMin.Value, MinVersion = min.Value, IsMaxInclusive = isMax.Value, MaxVersion = max.Value }).ToOption();
            

            /*
            var singleVersion =
                from v in ParseVersion(value)
                select (IVersionSpec)new VersionSpec { IsMinInclusive = true, MinVersion = v };

            var versionRange = L.F(() => {
                return from x in checkLength(value)
                       from isMin in minInclusive(value)
                       from isMax in maxInclusive(value)
                       let val = value.Substring(1, value.Length - 2)
                       let parts = val.Split(',')
                       from y in checkParts(parts)
                       from min in minVersion(parts)
                       from max in maxVersion(parts)
                       select (IVersionSpec)new VersionSpec { IsMinInclusive = isMin, MinVersion = min, IsMaxInclusive = isMax, MaxVersion = max };
            });

            return singleVersion.OrElse(versionRange)();
            */
        }

    }
}
