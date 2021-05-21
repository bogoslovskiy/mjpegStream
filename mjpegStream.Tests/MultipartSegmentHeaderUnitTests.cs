namespace mjpegStream.Tests
{
    using mjpegStream.Exceptions;
    using Xunit;

    public class MultipartSegmentHeaderUnitTests
    {
        [Theory]
        [InlineData("Content-Length 1")]
        [InlineData("Content-Length; 1")]
        [InlineData("Content-Length=1")]
        [InlineData("Content-Length")]
        [InlineData("Content-Length:")]
        [InlineData(":1")]
        public void PushHeaderHttpLine_ShouldThrowInvalidHttpHeaderException_WhenPushedInvalidHeader(string httpLine)
        {
            MultipartSegmentHeader target = new();

            Assert.Throws<InvalidHttpHeaderException>(() => target.PushHeaderHttpLine(httpLine));
        }
        
        [Theory]
        [InlineData("Content-Type: application/json")]
        [InlineData("Content-Length: abs")]
        [InlineData("Content-Length: 1,2")]
        [InlineData("Content-Length: 1.3")]
        public void PushHeaderHttpLine_ShouldThrowInvalidMultipartSegmentHeaderException_WhenPushedInvalidHeaderValue(string httpLine)
        {
            MultipartSegmentHeader target = new();

            Assert.Throws<InvalidMultipartSegmentHeaderException>(() => target.PushHeaderHttpLine(httpLine));
        }
        
        [Theory]
        [InlineData(new object[]{ new[] {"Content-Type: image/jpeg"}})]
        [InlineData(new object[]{ new[] {"Content-Type: image/jpeg", "Some-Header: 123"}})]
        [InlineData(new object[]{ new[] {"Content-Length: 12345"}})]
        [InlineData(new object[]{ new[] {"Content-Length: 12345", "Some-Header: 123"}})]
        public void Validate_ShouldReturnFalse_WhenPushedNoAllRequiredHeaders(string[] httpLines)
        {
            MultipartSegmentHeader target = new();

            foreach (string httpLine in httpLines)
            {
                target.PushHeaderHttpLine(httpLine);
            }
            
            Assert.False(target.Validate());
        }
        
        [Theory]
        [InlineData(new object[]{ new[] {"Content-Type: image/jpeg", "Content-Length: 12345"}})]
        [InlineData(new object[]{ new[] {"Some-Header: 123", "Content-Type: image/jpeg", "Content-Length: 12345"}})]
        [InlineData(new object[]{ new[] {"Content-Type: image/jpeg", "Some-Header: 123", "Content-Length: 12345"}})]
        [InlineData(new object[]{ new[] {"Content-Type: image/jpeg", "Content-Length: 12345", "Some-Header: 123"}})]
        public void Validate_ShouldReturnTrue_WhenPushedAllRequiredHeaders(string[] httpLines)
        {
            MultipartSegmentHeader target = new();

            foreach (string httpLine in httpLines)
            {
                target.PushHeaderHttpLine(httpLine);
            }
            
            Assert.True(target.Validate());
        }
    }
}