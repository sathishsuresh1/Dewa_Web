// <copyright file="GroupPassPemitResponsecs.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses.SmartVendor.WorkPermit
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Gatepassguid" />.
    /// </summary>
    public class Gatepassguid
    {
        /// <summary>
        /// Gets or sets the Docclass.
        /// </summary>
        [JsonProperty("docclass")]
        public string Docclass { get; set; }

        /// <summary>
        /// Gets or sets the Docguid.
        /// </summary>
        [JsonProperty("docguid")]
        public string Docguid { get; set; }

        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Permitpassnumber.
        /// </summary>
        [JsonProperty("permitpassnumber")]
        public string Permitpassnumber { get; set; }

        /// <summary>
        /// Gets or sets the Rmsid.
        /// </summary>
        [JsonProperty("rmsid")]
        public string Rmsid { get; set; }

        /// <summary>
        /// Gets or sets the Sflag.
        /// </summary>
        [JsonProperty("sflag")]
        public string Sflag { get; set; }

        /// <summary>
        /// Gets or sets the Srcinspflag.
        /// </summary>
        [JsonProperty("srcinspflag")]
        public string Srcinspflag { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Grouppasslocationlist" />.
    /// </summary>
    public class Grouppasslocationlistres
    {
        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the Locationcode.
        /// </summary>
        [JsonProperty("locationcode")]
        public string Locationcode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Groupworkpermitpassbothlist" />.
    /// </summary>
    public class Groupworkpermitpassbothlist
    {
        /// <summary>
        /// Gets or sets the Cityname.
        /// </summary>
        [JsonProperty("cityname")]
        public string Cityname { get; set; }

        /// <summary>
        /// Gets or sets the Companyaddress1.
        /// </summary>
        [JsonProperty("companyaddress1")]
        public string Companyaddress1 { get; set; }

        /// <summary>
        /// Gets or sets the Companyaddress2.
        /// </summary>
        [JsonProperty("companyaddress2")]
        public string Companyaddress2 { get; set; }

        /// <summary>
        /// Gets or sets the Companyname.
        /// </summary>
        [JsonProperty("companyname")]
        public string Companyname { get; set; }

        /// <summary>
        /// Gets or sets the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string Countrykey { get; set; }

        /// <summary>
        /// Gets or sets the createddate.
        /// </summary>
        [JsonProperty("createddate")]
        public string createddate { get; set; }

        /// <summary>
        /// Gets or sets the Drivinglicenseflag.
        /// </summary>
        [JsonProperty("drivinglicenseflag")]
        public string Drivinglicenseflag { get; set; }

        /// <summary>
        /// Gets or sets the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string Emailid { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesid.
        /// </summary>
        [JsonProperty("emiratesid")]
        public string Emiratesid { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesidenddate.
        /// </summary>
        [JsonProperty("emiratesidenddate")]
        public string Emiratesidenddate { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesidflag.
        /// </summary>
        [JsonProperty("emiratesidflag")]
        public string Emiratesidflag { get; set; }

        /// <summary>
        /// Gets or sets the Fromtime.
        /// </summary>
        [JsonProperty("fromtime")]
        public string Fromtime { get; set; }

        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the Mulkiyanumber.
        /// </summary>
        [JsonProperty("mulkiyanumber")]
        public string Mulkiyanumber { get; set; }

        /// <summary>
        /// Gets or sets the Passexpirydate.
        /// </summary>
        [JsonProperty("passexpirydate")]
        public string Passexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Passissuedate.
        /// </summary>
        [JsonProperty("passissuedate")]
        public string Passissuedate { get; set; }

        /// <summary>
        /// Gets or sets the Passportenddate.
        /// </summary>
        [JsonProperty("passportenddate")]
        public string Passportenddate { get; set; }

        /// <summary>
        /// Gets or sets the Passportflag.
        /// </summary>
        [JsonProperty("passportflag")]
        public string Passportflag { get; set; }

        /// <summary>
        /// Gets or sets the Passportnumber.
        /// </summary>
        [JsonProperty("passportnumber")]
        public string Passportnumber { get; set; }

        /// <summary>
        /// Gets or sets the Permitpass.
        /// </summary>
        [JsonProperty("permitpass")]
        public string Permitpass { get; set; }

        /// <summary>
        /// Gets or sets the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string Permitstatus { get; set; }

        /// <summary>
        /// Gets or sets the Permitsubreference.
        /// </summary>
        [JsonProperty("permitsubreference")]
        public string Permitsubreference { get; set; }

        /// <summary>
        /// Gets or sets the Photoflag.
        /// </summary>
        [JsonProperty("photoflag")]
        public string Photoflag { get; set; }

        /// <summary>
        /// Gets or sets the Platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string Platenumber { get; set; }

        /// <summary>
        /// Gets or sets the Ponumber.
        /// </summary>
        [JsonProperty("ponumber")]
        public string Ponumber { get; set; }

        /// <summary>
        /// Gets or sets the Profession.
        /// </summary>
        [JsonProperty("profession")]
        public string Profession { get; set; }

        /// <summary>
        /// Gets or sets the Projectname.
        /// </summary>
        [JsonProperty("projectname")]
        public string Projectname { get; set; }

        /// <summary>
        /// Gets or sets the Purposeofvisit.
        /// </summary>
        [JsonProperty("purposeofvisit")]
        public string Purposeofvisit { get; set; }

        [JsonProperty("rejectremarks")]
        public string Rejectremarks { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Renewalreference.
        /// </summary>
        [JsonProperty("renewalreference")]
        public string Renewalreference { get; set; }

        /// <summary>
        /// Gets or sets the Totime.
        /// </summary>
        [JsonProperty("totime")]
        public string Totime { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets the Vehicleavailableflag.
        /// </summary>
        [JsonProperty("vehicleavailableflag")]
        public string Vehicleavailableflag { get; set; }

        /// <summary>
        /// Gets or sets the Vendornumber.
        /// </summary>
        [JsonProperty("vendornumber")]
        public string Vendornumber { get; set; }

        /// <summary>
        /// Gets or sets the Visaendate.
        /// </summary>
        [JsonProperty("visaendate")]
        public string Visaendate { get; set; }

        /// <summary>
        /// Gets or sets the Visaflag.
        /// </summary>
        [JsonProperty("visaflag")]
        public string Visaflag { get; set; }

        /// <summary>
        /// Gets or sets the Visanumber.
        /// </summary>
        [JsonProperty("visanumber")]
        public string Visanumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Permitpassdetails" />.
    /// </summary>
    public class Permitpassdetails
    {
        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Passexpirydate.
        /// </summary>
        [JsonProperty("passexpirydate")]
        public string Passexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Passissuedate.
        /// </summary>
        [JsonProperty("passissuedate")]
        public string Passissuedate { get; set; }

        /// <summary>
        /// Gets or sets the Permitpass.
        /// </summary>
        [JsonProperty("permitpass")]
        public string Permitpass { get; set; }

        /// <summary>
        /// Gets or sets the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string Permitstatus { get; set; }

        /// <summary>
        /// Gets or sets the Renewalflag.
        /// </summary>
        [JsonProperty("renewalflag")]
        public string Renewalflag { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Projectcoordinatorlist" />.
    /// </summary>
    public class Projectcoordinatorlist
    {
        /// <summary>
        /// Gets or sets the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string Emailid { get; set; }

        /// <summary>
        /// Gets or sets the Employeeid.
        /// </summary>
        [JsonProperty("employeeid")]
        public string Employeeid { get; set; }

        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the Phonenumber.
        /// </summary>
        [JsonProperty("phonenumber")]
        public string Phonenumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Attachmentlistop" />.
    /// </summary>
    public class Attachmentlistop
    {
        /// <summary>
        /// Gets or sets the Filedata.
        /// </summary>
        [JsonProperty("filedata")]
        public string Filedata { get; set; }

        /// <summary>
        /// Gets or sets the Filename.
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the Folderid.
        /// </summary>
        [JsonProperty("folderid")]
        public string Folderid { get; set; }

        /// <summary>
        /// Gets or sets the Mimetype.
        /// </summary>
        [JsonProperty("mimetype")]
        public string Mimetype { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="SubContractorOP" />.
    /// </summary>
    public class SubContractorOP
    {
        /// <summary>
        /// Gets or sets the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string Countrykey { get; set; }

        /// <summary>
        /// Gets or sets the Deleteflag.
        /// </summary>
        [JsonProperty("deleteflag")]
        public string Deleteflag { get; set; }

        /// <summary>
        /// Gets or sets the Gatepassuserid.
        /// </summary>
        [JsonProperty("gatepassuserid")]
        public string Gatepassuserid { get; set; }

        /// <summary>
        /// Gets or sets the Permitsubreference.
        /// </summary>
        [JsonProperty("permitsubreference")]
        public string Permitsubreference { get; set; }

        /// <summary>
        /// Gets or sets the Subcontractorname.
        /// </summary>
        [JsonProperty("subcontractorname")]
        public string Subcontractorname { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicenseattached.
        /// </summary>
        [JsonProperty("tradelicenseattached")]
        public string Tradelicenseattached { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicenseenddate.
        /// </summary>
        [JsonProperty("tradelicenseenddate")]
        public string Tradelicenseenddate { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicenseissuedate.
        /// </summary>
        [JsonProperty("tradelicenseissuedate")]
        public string Tradelicenseissuedate { get; set; }

        /// <summary>
        /// Gets or sets the Tradelicensenumber.
        /// </summary>
        [JsonProperty("tradelicensenumber")]
        public string Tradelicensenumber { get; set; }

        /// <summary>
        /// Gets or sets the Vendoraccountnumber.
        /// </summary>
        [JsonProperty("vendoraccountnumber")]
        public string Vendoraccountnumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Workpermitpassdetailsop" />.
    /// </summary>
    public class Workpermitpassdetailsop
    {
        /// <summary>
        /// Gets or sets the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string Countrykey { get; set; }

        /// <summary>
        /// Gets or sets the Drivinglicenseflag.
        /// </summary>
        [JsonProperty("drivinglicenseflag")]
        public string Drivinglicenseflag { get; set; }

        /// <summary>
        /// Gets or sets the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string Emailid { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesid.
        /// </summary>
        [JsonProperty("emiratesid")]
        public string Emiratesid { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesidenddate.
        /// </summary>
        [JsonProperty("emiratesidenddate")]
        public string Emiratesidenddate { get; set; }

        /// <summary>
        /// Gets or sets the Emiratesidflag.
        /// </summary>
        [JsonProperty("emiratesidflag")]
        public string Emiratesidflag { get; set; }

        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the Mulkiyanumber.
        /// </summary>
        [JsonProperty("mulkiyanumber")]
        public string Mulkiyanumber { get; set; }

        /// <summary>
        /// Gets or sets the Passportenddate.
        /// </summary>
        [JsonProperty("passportenddate")]
        public string Passportenddate { get; set; }

        /// <summary>
        /// Gets or sets the Passportflag.
        /// </summary>
        [JsonProperty("passportflag")]
        public string Passportflag { get; set; }

        /// <summary>
        /// Gets or sets the Passportnumber.
        /// </summary>
        [JsonProperty("passportnumber")]
        public string Passportnumber { get; set; }

        /// <summary>
        /// Gets or sets the Permitpass.
        /// </summary>
        [JsonProperty("permitpass")]
        public string Permitpass { get; set; }

        /// <summary>
        /// Gets or sets the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string Permitstatus { get; set; }

        /// <summary>
        /// Gets or sets the Photoflag.
        /// </summary>
        [JsonProperty("photoflag")]
        public string Photoflag { get; set; }

        /// <summary>
        /// Gets or sets the Platenumber.
        /// </summary>
        [JsonProperty("platenumber")]
        public string Platenumber { get; set; }

        /// <summary>
        /// Gets or sets the Profession.
        /// </summary>
        [JsonProperty("profession")]
        public string Profession { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Renewalreference.
        /// </summary>
        [JsonProperty("renewalreference")]
        public string Renewalreference { get; set; }

        /// <summary>
        /// Gets or sets the Vehicleavailableflag.
        /// </summary>
        [JsonProperty("vehicleavailableflag")]
        public string Vehicleavailableflag { get; set; }

        /// <summary>
        /// Gets or sets the Visaendate.
        /// </summary>
        [JsonProperty("visaendate")]
        public string Visaendate { get; set; }

        /// <summary>
        /// Gets or sets the Visaflag.
        /// </summary>
        [JsonProperty("visaflag")]
        public string Visaflag { get; set; }

        /// <summary>
        /// Gets or sets the Visanumber.
        /// </summary>
        [JsonProperty("visanumber")]
        public string Visanumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Workpermitpassrequestop" />.
    /// </summary>
    public class Workpermitpassrequestop
    {
        /// <summary>
        /// Gets or sets the Cityname.
        /// </summary>
        [JsonProperty("cityname")]
        public string Cityname { get; set; }

        /// <summary>
        /// Gets or sets the Companyaddress1.
        /// </summary>
        [JsonProperty("companyaddress1")]
        public string Companyaddress1 { get; set; }

        /// <summary>
        /// Gets or sets the Companyaddress2.
        /// </summary>
        [JsonProperty("companyaddress2")]
        public string Companyaddress2 { get; set; }

        /// <summary>
        /// Gets or sets the Companyname.
        /// </summary>
        [JsonProperty("companyname")]
        public string Companyname { get; set; }

        /// <summary>
        /// Gets or sets the Countrykey.
        /// </summary>
        [JsonProperty("countrykey")]
        public string Countrykey { get; set; }

        /// <summary>
        /// Gets or sets the Emailid.
        /// </summary>
        [JsonProperty("emailid")]
        public string Emailid { get; set; }

        /// <summary>
        /// Gets or sets the Fromtime.
        /// </summary>
        [JsonProperty("fromtime")]
        public string Fromtime { get; set; }

        /// <summary>
        /// Gets or sets the Grouppassid.
        /// </summary>
        [JsonProperty("grouppassid")]
        public string Grouppassid { get; set; }

        /// <summary>
        /// Gets or sets the Lang.
        /// </summary>
        [JsonProperty("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets the Mobile.
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the Passexpirydate.
        /// </summary>
        [JsonProperty("passexpirydate")]
        public string Passexpirydate { get; set; }

        /// <summary>
        /// Gets or sets the Passissuedate.
        /// </summary>
        [JsonProperty("passissuedate")]
        public string Passissuedate { get; set; }

        /// <summary>
        /// Gets or sets the Permitstatus.
        /// </summary>
        [JsonProperty("permitstatus")]
        public string Permitstatus { get; set; }

        /// <summary>
        /// Gets or sets the Permitsubreference.
        /// </summary>
        [JsonProperty("permitsubreference")]
        public string Permitsubreference { get; set; }

        /// <summary>
        /// Gets or sets the Ponumber.
        /// </summary>
        [JsonProperty("ponumber")]
        public string Ponumber { get; set; }

        /// <summary>
        /// Gets or sets the Projectname.
        /// </summary>
        [JsonProperty("projectname")]
        public string Projectname { get; set; }

        /// <summary>
        /// Gets or sets the Purposeofvisit.
        /// </summary>
        [JsonProperty("purposeofvisit")]
        public string Purposeofvisit { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the Renewalflag.
        /// </summary>
        [JsonProperty("renewalflag")]
        public string Renewalflag { get; set; }

        /// <summary>
        /// Gets or sets the Renewalreference.
        /// </summary>
        [JsonProperty("renewalreference")]
        public string Renewalreference { get; set; }

        /// <summary>
        /// Gets or sets the Totime.
        /// </summary>
        [JsonProperty("totime")]
        public string Totime { get; set; }

        /// <summary>
        /// Gets or sets the Userid.
        /// </summary>
        [JsonProperty("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets the Vendornumber.
        /// </summary>
        [JsonProperty("vendornumber")]
        public string Vendornumber { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="GroupPassPemitResponse" />.
    /// </summary>
    public class GroupPassPemitResponse
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Gatepassguid.
        /// </summary>
        [JsonProperty("gatepassguid")]
        public Gatepassguid Gatepassguid { get; set; }

        /// <summary>
        /// Gets or sets the attachmentdetails.
        /// </summary>
        [JsonProperty("attachmentlist")]
        public List<Attachmentlistop> attachmentdetails { get; set; }

        /// <summary>
        /// Gets or sets the subcontractordetails.
        /// </summary>
        [JsonProperty("subcontractorlist")]
        public List<SubContractorOP> subcontractordetails { get; set; }

        /// <summary>
        /// Gets or sets the Grouppasslocationlist.
        /// </summary>
        [JsonProperty("grouplocationoplist")]
        public List<Grouppasslocationlistres> Grouppasslocationlist { get; set; }

        /// <summary>
        /// Gets or sets the Grouppasslocationreturnlist.
        /// </summary>
        [JsonProperty("grouppasslocationlist")]
        public List<Grouppasslocationlistres> Grouppasslocationreturnlist { get; set; }

        /// <summary>
        /// Gets or sets the Groupworkpermitpassbothlist.
        /// </summary>
        [JsonProperty("groupworkpermitpassbothlist")]
        public List<Groupworkpermitpassbothlist> Groupworkpermitpassbothlist { get; set; }

        /// <summary>
        /// Gets or sets the Permitpassdetails.
        /// </summary>
        [JsonProperty("permitpassdetails")]
        public Permitpassdetails Permitpassdetails { get; set; }

        /// <summary>
        /// Gets or sets the Projectcoordinatorlist.
        /// </summary>
        [JsonProperty("projectcoordinatorlist")]
        public List<Projectcoordinatorlist> Projectcoordinatorlist { get; set; }

        /// <summary>
        /// Gets or sets the Responsecode.
        /// </summary>
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }

        /// <summary>
        /// Gets or sets the Workpermitpassdetailsop.
        /// </summary>
        [JsonProperty("workpermitpassdetailsop")]
        public Workpermitpassdetailsop Workpermitpassdetailsop { get; set; }

        /// <summary>
        /// Gets or sets the Workpermitpassrequestop.
        /// </summary>
        [JsonProperty("workpermitpassrequestop")]
        public Workpermitpassrequestop Workpermitpassrequestop { get; set; }
    }
}
