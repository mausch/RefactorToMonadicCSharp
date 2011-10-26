using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorToMonadicCSharp {
    public static class Functional2 {
        public static IEnumerable<Version> ParseVersion(string value) {
            Version v;
            var ok = Version.TryParse(value, out v);
            if (ok)
                return new[] {v};
            return new Version[0];
        }

        public static IEnumerable<IVersionSpec> ParseVersionSpec(string value) {
            if (value == null)
                throw new ArgumentNullException("value");
            value = value.Trim();
            var checkLength = L.F((string val) => val.Length < 3 ? new int[0] : new[] {0});
            var minInclusive = L.F((string val) => {
                var c = val.First();
                if (c == '[')
                    return new[] {true};
                if (c == '(')
                    return new[] {false};
                return new bool[0];
            });
            var maxInclusive = L.F((string val) => {
                var c = val.Last();
                if (c == ']')
                    return new[] {true};
                if (c == ')')
                    return new[] {false};
                return new bool[0];
            });
            var checkParts =
                L.F((string[] parts) => parts.Length > 2 ? new int[0] : new [] {0});
            var minVersion = L.F((string[] parts) => {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    return new[] {new Version[0]};
                return ParseVersion(parts[0]).Select(v => new[] {v});
            });
            var maxVersion = L.F((string[] parts) => {
                var p = parts.Length == 2 ? parts[1] : parts[0];
                if (string.IsNullOrWhiteSpace(p))
                    return new[] {new Version[0]};
                return ParseVersion(p).Select(v => new[] {v});
            });

            var singleVersion =
                from v in ParseVersion(value)
                select (IVersionSpec) new VersionSpec {IsMinInclusive = true, MinVersion = v};

            var versionRange = L.F(() => from x in checkLength(value)
                                         from isMin in minInclusive(value)
                                         from isMax in maxInclusive(value)
                                         let val = value.Substring(1, value.Length - 2)
                                         let parts = val.Split(',')
                                         from y in checkParts(parts)
                                         from min in minVersion(parts)
                                         from max in maxVersion(parts)
                                         select (IVersionSpec) new VersionSpec {
                                             IsMinInclusive = isMin,
                                             MinVersion = min.Any() ? min.First() : null,
                                             IsMaxInclusive = isMax,
                                             MaxVersion = max.Any() ? max.First() : null,
                                         });

            return singleVersion.Any() ? singleVersion : versionRange();
        }
    }
}