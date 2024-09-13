using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "Sector,AcademicLevel,InstitutionDetails,TeamLeaderDetails,TeamMembers,SelectedAcademics,Tracker,conservationFormParameters,conservationTechnicalParameter,conservationAwarenessParameter,AttachedDocument")]
    public class FormTeamModel
    {
        public FormTeamModel()
        {
            InstitutionDetails = new InstitutionDetails();
            TeamLeaderDetails = new OtherDetails();
            TeamMembers = new NominatedPersons();
            SectorList = new ListDataSources();
            AcademicList = new List<CheckBoxListItem>();
            SelectedAcademics = new List<string>();
            conservationFormParameters = new ConservationFormParameters();
            conservationTechnicalParameter = new ConservationTechnicalParameter();
            conservationAwarenessParameter = new ConservationAwarenessParameter();
        }

        public string Sector { get; set; }
        public List<string> SelectedAcademics { get; set; }
        public string AcademicLevel { get; set; }

        //Name of institution
        //Land Line
        //Email
        public InstitutionDetails InstitutionDetails { get; set; }

        //Name
        //Mobile
        //Email
        public OtherDetails TeamLeaderDetails { get; set; }

        //1
        //2
        //3
        //4
        //5
        public NominatedPersons TeamMembers { get; set; }

        public ListDataSources SectorList { get; internal set; }
        public List<CheckBoxListItem> AcademicList { get; internal set; }

        public ConservationFormParameters conservationFormParameters { get; set; }
        public ConservationTechnicalParameter conservationTechnicalParameter { get; set; }
        public ConservationAwarenessParameter conservationAwarenessParameter { get; set; }
        public string Tracker { get; set; }

        [MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument { get; set; }

        public byte[] AttachmentFileBinary { get; set; }
        public string AttachmentFileType { get; set; }
    }
}