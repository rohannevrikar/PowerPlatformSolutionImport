using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(PowerPlatformSolutionImport.StartupAuth))]

namespace PowerPlatformSolutionImport
{
    public class StartupAuth
    {
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
        string appKey = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];

        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

        // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {
                // Sets the ClientId, authority, RedirectUri as obtained from web.config
                ClientId = clientId,
                RedirectUri = redirectUri,    
                Authority = authority,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false
                },
                // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async (context) =>
                    {
                        var code = context.Code;

                        ClientCredential credential = new ClientCredential(clientId, appKey);
                        string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                        AuthenticationContext authContext = new AuthenticationContext(authority, new TokenCache());
                        AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            code, new Uri(redirectUri), credential, clientId);
                        context.AuthenticationTicket.Identity.AddClaim(new Claim("AccessToken", result.AccessToken));


                    },
                    AuthenticationFailed = OnAuthenticationFailed
                }
                
            }
        );
        }
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
