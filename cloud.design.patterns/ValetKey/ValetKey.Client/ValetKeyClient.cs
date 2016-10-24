namespace ValetKey
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json;

    public class ValetKeyClient
    {
        public static void UploadBlob()
        {
            // Make sure the endpoint matches with the web role's endpoint.
            var tokenServiceEndpoint = "http://localhost:5000/api/sas";

            try
            {
                var blobSas = GetBlobSas(new Uri(tokenServiceEndpoint)).Result;
                using (var stream = GetFileToUpload(10))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");
                        var content = new StreamContent(stream);

                        var result = client.PutAsync(
                            blobSas.BlobUri.ToString() + blobSas.Credentials,
                            content).Result;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Blob uplodad successful: {0}", blobSas.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        private static async Task<StorageEntitySas> GetBlobSas(Uri blobUri)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(blobUri);
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<StorageEntitySas>(json);
            }
        }

        /// <summary>
        /// Create a sample file containing random bytes of data
        /// </summary>
        /// <param name="sizeMb"></param>
        /// <returns></returns>
        private static MemoryStream GetFileToUpload(int sizeMb)
        {
            var stream = new MemoryStream();

            var rnd = new Random();
            var buffer = new byte[1024 * 1024];

            for (int i = 0; i < sizeMb; i++)
            {
                rnd.NextBytes(buffer);
                stream.Write(buffer, 0, buffer.Length);
            }

            stream.Position = 0;

            return stream;
        }

        public struct StorageEntitySas
        {
            public string Credentials;
            public Uri BlobUri;
            public string Name;
        }
    }
}
