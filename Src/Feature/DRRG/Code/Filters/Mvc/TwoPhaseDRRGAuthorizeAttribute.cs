// <copyright file="TwoPhaseDRRGAuthorizeAttribute.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Filters.Mvc
{
    using DEWAXP.Foundation.CustomDB.DRRGDataModel;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Feature.DRRG.Models;
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using DEWAXP.Foundation.Content.Services;
    using DEWARoles = Foundation.Content.Roles;
    /// <summary>
    /// Defines the <see cref="TwoPhaseDRRGAuthorizeAttribute" />.
    /// </summary>
    public class TwoPhaseDRRGAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Defines the AUTHORIZATION_FLAG.
        /// </summary>
        private const string AUTHORIZATION_FLAG = "DewaAuthorized";

        /// <summary>
        /// Defines the _authService.
        /// </summary>
        private readonly IDewaAuthStateService _authService;

        /// <summary>
        /// Gets or sets a value indicating whether a primary account verification check should be performed as part of the authorization routine.
        /// When true, if no primary account has been set, the user will be redirected to the relevant workflow.
        /// The default value is true.
        /// </summary>
        public bool NotEvaluated { get; set; } = false;
        public bool EvaluatedPending { get; set; } = false;
        public bool EvaluatedRejected { get; set; } = false;

        public string OutStatus { get; set; }
        public string CurrentDRRGRole { get; set; } = DEWARoles.DRRG;
        //public bool LoginPage { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoPhaseDRRGAuthorizeAttribute"/> class.
        /// </summary>
        public TwoPhaseDRRGAuthorizeAttribute()
        {
            _authService = DependencyResolver.Current.GetService<IDewaAuthStateService>();

            if (string.IsNullOrWhiteSpace(Roles))
            {
                Roles = DEWARoles.DRRG+","+ DEWARoles.DRRGEvaulator + "," + DEWARoles.DRRGSchemaManager;
            }
        }

        /// <summary>
        /// The AuthorizeCore.
        /// </summary>
        /// <param name="httpContext">The httpContext<see cref="HttpContextBase"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            httpContext.Items[AUTHORIZATION_FLAG] = false;

            if (httpContext.Request.IsAuthenticated)
            {
                DewaProfile profile = _authService.GetActiveProfile();
                if (profile == null || profile.Equals(DewaProfile.Null))
                {
                    return false;
                }

                if (httpContext.Session[GenericConstants.AntiHijackCookieName] != null)
                {
                    HttpCookie antiHijackCookie = httpContext.Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                    string storedToken = httpContext.Session[GenericConstants.AntiHijackCookieName].ToString();
                    if (antiHijackCookie == null || string.IsNullOrWhiteSpace(storedToken))
                    {
                        return false;
                    }

                    if (string.Equals(storedToken, antiHijackCookie.Value))
                    {
                        if (!PrincipalIsAuthorized(profile.Role))
                        {
                            return false;
                        }
                        httpContext.Items[AUTHORIZATION_FLAG] = true;
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            if (CurrentDRRGRole.Equals(DEWARoles.DRRG))
                            {

                                ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                ObjectParameter statusParamresponse = new ObjectParameter(DRRGStandardValues.status, typeof(string));
                                context.SP_DRRG_CheckSession(profile.Username, profile.SessionToken, profile.BusinessPartner, statusParamresponse, myOutputParamresponse);
                                string myString = Convert.ToString(myOutputParamresponse.Value);
                                OutStatus = Convert.ToString(statusParamresponse.Value);
                                if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success) &&
                                   !string.IsNullOrWhiteSpace(OutStatus) && ((NotEvaluated && (OutStatus.Equals(DRRGStandardValues.Submitted) || OutStatus.Equals(DRRGStandardValues.Updated)))
                                   || (EvaluatedPending && (OutStatus.Equals(DRRGStandardValues.AuthorizedLetterSubmitted) || OutStatus.Equals(DRRGStandardValues.AuthorizedLetterUpdated))) ||
                                  (!EvaluatedPending &&!EvaluatedRejected&&!NotEvaluated&&  OutStatus.Equals(DRRGStandardValues.Approved) ) || (EvaluatedRejected && OutStatus.Equals(DRRGStandardValues.Rejected))))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                        }
                          else
                          {
                            string[] requiredcurrentRoles = CurrentDRRGRole.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(role => role.Trim())
                            .ToArray();

                            foreach (var role in requiredcurrentRoles)
                            {
                                bool isrole = profile.Role.Equals(role);
                                if (isrole)
                                {
                                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                    ObjectParameter statusParamresponse = new ObjectParameter(DRRGStandardValues.status, typeof(string));
                                    context.SP_DRRG_CheckAdminSession(profile.Username, profile.SessionToken, myOutputParamresponse);
                                    string myString = Convert.ToString(myOutputParamresponse.Value);
                                    OutStatus = Convert.ToString(statusParamresponse.Value);
                                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// The OnAuthorization.
        /// </summary>
        /// <param name="filterContext">The filterContext<see cref="AuthorizationContext"/>.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext.Items.Contains(AUTHORIZATION_FLAG))
            {
                bool authenticated = filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated;
                bool authorized = (bool)filterContext.HttpContext.Items[AUTHORIZATION_FLAG];
                string[] requiredRoles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(role => role.Trim())
                    .ToArray();

                if (!authenticated || !authorized)
                {
                    string returnUrl = filterContext.HttpContext.Request.Url != null ?
                        filterContext.HttpContext.Request.Url.PathAndQuery : string.Empty;

                    string redirect = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_HOME);
                    if (!CurrentDRRGRole.Equals(DEWARoles.DRRG))
                    {
                        redirect = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG__EVALUATOR_LOGIN);
                    }
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        redirect += string.Format("?returnUrl={0}", HttpUtility.UrlEncode(returnUrl));
                    }
                    filterContext.Result = new RedirectResult(redirect);
                }
            }
        }

        /// <summary>
        /// The PrincipalIsAuthorized.
        /// </summary>
        /// <param name="principalRole">The principalRole<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool PrincipalIsAuthorized(string principalRole)
        {
            if (string.IsNullOrWhiteSpace(Roles))
            {
                return true;
            }

            System.Collections.Generic.IEnumerable<string> requiredRoles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(role => role.Trim());

            return requiredRoles.Contains(principalRole);
        }

        /// <summary>
        /// The HandleUnauthorizedRequest.
        /// </summary>
        /// <param name="filterContext">The filterContext<see cref="AuthorizationContext"/>.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!CurrentDRRGRole.Equals(DEWARoles.DRRG))
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG__EVALUATOR_LOGIN));
            }
            if (!string.IsNullOrWhiteSpace(OutStatus)&& OutStatus.Equals(DRRGStandardValues.Submitted))
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_AUTHORIZED_LETTER));
            }
            else if (!string.IsNullOrWhiteSpace(OutStatus) && (OutStatus.Equals(DRRGStandardValues.AuthorizedLetterSubmitted) || OutStatus.Equals(DRRGStandardValues.AuthorizedLetterUpdated)))
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_PENDING_EVALUATION));
            }
            else if (!string.IsNullOrWhiteSpace(OutStatus) && OutStatus.Equals(DRRGStandardValues.Rejected))
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_REJECTED));
            }
            else
            {
                filterContext.Result = new RedirectResult(LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_HOME));
            }
        }
    }
}
