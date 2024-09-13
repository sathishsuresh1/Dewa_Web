namespace DEWAXP.Feature.HR.Helpers.CareerPortal
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Content.Repositories;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Web;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using SitecoreX = global::Sitecore.Context;

    public static class CareerPortalHelper
    {
        public static string CandidateAttachmentType = "0022";
        public static string CVAttachmentType = "0012";
        public static string PassportAttachmentType = "0016";
        public static string EmirateIdAttachmentType = "0032";
        public static string HighestEducationalAttachmentType = "0001";
        public static string CertificatesId = "40000008";
        public static string PersonalInformation = "1";
        public static string AddressContactInformation = "2";
        public static string EducationQualificationsCertifications = "3";
        public static string WorkExperience = "4";
        public static string Attachments = "5";
        public static string ReviewSubmit = "6";
        public static string Preferences = "7";
        public static string CoverLetter = "8";
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Database)));
        /// <summary>
        /// The GetPageSize
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/></returns>
        public static List<SelectListItem> GetPageSize()
        {
            
        List<SelectListItem> lists = null;
            try
            {
                var pagesizeitem = SitecoreX.Database.GetItem(DataSources.CAREER_PORTAL_PageSize);
                if (pagesizeitem != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(pagesizeitem));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (System.Exception)
            {
                lists = new List<SelectListItem>
                {
                    new SelectListItem { Text = "5", Value = "5" },
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "20", Value = "20" }
                };
            }

            return lists;
        }

        /// <summary>
        /// Get gender
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetGender()
        {
            List<SelectListItem> lists = null;
            try
            {
                var genderItmes = SitecoreX.Database.GetItem(DataSources.CAREER_PORTAL_Gender);
                if (genderItmes != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(genderItmes));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception)
            {
                return lists;
            }
            return lists;
        }

        /// <summary>
        /// Get Marital Status
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetMaritalStatus()
        {
            List<SelectListItem> lists = null;
            try
            {
                var maritalStatusItmes = SitecoreX.Database.GetItem(DataSources.CAREER_PORTAL_MaritalStatus);
                if (maritalStatusItmes != null)
                {
                    ListDataSources mylistdatasource = _contentRepository.GetItem<ListDataSources>(new GetItemByItemOptions(maritalStatusItmes));
                    IEnumerable<SelectListItem> convertedItems = mylistdatasource.Items.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
                    return convertedItems.ToList();
                }
            }
            catch (Exception)
            {
                return lists;
            }
            return lists;
        }
    }
}