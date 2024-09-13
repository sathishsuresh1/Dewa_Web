using DEWAXP.Foundation.Content.Models.Common;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "Sector,InstitutionDetails,InstitutionFacultyDetail,NominatedTeamMembers")]
    public class FormProjectModel
    {
        public FormProjectModel()
        {
            SectorList = new ListDataSources();
            InstitutionDetails = new InstitutionDetails();
            InstitutionFacultyDetail = new OtherDetails();
            NominatedTeamMembers = new NominatedPersons();
        }

        public string Sector { get; set; }
        public ListDataSources SectorList { get; set; }

        //Name of institution
        //Land Line
        //Email
        public InstitutionDetails InstitutionDetails { get; set; }

        //Name
        //Mobile
        //Email
        public OtherDetails InstitutionFacultyDetail { get; set; }

        //Nominated Team member's names (maximum of 3 students)

        public NominatedPersons NominatedTeamMembers { get; set; }
    }
}