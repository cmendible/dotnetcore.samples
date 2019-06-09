using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KubernetesController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodsController : ControllerBase
    {
        private KubernetesClientConfiguration k8sConfig = null;

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
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetPods()
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                var podList = client.ListNamespacedPod("default");
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
        public IActionResult Scale([FromBody]int replicas)
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                // Create a json patch fro the replicas
                var jsonPatch = new JsonPatchDocument<V1Scale>();
                jsonPatch.Replace(e => e.Spec.Replicas, replicas);
                var patch = new V1Patch(jsonPatch);

                // Patch the Deployment
                client.PatchNamespacedDeploymentScale(patch, "invader", "invaders");

                return NoContent();
            }
        }
    }
}
