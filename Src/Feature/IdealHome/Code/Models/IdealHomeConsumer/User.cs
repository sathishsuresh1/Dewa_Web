using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using X.PagedList;
using Sitecore.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using DEWAXP.Foundation.DataAnnotations;
using CompareAttribute = DEWAXP.Foundation.DataAnnotations.CompareAttribute;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{
    [SitecoreType(BranchId = "{59FFCAA5-E73E-4CD9-A248-E35F34C176DC}")]
    public class User : GlassBase
    {

        [SitecoreField(FieldName = "IsDEWACustomer")]
        public virtual bool IsDEWACustomer { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [RegularExpression("^(?=.*\\d)(?=.*[\\D])[0-9\\D]{8,}$", ValidationMessageKey = "login password validation message alphanumeric")]
        [SitecoreField(FieldName = "Password")]
        public virtual string Password { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public virtual string ConfirmPassword { get; set; }

        [SitecoreField(FieldName = "Contract Account")]
        public virtual string account_number { get; set; }

        [SitecoreField(FieldName = "Account Owner Name")]
        public virtual string owner_name { get; set; }

        [SitecoreField(FieldName = "Name")]
        public virtual string FullName { get; set; }

        [SitecoreField(FieldName = "Mobile")]
        public virtual string Mobile { get; set; }

        [SitecoreField(FieldName = "Email Address")]
        public virtual string EmailAddress { get; set; }

        [SitecoreField(FieldName = "Nationality")]
        public virtual string Nationality { get; set; }

        [SitecoreField(FieldName = "Type of Residence")]
        public virtual string TypeofResidence { get; set; }

        [SitecoreField(FieldName = "Number of Residence")]
        public virtual string NumberofResidence { get; set; }

        [SitecoreField(FieldName = "Title")]
        public virtual string Title { get; set; }

        public virtual List<SelectListItem> TitleList { get; set; }

        public virtual VideoList Videos { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<SurveyResponse> SurveyResponses { get; set; }

        [SitecoreChildren(InferType = true)]
        public virtual IEnumerable<VideoResponse> VideoResponses { get; set; }

        [SitecoreField(FieldName = "IsAdmin")]
        public virtual bool IsAdmin { get; set; }
        [SitecoreField(FieldName = "ResetPasswordDate")]
        public virtual string ResetPasswordDate { get; set; }
    }

    public class SurveyUsers : ContentBase
    {
        [SitecoreChildren]
        public virtual IEnumerable<User> Users { get; set; }

    }

    public class DisplayResult
    {
        public User user { get; set; }
        public SectionList sectionlist { get; set; }
    }

    public class UserViewModel
    {
        public int PageNo { get; set; }
        public int Total { get; set; }
        public IPagedList<User> ITEMPagedList { get; set; }
    }
}