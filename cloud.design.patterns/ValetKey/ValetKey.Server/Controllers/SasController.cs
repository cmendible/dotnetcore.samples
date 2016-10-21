// ==============================================================================================================
// Microsoft patterns & practices
// Cloud Design Patterns project
// ==============================================================================================================
// ©2013 Microsoft. All rights reserved. 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================
namespace ValetKey.Web.Api.Controllers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class SasController : Controller
    {
        private readonly string blobContainer = "valetkeysample";

        [HttpGet] 
        public IActionResult Get()
        {
            try
            {
                var blobName = Guid.NewGuid();

                // Retrieve a shared access signature of the location we should upload this file to
                var blobSas = this.GetSharedAccessReferenceForUpload(blobName.ToString());

                return new ObjectResult(blobSas);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// We return a limited access key that allows the caller to upload a file to this specific destination for defined period of time
        /// </summary>
        private StorageEntitySas GetSharedAccessReferenceForUpload(string blobName)
        {
            var storageKey = "";
            var accountName = "";
            var sasVersion = "2015-07-08";
            DateTimeOffset sharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
            DateTimeOffset sharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(-5);

            var st = sharedAccessStartTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var se = sharedAccessExpiryTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

            string canonicalNameFormat = $"/blob/{accountName}/{blobContainer}/{blobName}";

            var sas = GetSharedAccessSignature(accountName, sasVersion, storageKey, st, se);
            var credentials = UrlEncoder.Default.Encode($"?sv={sasVersion}&sr=b&sig={sas}&st={st}&se={se}&srt=o&sp=w");

            var blobSas = new StorageEntitySas
            {
                BlobUri =  new Uri($"https://{accountName}.blob.core.windows.net/{blobContainer}/{blobName}"),
                Credentials = credentials,
                Name = blobName
            };

            var currentDateTime = DateTime.UtcNow.ToString("U");
            string tosign = $"PUT\n\n\n\n\n\n\n\n\n\n\n\nx-ms-date:{currentDateTime}\nx-ms-version:2015-02-21\n/{accountName}/{blobContainer}\nrestype:container\ntimeout:30";
            string sharedKey = GetHash(tosign, storageKey);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-ms-date", currentDateTime);
                client.DefaultRequestHeaders.Add("x-ms-version", "2015-02-21");
                client.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKey", $" {accountName}:{sharedKey}");
                var content = new StringContent(string.Empty);

                var result = client.PutAsync(
                    blobSas.BlobUri.ToString(),
                    content).Result;
            }

            return blobSas;
        }

        private string GetSharedAccessSignature(string resourceName, string sasVersion, string key, string sharedAccessStartTime, string sharedAccessExpiryTime)
        {
            string stringToSign = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}", new object[]
            {
                "w",
                sharedAccessStartTime,
                sharedAccessExpiryTime,
                resourceName,
                string.Empty,
                string.Empty,
                "https",
                sasVersion,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
            });

            return GetHash(stringToSign, key);
        }

        private string GetHash(string stringToSign, string key)
        {
            byte[] keyValue = Convert.FromBase64String(key);

            using (HMACSHA256 hmac = new HMACSHA256(keyValue))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            }
        }
    }
}