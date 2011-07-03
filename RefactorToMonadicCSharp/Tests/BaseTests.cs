using System;
using Xunit;
using Microsoft.FSharp.Core;

namespace RefactorToMonadicCSharp
{
    public abstract class BaseTests
    {
        protected abstract FSharpOption<IVersionSpec> ParseVersionSpec(string v);

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBrackets()
        {
            var v = ParseVersionSpec("2.1");
            Assert.True(v.HasValue());
            Assert.Equal("2.1", v.Value.MinVersion.ToString());
            Assert.Null(v.Value.MaxVersion);
            Assert.False(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBracketsExtraSpaces()
        {
            var v = ParseVersionSpec("  1  .  2  ");
            Assert.True(v.HasValue());
            Assert.Equal("1.2", v.Value.MinVersion.ToString());
            Assert.Null(v.Value.MaxVersion);
            Assert.False(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyInclusive()
        {
            var v = ParseVersionSpec("(,1.2]");
            Assert.True(v.HasValue());
            Assert.Null(v.Value.MinVersion);
            Assert.False(v.Value.IsMinInclusive);
            Assert.Equal("1.2", v.Value.MaxVersion.ToString());
            Assert.True(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyExclusive()
        {
            var v = ParseVersionSpec("(,1.2)");
            Assert.True(v.HasValue());
            Assert.Null(v.Value.MinVersion);
            Assert.False(v.Value.IsMinInclusive);
            Assert.Equal("1.2", v.Value.MaxVersion.ToString());
            Assert.False(v.Value.IsMaxInclusive);
        }


        [Fact]
        public void ParseVersionSpecExactVersion()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("1.2", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMinOnlyExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,)");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal(null, versionInfo.Value.MaxVersion);
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,2.3)");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveInclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2,2.3)");
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusiveExtraSpaces()
        {
            // Act
            var versionInfo = ParseVersionSpec("   [  1 .2   , 2  .3   ]  ");

            Assert.True(versionInfo.HasValue());

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecShort()
        {
            var v = ParseVersionSpec("2");
            Assert.False(v.HasValue());
        }

    }
}
