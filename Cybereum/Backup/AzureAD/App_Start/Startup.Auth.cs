using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Cybereum.Models;
using System.Configuration;
using System.Globalization;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;

namespace Cybereum
{
    public partial class Startup
    {
        private static string myclientId = ConfigurationManager.AppSettings["ClientId"];
        private static string mytenant = ConfigurationManager.AppSettings["Tenant"];
        private static string myaadInstance = ConfigurationManager.AppSettings["AADInstance"];
        private static string mypostLogoutRedirectUri = ConfigurationManager.AppSettings["PostLogoutRedirectUri"];
        private string authority = string.Format(CultureInfo.InvariantCulture, myaadInstance, mytenant);

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login"),
            //    Provider = new CookieAuthenticationProvider
            //    {
            //        // Enables the application to validate the security stamp when the user logs in.
            //        // This is a security feature which is used when you change a password or add an external login to your account.  
            //        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
            //            validateInterval: TimeSpan.FromMinutes(30),
            //            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
            //    }
            //});            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);


            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(

                            new OpenIdConnectAuthenticationOptions
                            {
                                ClientId = myclientId,
                                Authority = authority,
                                PostLogoutRedirectUri = mypostLogoutRedirectUri,
                                Notifications = new OpenIdConnectAuthenticationNotifications
                                {
                                    //AuthenticationFailed = OnAuthenticationFailed
                                    AuthenticationFailed = (context) =>
                                    {
                                        context.HandleResponse();
                                        context.OwinContext.Response.Redirect("/Home/Index");
                                        return Task.FromResult(0);
                                    }
                                }
                            });
        }


        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }


    }
}