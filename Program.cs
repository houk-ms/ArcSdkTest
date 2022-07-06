using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.KubernetesConfiguration.Extensions;
using Microsoft.Azure.Management.KubernetesConfiguration.Extensions.Models;
using Azure.Identity;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Text.Json;

namespace ArcSdkTest
{
    class Program
    {
        private const string _aksClusterRP = "Microsoft.ContainerService";
        private const string _aksClusterType = "managedClusters";
        private const string _defaultNamespace = "default";
        private const string _scExtensionName = "sc-extension";
        private const string _scExtensionType = "microsoft.servicelinker.connection";
        private const string _scExtensionReleaseTrain = "dev";
        private const string _scExtensionVersion = "0.1.0";
        private const string _scExtensionNamespace = "sc-system";

        public static async Task Main()
        {
            var credentials = GetCredentialFromSdkContext();
            SourceControlConfigurationClient client = new SourceControlConfigurationClient(credentials);
            string subscriptionId = "937bc588-a144-4083-8612-5f9ffbbddb14";
            client.SubscriptionId = subscriptionId;

            Extension extension = new Extension(
                name: _scExtensionName,
                type: "Extensions",
                extensionType: _scExtensionType,
                autoUpgradeMinorVersion: false,
                releaseTrain: _scExtensionReleaseTrain,
                version: _scExtensionVersion,
                scope: new Scope(
                    cluster: new ScopeCluster(
                        releaseNamespace: _scExtensionNamespace
                    )
                ),
                configurationSettings: new Dictionary<string, string>() { },
                configurationProtectedSettings: new Dictionary<string, string>() { 
                    {"connection", "testsecretconnection"},
                    {"correlationid", "56e30e73-5b70-4f90-be58-33a67c736ff5"},
                    {"operation", "create"},
                    {"resources", "eyJuYW1lc3BhY2UiOiB7Im5hbWUiOiAiZGVmYXVsdCJ9LCAic2VjcmV0IjogeyJuYW1lIjogInNjLXRlc3RzZWNyZXRjb25uZWN0aW9uLXNlY3JldCIsICJuYW1lc3BhY2UiOiAiZGVmYXVsdCIsICJkYXRhIjogeyJzZWNyZXQxIjogInZhbHVlMSIsICJzZWNyZXQyIjogInZhbHVlMiJ9fX0="}
                }
            );
            
            
            // test create
            try {
                Extension scExtension = await client.Extensions.CreateAsync("houk-test", "Microsoft.ContainerService", "managedClusters", "houk-aks", "sc-extension", extension);
                // the ProvisioningState is `Creating` when using Microsoft.Azure.Management.KubernetesConfiguration=2.0.0
                Console.WriteLine(JsonSerializer.Serialize(scExtension));
            } catch (ErrorResponseException ex) {
                Console.WriteLine(JsonSerializer.Serialize(ex));
            }
            
            return;
        }

        public static AzureCredentials GetCredentialFromSdkContext()
        {  
            string tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
            string clientId = "7c6afe09-63db-4abd-92fe-81c9ba5ee160";
            string clientSecret = "";
            var credentials = SdkContext.AzureCredentialsFactory
            .FromServicePrincipal(clientId,
                clientSecret,
                tenantId,
                AzureEnvironment.AzureGlobalCloud);
            return credentials;
        }
    }
}
