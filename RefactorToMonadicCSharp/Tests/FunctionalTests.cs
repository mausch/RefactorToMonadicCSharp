using System;
using System.Linq;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public class FunctionalTests: BaseTests
    {
        protected override FSharpOption<IVersionSpec> ParseVersionSpec(string v) {
            var r = Functional2.ParseVersionSpec(v);
            return r.Any() ? r.First().ToOption() : FSharpOption<IVersionSpec>.None;
            //return Functional.ParseVersionSpec(v);
        }
    }
}
