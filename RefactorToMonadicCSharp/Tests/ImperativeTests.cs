using System;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public class ImperativeTests: BaseTests
    {
        protected override FSharpOption<IVersionSpec> ParseVersionSpec(string v)
        {
            return Imperative.ParseVersionSpec(v);
        }
    }
}
