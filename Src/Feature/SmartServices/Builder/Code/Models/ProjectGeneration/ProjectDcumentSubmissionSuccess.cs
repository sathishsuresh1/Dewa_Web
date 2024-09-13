using DEWAXP.Foundation.Content.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace DEWAXP.Feature.Builder.Models.ProjectGeneration
{
    public class ProjectDcumentSubmissionSuccessModel : GenericPageWithIntro
	{

		public string FileObjectID { get; set; }
		public string ProjectName { get; set; }

		
		public string Subject { get; set; }

        
		public string DocumentReference { get; set; }
		
		public DateTime ReferenceDate { get; set; }

        public byte[] Documentation { get; set; }

        public string AttachmentType { get; set; }

        public List<string> FileName { get; set; }

        
	}
}