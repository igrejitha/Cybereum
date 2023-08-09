using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using Cybereum.Models;

namespace Cybereum
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            //***********************Azure Active Directory************************
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(clientId, appKey);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                            AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                            var result = authContext.AcquireTokenByAuthorizationCodeAsync(
                            code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);

                            return Task.FromResult(0);
                        }
                    }
                });

            //***********************End************************


            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            //// Enables the application to remember the second login verification factor such as phone or email.
            //// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            //// This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}
