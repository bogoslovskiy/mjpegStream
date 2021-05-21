namespace mjpegStream.Tests
{
    using Xunit;
    
    public class CrLfMatcherUnitTests
    {
        [Theory]
        [InlineData("\r\n", 1)]
        [InlineData("\r\r\n", 1)]
        [InlineData("\r\n\n", 1)]
        [InlineData("\r\r\r\n", 1)]
        [InlineData("\r\r\n\r", 1)]
        [InlineData("\r\rsometext\r\n", 1)]
        [InlineData("\n\rsometext\r\n", 1)]
        [InlineData("sometext\r\n", 1)]
        [InlineData("\r\nsometext", 1)]
        [InlineData("some\r\ntext", 1)]
        [InlineData("\r\n\r\n", 2)]
        [InlineData("sometext\r\n\r\n", 2)]
        [InlineData("\r\nsometext\r\n", 2)]
        [InlineData("\r\nsome\r\ntext", 2)]
        [InlineData("\r\n\r\nsometext", 2)]
        [InlineData("\r\n\r\nsometext\r\r", 2)]
        [InlineData("\r\n\r\nsometext\n\r", 2)]
        public void Push_ShouldMatch_WhenFoundCrLfSequence(string charsSequence, int matchCount)
        {
            CrLfMatcher crLfMatcher = new();

            foreach (char @char in charsSequence)
            {
                bool matched = crLfMatcher.Push(@char);

                if (matched)
                    matchCount--;
            }
            
            Assert.Equal(0, matchCount);
        }
    }
}