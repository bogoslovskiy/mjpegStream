using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("mjpegStream.Tests")]
namespace mjpegStream
{
    using System;
    using Microsoft.Net.Http.Headers;
    using mjpegStream.Exceptions;

    internal class MultipartSegmentHeader
    {
        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public bool Validate()
        {
            return this.ContentLength > 0 && !string.IsNullOrWhiteSpace(this.ContentType);
        }
            
        public void PushHeaderHttpLine(string httpLine)
        {
            (string headerName, string headerValues) = HttpUtilities.ParseHttpHeader(httpLine);

            ReadContentTypeIfMatched(headerName: headerName, headerValues: headerValues);
            ReadContentLengthIfMatched(headerName: headerName, headerValues: headerValues);
        }
            
        private void ReadContentTypeIfMatched(string headerName, string headerValues)
        {
            if (string.Equals(headerName, HeaderNames.ContentType, StringComparison.OrdinalIgnoreCase))
            {
                bool validContentType =
                    !string.IsNullOrWhiteSpace(headerValues) &&
                    string.Equals(headerValues, "image/jpeg", StringComparison.OrdinalIgnoreCase);
                
                if (!validContentType)
                {
                    throw new InvalidMultipartSegmentHeaderException();
                }

                this.ContentType = headerValues;
            }
        }

        private void ReadContentLengthIfMatched(string headerName, string headerValues)
        {
            if (string.Equals(headerName, HeaderNames.ContentLength, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(headerValues, out int contentLength))
                {
                    this.ContentLength = contentLength;
                }
                else
                {
                    throw new InvalidMultipartSegmentHeaderException();
                }
            }
        }
    }
}