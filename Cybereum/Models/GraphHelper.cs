using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace Cybereum.Models
{
    public static class GraphHelper
    {
        public static async Task<CachedUser> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            var user = await graphClient.Me.Request()
                .Select(u => new {
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName  ,
                    u.Id  ,
                    u.CompanyName,
                    u.EmployeeId,
                    u.EmployeeType,
                    u.GivenName,
                    u.Identities,
                    u.Manager,
                    u.MobilePhone,
                    u.Surname,
                    u.UserType                 
                })
                .GetAsync();

            return new CachedUser
            {
                Avatar = string.Empty,
                DisplayName = user.DisplayName,
                token = user.Id,
                CompanyName = user.CompanyName,
                Email = string.IsNullOrEmpty(user.Mail) ?
                    user.UserPrincipalName : user.Mail
            };
        }
    }
}