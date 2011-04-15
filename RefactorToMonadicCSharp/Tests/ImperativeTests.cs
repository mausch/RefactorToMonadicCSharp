using System;

namespace RefactorToMonadicCSharp
{
    public class ImperativeTests: BaseTests
    {
        protected override Option<IVersionSpec> ParseVersionSpec(string v)
        {
            return Imperative.ParseVersionSpec(v);
        }
    }
}
