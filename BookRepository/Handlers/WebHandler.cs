using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BookRepository
{
    public static class WebHandler
    {
        public static async Task<BitmapImage> GetNewImageAsync(Uri uri)
        {
            BitmapImage bitmap = null;
            var httpClient = new HttpClient();

            using (var response = await httpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = new MemoryStream())
                    {
                        await response.Content.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }
            }

            return bitmap;
        }
    }
}
