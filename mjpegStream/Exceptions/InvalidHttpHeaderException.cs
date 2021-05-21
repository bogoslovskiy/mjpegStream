namespace mjpegStream.Exceptions
{
    using System;

    public class InvalidHttpHeaderException : Exception
    {
        public string Header { get; }
        
        public InvalidHttpHeaderException(string header)
        {
            this.Header = header;
        }
    }
}