namespace mjpegStream.Exceptions
{
    using System;

    public class InvalidHttpMultipartSegmentFooterException : Exception
    {
        public InvalidHttpMultipartSegmentFooterException() : 
            base("Multipart segment footer has invalid format. CRLF expected after multipart content body.")
        {
        }
    }
}