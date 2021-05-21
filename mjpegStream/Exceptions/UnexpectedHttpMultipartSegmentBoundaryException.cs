namespace mjpegStream.Exceptions
{
    using System;

    public class UnexpectedHttpMultipartSegmentBoundaryException : Exception
    {
        public string ExpectedBoundary { get; }
        
        public string ActualBoundary { get; }

        public UnexpectedHttpMultipartSegmentBoundaryException(string expectedBoundary, string actualBoundary) : 
            base($"Read unexpected multipart segment boundary. Expected: {expectedBoundary}, actual: {actualBoundary}.")
        {
            this.ExpectedBoundary = expectedBoundary;
            this.ActualBoundary = actualBoundary;
        }
    }
}