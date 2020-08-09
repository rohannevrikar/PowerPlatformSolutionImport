using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PowerPlatformSolutionImport.Controllers
{
    public class ImportController : Controller
    {
        private readonly string destEnvUrl = "your-environment-url";  //https://orgc1234.crm.dynamics.com

        public async Task<ActionResult> IndexAsync()
        {
            await ImportSolution();
            return View();
        }

        private async Task<string> GetTokenForEnvironment()
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
            string appKey = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
            string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];
            string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);

            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            string token = claims?.FirstOrDefault(x => x.Type.Equals("AccessToken", StringComparison.OrdinalIgnoreCase))?.Value; //get user's token which will be used to get token for CDS environment
            UserAssertion userAssertion = new UserAssertion(token);
            AuthenticationResult result = null;

            AuthenticationContext authContext = new AuthenticationContext(authority, new TokenCache());
            result = await authContext.AcquireTokenAsync(destEnvUrl, new ClientCredential(clientId, appKey), userAssertion);
            return result.AccessToken;
        }

        private async Task ImportSolution()
        {
            string tokenForEnvironment = await GetTokenForEnvironment();
            string solutionFilePath = "file-path"; //path to your solution zip file
            Uri serviceUrlDestination = new Uri(destEnvUrl + @"/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2");

            OrganizationWebProxyClient sdkServiceDestination = new OrganizationWebProxyClient(serviceUrlDestination, false);


            sdkServiceDestination.HeaderToken = tokenForEnvironment;
            CrmServiceClient serviceClient = new CrmServiceClient(sdkServiceDestination);

            if (serviceClient.IsReady)
            {

                byte[] fileBytes = System.IO.File.ReadAllBytes(solutionFilePath);

                ImportSolutionRequest impSolReq = new ImportSolutionRequest()
                {
                    OverwriteUnmanagedCustomizations = true,
                    CustomizationFile = fileBytes
                };

                serviceClient.Execute(impSolReq);

            }
            else            
                throw serviceClient.LastCrmException;
            
        }
    }
}