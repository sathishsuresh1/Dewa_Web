using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Filters.Mvc
{
    public class TwoPhaseAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private const string AUTHORIZATION_FLAG = "DewaAuthorized";

        private readonly IDewaAuthStateService _authService;

        private bool _redirectToSetPrimaryAccount = false;
        private bool _ensurePrimaryAccountIsSet = true;
        private bool _redirectTermsAndConditions = true;
        private bool _allowevusers = false;

        /// <summary>
        /// Gets or sets a value indicating whether a primary account verification check should be performed as part of the authorization routine.
        /// When true, if no primary account has been set, the user will be redirected to the relevant workflow.
        /// The default value is true
        /// </summary>
        public bool EnsurePrimaryAccountIsSet
        {
            get { return _ensurePrimaryAccountIsSet; }
            set { _ensurePrimaryAccountIsSet = value; }
        }

        public bool AllowEVUsers
        {
            get { return _allowevusers; }
            set { _allowevusers = value; }
        }

        public TwoPhaseAuthorizeAttribute()
        {
            _authService = DependencyResolver.Current.GetService<IDewaAuthStateService>();

            if (string.IsNullOrWhiteSpace(Roles))
            {
                Roles = DEWAXP.Foundation.Content.Roles.User;
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            httpContext.Items[AUTHORIZATION_FLAG] = false;

            if (httpContext.Request.IsAuthenticated)
            {
                var profile = _authService.GetActiveProfile();
                if (profile == null || profile.Equals(DewaProfile.Null)) return false;

                if (httpContext.Session[GenericConstants.AntiHijackCookieName] != null)
                {
                    var antiHijackCookie = httpContext.Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                    var storedToken = httpContext.Session[GenericConstants.AntiHijackCookieName].ToString();
                    if (antiHijackCookie == null || string.IsNullOrWhiteSpace(storedToken)) return false;

                    if (string.Equals(storedToken, antiHijackCookie.Value))
                    {
                        if (!PrincipalIsAuthorized(profile.Role))
                        {
                            return false;
                        }

                        httpContext.Items[AUTHORIZATION_FLAG] = true;

                        bool byPass = (profile.Role == DEWAXP.Foundation.Content.Roles.ScrapeSale || profile.Role == DEWAXP.Foundation.Content.Roles.Miscellaneous);

                        if (byPass)
                        {
                            return true;
                        }
                        if (!profile.AcceptedTerms)
                        {
                            _redirectTermsAndConditions = false;
                            return _redirectTermsAndConditions;
                        }
                        else
                            _redirectTermsAndConditions = true;

                        if (EnsurePrimaryAccountIsSet)
                        {
                            _redirectToSetPrimaryAccount = !profile.HasPrimaryAccount && profile.HasActiveAccounts;
                            if (AllowEVUsers)
                            {
                                _redirectToSetPrimaryAccount = profile.IsEVUser ? !profile.IsEVUser : (!profile.HasPrimaryAccount && profile.HasActiveAccounts);
                                return true;
                            }
                            if (profile.HasActiveAccounts)
                            {
                                return profile.HasPrimaryAccount;
                            }
                            return true;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.Items.Contains(AUTHORIZATION_FLAG))
            {
                var authenticated = filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated;
                var authorized = (bool)filterContext.HttpContext.Items[AUTHORIZATION_FLAG];
                var requiredRoles = this.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(role => role.Trim())
                    .ToArray();

                if (!authenticated || !authorized)
                {
                    var returnUrl = filterContext.HttpContext.Request.Url != null ?
                        filterContext.HttpContext.Request.Url.PathAndQuery : string.Empty;

                    var redirect = !requiredRoles.Any() || requiredRoles.Contains(DEWAXP.Foundation.Content.Roles.User)
                        ? LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE) : LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J8_GOVT_LOGIN_PAGE);

                    if (requiredRoles.Contains(DEWAXP.Foundation.Content.Roles.ScrapeSale) || requiredRoles.Contains(DEWAXP.Foundation.Content.Roles.Miscellaneous))
                    {
                        redirect = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.MISCELLANEOUS_LOGIN);
                    }

                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        redirect += string.Format("?returnUrl={0}", HttpUtility.UrlEncode(returnUrl));
                    }

                    filterContext.Result = new RedirectResult(redirect);
                }
            }
        }

        private bool PrincipalIsAuthorized(string principalRole)
        {
            if (string.IsNullOrWhiteSpace(this.Roles)) return true;

            var requiredRoles = this.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(role => role.Trim());

            return requiredRoles.Contains(principalRole);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!_redirectTermsAndConditions)
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.TERMS_AND_CONDITIONS));
            }
            else if (EnsurePrimaryAccountIsSet && _redirectToSetPrimaryAccount)
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J71_SET_PRIMARY_ACCOUNT));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}