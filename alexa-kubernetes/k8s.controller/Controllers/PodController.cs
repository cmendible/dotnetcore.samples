using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace KubernetesController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodsController : ControllerBase
    {
        private KubernetesClientConfiguration k8sConfig = null;

        private HttpClient restClient = null;

        private string endpoint = string.Empty;

        public PodsController(IConfiguration config)
        {
            var useKubeConfig = bool.Parse(config["UseKubeConfig"]);
            if (!useKubeConfig)
            {
                k8sConfig = KubernetesClientConfiguration.InClusterConfig();
            }
            else
            {
                k8sConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            }

            this.endpoint = config["eventGridEndPoint"].ToString();

            this.restClient = RestClient(config);
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetPods()
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                var podList = client.ListNamespacedPod("invaders");

                return podList.Items.Select(p => p.Metadata.Name).ToArray();
            }
        }

        [HttpGet("namespaces")]
        public ActionResult<IEnumerable<string>> GetNamespaces()
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                var namespaces = client.ListNamespace();

                return namespaces.Items.Select(ns => ns.Metadata.Name).ToArray();
            }
        }

        [HttpPatch("scale")]
        public async Task<IActionResult> Scale([FromBody]int replicas)
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                // Create a json patch fro the replicas
                var jsonPatch = new JsonPatchDocument<V1Scale>();
                jsonPatch.Replace(e => e.Spec.Replicas, replicas);
                var patch = new V1Patch(jsonPatch);

                await BubbleEvent("scale", replicas);

                // Patch the Deployment
                client.PatchNamespacedDeploymentScale(patch, "invaders", "invaders");

                return NoContent();
            }
        }

        private HttpClient RestClient(IConfiguration config)
        {
            if (this.restClient is null)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                var httpClient = new HttpClient(httpClientHandler);
                var sas = config["eventGridSAS"].ToString();
                httpClient.DefaultRequestHeaders.Add("aeg-sas-key", sas);
                this.restClient = httpClient;
            }

            return this.restClient;
        }

        private async Task BubbleEvent(string evenType, object data)
        {
            // Event must have this fields
            var customEvent = new GridEvent<object>
            {
                Subject = "Event",
                EventType = evenType,
                EventTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Data = data
            };

            // A List must be sent
            var eventList = new List<GridEvent<object>>() { customEvent };

            string jsonEvent = JsonConvert.SerializeObject(eventList);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this.endpoint)
            {
                Content = new StringContent(jsonEvent, Encoding.UTF8, "application/json")
            };

            await restClient.SendAsync(request);
        }

    }

    public class GridEvent<T> where T : class
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string EventType { get; set; }
        public T Data { get; set; }
        public DateTime EventTime { get; set; }
    }
}
