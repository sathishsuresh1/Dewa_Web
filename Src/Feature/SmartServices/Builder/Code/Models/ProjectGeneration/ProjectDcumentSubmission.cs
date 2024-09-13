using DEWAXP.Foundation.Content.Models.Common;
using Glass.Mapper.Sc.Configuration.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;


namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    public class ProjectDcumentSubmissionModel : GenericPageWithIntro
	{
        public string ProjectId { get; set; }

		public string Subject { get; set; }

        public string DocumentReference { get; set; }
		
		public string StrReferenceDate { get; set; }
		public DateTime ReferenceDate {
            get
            {
                CultureInfo culture;
                DateTimeStyles styles;
                culture = Sitecore.Context.Culture;
                if (!string.IsNullOrWhiteSpace(StrReferenceDate))
                {
                    StrReferenceDate = StrReferenceDate.Replace("يناير", "Jan").Replace("فبراير", "Feb").Replace("مارس", "Mar").Replace("أبريل", "Apr").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "Aug").Replace("سبتمبر", "Sept").Replace("أكتوبر", "Oct").Replace("نوفمبر", "Nov").Replace("ديسمبر", "Dec").Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    DateTime dateTime;
                    styles = DateTimeStyles.None;
                    if (DateTime.TryParse(StrReferenceDate, culture, styles, out dateTime))
                    {
                        return dateTime;
                    }
                }
                return DateTime.MinValue;
            }
            set { }
        }


        public byte[] Documentation { get; set; }

        public string AttachmentType { get; set; }

        public string FileName { get; set; }

        public IEnumerable<Project> Projects { get; set; }
	}
}