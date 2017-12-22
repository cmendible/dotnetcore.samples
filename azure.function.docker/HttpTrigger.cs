namespace Dni
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;

    public class HttpTrigger
    {
        public static IActionResult Run(HttpRequest req, TraceWriter log)
        {
            log.Info("DNI Validation function is processing a request.");

            string dni = req.Query["dni"];

            return dni != null
                ? (ActionResult)new OkObjectResult(ValidateDNI(dni))
                : new BadRequestObjectResult("Please pass a dni on the query string");
        }

        public static bool ValidateDNI(string dni)
        {
            var table = "TRWAGMYFPDXBNJZSQVHLCKE";
            var foreignerDigits = new Dictionary<char, char>()
        {
            { 'X', '0'},
            { 'Y', '1'},
            { 'Z', '2'}
        };
            var numbers = "1234567890";
            var parsedDNI = dni.ToUpper();
            if (parsedDNI.Length == 9)
            {
                var checkDigit = parsedDNI[8];
                parsedDNI = parsedDNI.Remove(8);
                if (foreignerDigits.ContainsKey(parsedDNI[0]))
                {
                    parsedDNI = parsedDNI.Replace(parsedDNI[0], foreignerDigits[parsedDNI[0]]);
                }

                return parsedDNI.Length == parsedDNI.Where(n => numbers.Contains(n)).Count() &&
                    table[int.Parse(parsedDNI) % 23] == checkDigit;
            }

            return false;
        }
    }
}