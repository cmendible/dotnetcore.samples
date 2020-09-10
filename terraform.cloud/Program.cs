namespace TerraformCloud
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Tfe.NetClient;
    using Tfe.NetClient.OAuthClients;
    using Tfe.NetClient.Runs;
    using Tfe.NetClient.Workspaces;
    using Tfe.NetClient.WorkspaceVariables;

    class Program
    {
        static async Task Main(string[] args)
        {
            var organizationName = "<organizationName>";
            var organizationToken = "<organizationToken>";
            var teamToken = "<teamToken>";
            var gitHubToken = "<GitHub Personal Access Token>";
            var gitHubRepo = "<github user or organization name>/<repo name>";

            // Create an HttpClient
            var httpClient = new HttpClient();

            // Create the Configiration used by the TFE client.
            // For management tasks you'll need to connect to Terraform Cloud using an Organization Token
            var config = new TfeConfig(organizationToken, httpClient);

            // Create the TFE client.
            var client = new TfeClient(config);

            // Connect Terraform Cloud and GitHub
            var oauthClientsRequest = new OAuthClientsRequest();
            oauthClientsRequest.Data.Attributes.ServiceProvider = "github";
            oauthClientsRequest.Data.Attributes.HttpUrl = new Uri("https://github.com");
            oauthClientsRequest.Data.Attributes.ApiUrl = new Uri("https://api.github.com");
            oauthClientsRequest.Data.Attributes.OAuthTokenString = gitHubToken; // Use the GitHub Personal Access Token
            var oauthResult = await client.OAuthClient.CreateAsync(organizationName, oauthClientsRequest);

            // Get the OAuthToken
            var oauthTokenId = oauthResult.Data.Relationships.OAuthTokens.Data[0].Id;

            // Create a Workspace connected to a GitHub repo.
            var workspacesRequest = new WorkspacesRequest();
            workspacesRequest.Data.Attributes.Name = "my-workspace";
            workspacesRequest.Data.Attributes.VcsRepo = new RequestVcsRepo();
            workspacesRequest.Data.Attributes.VcsRepo.Identifier = gitHubRepo; // Use the GitHub Repo
            workspacesRequest.Data.Attributes.VcsRepo.OauthTokenId = oauthTokenId;
            workspacesRequest.Data.Attributes.VcsRepo.Branch = "";
            workspacesRequest.Data.Attributes.VcsRepo.DefaultBranch = true;
            var workspaceResult = await client.Workspace.CreateAsync(organizationName, workspacesRequest);

            // Get the Workspace Id so we can add variales or request a plan or apply.
            var workspaceId = workspaceResult.Data.Id;

            // Create a variable in the workspace.
            // You can make the values invible setting the Sensitive attribute to true.
            // If you want to se an environement variable change the Category attribute to "env"
            // You'll have to create as any variables your script needs.
            var workspaceVariablesRequest = new WorkspaceVariablesRequest();
            workspaceVariablesRequest.Data.Attributes.Key = "variable_1";
            workspaceVariablesRequest.Data.Attributes.Value = "variable_1_value";
            workspaceVariablesRequest.Data.Attributes.Description = "variable_1 description";
            workspaceVariablesRequest.Data.Attributes.Category = "terraform";
            workspaceVariablesRequest.Data.Attributes.Hcl = false;
            workspaceVariablesRequest.Data.Attributes.Sensitive = false;
            var variableResult = await client.WorkspaceVariable.CreateAsync(workspaceId, workspaceVariablesRequest);

            // Get the workspace by name
            var workspace = client.Workspace.ShowAsync(organizationName, "my-workspace");

            // To create Runs and Apply thme you need to use a Team Token
            // So create a new TfeClient
            var runsClient = new TfeClient(new TfeConfig(teamToken, new HttpClient()));

            // Create the Run.
            // This is th equivalent to running: terraform plan. 
            var runsRequest = new RunsRequest();
            runsRequest.Data.Attributes.IsDestroy = false;
            runsRequest.Data.Attributes.Message = "Triggered by .NET Core";
            runsRequest.Data.Relationships.Workspace.Data.Type = "workspaces";
            runsRequest.Data.Relationships.Workspace.Data.Id = workspace.Result.Data.Id;
            var runsResult = await runsClient.Run.CreateAsync(runsRequest);

            // Get the Run Id. You'll need it to check teh state of the run and Apply it if possible.
            var runId = runsResult.Data.Id;

            var ready = false;
            while (!ready)
            {
                // Wait for the Run to be planned 
                await Task.Delay(5000);
                var run = await client.Run.ShowAsync(runId);
                ready = run.Data.Attributes.Status == "planned";

                // Throw an exception if the Run status is: errored 
                if (run.Data.Attributes.Status == "errored") {
                    throw new Exception("Plan failed...");
                }
            }

            // If the Run is planned then Apply your configuration.
            if (ready)
            {
                await runsClient.Run.ApplyAsync(runId, null);
            }
        }
    }
}
