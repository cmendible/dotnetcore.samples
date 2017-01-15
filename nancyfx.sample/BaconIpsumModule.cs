using Nancy;

namespace WebApplication
{
    public class BaconIpsumModule : NancyModule
    {
        public BaconIpsumModule(IBaconIpsumService baconIpsumService)
        {
            Get("/", args => "Super Duper Happy Path running on .NET Core");

            Get("/baconipsum", async args => await baconIpsumService.GenerateAsync());
        }
    }
}