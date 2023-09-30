<<<<<<< Updated upstream
﻿using System;
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
using Microsoft.Owin.Security.Notifications;
//using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;
//using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Azure;
using System.Web.Security;

namespace Cybereum
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string PostLoginRedirectUri = ConfigurationManager.AppSettings["ida:PostLoginRedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["ida:AppScopes"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        //string graphResourceId = "https://graph.windows.net";

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
                    RedirectUri = PostLoginRedirectUri,
                    Scope = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectScope.OpenIdProfile,
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidAudience = $"{clientId}",
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //// If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        //AuthorizationCodeReceived = (context) =>
                        //{
                        //    var code = context.Code;
                        //    ClientCredential credential = new ClientCredential(clientId, appKey);
                        //    string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                        //    //AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                        //    TokenCache TC = new TokenCache();
                        //    AuthenticationContext authContext = new AuthenticationContext(Authority, TC);
                        //    var result = authContext.AcquireTokenByAuthorizationCodeAsync(
                        //    code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);
                        //    var token = authContext.AcquireTokenAsync(graphResourceId, credential).Result.AccessToken;

                        //    return Task.FromResult(0);
                        //},
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync,
                        AuthenticationFailed = OnAuthenticationFailed
                    }
                });

            //app.UseMicrosoftAccountAuthentication(clientId, appKey);
            //***********************End************************

            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            //// Enables the application to remember the second login verification factor such as phone or email.
            //// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            //// This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }


        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            //context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            context.Response.Redirect("/Project/List");
            return Task.FromResult(0);
        }


        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            var idClient = Microsoft.Identity.Client.ConfidentialClientApplicationBuilder.Create(clientId)
                .WithRedirectUri(PostLoginRedirectUri)
                .WithClientSecret(appKey)
                .Build();

            var signedInUser = new System.Security.Claims.ClaimsPrincipal(notification.AuthenticationTicket.Identity);
            var tokenStore = new ADTokenStore(idClient.UserTokenCache, HttpContext.Current, signedInUser);

            try
            {
                string[] scopes = graphScopes.Split(' ');

                var result = await idClient.AcquireTokenByAuthorizationCode(
                    scopes, notification.Code).ExecuteAsync();
                
                var userDetails = await GraphHelper.GetUserDetailsAsync(result.AccessToken);

                tokenStore.SaveUserDetails(userDetails);

                notification.HandleCodeRedemption(null, result.IdToken);


            }
            catch (Microsoft.Identity.Client.MsalException ex)
            {
                string message = "AcquireTokenByAuthorizationCodeAsync threw an exception";
                //notification.HandleResponse();
                //notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                string message = "GetUserDetailsAsync threw an exception";
                //notification.HandleResponse();
                //notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
        }
    }
}
=======
﻿using System;
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
using Microsoft.Owin.Security.Notifications;
//using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;
//using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Azure;
using System.Web.Security;

namespace Cybereum
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string PostLoginRedirectUri = ConfigurationManager.AppSettings["ida:PostLoginRedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["ida:AppScopes"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        //string graphResourceId = "https://graph.windows.net";

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
                    RedirectUri = PostLoginRedirectUri,
                    Scope = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectScope.OpenIdProfile,
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidAudience = $"{clientId}",
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //// If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        //AuthorizationCodeReceived = (context) =>
                        //{
                        //    var code = context.Code;
                        //    ClientCredential credential = new ClientCredential(clientId, appKey);
                        //    string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                        //    //AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                        //    TokenCache TC = new TokenCache();
                        //    AuthenticationContext authContext = new AuthenticationContext(Authority, TC);
                        //    var result = authContext.AcquireTokenByAuthorizationCodeAsync(
                        //    code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);
                        //    var token = authContext.AcquireTokenAsync(graphResourceId, credential).Result.AccessToken;

                        //    return Task.FromResult(0);
                        //},
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync,
                        AuthenticationFailed = OnAuthenticationFailed
                    }
                });

            //app.UseMicrosoftAccountAuthentication(clientId, appKey);
            //***********************End************************

            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            //// Enables the application to remember the second login verification factor such as phone or email.
            //// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            //// This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }


        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            //context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            context.Response.Redirect("/Project/List");
            return Task.FromResult(0);
        }


        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            var idClient = Microsoft.Identity.Client.ConfidentialClientApplicationBuilder.Create(clientId)
                .WithRedirectUri(PostLoginRedirectUri)
                .WithClientSecret(appKey)
                .Build();

            var signedInUser = new System.Security.Claims.ClaimsPrincipal(notification.AuthenticationTicket.Identity);
            var tokenStore = new ADTokenStore(idClient.UserTokenCache, HttpContext.Current, signedInUser);

            try
            {
                string[] scopes = graphScopes.Split(' ');

                var result = await idClient.AcquireTokenByAuthorizationCode(
                    scopes, notification.Code).ExecuteAsync();
                
                var userDetails = await GraphHelper.GetUserDetailsAsync(result.AccessToken);

                tokenStore.SaveUserDetails(userDetails);

                notification.HandleCodeRedemption(null, result.IdToken);


            }
            catch (Microsoft.Identity.Client.MsalException ex)
            {
                string message = "AcquireTokenByAuthorizationCodeAsync threw an exception";
                //notification.HandleResponse();
                //notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                string message = "GetUserDetailsAsync threw an exception";
                //notification.HandleResponse();
                //notification.Response.Redirect($"/Home/Error?message={message}&debug={ex.Message}");
            }
        }
    }
}
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
