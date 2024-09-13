using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Diagnostics;
//using Glass.Mapper.Sc;
using Sitecore.Data.Items;
using System;
using DEWAXP.Foundation.Content.Repositories;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using DEWAXP.Foundation.Logger;

namespace DEWAXP.Foundation.Content.Filters.Mvc
{

    public class AuthorizedCustomRoleFilter : AuthorizationFilterAttribute
    {
        private readonly string _role;
        private bool _ensureAuthenticationIsSet = true;
        public bool EnsureAuthenticationIsSet
        {
            get
            {
                IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
                //var sitecoreService = new Glass.Mapper.Sc.SitecoreContext();
                var accountlimit = _contentRepository.GetItem<Item>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.Api_Count_Config)));
                return accountlimit.Fields["IsCheckedApi"].Value == "1" ? true : false;
            }
            set { _ensureAuthenticationIsSet = value; }
        }
        public AuthorizedCustomRoleFilter(string role)
        {
            _role = role;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (EnsureAuthenticationIsSet)
                {
                    IEnumerable<string> UserValues;
                    var userId = string.Empty;

                    if (actionContext.Request.Headers.TryGetValues("username", out UserValues))
                    {
                        userId = UserValues.FirstOrDefault();
                    }
                    IEnumerable<string> passwordValues;
                    var password = string.Empty;

                    if (actionContext.Request.Headers.TryGetValues("password", out passwordValues))
                    {
                        password = passwordValues.FirstOrDefault();
                    }
                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
                    {
                        var _context = GetUser("extranet", userId, password);

                        //if (Context.User.IsAdministrator || Context.User.IsInRole($"sitecore\\{_role}") && Context.User.IsAuthenticated)
                        if (_context != null && (_context.IsAuthenticated || _context.Roles.User.IsInRole($"extranet\\{_role}")))
                            return;
                    }
                    string message = Translate.Text("Unauthorized Access");
                    actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);

                }
                //Log.Info("EnsureAuthenticationIsSet checked -" + EnsureAuthenticationIsSet, this);
                base.OnAuthorization(actionContext);
            }
            catch (Exception ex)
            {
                 LogService.Error(ex, this);
            }
        }
        public Task OnAuthorizationAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(context.Result);
        }

        #region GetUser
        public static User GetUser(string domainName, string userName, string password)
        {
            try
            {
                if (!System.Web.Security.Membership.ValidateUser(domainName + @"\" + userName, password))
                    return null;
                if (User.Exists(domainName + @"\" + userName))
                    return User.FromName(domainName + @"\" + userName, true);
                return null;
            }
            catch (Exception ex)
            {
                LogService.Error(ex, string.Empty);
                return null;
            }
        }
        #endregion
    }
}