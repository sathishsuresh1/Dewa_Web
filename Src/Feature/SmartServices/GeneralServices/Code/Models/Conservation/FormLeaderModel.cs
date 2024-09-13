using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "Sector,InstitutionDetails,NominatedDistinguishedLeaderDetails,ConservationLeaderAssistanceDetails,NominatedAwardCoordinatorDetails,NominatedParentDetails,Tracker,conservationFormParameters,conservationTechnicalParameter,conservationAwarenessParameter,AttachedDocument")]
    public class FormLeaderModel
    {
        public FormLeaderModel()
        {
            InstitutionDetails = new InstitutionDetails();
            NominatedDistinguishedLeaderDetails = new OtherDetails();
            ConservationLeaderAssistanceDetails = new CoordinatorDetails();
            NominatedAwardCoordinatorDetails = new CoordinatorDetails();
            NominatedParentDetails = new CoordinatorDetails();
            SectorList = new ListDataSources();
            conservationFormParameters = new ConservationFormParameters();
            conservationTechnicalParameter = new ConservationTechnicalParameter();
            conservationAwarenessParameter = new ConservationAwarenessParameter();
        }

        public string Sector { get; set; }

        //Name of institution
        //Land Line
        //Email
        public InstitutionDetails InstitutionDetails { get; set; }

        //Name of institutions
        //Mobile
        //Email
        public OtherDetails NominatedDistinguishedLeaderDetails { get; set; }

        //Name
        //Mobile
        //Email
        public CoordinatorDetails ConservationLeaderAssistanceDetails { get; set; }

        //Name
        //Mobile
        //Email
        public CoordinatorDetails NominatedAwardCoordinatorDetails { get; set; }

        //Name
        //Mobile
        //Email
        public CoordinatorDetails NominatedParentDetails { get; set; }

        public ListDataSources SectorList { get; internal set; }

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