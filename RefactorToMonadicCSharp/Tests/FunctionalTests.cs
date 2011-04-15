using System;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public class FunctionalTests: BaseTests
    {
        protected override FSharpOption<IVersionSpec> ParseVersionSpec(string v)
        {
            return Functional.ParseVersionSpec(v);
        }
    }
}
