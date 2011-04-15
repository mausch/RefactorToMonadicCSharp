using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public static class Imperative
    {
        /// <summary>
        /// Copyright 2010 Outercurve Foundation 
        /// Licensed under the Apache License, Version 2.0 (the "License"); 
        /// you may not use this file except in compliance with the License. 
        /// You may obtain a copy of the License at 
        ///  http://www.apache.org/licenses/LICENSE-2.0 
        /// Unless required by applicable law or agreed to in writing, software 
        /// distributed under the License is distributed on an "AS IS" BASIS, 
        /// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
        /// See the License for the specific language governing permissions and 
        /// limitations under the License.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseVersionSpec(string value, out IVersionSpec result)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var versionSpec = new VersionSpec();
            value = value.Trim();

            // First, try to parse it as a plain version string 
            Version version;
            if (Version.TryParse(value, out version))
            {
                // A plain version is treated as an inclusive minimum range 
                result = new VersionSpec
                {
                    MinVersion = version,
                    IsMinInclusive = true
                };

                return true;
            }

            // It's not a plain version, so it must be using the bracket arithmetic range syntax

            result = null;

            // Fail early if the string is too short to be valid 
            if (value.Length < 3)
            {
                return false;
            }

            // The first character must be [ ot ( 
            switch (value.First())
            {
                case '[':
                    versionSpec.IsMinInclusive = true;
                    break;
                case '(':
                    versionSpec.IsMinInclusive = false;
                    break;
                default:
                    return false;
            }

            // The last character must be ] ot ) 
            switch (value.Last())
            {
                case ']':
                    versionSpec.IsMaxInclusive = true;
                    break;
                case ')':
                    versionSpec.IsMaxInclusive = false;
                    break;
                default:
                    return false;
            }

            // Get rid of the two brackets 
            value = value.Substring(1, value.Length - 2);

            // Split by comma, and make sure we don't get more than two pieces 
            string[] parts = value.Split(',');
            if (parts.Length > 2)
            {
                return false;
            }

            // If there is only one piece, we use it for both min and max 
            string minVersionString = parts[0];
            string maxVersionString = (parts.Length == 2) ? parts[1] : parts[0];

            // Only parse the min version if it's non-empty 
            if (!String.IsNullOrWhiteSpace(minVersionString))
            {
                if (!Version.TryParse(minVersionString, out version))
                {
                    return false;
                }
                versionSpec.MinVersion = version;
            }

            // Same deal for max 
            if (!String.IsNullOrWhiteSpace(maxVersionString))
            {
                if (!Version.TryParse(maxVersionString, out version))
                {
                    return false;
                }
                versionSpec.MaxVersion = version;
            }

            // Successful parse! 
            result = versionSpec;
            return true;
        }

        public static FSharpOption<IVersionSpec> ParseVersionSpec(string value)
        {
            IVersionSpec result;
            var ok = TryParseVersionSpec(value, out result);
            if (ok)
                return result.ToOption();
            return FSharpOption<IVersionSpec>.None;
        }

    }
}
