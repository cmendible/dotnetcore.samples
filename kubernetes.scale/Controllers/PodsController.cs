using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace kubernetes.scale
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodsController : ControllerBase
    {
        private KubernetesClientConfiguration k8sConfig = null;

        public PodsController(IConfiguration config)
        {
            // Reading configuration to know if running inside a cluster or in local mode.
            var useKubeConfig = bool.Parse(config["UseKubeConfig"]);
            if (!useKubeConfig)
            {
                // Running inside a k8s cluser
                k8sConfig = KubernetesClientConfiguration.InClusterConfig();
            }
            else
            {
                // Running on dev machine
                k8sConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            }
        }

        [HttpPatch("scale")]
        public IActionResult Scale([FromBody]ReplicaRequest request)
        {
            // Use the config object to create a client.
            using (var client = new Kubernetes(k8sConfig))
            {
                // Create a json patch for the replicas
                var jsonPatch = new JsonPatchDocument<V1Scale>();
                // Set the new number of repplcias
                jsonPatch.Replace(e => e.Spec.Replicas, request.Replicas);
                // Creat the patch
                var patch = new V1Patch(jsonPatch);

                // Patch the "minions" Deployment in the "default" namespace
                client.PatchNamespacedDeploymentScale(patch, request.Deployment, request.Namespace);

                return NoContent();
            }
        }
    }

    public class ReplicaRequest
    {
        public string Deployment { get; set; }
        public string Namespace { get; set; }
        public int Replicas { get; set; }
    }
}