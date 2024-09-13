using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Feature.CommonComponents.Models.Persona;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.ORM.Models;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
    public class Masthead
    {
        public Masthead(Home home, IEnumerable<MainLanding> mainLandings, ContentBase activeLanding, Renderings.Header header, string username, MyAccountMenu myAccountItem, ProfilePhotoModel currentUserProfilePhoto, ContentBase activeSubLanding, string currentUserRole)
        {
            ActiveLanding = activeLanding;
            ActiveSubLanding = activeSubLanding;
            Header = header;
            MainLandings = mainLandings;
            Home = home;
            UserName = username;
            MyAccountItem = myAccountItem;
            CurrentUserProfilePhoto = currentUserProfilePhoto;
            CurrentUserRole = currentUserRole;
            CurrentItem = global::Sitecore.Context.Item;
        }

        public Masthead(Renderings.Header header) => Header = header;

        public Masthead(Home home, Renderings.Header header)
        {
            Header = header;
            Home = home;
        }

        /// <summary>
        /// New masthead contructor with new revamp 2019 masthheadv1 property
        /// </summary>
        /// <param name="home"></param>
        /// <param name="mainLandings"></param>
        /// <param name="activeLanding"></param>
        /// <param name="mastheadv1"></param>
        /// <param name="username"></param>
        /// <param name="myAccountItem"></param>
        /// <param name="currentUserProfilePhoto"></param>
        /// <param name="activeSubLanding"></param>
        /// <param name="currentUserRole"></param>
        public Masthead(Home home, IEnumerable<MainLanding> mainLandings, ContentBase activeLanding, Renderings.MastheadItem mastheadv1, string username, MyAccountMenu myAccountItem, ProfilePhotoModel currentUserProfilePhoto, ContentBase activeSubLanding, string currentUserRole)
        {
            ActiveLanding = activeLanding;
            ActiveSubLanding = activeSubLanding;
            Mastheadv1 = mastheadv1;
            MainLandings = mainLandings;
            Home = home;
            UserName = username;
            MyAccountItem = myAccountItem;
            CurrentUserProfilePhoto = currentUserProfilePhoto;
            CurrentUserRole = currentUserRole;
            CurrentItem = global::Sitecore.Context.Item;
        }

        public Masthead(Home home, Renderings.MastheadItem mastheadv1, string username, MyAccountMenu myAccountItem, ProfilePhotoModel currentUserProfilePhoto, string currentUserRole)
        {
            //ActiveLanding = activeLanding;
            //ActiveSubLanding = activeSubLanding;
            Mastheadv1 = mastheadv1;
            //MainLandings = mainLandings;
            Home = home;
            UserName = username;
            MyAccountItem = myAccountItem;
            CurrentUserProfilePhoto = currentUserProfilePhoto;
            CurrentUserRole = currentUserRole;
            CurrentItem = global::Sitecore.Context.Item;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="home"></param>
        /// <param name="personaMainLandings"></param>
        /// <param name="activeLanding"></param>
        /// <param name="mastheadv1"></param>
        /// <param name="username"></param>
        /// <param name="myAccountItem"></param>
        /// <param name="currentUserProfilePhoto"></param>
        /// <param name="activeSubLanding"></param>
        /// <param name="currentUserRole"></param>
        public Masthead(Home home, IEnumerable<PersonaLanding> personaMainLandings, ContentBase activeLanding, Renderings.MastheadItem mastheadv1, string username, MyAccountMenu myAccountItem, ProfilePhotoModel currentUserProfilePhoto, string currentUserRole)
        {
            ActiveLanding = activeLanding;
            //ActiveSubLanding = activeSubLanding;
            Mastheadv1 = mastheadv1;
            PersonaMainLandings = personaMainLandings;
            Home = home;
            UserName = username;
            MyAccountItem = myAccountItem;
            CurrentUserProfilePhoto = currentUserProfilePhoto;
            CurrentUserRole = currentUserRole;
            CurrentItem = global::Sitecore.Context.Item;
        }

        public virtual Home Home { get; private set; }
        public virtual Renderings.Header Header { get; private set; }
        public virtual Renderings.MastheadItem Mastheadv1 { get; private set; }
        public virtual ContentBase ActiveLanding { get; private set; }
        public virtual ContentBase ActiveSubLanding { get; private set; }
        public virtual IEnumerable<MainLanding> MainLandings { get; private set; }
        public virtual IEnumerable<PersonaLanding> PersonaMainLandings { get; private set; }
        public virtual string UserName { get; private set; }
        public virtual MyAccountMenu MyAccountItem { get; private set; }
        public virtual ProfilePhotoModel CurrentUserProfilePhoto { get; set; }
        public virtual string CurrentUserRole { get; private set; }
        public virtual DewaProfile CustomerProfile { get; set; }
        public virtual Item CurrentItem { get; set; }
    }
}