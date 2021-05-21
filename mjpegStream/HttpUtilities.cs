namespace mjpegStream
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Net.Http.Headers;
    using mjpegStream.Exceptions;

    public static class HttpUtilities
    {
        public static string ReadHttpLine(Stream stream)
        {
            StringBuilder stringBuilder = new();

            CrLfMatcher crLfMatcher = new();

            while (true)
            {
                int read = stream.ReadByte();

                if (read == -1)
                {
                    throw new Exception();
                }

                char @char = (char) read;

                bool endOfLine = crLfMatcher.Push(@char);

                if (endOfLine)
                {
                    break;
                }

                if (@char != '\r' && @char != '\n')
                {
                    stringBuilder.Append(@char);
                }
            }

            return stringBuilder.ToString();
        }
        
        public static (string HeaderName, string HeaderValues) ParseHttpHeader(string httpLine)
        {
            if (string.IsNullOrWhiteSpace(httpLine))
            {
                throw new ArgumentException(nameof(httpLine));
            }

            string[] headerAndValuesStrings = httpLine.Split(":");

            bool validHttpHeader =
                headerAndValuesStrings.Length == 2 &&
                !string.IsNullOrWhiteSpace(headerAndValuesStrings[0]) &&
                !string.IsNullOrWhiteSpace(headerAndValuesStrings[1]);
            
            if (!validHttpHeader)
            {
                throw new InvalidHttpHeaderException(httpLine);
            }

            return (HeaderName: headerAndValuesStrings[0].Trim(), HeaderValues: headerAndValuesStrings[1].Trim());
        }
        
        public static string GetBoundary(MediaTypeHeaderValue contentType)
        {
            string boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new MissingMultipartContentTypeBoundaryException();
            }

            return boundary;
        }
    }
}