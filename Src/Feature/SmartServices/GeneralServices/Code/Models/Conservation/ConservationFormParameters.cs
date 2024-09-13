using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    public class ConservationFormParameters
    {
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormTypeofInstitution { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormAcedamicLevel { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormAirconditioning { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormLighting { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormOfficeEquipment { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormWater { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormOther { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormNone { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormEnvironmentalActivites { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormLecturesWorkshops { get; set; }
        public IEnumerable<DEWAXP.Foundation.Integration.DewaSvc.clearanceMaster> CoFormMethodOfEducation { get; set; }
    }
}