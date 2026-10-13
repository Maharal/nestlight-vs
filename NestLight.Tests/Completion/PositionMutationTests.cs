using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using Xunit;

namespace NestLight.Tests
{
    public class PositionMutationTests
    {
        // ---- Line 38: Secondary null coalescing ----

        [Fact]
        public void Position_with_null_secondary_has_empty_list()
        {
            // Line 38: secondary == null -> new string[0], but null coalescing mutation would use secondary.ToList()
            var pos = new Position("test", null, secondary: null);
            Assert.NotNull(pos.Secondary);
            Assert.Empty(pos.Secondary);
        }

        [Fact]
        public void Position_with_non_null_secondary_keeps_it()
        {
            var secondary = new List<string> { "a", "b" };
            var pos = new Position("test", null, secondary: secondary);
            Assert.Equal(2, pos.Secondary.Count);
            Assert.Contains("a", pos.Secondary);
        }

        [Fact]
        public void Position_with_expected_list_keeps_it()
        {
            var expected = new List<string> { "select", "from" };
            var pos = new Position("test", expected);
            Assert.Equal(2, pos.Expected.Count);
        }

        [Fact]
        public void Position_with_null_expected_has_empty_list()
        {
            var pos = new Position("test", null);
            Assert.NotNull(pos.Expected);
            Assert.Empty(pos.Expected);
        }

        [Fact]
        public void Position_preserves_all_properties()
        {
            var pos = new Position("test", new[] { "a" }, wordsFirst: true, onlyWords: true, priorOrder: true,
                role: PlaceRole.Table, qualifier: "q");
            Assert.Equal("test", pos.Name);
            Assert.True(pos.WordsFirst);
            Assert.True(pos.OnlyWords);
            Assert.True(pos.PriorOrder);
            Assert.Equal(PlaceRole.Table, pos.Role);
            Assert.Equal("q", pos.Qualifier);
        }
    }
}
