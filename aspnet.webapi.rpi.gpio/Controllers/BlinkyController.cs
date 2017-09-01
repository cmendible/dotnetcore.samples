using Microsoft.AspNetCore.Mvc;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace aspnet.webapi.rpi.gpio.Controllers
{
    [Route("api/[controller]")]
    public class BlinkyController : Controller
    {
        [HttpPost]
        public void Post([FromBody]bool isOn)
        {
            var pin = Pi.Gpio.Pin05;
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(!isOn);
        }
    }
}
