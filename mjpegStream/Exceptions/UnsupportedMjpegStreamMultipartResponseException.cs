namespace mjpegStream.Exceptions
{
    using System;

    public class UnsupportedMjpegStreamMultipartResponseException : Exception
    {
        public UnsupportedMjpegStreamMultipartResponseException(string contentType) :
            base($"Unsupported MJPEG stream multipart content type: {contentType ?? "<Undefined>"}")
        {
        }
    }
}