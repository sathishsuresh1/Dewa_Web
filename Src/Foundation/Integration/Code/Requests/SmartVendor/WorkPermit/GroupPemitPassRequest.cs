// <copyright file="GroupPemitPassRequest.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Requests.SmartVendor.WorkPermit
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Attachmentlist" />.
    /// </summary>
    public class Attachmentlist
    {
        /// <summary>
        /// Defines the Filedata.
        /// </summary>
        [JsonProperty("filedata")]
        public string filedata;

        /// <summary>
        /// Defines the Filename.
        /// </summary>
        [JsonProperty("filename")]
        public string filename;

        /// <summary>
        /// Defines the Folderid.
        /// </summary>
        [JsonProperty("folderid")]
        public string folderid;

        /// <summary>
        /// Defines the Mimetype.
        /// </summary>
        [JsonProperty("mimetype")]
        public string mimetype;
    }

    /// <summary>
    /// Defines the <see cref="Grouppasslocationlist" />.
    /// </summary>
    public class Grouppasslocationlist
    {
        /// <summary>
        /// Defines the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string grouppassid;

        /// <summary>
        /// Defines the Location.
        /// </summary>
        [JsonProperty("location")]
        public string location;

        /// <summary>
        /// Defines the Locationcode.
        /// </summary>
        [JsonProperty("locationcode")]
        public string locationcode;
    }

    /// <summary>
    /// Defines the <see cref="Projectcoordinatordetails" />.
    /// </summary>
    public class Projectcoordinatordetails
    {
        /// <summary>
        /// Defines the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string emailid;

        /// <summary>
        /// Defines the Employeeid.
        /// </summary>
        [JsonProperty("employeeid")]
        public string employeeid;

        /// <summary>
        /// Defines the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string fullname;

        /// <summary>
        /// Defines the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string grouppassid;

        /// <summary>
        /// Defines the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string mobile;

        /// <summary>
        /// Defines the Phonenumber.
        /// </summary>
        [JsonProperty("phonenumber")]
        public string phonenumber;
    }

    /// <summary>
    /// Defines the <see cref="Workpermitpassdetailsip" />.
    /// </summary>
    public class Workpermitpassdetailsip
    {
        /// <summary>
        /// Defines the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string countrykey;

        /// <summary>
        /// Defines the Drivinglicenseflag.
        /// </summary>
        [JsonProperty("drivinglicenseflag")]
        public string drivinglicenseflag;

        /// <summary>
        /// Defines the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string emailid;

        /// <summary>
        /// Defines the Emiratesid.
        /// </summary>
        [JsonProperty("emiratesid")]
        public string emiratesid;

        /// <summary>
        /// Defines the Emiratesidenddate.
        /// </summary>
        [JsonProperty("emiratesidenddate")]
        public string emiratesidenddate;

        /// <summary>
        /// Defines the Emiratesidflag.
        /// </summary>
        [JsonProperty("emiratesidflag")]
        public string emiratesidflag;

        /// <summary>
        /// Defines the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string fullname;

        /// <summary>
        /// Defines the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string grouppassid;

        /// <summary>
        /// Defines the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string mobile;

        /// <summary>
        /// Defines the Mulkiyanumber.
        /// </summary>
        [JsonProperty("mulkiyanumber")]
        public string mulkiyanumber;

        /// <summary>
        /// Defines the Passportenddate.
        /// </summary>
        [JsonProperty("passportenddate")]
        public string passportenddate;

        /// <summary>
        /// Defines the Passportflag.
        /// </summary>
        [JsonProperty("passportflag")]
        public string passportflag;

        /// <summary>
        /// Defines the Passportnumber.
        /// </summary>
        [JsonProperty("passportnumber")]
        public string passportnumber;

        /// <summary>
        /// Defines the Permitpass.
        /// </summary>
        [JsonProperty("permitpass")]
        public string permitpass;

        /// <summary>
        /// Defines the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string permitstatus;

        /// <summary>
        /// Defines the Photoflag.
        /// </summary>
        [JsonProperty("photoflag")]
        public string photoflag;

        /// <summary>
        /// Defines the Platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string platenumber;

        /// <summary>
        /// Defines the Profession.
        /// </summary>
        [JsonProperty("profession")]
        public string profession;

        /// <summary>
        /// Defines the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string remarks;

        /// <summary>
        /// Defines the Renewalreference.
        /// </summary>
        [JsonProperty("renewalreference")]
        public string renewalreference;

        /// <summary>
        /// Defines the Vehicleavailableflag.
        /// </summary>
        [JsonProperty("vehicleavailableflag")]
        public string vehicleavailableflag;

        /// <summary>
        /// Defines the Visaendate.
        /// </summary>
        [JsonProperty("visaendate")]
        public string visaendate;

        /// <summary>
        /// Defines the Visaflag.
        /// </summary>
        [JsonProperty("visaflag")]
        public string visaflag;

        /// <summary>
        /// Defines the Visanumber.
        /// </summary>
        [JsonProperty("visanumber")]
        public string visanumber;
    }

    /// <summary>
    /// Defines the <see cref="Workpermitpassrequestip" />.
    /// </summary>
    public class Workpermitpassrequestip
    {
        /// <summary>
        /// Defines the Cityname.
        /// </summary>
        [JsonProperty("cityname")]
        public string cityname;

        /// <summary>
        /// Defines the Companyaddress1.
        /// </summary>
        [JsonProperty("companyaddress1")]
        public string companyaddress1;

        /// <summary>
        /// Defines the Companyaddress2.
        /// </summary>
        [JsonProperty("companyaddress2")]
        public string companyaddress2;

        /// <summary>
        /// Defines the Companyname.
        /// </summary>
        [JsonProperty("companyname")]
        public string companyname;

        /// <summary>
        /// Defines the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string countrykey;

        /// <summary>
        /// Defines the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string emailid;

        /// <summary>
        /// Defines the Fromtime.
        /// </summary>
        [JsonProperty("fromtime")]
        public string fromtime;

        /// <summary>
        /// Defines the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string grouppassid;

        /// <summary>
        /// Defines the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang;

        /// <summary>
        /// Defines the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string mobile;

        /// <summary>
        /// Defines the Passexpirydate.
        /// </summary>
        [JsonProperty("passexpirydate")]
        public string passexpirydate;

        /// <summary>
        /// Defines the Passissuedate.
        /// </summary>
        [JsonProperty("passissuedate")]
        public string passissuedate;

        /// <summary>
        /// Defines the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string permitstatus;

        /// <summary>
        /// Defines the Permitsubreference.
        /// </summary>
        [JsonProperty("permitsubreference")]
        public string permitsubreference;

        /// <summary>
        /// Defines the Ponumber.
        /// </summary>
        [JsonProperty("ponumber")]
        public string ponumber;

        /// <summary>
        /// Defines the Projectname.
        /// </summary>
        [JsonProperty("projectname")]
        public string projectname;

        /// <summary>
        /// Defines the Purposeofvisit.
        /// </summary>
        [JsonProperty("purposeofvisit")]
        public string purposeofvisit;

        /// <summary>
        /// Defines the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string remarks;

        /// <summary>
        /// Defines the Renewalflag.
        /// </summary>
        [JsonProperty("renewalflag")]
        public string renewalflag;

        /// <summary>
        /// Defines the Renewalreference.
        /// </summary>
        [JsonProperty("renewalreference")]
        public string renewalreference;

        /// <summary>
        /// Defines the Totime.
        /// </summary>
        [JsonProperty("totime")]
        public string totime;

        /// <summary>
        /// Defines the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid;

        /// <summary>
        /// Defines the Vendornumber.
        /// </summary>
        [JsonProperty("vendornumber")]
        public string vendornumber;
    }

    /// <summary>
    /// Defines the <see cref="Grouppassinput" />.
    /// </summary>
    public class Grouppassinput
    {
        /// <summary>
        /// Defines the Appidentifier.
        /// </summary>
        [JsonProperty("appidentifier")]
        public string appidentifier;

        /// <summary>
        /// Defines the Appversion.
        /// </summary>
        [JsonProperty("appversion")]
        public string appversion;

        /// <summary>
        /// Defines the Attachmentlist.
        /// </summary>
        [JsonProperty("attachmentlist")]
        public Attachmentlist[] attachmentlist;


        /// <summary>
        /// Defines the Groupid.
        /// </summary>
        [JsonProperty("groupid")]
        public string groupid;

        /// <summary>
        /// Defines the Grouppasslocationlist.
        /// </summary>
        [JsonProperty("grouppasslocationlist")]
        public Grouppasslocationlist[] grouppasslocationlist;

        /// <summary>
        /// Defines the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string lang;

        /// <summary>
        /// Defines the Mobileosversion.
        /// </summary>
        [JsonProperty("mobileosversion")]
        public string mobileosversion;

        /// <summary>
        /// Defines the Permitpass.
        /// </summary>
        [JsonProperty("permitpass")]
        public string permitpass;

        /// <summary>
        /// Defines the Processcode.
        /// </summary>
        [JsonProperty("processcode")]
        public string processcode;

        /// <summary>
        /// Defines the Projectcoordinatordetails.
        /// </summary>
        [JsonProperty("projectcoordinatordetails")]
        public Projectcoordinatordetails projectcoordinatordetails;

        /// <summary>
        /// Defines the Save.
        /// </summary>
        [JsonProperty("save")]
        public string save;

        /// <summary>
        /// Defines the Sessionid.
        /// </summary>
        [JsonProperty("sessionid")]
        public string sessionid;

        /// <summary>
        /// Defines the Type.
        /// </summary>
        [JsonProperty("type")]
        public string type;

        /// <summary>
        /// Defines the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string userid;

        /// <summary>
        /// Defines the Vendorid.
        /// </summary>
        [JsonProperty("vendorid")]
        public string vendorid;

        /// <summary>
        /// Defines the Workpermitpassdetailsip.
        /// </summary>
        [JsonProperty("workpermitpassdetailsip")]
        public Workpermitpassdetailsip workpermitpassdetailsip;

        /// <summary>
        /// Defines the Workpermitpassrequestip.
        /// </summary>
        [JsonProperty("workpermitpassrequestip")]
        public Workpermitpassrequestip workpermitpassrequestip;
    }

    /// <summary>
    /// Defines the <see cref="GroupPemitPassRequest" />.
    /// </summary>
    public class GroupPemitPassRequest
    {
        /// <summary>
        /// Defines the Grouppassinput.
        /// </summary>
        [JsonProperty("grouppassinput")]
        public Grouppassinput grouppassinput;
    }
}
