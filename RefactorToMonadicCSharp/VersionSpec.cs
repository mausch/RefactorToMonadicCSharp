using System;

namespace RefactorToMonadicCSharp
{
    public interface IVersionSpec
    {
        Version MinVersion { get; }
        bool IsMinInclusive { get; }
        Version MaxVersion { get; }
        bool IsMaxInclusive { get; }
    }

    public class VersionSpec : IVersionSpec
    {
        public Version MinVersion { get; set; }
        public bool IsMinInclusive { get; set; }
        public Version MaxVersion { get; set; }
        public bool IsMaxInclusive { get; set; }
    }
}
