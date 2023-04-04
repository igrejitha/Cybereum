using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Cybereum.Models;
//using Gremlin.Net;
//using Microsoft.Extensions.DependencyInjection;

namespace Cybereum
{
    public partial class Startup
    {
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

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }


        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSingleton<GremlinClient>(
        //        (serviceProvider) =>
        //        {
        //            var gremlinServer = new GremlinServer(
        //                hostname: "localhost",
        //                port: 8182,
        //                enableSsl: false,
        //                username: null,
        //                password: null
        //            );

        //            var connectionPoolSettings = new ConnectionPoolSettings
        //            {
        //                MaxInProcessPerConnection = 32,
        //                PoolSize = 4,
        //                ReconnectionAttempts = 4,
        //                ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
        //            };

        //            return new GremlinClient(
        //                gremlinServer: gremlinServer,
        //                connectionPoolSettings: connectionPoolSettings
        //            );
        //        }
        //    );

        //    services.AddSingleton<GraphTraversalSource>(
        //        (serviceProvider) =>
        //        {
        //            GremlinClient gremlinClient = serviceProvider.GetService<GremlinClient>();
        //            var driverRemoteConnection = new DriverRemoteConnection(gremlinClient, "g");
        //            return AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
        //        }
        //    );

        //    services.AddRazorPages();
        //}
    }
}