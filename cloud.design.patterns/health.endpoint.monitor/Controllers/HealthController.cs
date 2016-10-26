namespace ValetKey.Web.Api.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        public HealthController(IConfigurationRoot configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync($"http://localhost:5000/api/sas/");

                    result.EnsureSuccessStatusCode();
                }

                return Ok("Everything is running smooth");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }       
    }
}