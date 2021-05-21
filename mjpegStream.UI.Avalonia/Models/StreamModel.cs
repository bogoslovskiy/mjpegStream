namespace mjpegStream.UI.Avalonia.Models
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class StreamModel : IDisposable
    {
        public class JpegBinaryUpdatedEventArgs : EventArgs
        {
            public byte[] Data { get; }
            
            public JpegBinaryUpdatedEventArgs(byte[] data)
            {
                this.Data = data;
            }
        }
        
        private readonly string _uri;
        private CancellationTokenSource _stoppingTokenSource = new();

        public event EventHandler<JpegBinaryUpdatedEventArgs> OnJpegBinaryUpdated;

        public event EventHandler<EventArgs> OnUnexpectedError; 

        public StreamModel(string uri)
        {
            this._uri = uri;

            OpenStreamWrapped(_stoppingTokenSource.Token).ConfigureAwait(false);
        }

        async Task OpenStreamWrapped(CancellationToken stoppingToken)
        {
            try
            {
                await OpenStream(stoppingToken).ConfigureAwait(false);
            }
            catch
            {
                OnUnexpectedError?.Invoke(this, new EventArgs());
            }
        }

        async Task OpenStream(CancellationToken stoppingToken)
        {
            using HttpClient httpClient = new();

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _uri);

            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(
                httpRequestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                stoppingToken
            );

            MultipartSegmentReader reader = MultipartSegmentReader.CreateFrom(httpResponseMessage);

            byte[] buffer = new byte[4 * 1024 * 1024];

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                MultipartSegmentBodyReader multipartBodyReader = await reader.GetNextSegmentBodyReaderAsync();

                await using MemoryStream bitmapStream = new();

                int bytesRead;

                do
                {
                    bytesRead = await multipartBodyReader.ReadBodyBytesAsync(buffer, 0, buffer.Length, stoppingToken);

                    if (bytesRead > 0)
                    {
                        await bitmapStream.WriteAsync(buffer, 0, bytesRead, stoppingToken);
                    }
                    else
                    {
                        bitmapStream.Seek(0, SeekOrigin.Begin);

                        OnJpegBinaryUpdated?.Invoke(this, new JpegBinaryUpdatedEventArgs(bitmapStream.ToArray()));
                    }
                }
                while (bytesRead > 0);

                reader.ReadFooter();
            }
        }

        public void Dispose()
        {
            _stoppingTokenSource.Cancel();
            _stoppingTokenSource.Dispose();
        }
    }
}