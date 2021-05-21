namespace mjpegStream
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class MultipartSegmentBodyReader
    {
        private readonly Stream stream;
        private readonly int contentLength;

        private int read;

        public MultipartSegmentBodyReader(Stream stream, int contentLength)
        {
            this.stream = stream;
            this.contentLength = contentLength;

            read = 0;
        }

        public async Task<int> ReadBodyBytesAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            if (read == contentLength)
            {
                return 0;
            }

            int remainBytes = contentLength - read;

            int bytesToRead = Math.Min(count, remainBytes);

            int bytesRead = await stream.ReadAsync(buffer, offset, bytesToRead, cancellationToken);

            read += bytesRead;

            return bytesRead;
        }
    }
}