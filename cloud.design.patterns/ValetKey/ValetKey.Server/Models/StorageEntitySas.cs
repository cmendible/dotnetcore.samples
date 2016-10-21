namespace ValetKey.Web.Api.Controllers
{
    using System;

    public struct StorageEntitySas
    {
        public string Credentials;
        public Uri BlobUri;
        public string Name;
    }
}