using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading;


namespace Cybereum.Models
{
    public class ADTokenStore
    {
        private static readonly ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private HttpContext httpContext = null;
        private string tokenCacheKey = string.Empty;
        private string userCacheKey = string.Empty;

        cybereumEntities objdmsEntities = new cybereumEntities();

        public ADTokenStore(ITokenCache tokenCache, HttpContext context, ClaimsPrincipal user)
        {
            httpContext = context;

            if (tokenCache != null)
            {
                tokenCache.SetBeforeAccess(BeforeAccessNotification);
                tokenCache.SetAfterAccess(AfterAccessNotification);
            }

            var userId = GetUsersUniqueId(user);
            tokenCacheKey = $"{userId}_TokenCache";
            userCacheKey = $"{userId}_UserCache";
        }

        public bool HasData()
        {
            return (httpContext.Session[tokenCacheKey] != null &&
                ((byte[])httpContext.Session[tokenCacheKey]).Length > 0);
        }

        public void Clear()
        {
            sessionLock.EnterWriteLock();

            try
            {
                httpContext.Session.Remove(tokenCacheKey);
            }
            finally
            {
                sessionLock.ExitWriteLock();
            }
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            sessionLock.EnterReadLock();

            try
            {
                // Load the cache from the session
                args.TokenCache.DeserializeMsalV3((byte[])httpContext.Session[tokenCacheKey]);
            }
            finally
            {
                sessionLock.ExitReadLock();
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                sessionLock.EnterWriteLock();

                try
                {
                    // Store the serialized cache in the session
                    httpContext.Session[tokenCacheKey] = args.TokenCache.SerializeMsalV3();
                }
                finally
                {
                    sessionLock.ExitWriteLock();
                }
            }
        }

        public void SaveUserDetails(CachedUser user)
        {
            sessionLock.EnterWriteLock();
            httpContext.Session[userCacheKey] = JsonConvert.SerializeObject(user);
            httpContext.Session["uniqueid"] = user.token;
            //Save to User table
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.username==user.token && x.isactive != 2).FirstOrDefault();
=======
            var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.username == user.token && x.isactive != 2).FirstOrDefault();
>>>>>>> Stashed changes
=======
            var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.username == user.token && x.isactive != 2).FirstOrDefault();
>>>>>>> Stashed changes
=======
            var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.username == user.token && x.isactive != 2).FirstOrDefault();
>>>>>>> Stashed changes
            if (objList == null)
            {
                tbl_user usertbl = new tbl_user();
                usertbl.firstname = user.DisplayName;
                usertbl.lastname = "";
                usertbl.username = user.token;
                usertbl.emailid = user.Email;
                usertbl.password = "";
                usertbl.createddate = DateTime.Now;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                usertbl.organization = "Cybereum";
                usertbl.roleid = (int)Role.ProjectManager;
=======
                usertbl.organization = string.IsNullOrEmpty(user.CompanyName) ? "Cybereum" : user.CompanyName;
                usertbl.roleid = (int)Role.OrganizationAdmin;
>>>>>>> Stashed changes
=======
                usertbl.organization = string.IsNullOrEmpty(user.CompanyName) ? "Cybereum" : user.CompanyName;
                usertbl.roleid = (int)Role.OrganizationAdmin;
>>>>>>> Stashed changes
=======
                usertbl.organization = string.IsNullOrEmpty(user.CompanyName) ? "Cybereum" : user.CompanyName;
                usertbl.roleid = (int)Role.OrganizationAdmin;
>>>>>>> Stashed changes
                usertbl.isactive = 1;
                usertbl.emailverification = true;
                objdmsEntities.tbl_user.Add(usertbl);
                objdmsEntities.SaveChanges();
            }
            else
            {

            }


            sessionLock.ExitWriteLock();
        }

        public CachedUser GetUserDetails()
        {
            sessionLock.EnterReadLock();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            var cachedUser = JsonConvert.DeserializeObject<CachedUser>((string)httpContext.Session[userCacheKey]);                                                
=======
            var cachedUser = JsonConvert.DeserializeObject<CachedUser>((string)httpContext.Session[userCacheKey]);
>>>>>>> Stashed changes
=======
            var cachedUser = JsonConvert.DeserializeObject<CachedUser>((string)httpContext.Session[userCacheKey]);
>>>>>>> Stashed changes
=======
            var cachedUser = JsonConvert.DeserializeObject<CachedUser>((string)httpContext.Session[userCacheKey]);
>>>>>>> Stashed changes
            sessionLock.ExitReadLock();
            return cachedUser;
        }

        public string GetUsersUniqueId(ClaimsPrincipal user)
        {
            // Combine the user's object ID with their tenant ID

            if (user != null)
            {
                var userObjectId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value ??
                    user.FindFirst("oid").Value;

                var userTenantId = user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value ??
                    user.FindFirst("tid").Value;

                if (!string.IsNullOrEmpty(userObjectId) && !string.IsNullOrEmpty(userTenantId))
                {
                    return $"{userObjectId}.{userTenantId}";
                }
            }

            return null;
        }
    }
}