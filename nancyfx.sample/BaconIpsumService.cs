
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication
{
    public interface IBaconIpsumService
    {
        Task<string> Generate();
    }

    public class BaconIpsumService : IBaconIpsumService
    {
        public async Task<string> Generate()
        {
            using (var client = new HttpClient())
            {
                // Post the message
                return await client.GetStringAsync(
                    $"https://baconipsum.com/api/?type=meat-and-filler");
            }
        }
    }
}
