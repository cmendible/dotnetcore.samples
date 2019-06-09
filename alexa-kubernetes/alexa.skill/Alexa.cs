using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Xml;
using Microsoft.SyndicationFeed.Rss;
using System.Linq;
using System;
using System.Net.Http;
using System.Text;

namespace AlexaSkill
{
    public static class Alexa
    {
        private static HttpClient restClient => RestClient();

        private static HttpClient RestClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            return new HttpClient(httpClientHandler);
        }

        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            //this is the language used to invoke the skill
            string language = skillRequest.Request.Locale;

            bool isValid = await ValidateRequest(req, log, skillRequest);
            if (!isValid)
            {
                return new BadRequestResult();
            }

            var requestType = skillRequest.GetRequestType();

            SkillResponse response = null;

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Ask("Welcome to dNext!!! Thanks for the invite Carlos! Glad to see my everis brothers and sisters today!", new Reprompt());
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                if (intentRequest.Intent.Name == "namespaces")
                {
                    var endpoint = "https://localhost:5001/api/pods/namespaces";
                    var k8sresponse = await restClient.GetAsync(endpoint);

                    if (k8sresponse.IsSuccessStatusCode)
                    {
                        var namespaces = await k8sresponse.Content.ReadAsAsync<string[]>();
                        var message = string.Join(",", namespaces);
                        response = ResponseBuilder.Tell($"Found the following namespaces: {message}");
                    }
                }

                if (intentRequest.Intent.Name == "pods")
                {
                    var endpoint = "https://localhost:5001/api/pods";
                    var k8sresponse = await restClient.GetAsync(endpoint);

                    if (k8sresponse.IsSuccessStatusCode)
                    {
                        var pods = await k8sresponse.Content.ReadAsAsync<string[]>();
                        var message = string.Join(",", pods);
                        response = ResponseBuilder.Tell($"Found the following pods in the default namespace: {pods}");
                    }
                }

                if (intentRequest.Intent.Name == "scale")
                {
                    var endpoint = "https://localhost:5001/api/pods/scale";

                    var replicas = intentRequest.Intent.Slots["replicas"].Value;

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
                    {
                        Content = new StringContent(replicas, Encoding.UTF8, "application/json")
                    };

                    var k8sresponse = await restClient.SendAsync(request);

                    if (k8sresponse.IsSuccessStatusCode)
                    {
                        response = ResponseBuilder.Tell($"Deployment scaled to {replicas} invaders");
                    }
                }
                else if (intentRequest.Intent.Name == "AMAZON.CancelIntent")
                {
                    response = ResponseBuilder.Tell("I'm cancelling the request...");
                }
                else if (intentRequest.Intent.Name == "AMAZON.HelpIntent")
                {
                    response = ResponseBuilder.Tell("Sorry due you are on your own.");
                    response.Response.ShouldEndSession = false;
                }
                else if (intentRequest.Intent.Name == "AMAZON.StopIntent")
                {
                    response = ResponseBuilder.Tell("bye");
                }
            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                log.LogInformation("Session ended");
                response = ResponseBuilder.Empty();
                response.Response.ShouldEndSession = true;
            }

            return new OkObjectResult(response);
        }

        private static async Task<bool> ValidateRequest(HttpRequest request, ILogger log, SkillRequest skillRequest)
        {
            request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureChainUrl);
            if (string.IsNullOrWhiteSpace(signatureChainUrl))
            {
                log.LogError("Validation failed. Empty SignatureCertChainUrl header");
                return false;
            }

            Uri certUrl;
            try
            {
                certUrl = new Uri(signatureChainUrl);
            }
            catch
            {
                log.LogError($"Validation failed. SignatureChainUrl not valid: {signatureChainUrl}");
                return false;
            }

            request.Headers.TryGetValue("Signature", out var signature);
            if (string.IsNullOrWhiteSpace(signature))
            {
                log.LogError("Validation failed - Empty Signature header");
                return false;
            }

            request.Body.Position = 0;
            var body = await request.ReadAsStringAsync();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                log.LogError("Validation failed - the JSON is empty");
                return false;
            }

            var date = skillRequest.Request.Timestamp.ToLocalTime();
            bool isTimestampValid = RequestVerification.RequestTimestampWithinTolerance(date);
            bool valid = await RequestVerification.Verify(signature, certUrl, body);

            if (!valid || !isTimestampValid)
            {
                log.LogError("Validation failed - RequestVerification failed");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}