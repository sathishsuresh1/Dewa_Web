using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration;
using System;
using System.Web;
using System.Web.Security;

namespace DEWAXP.Foundation.Content.Services
{
    [Service(typeof(IDewaAuthStateService), Lifetime = Lifetime.Transient)]
    public class FormsBasedDewaAuthStateService : IDewaAuthStateService
    {
        private ICacheProvider _cacheProvider;
        private DewaProfile CurrentPrincipal;
        private IDewaServiceClient DewaApiClient;
        private IContextRepository _contextRepository;

        public FormsBasedDewaAuthStateService(ICacheProvider cacheProvider, DewaProfile currentPrincipal, IDewaServiceClient dewaServiceClient, IContextRepository contextRepository)
        {
            _cacheProvider = cacheProvider;
            CurrentPrincipal = currentPrincipal;
            DewaApiClient = dewaServiceClient;
            _contextRepository = contextRepository;
        }

        public void Save(DewaProfile profile)
        {
            if (profile != null)
            {
                UpdateAuthTicket(profile);
                var cookie = HttpContext.Current.Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                var storedToken = HttpContext.Current.Session[GenericConstants.AntiHijackCookieName];
                if (cookie == null || storedToken == null)
                {
                    SetAntiHijackCookie(Guid.NewGuid().ToString("N"));
                }
            }
        }

        public DewaProfile GetActiveProfile()
        {
            return (HttpContext.Current.Session[GenericConstants.PROFILE_SESSION_KEY] as DewaProfile) ?? DewaProfile.Null;
        }

        private void UpdateAuthTicket(DewaProfile profile)
        {
            FormsAuthentication.SetAuthCookie(profile.Username, false);
            HttpContext.Current.Session[GenericConstants.PROFILE_SESSION_KEY] = profile;
        }

        private void SetAntiHijackCookie(string token)
        {
            HttpContext.Current.Response.SetCookie(GetOrCreateAntiHijackCookie(HttpContext.Current, token));
            HttpContext.Current.Session[GenericConstants.AntiHijackCookieName] = token;
        }

        private HttpCookie GetOrCreateAntiHijackCookie(HttpContext context, string token)
        {
            var cookie = context.Request.Cookies[GenericConstants.AntiHijackCookieName];
            if (cookie == null)
            {
                return new HttpCookie(GenericConstants.AntiHijackCookieName, token) { HttpOnly = true };
            }
            cookie.Value = token;
            return cookie;
        }
    }
}