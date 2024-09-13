using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.DataAnnotations;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "Sector,InstitutionDetails,HeadmasterOrHeadmistressDetails,AwardCoordinatorDetails,SelectedAcademics,MethodofEducation,Tracker,conservationTechnicalParameter,conservationAwarenessParameter,AttachedDocument")]
    public class FormEducationalInstitutionModel
    {
        public FormEducationalInstitutionModel()
        {
            HeadmasterOrHeadmistressDetails = new OtherDetails();
            InstitutionDetails = new InstitutionDetails();
            AwardCoordinatorDetails = new CoordinatorDetails();
            SectorList = new ListDataSources();
            AcademicList = new List<CheckBoxListItem>();
            SelectedAcademics = new List<string>();
            MethodofEducation = new List<string>();
            conservationFormParameters = new ConservationFormParameters();
            conservationTechnicalParameter = new ConservationTechnicalParameter();
            conservationAwarenessParameter = new ConservationAwarenessParameter();
        }

        public string Sector { get; set; }

        //Name
        //First telephone no.: dialling code+number
        //E-Mail Address
        //Total No.of Faculty
        //Total No.of Students
        //Serial Number
        //Serial Number
        public InstitutionDetails InstitutionDetails { get; set; }

        //Full Name of Person
        //First Cell Phone Number: Dialing Code + Number
        //E-Mail Address
        public OtherDetails HeadmasterOrHeadmistressDetails { get; set; }

        public CoordinatorDetails AwardCoordinatorDetails { get; set; }
        public ListDataSources SectorList { get; internal set; }
        public List<CheckBoxListItem> AcademicList { get; internal set; }
        public List<string> SelectedAcademics { get; set; }
        public List<string> MethodofEducation { get; set; }

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