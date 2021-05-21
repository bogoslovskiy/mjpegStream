namespace mjpegStream
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using mjpegStream.Exceptions;

    public class MultipartSegmentReader
    {
        private readonly string multipartBoundary;
        private readonly Stream stream;

        private MultipartSegmentReader(string multipartBoundary, Stream stream)
        {
            this.multipartBoundary = multipartBoundary;
            this.stream = stream;
        }

        public static MultipartSegmentReader CreateFrom(HttpResponseMessage httpResponseMessage)
        {
            bool multipartXMixedReplaceContentType = string.Equals(
                httpResponseMessage.Content.Headers.ContentType?.MediaType,
                "multipart/x-mixed-replace",
                StringComparison.OrdinalIgnoreCase
            ); 
            
            if (!multipartXMixedReplaceContentType)
            {
                throw new UnsupportedMjpegStreamMultipartResponseException(httpResponseMessage.Content.Headers.ContentType?.MediaType);
            }
            
            string boundary = HttpUtilities.GetBoundary(
                contentType: Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(
                    httpResponseMessage.Content.Headers.ContentType?.ToString()
                )
            );

            Stream contentStream = httpResponseMessage.Content.ReadAsStream();

            return new MultipartSegmentReader(boundary, contentStream);
        }

        public void ReadFooter()
        {
            string footer = HttpUtilities.ReadHttpLine(stream);

            if (!string.IsNullOrEmpty(footer))
            {
                throw new InvalidHttpMultipartSegmentFooterException();
            }
        }

        public async Task<MultipartSegmentBodyReader> GetNextSegmentBodyReaderAsync()
        {
            MultipartSegmentHeader multipartHeader = await ReadHeaderAsync();
            
            return new MultipartSegmentBodyReader(stream, multipartHeader.ContentLength);
        }
        
        private async Task<MultipartSegmentHeader> ReadHeaderAsync()
        {
            string boundary = HttpUtilities.ReadHttpLine(stream);

            bool boundaryMatched =
                !string.IsNullOrWhiteSpace(boundary) &&
                boundary.EndsWith(multipartBoundary);
            
            if (!boundaryMatched)
            {
                throw new UnexpectedHttpMultipartSegmentBoundaryException(expectedBoundary: multipartBoundary, actualBoundary: boundary);
            }

            MultipartSegmentHeader header = new();

            string httpLine;

            do
            {
                httpLine = HttpUtilities.ReadHttpLine(stream);

                if (!string.IsNullOrEmpty(httpLine))
                {
                    header.PushHeaderHttpLine(httpLine);
                }
            }
            while (!string.IsNullOrEmpty(httpLine));

            if (!header.Validate())
            {
                // TODO: custom exception
                throw new Exception("Invalid MJPEG Http multipart header");
            }

            return header;
        }
    }
}