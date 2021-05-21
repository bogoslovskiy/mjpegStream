namespace mjpegStream
{
    public struct CrLfMatcher
    {
        private bool crDetected;

        public bool Push(char c)
        {
            if (c == '\r')
            {
                crDetected = true;

                return false;
            }

            if (c == '\n')
            {
                if (crDetected)
                {
                    crDetected = false;

                    return true;
                }
            }

            crDetected = false;

            return false;
        }
    }
}