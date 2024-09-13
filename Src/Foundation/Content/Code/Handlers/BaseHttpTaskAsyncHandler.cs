using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Integration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace DEWAXP.Foundation.Content.Handlers
{
    public abstract class BaseHttpTaskAsyncHandler : HttpTaskAsyncHandler, IReadOnlySessionState
    {
        protected IDewaServiceClient DewaApiClient
        {
            get { return DependencyResolver.Current.GetService<IDewaServiceClient>(); }
        }

        protected ICacheProvider CacheProvider
        {
            get { return DependencyResolver.Current.GetService<ICacheProvider>(); }
        }

        protected DewaProfile CurrentPrincipal { get; private set; }

        protected ProfilePhotoModel CurrentUserProfilePhoto
        {
            get
            {
                ProfilePhotoModel photo;
                if (CacheProvider.TryGet(CacheKeys.PROFILE_PHOTO, out photo))
                {
                    return photo;
                }
                return new ProfilePhotoModel() { HasProfilePhoto = false, ProfilePhoto = new byte[0] }; ;
            }
        }

        public override Task ProcessRequestAsync(HttpContext context)
        {
            if (CurrentPrincipal == null || CurrentPrincipal.Equals(DewaProfile.Null))
            {
                CurrentPrincipal = (context.Session["__DEWAPROFILE__"] as DewaProfile) ?? DewaProfile.Null;
            }
            return ProcessInternal(context);
        }

        protected abstract Task ProcessInternal(HttpContext context);
    }
}