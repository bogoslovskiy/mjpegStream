namespace mjpegStream.Tests
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MultipartSegmentBodyReaderUnitTests
    {
        [Theory]
        [InlineData(
            "asfhjk2138sdfuiysdiufhjkHHHJ234234GJHSDF1267",
            "fhrycnbvagbbdsfjg15623BCbnsdgHC",
            // Reading buffer sizes.
            new[]
            {
                /* Less than count */ 1, 2, 4, 8, 16, 32, 
                /* Equal count */ 44, 
                /* Greater than count */ 64, 128, 256, 1024
            }
        )]
        public async Task ReadBodyBytesAsync_ShouldReadStreamBoundedPart(
            string boundedPart,
            string noise,
            int[] bufferSizes)
        {
            string fullStreamString = boundedPart + noise;
            
            foreach (int bufferSize in bufferSizes)
            {
                await using MemoryStream sourceStream = new(Encoding.ASCII.GetBytes(fullStreamString));
                await using MemoryStream targetStream = new();
                
                int contentLength = boundedPart.Length;
                
                byte[] buffer = new byte[bufferSize];

                MultipartSegmentBodyReader target = new(stream: sourceStream, contentLength: contentLength);

                int bytesRead;

                do
                {
                    bytesRead = await target.ReadBodyBytesAsync(buffer: buffer, offset: 0, count: buffer.Length);

                    if (bytesRead > 0)
                    {
                        await targetStream.WriteAsync(buffer: buffer, offset: 0, count: bytesRead);
                    }
                }
                while (bytesRead > 0);

                string actualBoundedPart = Encoding.ASCII.GetString(targetStream.ToArray());
                
                Assert.Equal(boundedPart, actualBoundedPart);
            }
        }
    }
}