using System;
using System.Linq;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public static class Functional
    {
        public static FSharpOption<Version> ParseVersion(string value)
        {
            Version v;
            var ok = Version.TryParse(value, out v);
            if (ok)
                return v.ToOption();
            return FSharpOption<Version>.None;
        }

        public static FSharpOption<IVersionSpec> ParseVersionSpec(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            value = value.Trim();
            var checkLength = L.F((string val) => val.Length < 3 ? FSharpOption<Unit>.None : FSharpOption.SomeUnit);
            var minInclusive = L.F((string val) => {
                var c = val.First();
                if (c == '[')
                    return true.ToOption();
                if (c == '(')
                    return false.ToOption();
                return FSharpOption<bool>.None;
            });
            var maxInclusive = L.F((string val) => {
                var c = val.Last();
                if (c == ']')
                    return true.ToOption();
                if (c == ')')
                    return false.ToOption();
                return FSharpOption<bool>.None;
            });
            var checkParts = L.F((string[] parts) => parts.Length > 2 ? FSharpOption<Unit>.None : FSharpOption.SomeUnit);
            var minVersion = L.F((string[] parts) => {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    return FSharpOption<FSharpOption<Version>>.Some(FSharpOption<Version>.None);
                return ParseVersion(parts[0]).Select(v => v.ToOption());
            });
            var maxVersion = L.F((string[] parts) =>  {
                var p = parts.Length == 2 ? parts[1] : parts[0];
                if (string.IsNullOrWhiteSpace(p))
                    return FSharpOption<FSharpOption<Version>>.Some(FSharpOption<Version>.None);
                return ParseVersion(p).Select(v => v.ToOption());
            });
            /*
            var singleVersion = ParseVersion(value);
            if (singleVersion.HasValue())
                return ((IVersionSpec)new VersionSpec { IsMinInclusive = true, MinVersion = singleVersion.Value }).ToOption();

            if (!checkLength(value).HasValue())
                return FSharpOption<IVersionSpec>.None;
            var isMin = minInclusive(value);
            if (!isMin.HasValue())
                return FSharpOption<IVersionSpec>.None;
            var isMax = maxInclusive(value);
            if (!isMax.HasValue())
                return FSharpOption<IVersionSpec>.None;
            var svalue = value.Substring(1, value.Length - 2);
            var sparts = svalue.Split(',');
            var min = minVersion(sparts);
            if (!min.HasValue())
                return FSharpOption<IVersionSpec>.None;
            var max = maxVersion(sparts);
            if (!max.HasValue())
                return FSharpOption<IVersionSpec>.None;
            return ((IVersionSpec)new VersionSpec { IsMinInclusive = isMin.Value, MinVersion = min.Value, IsMaxInclusive = isMax.Value, MaxVersion = max.Value }).ToOption();
            */


            var singleVersion =
                from v in ParseVersion(value)
                select (IVersionSpec)new VersionSpec { IsMinInclusive = true, MinVersion = v };

            var versionRange = L.F(() => from x in checkLength(value)
                                         from isMin in minInclusive(value)
                                         from isMax in maxInclusive(value)
                                         let val = value.Substring(1, value.Length - 2)
                                         let parts = val.Split(',')
                                         from y in checkParts(parts)
                                         from min in minVersion(parts)
                                         from max in maxVersion(parts)
                                         select (IVersionSpec)new VersionSpec {
                                             IsMinInclusive = isMin, 
                                             MinVersion = min.HasValue() ? min.Value : null, 
                                             IsMaxInclusive = isMax, 
                                             MaxVersion = max.HasValue() ? max.Value : null,
                                         });

            return singleVersion.OrElse(versionRange)();

        }

    }
}
