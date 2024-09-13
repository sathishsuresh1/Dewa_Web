using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.Responses.VillaCostExemption
{
    public class Attachmenttype
    {
        [JsonProperty("attachmentdescription")]
        public string Description { get; set; }

        [JsonProperty("attachmenttype")]
        public string Type { get; set; }

        [JsonProperty("ownertype")]
        public string Ownertype { get; set; }

        [JsonProperty("requiredflag")]
        public string Requiredflag { get; set; }
    }

    public class History
    {
        [JsonProperty("hisapplicationnumber")]
        public string Hisapplicationnumber { get; set; }

        [JsonProperty("hisapplicationreferencenumber")]
        public string Hisapplicationreferencenumber { get; set; }

        [JsonProperty("hisapplicationsequencenumber")]
        public string HisapplicationSequencenumber { get; set; }

        [JsonProperty("hisapplicationstatus")]
        public string Hisapplicationstatus { get; set; }

        [JsonProperty("hisapplicationstatusdesc")]
        public string Hisapplicationstatusdesc { get; set; }

        [JsonProperty("hiscustomernumber")]
        public string Hiscustomernumber { get; set; }

        [JsonProperty("hisdateofrecordcreation")]
        public string Hisdateofrecordcreation { get; set; }

        [JsonProperty("hisestimate")]
        public string Hisestimate { get; set; }

        [JsonProperty("hisnotificationnumber")]
        public string Hisnotificationnumber { get; set; }

        [JsonProperty("hisownertype")]
        public string Hisownertype { get; set; }

        [JsonProperty("hisownertypedescription")]
        public string Hisownertypedescription { get; set; }

        [JsonProperty("hisremarks")]
        public string Hisremarks { get; set; }

        [JsonProperty("histimeofrecordcreation")]
        public string Histimeofrecordcreation { get; set; }

        [JsonProperty("hisversion")]
        public string Hisversion { get; set; }
    }

    public class Ownerdetail
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emiratesid")]
        public string Emiratesid { get; set; }

        [JsonProperty("idtype")]
        public string IdType { get; set; }

        [JsonProperty("itemnumber")]
        public string Itemnumber { get; set; }

        [JsonProperty("marsoom")]
        public string Marsoom { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("mobile2")]
        public string Mobile2 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ownerapplicationreferencenumber")]
        public string Ownerapplicationreferencenumber { get; set; }

        [JsonProperty("passport")]
        public string Passport { get; set; }

        [JsonProperty("passportexpiry")]
        public string Passportexpiry { get; set; }

        [JsonProperty("passportissue_authority")]
        public string PassportissueAuthority { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("dob")]
        public string DateOfBirth { get; set; }

        [JsonProperty("nationlaity")]
        public string Nationlaity { get; set; }
    }

    public class Ownerattachment
    {
        [JsonProperty("dateofrecordcreation")]
        public string Dateofrecordcreation { get; set; }

        [JsonProperty("documentnumber")]
        public string Documentnumber { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("ownerapplicationitemnumber")]
        public string Ownerapplicationitemnumber { get; set; }

        [JsonProperty("ownerapplicationreferencenumber")]
        public string Ownerapplicationreferencenumber { get; set; }

        [JsonProperty("requiredflag")]
        public string Requiredflag { get; set; }

        [JsonProperty("timeofrecordcreation")]
        public string Timeofrecordcreation { get; set; }

        [JsonProperty("uploadedflag")]
        public string Uploadedflag { get; set; }
    }

    public class Customerdetail
    {
        [JsonProperty("applicationnumber")]
        public string Applicationnumber { get; set; }

        [JsonProperty("applicationreferencenumber")]
        public string Applicationreferencenumber { get; set; }

        [JsonProperty("applicationsequencenumber")]
        public string ApplicationSequencenumber { get; set; }

        [JsonProperty("applicationstatus")]
        public string Applicationstatus { get; set; }

        [JsonProperty("applicationstatusdesc")]
        public string Applicationstatusdesc { get; set; }

        [JsonProperty("customernumber")]
        public string Customernumber { get; set; }

        [JsonProperty("dateofrecordcreation")]
        public string Dateofrecordcreation { get; set; }

        [JsonProperty("estimate")]
        public string Estimate { get; set; }

        [JsonProperty("history")]
        public List<History> History { get; set; }

        [JsonProperty("notificationnumber")]
        public string Notificationnumber { get; set; }

        [JsonProperty("ownerattachments")]
        public List<Ownerattachment> Ownerattachments { get; set; }

        [JsonProperty("ownerdetails")]
        public List<Ownerdetail> Ownerdetails { get; set; }

        [JsonProperty("ownertype")]
        public string Ownertype { get; set; }

        [JsonProperty("ownertypedescription")]
        public string Ownertypedescription { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("timeofrecordcreation")]
        public string Timeofrecordcreation { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

    }

    public class Ownertype : KeyValueBase
    {

    }

    public class Statustype : KeyValueBase
    {

    }

    public class DashboardResponse
    {
        [JsonProperty("attachmenttypes")]
        public List<Attachmenttype> Attachmenttypes { get; set; }

        [JsonProperty("customerdetails")]
        public List<Customerdetail> Customerdetails { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("ownertypes")]
        public List<Ownertype> Ownertypes { get; set; }

        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        [JsonProperty("statustypes")]
        public List<Statustype> Statustypes { get; set; }
    }

    public class KeyValueBase
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
