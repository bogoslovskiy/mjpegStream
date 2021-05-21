namespace mjpegStream.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using mjpegStream.Exceptions;
    using Xunit;

    public class HttpUtilitiesUnitTests
    {
        [Theory]
        [InlineData("Content-Length: 1", "Content-Type: application/json")]
        [InlineData("X-Forwarded-For: 127.0.0.1, 90.90.90.90", "Content-Type: application/json")]
        public void ReadHttpLine_ShouldReturnExpectedHttpLine(string expectedHttpLine, string noise)
        {
            string httpStreamString = $"{expectedHttpLine}\r\n{noise}\r\n\r\n";

            using MemoryStream memoryStream = new(Encoding.ASCII.GetBytes(httpStreamString));

            string actualHttpLine = HttpUtilities.ReadHttpLine(memoryStream);

            Assert.Equal(expectedHttpLine, actualHttpLine);
        }

        [Theory]
        [InlineData(new object[] {
            new[]
            {
                "Content-Length: 12345",
                "Content-Type: application/json",
                "X-Forwarded-For: 127.0.0.1, 90.90.90.90",
                "Som-Header: somevalue"
            }
        })]
        public void ReadHttpLine_ShouldReturnExpectedHttpLines(string[] expectedHttpLines)
        {
            StringBuilder httpHeadersStringBuilder = new();

            foreach (string expectedHttpLine in expectedHttpLines)
            {
                httpHeadersStringBuilder.AppendFormat("{0}\r\n", expectedHttpLine);
            }

            httpHeadersStringBuilder.Append("\r\n");

            using MemoryStream memoryStream = new(Encoding.ASCII.GetBytes(httpHeadersStringBuilder.ToString()));

            List<string> httpLines = new();
            
            string httpLine;
            
            do
            {
                httpLine = HttpUtilities.ReadHttpLine(memoryStream);

                if (!string.IsNullOrWhiteSpace(httpLine))
                {
                    httpLines.Add(httpLine);
                }
            }
            while (!string.IsNullOrWhiteSpace(httpLine));

            Assert.Equal(expectedHttpLines, httpLines);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ParseHttpHeader_ShouldThrowArgumentException_WhenHttpLineIsNullOrWhiteSpace(string httpLine)
        {
            Assert.Throws<ArgumentException>(() => { _ = HttpUtilities.ParseHttpHeader(httpLine); });
        }
        
        [Theory]
        [InlineData("Content-Length 1")]
        [InlineData("Content-Length; 1")]
        [InlineData("Content-Length=1")]
        [InlineData("Content-Length")]
        [InlineData("Content-Length:")]
        [InlineData(":1")]
        public void ParseHttpHeader_ShouldThrowInvalidHttpHeaderException_WhenHttpLineIsNotValidHttpHeader(string httpLine)
        {
            Assert.Throws<InvalidHttpHeaderException>(() => { _ = HttpUtilities.ParseHttpHeader(httpLine); });
        }

        [Theory]
        [InlineData("Content-Length", "1", ":")]
        [InlineData("Content-Length", "1", " :")]
        [InlineData("Content-Length", "1", ": ")]
        [InlineData("Content-Length", "1", " : ")]
        [InlineData("Content-Type", "multipart/x-mixed-replace", ":")]
        public void ParseHttpHeader_ShouldReturnExpectedHeaderNameAndValue(
            string expectedHeaderName,
            string expectedHeaderValues,
            string separator)
        {
            string httpLine = $"{expectedHeaderName}{separator}{expectedHeaderValues}";

            (string headerName, string headerValues) = HttpUtilities.ParseHttpHeader(httpLine);

            Assert.Equal(expectedHeaderName, headerName);
            Assert.Equal(expectedHeaderValues, headerValues);
        }
    }
}