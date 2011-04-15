using System;

namespace RefactorToMonadicCSharp
{
    public class FunctionalTests: BaseTests
    {
        protected override Option<IVersionSpec> ParseVersionSpec(string v)
        {
            return Functional.ParseVersionSpec(v);
        }
    }
}
