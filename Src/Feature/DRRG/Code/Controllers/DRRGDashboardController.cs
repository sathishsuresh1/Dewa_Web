// <copyright file="DRRGRegistrationController.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.CustomDB.DRRGDataModel;
    using DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG;
    using DEWAXP.Feature.DRRG.Models;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Data.Entity.Core.Objects;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Feature.DRRG.Filters.Mvc;
    using DEWAXP.Feature.DRRG.Extensions;
    using System.Globalization;
    using DEWAXP.Foundation.Content.Repositories;
    using Sitecore.Mvc.Presentation;
    using System.Threading;
    using System.Data;
    using Exception = System.Exception;
    using EntityFrameworkExtras.EF6;

    public class DRRGDashboardController : DRRGBaseController
    {
        public long FileSizeLimit = 2048000;
        public string[] supportedTypes = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".PDF", ".PNG", ".JPG", ".JPEG" };
        public string[] supportedLogoTypes = new[] { ".jpg", ".png", ".jpeg", ".PNG", ".JPG", ".JPEG" };

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult Dashboard()
        {
            ViewBag.Name = CurrentPrincipal.FullName;
            return View("~/Views/Feature/DRRG/Dashboard/Dashboard.cshtml");
        }

        [HttpGet]
        public ActionResult DashboardUserDetails()
        {
            DashboardUserDetails model = new DashboardUserDetails();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    //ObjectParameter FirstnameParamresponse = new ObjectParameter(DRRGStandardValues.firstname, typeof(string));
                    //ObjectParameter LastnameParamresponse = new ObjectParameter(DRRGStandardValues.lastname, typeof(string));
                    //ObjectParameter ManufacturernameParamresponse = new ObjectParameter(DRRGStandardValues.manufacturername, typeof(string));
                    //ObjectParameter LogoFileParamresponse = new ObjectParameter(DRRGStandardValues.logofile, typeof(string));
                    //ObjectParameter LogoFileNameParamresponse = new ObjectParameter(DRRGStandardValues.logofileName, typeof(string));
                    //ObjectParameter LogoFileContentTypeParamresponse = new ObjectParameter(DRRGStandardValues.logoContentType, typeof(string));
                    //ObjectParameter LogoFileExtensionParamresponse = new ObjectParameter(DRRGStandardValues.logoExtension, typeof(string));

                    var userDetails = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower().Equals(CurrentPrincipal.BusinessPartner.ToLower())).FirstOrDefault();
                    if (userDetails != null)
                    {
                        model.Firstname = userDetails.User_First_Name;
                        model.Lastname = userDetails.User_Last_Name;
                        model.ManufacturerCode = userDetails.Manufacturer_Code;
                        model.Manufacturername = userDetails.Manufacturer_Name;
                        model.Address = userDetails.Manufacturer_Country;
                        model.MobileNumber = userDetails.User_Mobile_Code + "-" + userDetails.User_Mobile_Number;
                    }

                    var fileDetails = context.DRRG_Files.Where(x => x.Reference_ID.ToLower().Equals(CurrentPrincipal.BusinessPartner.ToLower()) && x.File_Type == FileType.TrademarkLogo && x.Entity == FileEntity.Manufacturer).FirstOrDefault();
                    if (fileDetails != null && model != null)
                    {
                        model.Logofilename = fileDetails.Name;
                        model.Logofilecontenttype = fileDetails.ContentType;
                        model.Logofileextension = fileDetails.Extension;
                        model.Logofile = fileDetails.Content != null ? Convert.ToBase64String((byte[])fileDetails.Content) : string.Empty;
                    }
                    //context.SP_DRRG_Userdetails(CurrentPrincipal.Username, CurrentPrincipal.BusinessPartner, FirstnameParamresponse, LastnameParamresponse,
                    //    ManufacturernameParamresponse, LogoFileParamresponse, LogoFileNameParamresponse, LogoFileContentTypeParamresponse, LogoFileExtensionParamresponse);
                    //model = new DashboardUserDetails
                    //{
                    //    Firstname = Convert.ToString(FirstnameParamresponse.Value),
                    //    Lastname = Convert.ToString(LastnameParamresponse.Value),
                    //    Manufacturername = Convert.ToString(ManufacturernameParamresponse.Value),
                    //    Logofilename = Convert.ToString(LogoFileNameParamresponse.Value),
                    //    Logofilecontenttype = Convert.ToString(LogoFileContentTypeParamresponse.Value),
                    //    Logofileextension = Convert.ToString(LogoFileExtensionParamresponse.Value),
                    //    //Logofile = Convert.FromBase64String(Convert.ToString(LogoFileParamresponse.Value))
                    //    Logofile = !string.IsNullOrWhiteSpace(LogoFileParamresponse.Value.ToString()) ? Convert.ToBase64String((byte[])LogoFileParamresponse.Value) : string.Empty
                    //};
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/DashboardUserDetails.cshtml", model);
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult AddPVmodule(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var result = context.DRRG_PVMODULE.Where(i => i.PV_ID != null && i.PV_ID.ToLower() == id.ToLower()).FirstOrDefault();
                        if (result != null && result.Status != null && (result.Status.Equals("Approved") || result.Status.Equals("ReviewerRejected")))
                        {
                            var resultNominalPower = context.DRRG_PVModule_Nominal.Where(x => x.PV_ID != null && x.PV_ID.ToLower() == id.ToLower()).ToList();
                            var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.IEC61215 ||
                                                                            x.File_Type == FileType.IEC61730 ||
                                                                            x.File_Type == FileType.IEC61701 ||
                                                                            x.File_Type == FileType.IEC62716 ||
                                                                           x.File_Type == FileType.IEC60068 ||
                                                                           x.File_Type == FileType.ModelDataSheet) &&
                                                                           x.Entity == FileEntity.PVmodule &&
                                                                           x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                                           x.Reference_ID == id).ToList();
                            if (result != null)
                            {
                                PVModule m = new PVModule();
                                m.Status = result.Status;
                                m.Id = result.id;
                                m.PVId = result.PV_ID;
                                m.Manufacturer_Code = result.Manufacturer_Code;
                                m.BackSuperstrate = result.Back_Superstrate;
                                m.OtherBackSuperstrate = result.Other_Back_Superstrate;
                                m.CellTechnology = result.Cell_Technology;
                                m.SelectedCellStructure = result.PV_Cell_Structure;
                                m.OtherCellStructure = result.Other_Cell_Structure;
                                m.OtherCellTechnology = result.Other_Cell_Technology;
                                m.Bifacial = result.Bifacial;
                                m.DCSystemGroup = result.DC_System_Grounding;
                                m.DCSystemGroupMandatory = result.DC_System_Grounding_Mandatory;
                                m.Encapsulant = result.Encapsulant;
                                m.OtherEncapsulant = result.Other_Encapsulant;
                                m.FeaturesofJunctionBox = result.Features_JB;
                                m.OtherFeaturesofJunctionBox = result.Other_Features_JB;
                                m.Framed = result.Framed;
                                m.FrontSuperstrate = result.Front_Superstrate;
                                m.OtherFrontSuperstrate = result.Othe_Front_Superstrate;
                                m.MaterialofJunctionBox = result.Material_JB;
                                m.OtherMaterialofJunctionBox = result.Other_Material_JB;
                                m.ModelName = result.Model_Name;
                                m.ModuleLength = Convert.ToString(result.Module_Length);
                                m.ModuleWidth = Convert.ToString(result.Module_Width);
                                m.NominalPower = result.Nominal_Power;
                                m.PositionofJunctionBox = result.Position_JB;
                                m.Terminations = result.Terminations;
                                m.OtherTerminations = result.Other_Terminations;
                                m.pVMultiNominals = resultNominalPower?.Select(x => new PVMultiNominal
                                {
                                    wp1 = x.wp1,
                                    wp2 = x.wp2,
                                    wp3 = x.wp3,
                                    //mpc1 = x.mpc1,
                                    //mpv1 = x.mpv1,
                                    //noct1 = x.noct1,
                                    //npnoct1 = x.npnoct1,
                                    //ocv1 = x.ocv1,
                                    //scc1 = x.scc1,
                                    //tci1 = x.tci1,
                                    //tcv1 = x.tcv1
                                }).ToList();
                                if (!string.IsNullOrWhiteSpace(m.NominalPower) && m.NominalPower.Equals("Single Nominal Power Entry"))
                                {
                                    m.Singlenominalpower = (m.pVMultiNominals != null && m.pVMultiNominals.Count > 0) ? m.pVMultiNominals[0].wp1 : "";
                                }
                                m.ModelDataSheetBinary = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.Select(x => x.Content)?.FirstOrDefault();
                                m.Document1Binary = resultfiles.Where(x => x.File_Type == FileType.IEC61215)?.Select(x => x.Content)?.FirstOrDefault();
                                m.Document2Binary = resultfiles.Where(x => x.File_Type == FileType.IEC61730)?.Select(x => x.Content)?.FirstOrDefault();
                                m.Document3Binary = resultfiles.Where(x => x.File_Type == FileType.IEC61701)?.Select(x => x.Content)?.FirstOrDefault();
                                m.Document4Binary = resultfiles.Where(x => x.File_Type == FileType.IEC62716)?.Select(x => x.Content)?.FirstOrDefault();
                                m.Document5Binary = resultfiles.Where(x => x.File_Type == FileType.IEC60068)?.Select(x => x.Content)?.FirstOrDefault();

                                m.ModelDataSheetFilename = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.Select(x => x.Name)?.FirstOrDefault();
                                m.Document1Filename = resultfiles.Where(x => x.File_Type == FileType.IEC61215)?.Select(x => x.Name)?.FirstOrDefault();
                                m.Document2Filename = resultfiles.Where(x => x.File_Type == FileType.IEC61730)?.Select(x => x.Name)?.FirstOrDefault();
                                m.Document3Filename = resultfiles.Where(x => x.File_Type == FileType.IEC61701)?.Select(x => x.Name)?.FirstOrDefault();
                                m.Document4Filename = resultfiles.Where(x => x.File_Type == FileType.IEC62716)?.Select(x => x.Name)?.FirstOrDefault();
                                m.Document5Filename = resultfiles.Where(x => x.File_Type == FileType.IEC60068)?.Select(x => x.Name)?.FirstOrDefault();

                                return View("~/Views/Feature/DRRG/Dashboard/PVModule.cshtml", m);
                            }
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogService.Error(ex, this);
                }

            }

            return View("~/Views/Feature/DRRG/Dashboard/PVModule.cshtml", new PVModule());
        }

        [TwoPhaseDRRGAuthorize, HttpPost]
        public ActionResult AddPVmodule(PVModule model)
        {
            string error = string.Empty;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            string pvid = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected"))
                {
                    pvid = PVModuleCode();
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.IEC61215 ||
                                                                       x.File_Type == FileType.IEC61730 ||
                                                                       x.File_Type == FileType.IEC61701 ||
                                                                       x.File_Type == FileType.IEC62716 ||
                                                                      x.File_Type == FileType.IEC60068 ||
                                                                      x.File_Type == FileType.ModelDataSheet) &&
                                                                      x.Entity == FileEntity.PVmodule &&
                                                                      x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                                      x.Reference_ID == model.PVId).ToList();

                        if (model.ModelDataSheet == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document1 == null)
                        {
                            var fileDocument1 = resultfiles.Where(x => x.File_Type == FileType.IEC61215)?.FirstOrDefault();
                            if (fileDocument1 != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileDocument1.Name,
                                    ContentType = fileDocument1.ContentType,
                                    Extension = fileDocument1.Extension,
                                    File_Type = fileDocument1.File_Type,
                                    Entity = fileDocument1.Entity,
                                    Content = fileDocument1.Content,
                                    Size = fileDocument1.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileDocument1.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document2 == null)
                        {
                            var fileDocument2 = resultfiles.Where(x => x.File_Type == FileType.IEC61730)?.FirstOrDefault();
                            if (fileDocument2 != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileDocument2.Name,
                                    ContentType = fileDocument2.ContentType,
                                    Extension = fileDocument2.Extension,
                                    File_Type = fileDocument2.File_Type,
                                    Entity = fileDocument2.Entity,
                                    Content = fileDocument2.Content,
                                    Size = fileDocument2.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileDocument2.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document3 == null)
                        {
                            var fileDocument3 = resultfiles.Where(x => x.File_Type == FileType.IEC61701)?.FirstOrDefault();
                            if (fileDocument3 != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileDocument3.Name,
                                    ContentType = fileDocument3.ContentType,
                                    Extension = fileDocument3.Extension,
                                    File_Type = fileDocument3.File_Type,
                                    Entity = fileDocument3.Entity,
                                    Content = fileDocument3.Content,
                                    Size = fileDocument3.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileDocument3.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document4 == null)
                        {
                            var fileDocument4 = resultfiles.Where(x => x.File_Type == FileType.IEC62716)?.FirstOrDefault();
                            if (fileDocument4 != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileDocument4.Name,
                                    ContentType = fileDocument4.ContentType,
                                    Extension = fileDocument4.Extension,
                                    File_Type = fileDocument4.File_Type,
                                    Entity = fileDocument4.Entity,
                                    Content = fileDocument4.Content,
                                    Size = fileDocument4.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileDocument4.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document5 == null)
                        {
                            var fileDocument5 = resultfiles.Where(x => x.File_Type == FileType.IEC60068)?.FirstOrDefault();
                            if (fileDocument5 != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileDocument5.Name,
                                    ContentType = fileDocument5.ContentType,
                                    Extension = fileDocument5.Extension,
                                    File_Type = fileDocument5.File_Type,
                                    Entity = fileDocument5.Entity,
                                    Content = fileDocument5.Content,
                                    Size = fileDocument5.Size,
                                    Reference_ID = pvid,
                                    Manufacturercode = fileDocument5.Manufacturer_Code
                                });
                            }
                        }
                    }
                }
                else
                {
                    pvid = (model.Id > 0 ? model.PVId : PVModuleCode());
                }
                if ((string.IsNullOrWhiteSpace(model.PVId) && (model.Status == null)) || (model.Status != null && model.Status.Equals("ReviewerRejected")))
                {
                    Addfile(model.ModelDataSheet, FileType.ModelDataSheet, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document1, FileType.IEC61215, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document2, FileType.IEC61730, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document3, FileType.IEC61701, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document4, FileType.IEC62716, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document5, FileType.IEC60068, FileEntity.PVmodule, pvid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);

                }
                if (!string.IsNullOrWhiteSpace(model.signatureCopy))
                {
                    if (model.Status != null && model.Status.Equals("Approved"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var pvresult = context.DRRG_PVMODULE.Where(x => x.PV_ID == pvid).FirstOrDefault();
                            model.ModelName = pvresult.Model_Name;
                            model.CellTechnology = pvresult.Cell_Technology;
                            model.OtherCellTechnology = pvresult.Other_Cell_Technology;
                        }
                    }
                    var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
                    //var sizeInKb = sizeInBytes / 1000;
                    dRRG_Files_TYlist.Add(new DRRG_Files_TY
                    {
                        Name = pvid + "_Declaration.pdf",
                        ContentType = "application/pdf",
                        Extension = "pdf",
                        File_Type = FileType.SignatureCopy,
                        Entity = FileEntity.PVmodule,
                        Content = GenerateDecalartionLetter(pvid, model, null, null),
                        Size = Convert.ToString(sizeInBytes),
                        Reference_ID = pvid,
                        Manufacturercode = CurrentPrincipal.BusinessPartner,
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("DRRG_SignatureRequired"));
                }
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.PVId) && (model.Status != null && (model.Status.Equals("Approved"))))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                            {
                                var signFile = dRRG_Files_TYlist.Where(x => x.File_Type == FileType.SignatureCopy).FirstOrDefault();
                                if (signFile != null)
                                {
                                    context.DRRG_Files.Add(new DRRG_Files
                                    {
                                        Name = signFile.Name,
                                        ContentType = signFile.ContentType,
                                        Extension = signFile.Extension,
                                        File_Type = signFile.File_Type,
                                        Entity = signFile.Entity,
                                        Content = signFile.Content,
                                        Size = signFile.Size,
                                        Reference_ID = signFile.Reference_ID,
                                        Manufacturer_Code = signFile.Manufacturercode
                                    });
                                    context.SaveChanges();
                                }
                            }
                            var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.IEC61215 ||
                                                                       x.File_Type == FileType.IEC61730 ||
                                                                       x.File_Type == FileType.IEC61701 ||
                                                                       x.File_Type == FileType.IEC62716 ||
                                                                      x.File_Type == FileType.IEC60068 ||
                                                                      x.File_Type == FileType.ModelDataSheet) &&
                                                                      x.Entity == FileEntity.PVmodule &&
                                                                      x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                                      x.Reference_ID == model.PVId).ToList();

                            if (model.ModelDataSheet != null && model.ModelDataSheet.ContentLength > 0)
                            {
                                var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.FirstOrDefault();
                                if (fileModelDataSheet != null)
                                {
                                    context.DRRG_Files.Remove(fileModelDataSheet);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.ModelDataSheet.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.ModelDataSheet.FileName,
                                            ContentType = model.ModelDataSheet.ContentType,
                                            Extension = model.ModelDataSheet.GetTrimmedFileExtension(),
                                            File_Type = FileType.ModelDataSheet,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.ModelDataSheet.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Document1 != null && model.Document1.ContentLength > 0)
                            {
                                var fileDocument1 = resultfiles.Where(x => x.File_Type == FileType.IEC61215)?.FirstOrDefault();
                                if (fileDocument1 != null)
                                {
                                    context.DRRG_Files.Remove(fileDocument1);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.Document1.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.Document1.FileName,
                                            ContentType = model.Document1.ContentType,
                                            Extension = model.Document1.GetTrimmedFileExtension(),
                                            File_Type = FileType.IEC61215,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.Document1.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Document2 != null && model.Document2.ContentLength > 0)
                            {
                                var fileDocument2 = resultfiles.Where(x => x.File_Type == FileType.IEC61730)?.FirstOrDefault();
                                if (fileDocument2 != null)
                                {
                                    context.DRRG_Files.Remove(fileDocument2);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.Document2.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.Document2.FileName,
                                            ContentType = model.Document2.ContentType,
                                            Extension = model.Document2.GetTrimmedFileExtension(),
                                            File_Type = FileType.IEC61730,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.Document2.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Document3 != null && model.Document3.ContentLength > 0)
                            {
                                var fileDocument3 = resultfiles.Where(x => x.File_Type == FileType.IEC61701)?.FirstOrDefault();
                                if (fileDocument3 != null)
                                {
                                    context.DRRG_Files.Remove(fileDocument3);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.Document3.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.Document3.FileName,
                                            ContentType = model.Document3.ContentType,
                                            Extension = model.Document3.GetTrimmedFileExtension(),
                                            File_Type = FileType.IEC61701,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.Document3.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Document4 != null && model.Document4.ContentLength > 0)
                            {
                                var fileDocument4 = resultfiles.Where(x => x.File_Type == FileType.IEC62716)?.FirstOrDefault();
                                if (fileDocument4 != null)
                                {
                                    context.DRRG_Files.Remove(fileDocument4);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.Document4.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.Document4.FileName,
                                            ContentType = model.Document4.ContentType,
                                            Extension = model.Document4.GetTrimmedFileExtension(),
                                            File_Type = FileType.IEC62716,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.Document4.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Document5 != null && model.Document5.ContentLength > 0)
                            {
                                var fileDocument5 = resultfiles.Where(x => x.File_Type == FileType.IEC60068)?.FirstOrDefault();
                                if (fileDocument5 != null)
                                {
                                    context.DRRG_Files.Remove(fileDocument5);
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.Document5.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.Document5.FileName,
                                            ContentType = model.Document5.ContentType,
                                            Extension = model.Document5.GetTrimmedFileExtension(),
                                            File_Type = FileType.IEC60068,
                                            Entity = FileEntity.PVmodule,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.Document5.ContentLength.ToString(),
                                            Reference_ID = pvid,
                                            Manufacturer_Code = CurrentPrincipal.BusinessPartner
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }

                            //model.NominalPower = model.SelectedNominalPowerEntry;
                            List<DRRG_PVModule_Nominal> deleteItems = context.DRRG_PVModule_Nominal.Where(x => x.PV_ID.ToLower() == model.PVId.ToLower()).ToList();
                            foreach (var item in deleteItems)
                            {
                                context.DRRG_PVModule_Nominal.Remove(item);
                                context.SaveChanges();
                            }
                            if (model.NominalPowerEntries != null && model.NominalPowerEntries.Count > 0)
                            {
                                if (model.NominalPower.Equals(NominalPowerType.SingleNominalPowerEntry))
                                {
                                    context.DRRG_PVModule_Nominal.Add(new DRRG_PVModule_Nominal
                                    {
                                        PV_ID = pvid,
                                        wp1 = model.Singlenominalpower,
                                        //mpc1 = model.MaximumPowerCurrent,
                                        //mpv1 = model.MaximumPowerVoltage,
                                        //ocv1 = model.OpenCircuitVoltage,
                                        //scc1 = model.ShortCircuitCurrent,
                                        //tci1 = model.TemperatureCoefficientofIsc,
                                        //tcv1 = model.TemperatureCoefficientofVoc,
                                        //noct1 = model.NominalOperatingCellTemp,
                                        //npnoct1 = model.NominalPoweratNOCTCondition
                                    });
                                    context.SaveChanges();
                                }
                                else if (model.NominalPower.Equals(NominalPowerType.NominalPowerRange) && model.MultinominalPower != null && model.MultinominalPower.Count > 0)
                                {
                                    context.DRRG_PVModule_Nominal.Add(new DRRG_PVModule_Nominal
                                    {
                                        PV_ID = pvid,
                                        wp1 = model.MultinominalPower[0],
                                        wp2 = model.MultinominalPower[1],
                                        wp3 = model.MultinominalPower[2],
                                        //mpc1 = model.MaximumPowerCurrent,
                                        //mpv1 = model.MaximumPowerVoltage,
                                        //ocv1 = model.OpenCircuitVoltage,
                                        //scc1 = model.ShortCircuitCurrent,
                                        //tci1 = model.TemperatureCoefficientofIsc,
                                        //tcv1 = model.TemperatureCoefficientofVoc,
                                        //noct1 = model.NominalOperatingCellTemp,
                                        //npnoct1 = model.NominalPoweratNOCTCondition
                                    });
                                    context.SaveChanges();
                                }
                                else if (model.NominalPower.Equals(NominalPowerType.MultipleNominalPowerEntries) && !string.IsNullOrWhiteSpace(model.NumberofMultinominals))
                                {
                                    var numberofnominal = Convert.ToInt32(model.NumberofMultinominals);
                                    foreach (var nominal in model.pVMultiNominals)
                                    {
                                        context.DRRG_PVModule_Nominal.Add(new DRRG_PVModule_Nominal
                                        {
                                            PV_ID = pvid,
                                            wp1 = nominal.wp1,
                                            wp2 = nominal.wp2,
                                            wp3 = nominal.wp3,
                                            //mpc1 = nominal.mpc1,
                                            //mpv1 = nominal.mpv1,
                                            //ocv1 = nominal.ocv1,
                                            //scc1 = nominal.scc1,
                                            //tci1 = nominal.tci1,
                                            //tcv1 = nominal.tcv1,
                                            //noct1 = nominal.noct1,
                                            //npnoct1 = nominal.npnoct1
                                        });
                                        context.SaveChanges();
                                    }
                                }
                            }
                            ViewBag.Referenceid = pvid; ViewBag.Module = FileEntity.PVmodule;
                            var updateItem = context.DRRG_PVMODULE.Where(x => x.PV_ID == model.PVId).FirstOrDefault();
                            if (updateItem != null)
                            {
                                updateItem.Status = "Updated";
                                updateItem.Nominal_Power = model.NominalPower;
                                updateItem.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                context.SaveChanges();
                            }
                            DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Updated by " + CurrentPrincipal.Name, "PV Module", pvid, DateTime.Now, CurrentPrincipal.UserId, "Updated");
                            SendDRRGUpdateModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, pvid, FileEntity.PVmodule);
                            return View("~/Views/Feature/DRRG/Dashboard/ModuleSuccess.cshtml");
                        }
                    }
                    else
                    {
                        List<DRRG_PVModule_TY> DRRG_PVModule = new List<DRRG_PVModule_TY>()
                    {
                        new DRRG_PVModule_TY
                        {
                            Userid = CurrentPrincipal.UserId,
                            Session = CurrentPrincipal.SessionToken,
                            Manufacturer_Code = (model.Id > 0 ? model.Manufacturer_Code : CurrentPrincipal.BusinessPartner),
                            Back_Superstrate = model.BackSuperstrate,
                            Other_Back_Superstrate = model.OtherBackSuperstrate,
                            Cell_Technology =model.CellTechnology,
                            Other_Cell_Technology = model.OtherCellTechnology,
                            PV_Cell_Structure = model.CellStructure != null ? string.Join(";", model.CellStructure) : string.Empty,
                            Other_PV_Cell_Structure = model.OtherCellStructure,
                            DC_System_Grounding_Mandatory = model.DCSystemGroupMandatory,
                            DC_System_Grounding = model.DCSystemGroup,
                            Encapsulant = model.Encapsulant,
                            Othe_Encapsulant = model.OtherEncapsulant,
                            Features_JB = model.FeaturesofJunctionBox,
                            Other_Features_JB = model.OtherFeaturesofJunctionBox,
                            Framed = model.Framed,
                            Front_Superstrate =model.FrontSuperstrate,
                            Othe_Front_Superstrate =model.OtherFrontSuperstrate,
                            Material_JB = model.MaterialofJunctionBox,
                            Other_Material_JB = model.OtherMaterialofJunctionBox,
                            Model_Name = model.ModelName,
                            Module_Length= model.ModuleLength,
                            Module_Width= model.ModuleWidth,
                            Nominal_Power= model.NominalPower,
                            Position_JB = model.PositionofJunctionBox,
                            PVID = pvid,
                            Parent_PV_ID = (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected") ? model.PVId : string.Empty),
                            Terminations = model.Terminations,
                            Other_Terminations = model.OtherTerminations
                        }
                    };
                        List<DRRG_PVModulenominal_TY> DRRG_PVmodulenominal = new List<DRRG_PVModulenominal_TY>();
                        if (model.NominalPower.Equals(NominalPowerType.SingleNominalPowerEntry))
                        {
                            DRRG_PVmodulenominal.Add(new DRRG_PVModulenominal_TY
                            {
                                PVID = pvid,
                                wp1 = model.Singlenominalpower,
                                //mpc1 = model.MaximumPowerCurrent,
                                //mpv1 = model.MaximumPowerVoltage,
                                //ocv1 = model.OpenCircuitVoltage,
                                //scc1 = model.ShortCircuitCurrent,
                                //tci1 = model.TemperatureCoefficientofIsc,
                                //tcv1 = model.TemperatureCoefficientofVoc,
                                //noct1 = model.NominalOperatingCellTemp,
                                //npnoct1 = model.NominalPoweratNOCTCondition
                            });
                        }
                        else if (model.NominalPower.Equals(NominalPowerType.NominalPowerRange) && model.MultinominalPower != null && model.MultinominalPower.Count > 0)
                        {
                            DRRG_PVmodulenominal.Add(new DRRG_PVModulenominal_TY
                            {
                                PVID = pvid,
                                wp1 = model.MultinominalPower[0],
                                wp2 = model.MultinominalPower[1],
                                wp3 = model.MultinominalPower[2],
                                //mpc1 = model.MaximumPowerCurrent,
                                //mpv1 = model.MaximumPowerVoltage,
                                //ocv1 = model.OpenCircuitVoltage,
                                //scc1 = model.ShortCircuitCurrent,
                                //tci1 = model.TemperatureCoefficientofIsc,
                                //tcv1 = model.TemperatureCoefficientofVoc,
                                //noct1 = model.NominalOperatingCellTemp,
                                //npnoct1 = model.NominalPoweratNOCTCondition
                            });
                        }
                        else if (model.NominalPower.Equals(NominalPowerType.MultipleNominalPowerEntries) && !string.IsNullOrWhiteSpace(model.NumberofMultinominals))
                        {
                            var numberofnominal = Convert.ToInt32(model.NumberofMultinominals);
                            foreach (var nominal in model.pVMultiNominals)
                            {
                                DRRG_PVmodulenominal.Add(new DRRG_PVModulenominal_TY
                                {
                                    PVID = pvid,
                                    wp1 = nominal.wp1,
                                    wp2 = nominal.wp2,
                                    wp3 = nominal.wp3,
                                    //mpc1 = nominal.mpc1,
                                    //mpv1 = nominal.mpv1,
                                    //ocv1 = nominal.ocv1,
                                    //scc1 = nominal.scc1,
                                    //tci1 = nominal.tci1,
                                    //tcv1 = nominal.tcv1,
                                    //noct1 = nominal.noct1,
                                    //npnoct1 = nominal.npnoct1
                                });
                            }

                        }
                        Proc_DRRG_InsertPVModule procedure = new Proc_DRRG_InsertPVModule()
                        {
                            referenceID = pvid,
                            updateId = (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected") ? 0 : (model.Id > 0 ? model.Id : 0)),
                            useremail = CurrentPrincipal.UserId,
                            DRRG_PVModule = DRRG_PVModule,
                            DRRG_PVModule_nominal = DRRG_PVmodulenominal,
                            dRRG_Files_TY = dRRG_Files_TYlist
                        };
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            context.Database.ExecuteStoredProcedure(procedure);
                            string errormessage = procedure.error;
                            if (!string.IsNullOrWhiteSpace(procedure.error))
                            {
                                ModelState.AddModelError(string.Empty, GetDRRGErrormessage(procedure.error));
                            }
                            else
                            {
                                ViewBag.Referenceid = pvid; ViewBag.Module = FileEntity.PVmodule;
                                if (model.Id <= 0 || (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected")))
                                {
                                    DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Submitted by " + CurrentPrincipal.Name, "PV Module", pvid, DateTime.Now, CurrentPrincipal.UserId, "In Progress");
                                    SendDRRGModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, pvid, FileEntity.PVmodule);
                                }
                                else
                                {
                                    DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Updated by " + CurrentPrincipal.Name, "PV Module", pvid, DateTime.Now, CurrentPrincipal.UserId, "Updated");
                                    SendDRRGUpdateModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, pvid, FileEntity.PVmodule);
                                }
                                return View("~/Views/Feature/DRRG/Dashboard/ModuleSuccess.cshtml");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View("~/Views/Feature/DRRG/Dashboard/PVModule.cshtml", model);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/PVModule.cshtml", model);
        }

        [HttpPost]
        public JsonResult ResetSession()
        {
            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
            }
            System.Web.HttpContext.Current.Session.Timeout = 60;
            return Json(Session.Timeout, JsonRequestBehavior.AllowGet);
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult AddInvertermodule(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var result = context.DRRG_InverterModule.Where(i => i.Inverter_ID != null && i.Inverter_ID.ToLower() == id.ToLower()).FirstOrDefault();
                        if (result != null && result.Status != null && (result.Status.Equals("Approved") || result.Status.Equals("ReviewerRejected")))
                        {
                            var resultRatedPower = context.DRRG_InverterModule_RP.Where(i => i.Inverter_ID != null && i.Inverter_ID.ToLower() == id.ToLower()).ToList();
                            var resultACApparentPower = context.DRRG_InverterModule_AP.Where(i => i.Inverter_ID != null && i.Inverter_ID.ToLower() == id.ToLower()).ToList();
                            var resultMaximumPower = context.DRRG_InverterModule_MAP.Where(i => i.Inverter_ID != null && i.Inverter_ID.ToLower() == id.ToLower()).ToList();

                            var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.HarmonicSpectrum ||
                                                       x.File_Type == FileType.DEWADRRGSTANDARDS ||
                                                       x.File_Type == FileType.IEC6100064 ||
                                                       x.File_Type == FileType.IEC6100063 ||
                                                       x.File_Type == FileType.IEC6100062 ||
                                                       x.File_Type == FileType.IEC6100061 ||
                                                       x.File_Type == FileType.IEC6100032 ||
                                                       x.File_Type == FileType.UL1741 ||
                                                       x.File_Type == FileType.IEC62109 ||
                                                       x.File_Type == FileType.ModelDataSheet ||
                                                       x.File_Type == FileType.IEC61000312) &&
                                                       x.Entity == FileEntity.Invertermodule &&
                                                       x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                       x.Reference_ID == id).ToList();


                            if (result != null)
                            {
                                InverterModule m = new InverterModule();
                                m.ModelName = result.Model_Name;
                                m.PowerDerating = Convert.ToString(result.Power_Derating);
                                m.DegreeofProtection = Convert.ToString(result.Protection_Degree);
                                m.DCACSection = result.AC_DC_Section;
                                m.Functionofstring = result.Function_String;
                                m.UsageCategory = result.Function_String;
                                m.InternalInterface = result.Internal_Interface;
                                m.MultiMPPTSection = result.MPPT_Section;
                                m.Numberofsection = result.Number_Section;
                                m.NumberofString = result.Number_String;
                                m.Id = result.id;
                                m.InverterID = result.Inverter_ID;
                                m.Manufacturer_Code = result.Manufacturer_Code;
                                m.Status = result.Status;
                                m.RatedPower = resultRatedPower?.Select(x => x.Rated_Power).ToList();
                                m.MaximumAcApparentPower = resultACApparentPower?.Select(x => x.AC_Apparent_Power).ToList();
                                m.MaximumActivePower = resultMaximumPower?.Select(x => x.Max_Active_Power).ToList();
                                m.PossibilityEarthing = result.Possibility_DC_Conductors;
                                if (!string.IsNullOrWhiteSpace(result.Power_Factor_Range) && !result.Power_Factor_Range.ToLower().Equals("other"))
                                    m.PowerFactorRange = result.Power_Factor_Range;
                                else
                                {
                                    m.PowerFactorRange = "Other";
                                    m.OtherPowerFactorRange = result.Power_Factor_Range;
                                }
                                m.NumberOfPhases = result.Number_Of_Phases;
                                m.RemoteControl = result.Remote_Control;
                                m.RemoteMonitoring = result.Remote_Monitoring;
                                m.LVRT = result.LVRT;
                                m.Multimasterfeature = result.Multi_Master_Feature;
                                if (resultfiles != null)
                                {
                                    foreach (var item in resultfiles)
                                    {
                                        if (!m.FileList.ContainsKey(item.File_Type))
                                            m.FileList.Add(item.File_Type, new fileResult { fileId = item.File_ID, fileName = item.Name, content = item.Content });
                                    }
                                }
                                return View("~/Views/Feature/DRRG/Dashboard/InverterModule.cshtml", m);
                            }
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return View("~/Views/Feature/DRRG/Dashboard/InverterModule.cshtml", new InverterModule());
        }

        [TwoPhaseDRRGAuthorize, HttpPost]
        public ActionResult AddInvertermodule(InverterModule model)
        {
            string error = string.Empty;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            //string ivid = (model.Id > 0 ? model.InverterID : InverterModuleCode());
            string ivid = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected"))
                {
                    ivid = InverterModuleCode();
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.HarmonicSpectrum ||
                                                       x.File_Type == FileType.DEWADRRGSTANDARDS ||
                                                       x.File_Type == FileType.IEC6100064 ||
                                                       x.File_Type == FileType.IEC6100063 ||
                                                       x.File_Type == FileType.IEC6100062 ||
                                                       x.File_Type == FileType.IEC6100061 ||
                                                       x.File_Type == FileType.IEC6100032 ||
                                                       x.File_Type == FileType.UL1741 ||
                                                       x.File_Type == FileType.IEC62109 ||
                                                       x.File_Type == FileType.ModelDataSheet ||
                                                       x.File_Type == FileType.IEC61000312) &&
                                                       x.Entity == FileEntity.Invertermodule &&
                                                       x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                       x.Reference_ID == model.InverterID).ToList();

                        if (model.ModelDataSheet == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.HarmonicSpectrum == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.HarmonicSpectrum)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.DewaDRRGStandard == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.DEWADRRGSTANDARDS)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document621091 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC62109)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document1741 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.UL1741)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document6100061 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC6100061)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document6100062 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC6100062)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document6100063 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC6100063)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document6100064 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC6100064)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document6100032 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC6100032)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document61000312 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC61000312)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                    }
                }
                else
                {
                    ivid = (model.Id > 0 ? model.InverterID : InverterModuleCode());
                }
                if ((string.IsNullOrWhiteSpace(model.InverterID) && (model.Status == null)) || (model.Status != null && model.Status.Equals("ReviewerRejected")))
                {
                    Addfile(model.ModelDataSheet, FileType.ModelDataSheet, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document621091, FileType.IEC62109, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document1741, FileType.UL1741, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document6100032, FileType.IEC6100032, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document61000312, FileType.IEC61000312, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document6100061, FileType.IEC6100061, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document6100062, FileType.IEC6100062, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document6100063, FileType.IEC6100063, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document6100064, FileType.IEC6100064, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.DewaDRRGStandard, FileType.DEWADRRGSTANDARDS, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.HarmonicSpectrum, FileType.HarmonicSpectrum, FileEntity.Invertermodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);

                }
                if (!string.IsNullOrWhiteSpace(model.signatureCopy))
                {
                    if (model.Status != null && model.Status.Equals("Approved"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var invresult = context.DRRG_InverterModule.Where(x => x.Inverter_ID == ivid).FirstOrDefault();
                            model.ModelName = invresult.Model_Name;
                            model.UsageCategory = invresult.Function_String;
                        }
                    }
                    var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
                    //var sizeInKb = sizeInBytes / 1000;
                    dRRG_Files_TYlist.Add(new DRRG_Files_TY
                    {
                        Name = ivid + "_Declaration.pdf",
                        ContentType = "application/pdf",
                        Extension = "pdf",
                        File_Type = FileType.SignatureCopy,
                        Entity = FileEntity.Invertermodule,
                        Content = GenerateDecalartionLetter(ivid, null, model, null),
                        Size = Convert.ToString(sizeInBytes),
                        Reference_ID = ivid,
                        Manufacturercode = CurrentPrincipal.BusinessPartner
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("DRRG_SignatureRequired"));
                }
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrWhiteSpace(model.InverterID) && (model.Status != null && (model.Status.Equals("Approved"))))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                            {
                                var signFile = dRRG_Files_TYlist.Where(x => x.File_Type == FileType.SignatureCopy).FirstOrDefault();
                                if (signFile != null)
                                {
                                    context.DRRG_Files.Add(new DRRG_Files
                                    {
                                        Name = signFile.Name,
                                        ContentType = signFile.ContentType,
                                        Extension = signFile.Extension,
                                        File_Type = signFile.File_Type,
                                        Entity = signFile.Entity,
                                        Content = signFile.Content,
                                        Size = signFile.Size,
                                        Reference_ID = signFile.Reference_ID,
                                        Manufacturer_Code = signFile.Manufacturercode
                                    });
                                    context.SaveChanges();
                                }
                            }
                            #region [Rated Power]
                            List<DRRG_InverterModule_RP> deleteItems = context.DRRG_InverterModule_RP.Where(x => x.Inverter_ID.ToLower() == model.InverterID.ToLower()).ToList();
                            foreach (var item in deleteItems)
                            {
                                context.DRRG_InverterModule_RP.Remove(item);
                                context.SaveChanges();
                            }
                            if (model.RatedPower != null && model.RatedPower.Count > 0)
                            {
                                foreach (string power in model.RatedPower)
                                {
                                    context.DRRG_InverterModule_RP.Add(new DRRG_InverterModule_RP
                                    {
                                        Inverter_ID = model.InverterID,
                                        Rated_Power = power
                                    });
                                    context.SaveChanges();
                                }
                            }
                            #endregion

                            #region [Max AC apparent power]
                            List<DRRG_InverterModule_AP> deleteAPItems = context.DRRG_InverterModule_AP.Where(x => x.Inverter_ID.ToLower() == model.InverterID.ToLower()).ToList();
                            foreach (var item in deleteAPItems)
                            {
                                context.DRRG_InverterModule_AP.Remove(item);
                                context.SaveChanges();
                            }
                            if (model.MaximumAcApparentPower != null && model.MaximumAcApparentPower.Count > 0)
                            {
                                foreach (string power in model.MaximumAcApparentPower)
                                {
                                    context.DRRG_InverterModule_AP.Add(new DRRG_InverterModule_AP
                                    {
                                        Inverter_ID = model.InverterID,
                                        AC_Apparent_Power = power
                                    });
                                    context.SaveChanges();
                                }
                            }
                            #endregion

                            #region [Max active power]
                            List<DRRG_InverterModule_MAP> deleteMapItems = context.DRRG_InverterModule_MAP.Where(x => x.Inverter_ID.ToLower() == model.InverterID.ToLower()).ToList();
                            foreach (var item in deleteMapItems)
                            {
                                context.DRRG_InverterModule_MAP.Remove(item);
                                context.SaveChanges();
                            }
                            if (model.MaximumActivePower != null && model.MaximumActivePower.Count > 0)
                            {
                                foreach (string power in model.MaximumAcApparentPower)
                                {
                                    context.DRRG_InverterModule_MAP.Add(new DRRG_InverterModule_MAP
                                    {
                                        Inverter_ID = model.InverterID,
                                        Max_Active_Power = power
                                    });
                                    context.SaveChanges();
                                }
                            }
                            #endregion

                            var updateItem = context.DRRG_InverterModule.Where(x => x.Inverter_ID == model.InverterID).FirstOrDefault();
                            if (updateItem != null)
                            {
                                updateItem.Status = "Updated";
                                updateItem.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                context.SaveChanges();
                            }
                            ViewBag.Referenceid = ivid; ViewBag.Module = FileEntity.Invertermodule;
                            DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Updated by " + CurrentPrincipal.Name, "Inverter Module", ivid, DateTime.Now, CurrentPrincipal.UserId, "Updated");
                            SendDRRGUpdateModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ivid, FileEntity.Invertermodule);
                            return View("~/Views/Feature/DRRG/Dashboard/ModuleSuccess.cshtml");
                        }
                    }
                    else
                    {
                        List<DRRG_Inverter_TY> DRRG_InverterModule = new List<DRRG_Inverter_TY>()
                    {
                        new DRRG_Inverter_TY
                        {
                            Userid = CurrentPrincipal.UserId,
                            Session = CurrentPrincipal.SessionToken,
                            Manufacturer_Code = (model.Id > 0 ? model.Manufacturer_Code : CurrentPrincipal.BusinessPartner),
                            InverterID= ivid,
                            Model_Name = model.ModelName,
                            AC_DC_Section = model.DCACSection,
                            //Function_String = model.Functionofstring,
                            Function_String = model.UsageCategory,
                            Number_String = model.NumberofString,
                            Internal_Interface = model.InternalInterface,
                            MPPT_Section = model.MultiMPPTSection,
                            Multi_Master_Feature = model.Multimasterfeature,
                            Number_Section= model.Numberofsection,
                            Power_Derating= model.PowerDerating,
                            Protection_Degree =model.DegreeofProtection,
                            Power_Factor_Range = (model.PowerFactorRange.ToLower() == "other" ? model.OtherPowerFactorRange : model.PowerFactorRange),
                            Possibility_DC_Conductors = model.PossibilityEarthing,
                            Number_Of_Phases = model.NumberOfPhases,
                            Remote_Control = model.RemoteControl,
                            Remote_Monitoring = model.RemoteMonitoring,
                            LVRT = model.LVRT,
                            Parent_Inv_ID = (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected") ? model.InverterID : string.Empty),
                        }
                    };
                        List<DRRG_Inverter_RP_TY> dRRG_Inverter_RP_s = new List<DRRG_Inverter_RP_TY>();
                        if (model.RatedPower != null && model.RatedPower.Count > 0)
                        {
                            foreach (string power in model.RatedPower)
                            {
                                dRRG_Inverter_RP_s.Add(new DRRG_Inverter_RP_TY { InverterID = ivid, Rated_Power = power });
                            }
                        }
                        List<DRRG_Inverter_AP_TY> dRRG_Inverter_AP_s = new List<DRRG_Inverter_AP_TY>();
                        if (model.MaximumAcApparentPower != null && model.MaximumAcApparentPower.Count > 0)
                        {
                            foreach (string power in model.MaximumAcApparentPower)
                            {
                                dRRG_Inverter_AP_s.Add(new DRRG_Inverter_AP_TY { InverterID = ivid, AC_Apparent_Power = power });
                            }
                        }
                        List<DRRG_Inverter_MAP_TY> dRRG_Inverter_MAP_s = new List<DRRG_Inverter_MAP_TY>();
                        if (model.MaximumActivePower != null && model.MaximumActivePower.Count > 0)
                        {
                            foreach (string power in model.MaximumActivePower)
                            {
                                dRRG_Inverter_MAP_s.Add(new DRRG_Inverter_MAP_TY { InverterID = ivid, Max_Active_Power = power });
                            }
                        }
                        Proc_DRRG_InsertInverterModule procedure = new Proc_DRRG_InsertInverterModule()
                        {
                            useremail = CurrentPrincipal.UserId,
                            DRRG_Inverter = DRRG_InverterModule,
                            DRRG_Inverter_RP = dRRG_Inverter_RP_s,
                            DRRG_Inverter_AP = dRRG_Inverter_AP_s,
                            DRRG_Inverter_MAP = dRRG_Inverter_MAP_s,
                            dRRG_Files_TY = dRRG_Files_TYlist
                        };
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            context.Database.ExecuteStoredProcedure(procedure);
                            string errormessage = procedure.error;
                            if (!string.IsNullOrWhiteSpace(procedure.error))
                            {
                                ModelState.AddModelError(string.Empty, GetDRRGErrormessage(procedure.error));
                            }
                            else
                            {
                                ViewBag.Referenceid = ivid; ViewBag.Module = FileEntity.Invertermodule;
                                if (model.Id <= 0 || (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected")))
                                {
                                    DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Submitted by " + CurrentPrincipal.Name, "Inverter Module", ivid, DateTime.Now, CurrentPrincipal.UserId, "In Progress");
                                    SendDRRGModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ivid, FileEntity.Invertermodule);
                                }
                                else
                                {
                                    DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Updated by " + CurrentPrincipal.Name, "Inverter Module", ivid, DateTime.Now, CurrentPrincipal.UserId, "Updated");
                                    SendDRRGUpdateModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ivid, FileEntity.Invertermodule);
                                }
                                return View("~/Views/Feature/DRRG/Dashboard/ModuleSuccess.cshtml");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/InverterModule.cshtml", new InverterModule());
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult AddInterfacemodule(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var result = context.DRRG_InterfaceModule.Where(i => i.Interface_ID != null && i.Interface_ID.ToLower() == id.ToLower()).FirstOrDefault();
                        var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.ModelDataSheet ||
                                                   x.File_Type == FileType.IEC610101 ||
                                                   x.File_Type == FileType.DEWADRRGSTANDARDS ||
                                                   x.File_Type == FileType.IEC61850) &&
                                                   x.Entity == FileEntity.Interfacemodule &&
                                                   x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                   x.Reference_ID == id).ToList();
                        if (result != null)
                        {
                            InterfaceModule m = new InterfaceModule();
                            m.Application = result.Application;
                            m.CommunicationProtocol = result.CommunicationProtocol;
                            m.Compliance = result.Compliance;
                            m.Id = result.id;
                            m.Interface_ID = result.Interface_ID;
                            m.Manufacturer_Code = result.Manufacturer_Code;
                            m.ModelName = result.Model_Name;
                            m.Status = result.Status;
                            if (resultfiles != null)
                            {
                                foreach (var item in resultfiles)
                                {
                                    if (!m.FileList.ContainsKey(item.File_Type))
                                        m.FileList.Add(item.File_Type, new fileResult { fileId = item.File_ID, fileName = item.Name, content = item.Content });
                                }
                            }
                            return View("~/Views/Feature/DRRG/Dashboard/InterfaceModule.cshtml", m);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return View("~/Views/Feature/DRRG/Dashboard/InterfaceModule.cshtml", new InterfaceModule());
        }

        [TwoPhaseDRRGAuthorize, HttpPost]
        public ActionResult AddInterfacemodule(InterfaceModule model)
        {
            string error = string.Empty;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            //string ivid = (model.Id > 0 ? model.Interface_ID : InterfaceModuleCode());
            string ivid = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected"))
                {
                    ivid = InterfaceModuleCode();
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var resultfiles = context.DRRG_Files.Where(x => (x.File_Type == FileType.ModelDataSheet ||
                                                   x.File_Type == FileType.IEC610101 ||
                                                   x.File_Type == FileType.DEWADRRGSTANDARDS ||
                                                   x.File_Type == FileType.IEC61850) &&
                                                   x.Entity == FileEntity.Interfacemodule &&
                                                   x.Manufacturer_Code == CurrentPrincipal.BusinessPartner &&
                                                   x.Reference_ID == model.Interface_ID).ToList();

                        if (model.ModelDataSheet == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.ModelDataSheet)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document61850 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC61850)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.DocumentDEWAStandard == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.DEWADRRGSTANDARDS)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                        if (model.Document61010 == null)
                        {
                            var fileModelDataSheet = resultfiles.Where(x => x.File_Type == FileType.IEC610101)?.FirstOrDefault();
                            if (fileModelDataSheet != null)
                            {
                                dRRG_Files_TYlist.Add(new DRRG_Files_TY
                                {
                                    Name = fileModelDataSheet.Name,
                                    ContentType = fileModelDataSheet.ContentType,
                                    Extension = fileModelDataSheet.Extension,
                                    File_Type = fileModelDataSheet.File_Type,
                                    Entity = fileModelDataSheet.Entity,
                                    Content = fileModelDataSheet.Content,
                                    Size = fileModelDataSheet.Size,
                                    Reference_ID = ivid,
                                    Manufacturercode = fileModelDataSheet.Manufacturer_Code
                                });
                            }
                        }
                    }
                }
                else
                {
                    ivid = (model.Id > 0 ? model.Interface_ID : InterfaceModuleCode());
                }
                if ((string.IsNullOrWhiteSpace(model.Interface_ID) && (model.Status == null)) || (model.Status != null && model.Status.Equals("ReviewerRejected")))
                {
                    Addfile(model.ModelDataSheet, FileType.ModelDataSheet, FileEntity.Interfacemodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document61010, FileType.IEC610101, FileEntity.Interfacemodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.DocumentDEWAStandard, FileType.DEWADRRGSTANDARDS, FileEntity.Interfacemodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    Addfile(model.Document61850, FileType.IEC61850, FileEntity.Interfacemodule, ivid, error, FileSizeLimit, supportedTypes, CurrentPrincipal.BusinessPartner, dRRG_Files_TYlist);
                    if (!string.IsNullOrWhiteSpace(model.signatureCopy))
                    {
                        var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
                        //var sizeInKb = sizeInBytes / 1000;
                        dRRG_Files_TYlist.Add(new DRRG_Files_TY
                        {
                            Name = ivid + "_Declaration.pdf",
                            ContentType = "application/pdf",
                            Extension = "pdf",
                            File_Type = FileType.SignatureCopy,
                            Entity = FileEntity.Interfacemodule,
                            Content = GenerateDecalartionLetter(ivid, null, null, model),
                            Size = Convert.ToString(sizeInBytes),
                            Reference_ID = ivid,
                            Manufacturercode = CurrentPrincipal.BusinessPartner
                        });
                    }

                }
                if (ModelState.IsValid)
                {
                    List<DRRG_Interface_TY> DRRG_InterfaceModule = new List<DRRG_Interface_TY>()
                {
                    new DRRG_Interface_TY
                    {
                        Userid = CurrentPrincipal.UserId,
                        Session = CurrentPrincipal.SessionToken,
                        Manufacturer_Code = (model.Id > 0 ? model.Manufacturer_Code : CurrentPrincipal.BusinessPartner),
                        InterfaceID= ivid,
                        Model_Name = model.ModelName,
                        ApplicationName = model.Application,
                        CommunicationProtocol = model.CommunicationProtocol,
                        Compliance = model.Compliance,
                        Parent_IP_ID = (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected") ? model.Interface_ID : string.Empty),
                    }
                };

                    Proc_DRRG_InsertInterfaceModule procedure = new Proc_DRRG_InsertInterfaceModule()
                    {
                        updateId = (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected") ? 0 : (model.Id > 0 ? model.Id : 0)),
                        useremail = CurrentPrincipal.UserId,
                        DRRG_Interface = DRRG_InterfaceModule,
                        dRRG_Files_TY = dRRG_Files_TYlist
                    };
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        context.Database.ExecuteStoredProcedure(procedure);
                        string errormessage = procedure.error;
                        if (!string.IsNullOrWhiteSpace(procedure.error))
                        {
                            ModelState.AddModelError(string.Empty, GetDRRGErrormessage(procedure.error));
                        }
                        else
                        {
                            ViewBag.Referenceid = ivid; ViewBag.Module = FileEntity.Interfacemodule;
                            if (model.Id <= 0 || (!string.IsNullOrWhiteSpace(model.Status) && model.Status.Equals("ReviewerRejected")))
                            {
                                DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Submitted by " + CurrentPrincipal.Name, "Interface Protection Module", ivid, DateTime.Now, CurrentPrincipal.UserId, "In Progress");
                                SendDRRGModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ivid, FileEntity.Interfacemodule);
                            }
                            else
                            {
                                DRRGExtensions.Logger(!string.IsNullOrWhiteSpace(model.ModelName) ? model.ModelName : "Model Name", "Updated by " + CurrentPrincipal.Name, "Interface Protection Module", ivid, DateTime.Now, CurrentPrincipal.UserId, "Updated");
                                SendDRRGUpdateModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ivid, FileEntity.Invertermodule);
                            }
                            return View("~/Views/Feature/DRRG/Dashboard/ModuleSuccess.cshtml");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/InterfaceModule.cshtml", new InterfaceModule());
        }
        private string GetViewStatus(string status)
        {
            string strStatus = string.Empty;

            if (!string.IsNullOrWhiteSpace(status) && (status.Equals("Submitted") || status.Equals("ReviewerApproved") || status.Equals("Updated") || status.Equals("Rejected")))
            {
                strStatus = Translate.Text("Under Evaluation");
            }
            else if (!string.IsNullOrWhiteSpace(status) && status.Equals("ReviewerRejected"))
            {
                strStatus = Translate.Text("Rejected");
            }
            else if (!string.IsNullOrWhiteSpace(status) && status.Equals("Approved"))
            {
                strStatus = Translate.Text("Published");
            }
            else
            {
                strStatus = status;
            }
            return strStatus;
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult ViewApplicationStatus()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var result = context.SP_DRRG_GETModules(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                    result.Where(y => y != null && !y.Status.Equals("Deleted")).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.modelname,
                        status = x.Status,
                        statusText = GetViewStatus(x.Status),
                        remarks = GetRemarks(x.Status, x.referenceid),
                        referenceNumber = x.referenceid,
                        equipmentType = Translate.Text(x.model),
                        type = x.model,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                    }));
                    Lstmodule = Lstmodule.OrderBy(x => x.datedtSubmitted).ToList();
                    CacheProvider.Store(CacheKeys.DRRG_MODULELIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/ViewApplicationStatus.cshtml");
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult ResubmissionRejectedApplication()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (RenderingContext.Current.Rendering.Parameters["type"] != null)
                {
                    int type = Convert.ToInt32(RenderingContext.Current.Rendering.Parameters["type"]);

                    ViewBag.Type = type;

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        //var result = context.SP_DRRG_GetRejectedApplicationModules(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId, (Int16)type).ToList();
                        switch (type)
                        {
                            case 1:
                                var pvresult = context.DRRG_PVMODULE.Where(x => x.Status.ToLower().Equals("ReviewerRejected") && x.Parent_PV_ID == null && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                                pvresult.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                                {
                                    id = x.id,
                                    modelName = x.Model_Name,
                                    status = x.Status,
                                    statusText = (!string.IsNullOrWhiteSpace(x.Status) && x.Status.Equals("ReviewerRejected") ? "Rejected" : x.Status),
                                    remarks = GetRemarks(x.Status, x.PV_ID),
                                    referenceNumber = x.PV_ID,
                                    type = Translate.Text("PVMODULE"),
                                    datedtSubmitted = x.CreatedDate,
                                    dateSubmitted = x.CreatedDate.ToString(),
                                }));
                                break;
                            case 2:
                                var invresult = context.DRRG_InverterModule.Where(x => x.Status.ToLower().Equals("ReviewerRejected") && x.Parent_Inv_ID == null && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                                invresult.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                                {
                                    id = x.id,
                                    modelName = x.Model_Name,
                                    status = x.Status,
                                    statusText = (!string.IsNullOrWhiteSpace(x.Status) && x.Status.Equals("ReviewerRejected") ? "Rejected" : x.Status),
                                    remarks = GetRemarks(x.Status, x.Inverter_ID),
                                    referenceNumber = x.Inverter_ID,
                                    type = Translate.Text("INVERTERMODULE"),
                                    datedtSubmitted = x.CreatedDate,
                                    dateSubmitted = x.CreatedDate.ToString(),
                                }));
                                break;
                            case 3:
                                var ipresult = context.DRRG_InterfaceModule.Where(x => x.Status.ToLower().Equals("ReviewerRejected") && x.Parent_IP_ID == null && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                                ipresult.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                                {
                                    id = x.id,
                                    modelName = x.Model_Name,
                                    status = x.Status,
                                    statusText = (!string.IsNullOrWhiteSpace(x.Status) && x.Status.Equals("ReviewerRejected") ? "Rejected" : x.Status),
                                    remarks = GetRemarks(x.Status, x.Interface_ID),
                                    referenceNumber = x.Interface_ID,
                                    type = Translate.Text("INTERFACEMODULE"),
                                    datedtSubmitted = x.CreatedDate,
                                    dateSubmitted = x.CreatedDate.ToString(),
                                }));
                                break;
                            default:
                                break;
                        }

                        Lstmodule = Lstmodule.OrderBy(x => x.datedtSubmitted).ToList();
                        CacheProvider.Store(CacheKeys.DRRG_RESUBMISSIONAPPLICATIONLIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/ResubmissionRejectedApplication.cshtml");
        }

        [HttpPost, TwoPhaseDRRGAuthorize]
        public ActionResult ResubmissionApplicationAjax(int pagesize = 5, string keyword = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG_RESUBMISSIONAPPLICATIONLIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGModules.Lstmodule.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize]
        public ActionResult ViewApplicationStatusAjax(int pagesize = 5, string keyword = "", string applicationtype = "", string statustxt = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG_MODULELIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(applicationtype))
                        {
                            if (applicationtype.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.PVmodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Invertermodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Interfacemodule.ToString())).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(statustxt))
                        {
                            if (statustxt.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Submitted") || x.status.Equals("Rejected") || x.status.Equals("Updated")).ToList();
                            }
                            else if (statustxt.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Approved")).ToList();
                            }
                            else if (statustxt.Equals("4"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("ReviewerRejected")).ToList();
                            }
                            else if (statustxt.Equals("6"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Deleted")).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGModules.Lstmodule.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize]
        public ActionResult ApplicationStatusAjax(int pagesize = 5, string keyword = "", string applicationtype = "", string statustxt = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG_MODULELIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(applicationtype))
                        {
                            if (applicationtype.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.PVmodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Invertermodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Interfacemodule.ToString())).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(statustxt))
                        {
                            if (statustxt.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Submitted")).ToList();
                            }
                            else if (statustxt.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Updated")).ToList();
                            }
                            else if (statustxt.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Approved")).ToList();
                            }
                            else if (statustxt.Equals("4"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("ReviewerRejected")).ToList();
                            }
                            else if (statustxt.Equals("5"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Rejected")).ToList();
                            }
                            else if (statustxt.Equals("6"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.Equals("Deleted")).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGModules.Lstmodule.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult GetDRRGModuleDetails(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {

                    if (id.ToLower().StartsWith("pv"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            //var pvresult = context.SP_DRRG_GETFilteredPVModulebyID(CurrentPrincipal.BusinessPartner, id, CurrentPrincipal.UserId).ToList();
                            var pvresult = context.DRRG_PVMODULE.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && x.PV_ID == id).ToList();
                            var pvfiltered = pvresult.Where(y => y != null);
                            if (pvfiltered != null && pvfiltered.ToList() != null && pvfiltered.Count() > 0)
                            {
                                //var pvresultfiles = context.SP_DRRG_GETFilesbyID(id, CurrentPrincipal.BusinessPartner).ToList();
                                var pvresultfiles = context.DRRG_Files.Where(x => x.Reference_ID == id && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                                var result = GetPVModuleDetail(id, pvresult, pvresultfiles);
                                return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml", result);
                            }
                        }
                    }
                    else if (id.ToLower().StartsWith("ip"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            //var ipresult = context.SP_DRRG_GETFilteredInterfaceModulebyID(CurrentPrincipal.BusinessPartner, id, CurrentPrincipal.UserId).ToList();
                            var ipresult = context.DRRG_InterfaceModule.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && x.Interface_ID == id).ToList();

                            var ipfiltered = ipresult.Where(y => y != null);
                            if (ipfiltered != null && ipfiltered.ToList() != null && ipfiltered.Count() > 0)
                            {
                                // var pvresultfiles = context.SP_DRRG_GETFilesbyID(id, CurrentPrincipal.BusinessPartner).ToList();
                                var pvresultfiles = context.DRRG_Files.Where(x => x.Reference_ID == id && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                                var result = GetInterfaceModuleDetail(id, ipresult, pvresultfiles);
                                return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml", result);
                            }
                        }
                    }
                    else if (id.ToLower().StartsWith("inv"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            //var ivresult = context.SP_DRRG_GETFilteredInverterModulebyID(CurrentPrincipal.BusinessPartner, id, CurrentPrincipal.UserId).ToList();
                            var ivresult = context.DRRG_InverterModule.Where(x => x.Inverter_ID == id && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();

                            var ivfiltered = ivresult.Where(y => y != null);
                            if (ivfiltered != null && ivfiltered.ToList() != null && ivfiltered.Count() > 0)
                            {
                                //var pvresultfiles = context.SP_DRRG_GETFilesbyID(id, CurrentPrincipal.BusinessPartner).ToList();
                                var pvresultfiles = context.DRRG_Files.Where(x => x.Reference_ID == id && x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();

                                var result = GetInverterModuleDetail(id, ivresult, pvresultfiles);
                                return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml", result);
                            }
                        }
                    }
                    else if (id.ToLower().StartsWith("manuf"))
                    {
                        if (id == CurrentPrincipal.BusinessPartner)
                        {
                            using (DRRGEntities context = new DRRGEntities())
                            {
                                // var manuresult = context.SP_DRRG_GETManufacturerbyID(id, "Approved").ToList();
                                var manuresult = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code == id && x.Status == "Approved").ToList();
                                if (manuresult != null && manuresult.Count() > 0)
                                {
                                    var factresult = context.SP_DRRG_GETFactorybyID(id).ToList();
                                    if (factresult != null && factresult.Count() > 0)
                                    {
                                        var pvresultfiles = context.SP_DRRG_GETFilesbyIDAdmin(id).ToList();
                                        var rejectedcomments = context.SP_DRRG_GETManuRejectedComments(id).ToList();
                                        var rejectedfiles = context.SP_DRRG_GETManuRejectedFilesbyIDAdmin(id).ToList();
                                        var result = GetManufacturerDetail(id, manuresult, factresult, pvresultfiles, rejectedcomments, rejectedfiles);
                                        result.UserRole = CurrentPrincipal.Role;
                                        return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml", result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml");
        }

        [HttpGet, TwoPhaseDRRGAuthorize]
        public ActionResult Attachment(string id)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var pvresultfiles = context.SP_DRRG_GETFilesbyfileID(id, CurrentPrincipal.BusinessPartner).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return null;
        }
        [HttpGet, TwoPhaseDRRGAuthorize]
        public ActionResult DownloadFile(string id)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    long fileId = Convert.ToInt64(id);
                    var pvresultfiles = context.DRRG_Files.Where(x => x.File_ID == fileId).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }
        public string GetNominalPowerDetails(DRRG_PVMODULE pvitem)
        {
            var nominalpower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var nominalPowerLst = context.DRRG_PVModule_Nominal.Where(x => x.PV_ID.ToLower() == pvitem.PV_ID.ToLower()).ToList();
                    if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals("Nominal Power Range"))
                    {
                        if (nominalPowerLst != null && nominalPowerLst.Count > 0)
                        {
                            nominalpower = nominalPowerLst[0].wp1 + " Wp - " + nominalPowerLst[0].wp2 + " Wp , in step of " + nominalPowerLst[0].wp3 + " W";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals("Multiple Nominal Power Entries"))
                    {
                        foreach (var item in nominalPowerLst)
                        {
                            if (!string.IsNullOrWhiteSpace(nominalpower))
                            {
                                nominalpower = nominalpower + ", " + item.wp1.ToString() + " Wp";
                            }
                            else
                            {
                                nominalpower = item.wp1 + " Wp";
                            }
                        }
                    }
                    else
                    {
                        if (nominalPowerLst != null && nominalPowerLst.Count > 0)
                            nominalpower = nominalPowerLst[0].wp1 + " Wp";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return nominalpower;
        }
        public string GetRatedPower(DRRG_InverterModule invmodule)
        {
            var RatedPower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var resultRP = context.DRRG_InverterModule_RP.Where(x => x.Inverter_ID.ToLower() == invmodule.Inverter_ID.ToLower()).ToList();

                    foreach (var item in resultRP)
                    {
                        if (!string.IsNullOrWhiteSpace(RatedPower))
                            RatedPower = RatedPower + ", " + item.Rated_Power.ToString() + " kW";
                        else
                            RatedPower = item.Rated_Power.ToString() + " kW";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return RatedPower;
        }
        public string GetACParentPower(DRRG_InverterModule invmodule)
        {
            var ACParentPower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var resultAP = context.DRRG_InverterModule_AP.Where(x => x.Inverter_ID.ToLower() == invmodule.Inverter_ID.ToLower()).ToList();

                    foreach (var item in resultAP)
                    {
                        if (!string.IsNullOrWhiteSpace(ACParentPower))
                            ACParentPower = ACParentPower + ", " + item.AC_Apparent_Power.ToString() + " kVA";
                        else
                            ACParentPower = item.AC_Apparent_Power.ToString() + " kVA";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return ACParentPower;
        }
        [HttpGet, TwoPhaseDRRGAuthorize]
        public ActionResult RemoveUpdateList()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {

                    var resultsPVModules = context.DRRG_PVMODULE.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && (x.Status.ToLower() == "approved")).ToList();
                    var resultsInverterModules = context.DRRG_InverterModule.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && (x.Status.ToLower() == "approved")).ToList();
                    var resultsInterfaceModules = context.DRRG_InterfaceModule.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && (x.Status.ToLower() == "approved")).ToList();
                    //var result = context.SP_DRRG_GETModules(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                    //int index = 0;
                    #region PV Modules
                    resultsPVModules.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.Model_Name,
                        status = x.Status,
                        remarks = "",
                        referenceNumber = x.PV_ID,
                        type = Translate.Text("PVMODULE"),
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        nominalpower = GetNominalPowerDetails(x),
                        celltechnology = x.Cell_Technology,
                        testMethod = x.Salt_Mist_Test_Method,
                        extraCompliance = getExtraCompliance(x.PV_ID, "pv", context),
                        manufacturerCode = x.Manufacturer_Code
                        //serialnumber = (Interlocked.Increment(ref index)).ToString()
                    }));
                    #endregion

                    #region Inverter Module
                    resultsInverterModules.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.Model_Name,
                        status = x.Status,
                        remarks = "",
                        referenceNumber = x.Inverter_ID,
                        type = Translate.Text("INVERTERMODULE"),
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        ratedpower = GetRatedPower(x),
                        acparentpower = GetACParentPower(x),
                        usageCategory = x.Function_String,
                        manufacturerCode = x.Manufacturer_Code
                        //serialnumber = (Interlocked.Increment(ref index)).ToString(),

                    }));
                    #endregion

                    #region Interface Module
                    resultsInterfaceModules.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.Model_Name,
                        status = x.Status,
                        remarks = "",
                        referenceNumber = x.Interface_ID,
                        type = Translate.Text("INTERFACEMODULE"),
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        application = x.Application,
                        communicationprotocol = x.CommunicationProtocol,
                        extraCompliance = getExtraCompliance(x.Interface_ID, "ip", context),
                        manufacturerCode = x.Manufacturer_Code
                        //serialnumber = (Interlocked.Increment(ref index)).ToString()
                    }));
                    #endregion

                    //Lstmodule = Lstmodule.OrderBy(x => x.datedtSubmitted).ToList();
                    //CacheProvider.Store(CacheKeys.DRRG_MODULELIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/RemoveUpdateList.cshtml", Lstmodule);
        }

        [TwoPhaseDRRGAuthorize, HttpGet]
        public ActionResult RemoveDRRGModule(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    if (id.ToLower().StartsWith("pv"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var pv = context.DRRG_PVMODULE.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && x.PV_ID == id).FirstOrDefault();
                            if (pv != null)
                            {
                                pv.Status = "Deleted";
                                context.Entry(pv).State = (System.Data.Entity.EntityState)EntityState.Modified;
                                context.SaveChanges();

                                DRRGExtensions.Logger(pv.Model_Name, "Deleted by " + CurrentPrincipal.Name, "PV Module", pv.PV_ID, DateTime.Now, CurrentPrincipal.UserId, "Deleted");

                                SendDRRGRemoveModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, pv.Model_Name, FileEntity.PVmodule);
                                return Json(new { status = true, Message = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    else if (id.ToLower().StartsWith("ip"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var ip = context.DRRG_InterfaceModule.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && x.Interface_ID == id).FirstOrDefault();
                            if (ip != null)
                            {
                                ip.Status = "Deleted";
                                context.Entry(ip).State = (System.Data.Entity.EntityState)EntityState.Modified;
                                context.SaveChanges();

                                DRRGExtensions.Logger(ip.Model_Name, "Deleted by " + CurrentPrincipal.Name, "Interface Protection Module", ip.Interface_ID, DateTime.Now, CurrentPrincipal.UserId, "Deleted");

                                SendDRRGRemoveModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, ip.Model_Name, FileEntity.Interfacemodule);
                                return Json(new { status = true, Message = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (id.ToLower().StartsWith("inv"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var iv = context.DRRG_InverterModule.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner && x.Inverter_ID == id).FirstOrDefault();
                            if (iv != null)
                            {
                                iv.Status = "Deleted";
                                context.Entry(iv).State = (System.Data.Entity.EntityState)EntityState.Modified;
                                context.SaveChanges();

                                DRRGExtensions.Logger(iv.Model_Name, "Deleted by " + CurrentPrincipal.Name, "Inverter Module", iv.Inverter_ID, DateTime.Now, CurrentPrincipal.UserId, "Deleted");

                                SendDRRGRemoveModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, iv.Model_Name, FileEntity.Invertermodule);
                                return Json(new { status = true, Message = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, TwoPhaseDRRGAuthorize]
        public ActionResult UpdateProfile()
        {
            RegistrationViewModel model = new RegistrationViewModel { CountryMobileList = GetCountryMobilelist(), NationalityList = GetCountrylist() };
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    //var result = context.SP_DRRG_GETManufacturerdetails(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                    var result = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();

                    if (result != null && result.Count() > 0)
                    {
                        var resultitem = result.FirstOrDefault();
                        //var factorydetails = context.SP_DRRG_GETFactorydetails(CurrentPrincipal.BusinessPartner, CurrentPrincipal.UserId).ToList();
                        var factorydetails = context.DRRG_Factory_Details.Where(x => x.Manufacturer_Code == CurrentPrincipal.BusinessPartner).ToList();
                        var resultfiles = context.SP_DRRG_GETFilesbyIDAdmin(CurrentPrincipal.BusinessPartner).ToList();
                        if (resultfiles != null)
                        {
                            foreach (var item in resultfiles)
                            {
                                if (!model.FileList.ContainsKey(item.File_ID.ToString()) && item.File_Type == FileType.SupportingDocument)
                                    model.FileList.Add(item.File_ID.ToString(), new fileResult { fileId = item.File_ID, fileName = item.Name, content = item.Content });
                            }
                        }
                        model.ManufacturerCode = resultitem.Manufacturer_Code;
                        model.ManufacturerId = resultitem.Manufacturer_ID;
                        model.ManufacturerFullName = resultitem.Manufacturer_Name;
                        model.BrandName = resultitem.Brand_Name;
                        model.Manufacturercountry = resultitem.Manufacturer_Country;
                        model.ManufacturerEmailAddress = resultitem.Corporate_Email;
                        model.ManufacturerPhoneNumber = resultitem.Corporate_Phone_Number;
                        model.ManufacturerPhonecode = resultitem.Corporate_Phone_Code;
                        model.ManufacturerFaxcode = resultitem.Corporate_Fax_Code;
                        model.ManufacturerFaxNumber = resultitem.Corporate_Fax_Number;
                        model.Website = resultitem.Website;
                        model.UserLocalRepresentative = resultitem.Local_Representative;
                        model.UserDesignation = resultitem.User_Designation;
                        model.UserEmail = resultitem.User_Email_Address;
                        model.Userfirstname = resultitem.User_First_Name;
                        model.UserLastName = resultitem.User_Last_Name;
                        model.UserGender = !string.IsNullOrWhiteSpace(resultitem.User_Gender) && resultitem.User_Gender.Equals(UserGender.Male); //= model.UserGender ? UserGender.Male : UserGender.Female,
                        model.UserPhoneNumber = resultitem.User_Mobile_Number;
                        model.UserPhonecode = resultitem.User_Mobile_Code;
                        model.UserCountry = resultitem.User_Nationality;
                        model.Userrepresentativename = resultitem.Company_Full_Name;
                        model.updateprofile = true;
                        if (factorydetails != null && factorydetails.Count() > 0)
                        {
                            List<Factory> factories = new List<Factory>();
                            factorydetails.ToList().ForEach(x =>
                            factories.Add(
                            new Factory
                            {
                                FactoryAddress = x.Address,
                                FactoryCountry = x.Country,
                                EOLPVModule = x.EOL_PV_Module,
                                FactoryFullName = x.Factory_Name,
                                FactoryCode = x.Factory_Code,
                                EOLFileBinary = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.EOLPVModule)?.Select(f => f.Content)?.FirstOrDefault(),
                                EOLFileName = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.EOLPVModule)?.Select(f => f.Name)?.FirstOrDefault(),
                                QMSFileBinary = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.QualityManagementSupport)?.Select(f => f.Content)?.FirstOrDefault(),
                                QMSFileName = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.QualityManagementSupport)?.Select(f => f.Name)?.FirstOrDefault(),
                                EnvironmentalFileBinary = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.EnvironmentalManagementSupport)?.Select(f => f.Content)?.FirstOrDefault(),
                                EnvironmentalFileName = context.DRRG_Files.Where(f => f.Reference_ID.ToLower() == x.Factory_Code.ToLower() && f.File_Type == FileType.EnvironmentalManagementSupport)?.Select(f => f.Name)?.FirstOrDefault(),
                            }));
                            model.factory = factories.ToArray();
                        }

                        model.TradeMarkLogoBinary = resultfiles.Where(x => x.File_Type == FileType.TrademarkLogo)?.Select(x => x.Content)?.FirstOrDefault();
                        model.TradeLicenseDocumentBinary = resultfiles.Where(x => x.File_Type == FileType.TradeLicenseDoc)?.Select(x => x.Content)?.FirstOrDefault();
                        model.TradeMarkLogo_FileName = resultfiles.Where(x => x.File_Type == FileType.TrademarkLogo)?.Select(x => x.Name)?.FirstOrDefault();
                        model.TradeLicenseDocument_FileName = resultfiles.Where(x => x.File_Type == FileType.TradeLicenseDoc)?.Select(x => x.Name)?.FirstOrDefault();

                        return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", null);
        }
        [HttpPost, TwoPhaseDRRGAuthorize]
        public ActionResult UpdateProfile(RegistrationViewModel model)
        {
            string error = string.Empty;

            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            model.ManufacturerCode = CurrentPrincipal.BusinessPartner;
            if (ModelState.IsValid)
            {
                try
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                #region [Update Signature]
                                if (!string.IsNullOrWhiteSpace(model.signatureCopy))
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.SignatureCopy).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    var sizeInBytes = Math.Ceiling((double)model.signatureCopy.Length / 4) * 3;
                                    context.DRRG_Files.Add(new DRRG_Files
                                    {
                                        Name = model.ManufacturerCode + "_Signature.png",
                                        ContentType = "image/png",
                                        Extension = "png",
                                        File_Type = FileType.SignatureCopy,
                                        Entity = FileEntity.Manufacturer,
                                        Content = Convert.FromBase64String(model.signatureCopy),
                                        Size = Convert.ToString(sizeInBytes),
                                        Reference_ID = model.ManufacturerCode,
                                        Manufacturer_Code = DRRGStandardValues.Registration
                                    });
                                    context.SaveChanges();
                                }
                                #endregion

                                #region [Update Trade Mark Logo]
                                if (model.TradeMarkLogo != null && model.TradeMarkLogo.ContentLength > 0)
                                {

                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.TrademarkLogo).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.TradeMarkLogo.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.TradeMarkLogo.FileName,
                                            ContentType = model.TradeMarkLogo.ContentType,
                                            Extension = model.TradeMarkLogo.GetTrimmedFileExtension(),
                                            File_Type = FileType.TrademarkLogo,
                                            Entity = FileEntity.Manufacturer,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.TradeMarkLogo.ContentLength.ToString(),
                                            Reference_ID = model.ManufacturerCode,
                                            Manufacturer_Code = DRRGStandardValues.Registration
                                        });
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #region [Update Trade License Document]
                                if (model.TradeLicenseDocument != null && model.TradeLicenseDocument.ContentLength > 0)
                                {
                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == model.ManufacturerCode.ToLower() && x.File_Type == FileType.TradeLicenseDoc).ToList();
                                    foreach (var item in rRG_Files)
                                    {
                                        context.DRRG_Files.Remove(item);
                                        context.SaveChanges();
                                    }
                                    using (MemoryStream memoryStream_8 = new MemoryStream())
                                    {
                                        model.TradeLicenseDocument.InputStream.CopyTo(memoryStream_8);
                                        context.DRRG_Files.Add(new DRRG_Files
                                        {
                                            Name = model.TradeLicenseDocument.FileName,
                                            ContentType = model.TradeLicenseDocument.ContentType,
                                            Extension = model.TradeLicenseDocument.GetTrimmedFileExtension(),
                                            File_Type = FileType.TradeLicenseDoc,
                                            Entity = FileEntity.Manufacturer,
                                            Content = memoryStream_8.ToArray() ?? new byte[0],
                                            Size = model.TradeLicenseDocument.ContentLength.ToString(),
                                            Reference_ID = model.ManufacturerCode,
                                            Manufacturer_Code = DRRGStandardValues.Registration
                                        });
                                        context.SaveChanges();
                                    }

                                }
                                #endregion

                                #region [Update Supporting Documents]
                                if (model.SupportingDocument != null && model.SupportingDocument.Count > 0)
                                {
                                    foreach (HttpPostedFileBase supportingdocument in model.SupportingDocument)
                                    {
                                        if (supportingdocument != null)
                                        {
                                            using (MemoryStream memoryStream_8 = new MemoryStream())
                                            {
                                                supportingdocument.InputStream.CopyTo(memoryStream_8);
                                                context.DRRG_Files.Add(new DRRG_Files
                                                {
                                                    Name = supportingdocument.FileName,
                                                    ContentType = supportingdocument.ContentType,
                                                    Extension = supportingdocument.GetTrimmedFileExtension(),
                                                    File_Type = FileType.SupportingDocument,
                                                    Entity = FileEntity.Manufacturer,
                                                    Content = memoryStream_8.ToArray() ?? new byte[0],
                                                    Size = supportingdocument.ContentLength.ToString(),
                                                    Reference_ID = model.ManufacturerCode,
                                                    Manufacturer_Code = DRRGStandardValues.Registration
                                                });
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                #endregion

                                #region [Update Factory]
                                if (model.factory != null && model.factory.Length > 0)
                                {
                                    model.factory.ToList().ForEach(x => { if (string.IsNullOrWhiteSpace(x.FactoryCode)) { x.FactoryCode = FactoryCode(); } });
                                    foreach (Factory factory in model.factory)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrWhiteSpace(factory.FactoryFullName))
                                            {
                                                DRRG_Factory_Details rRG_Factory_Details = context.DRRG_Factory_Details.Where(x => x.Factory_Code.ToLower().Equals(factory.FactoryCode.ToLower())).FirstOrDefault();
                                                if (rRG_Factory_Details != null)
                                                {
                                                    rRG_Factory_Details.Address = factory.FactoryAddress;
                                                    rRG_Factory_Details.Country = !string.IsNullOrWhiteSpace(factory.FactoryCountry) ? factory.FactoryCountry : "United Arab Emirates";
                                                    rRG_Factory_Details.EOL_PV_Module = factory.EOLPVModule;
                                                    rRG_Factory_Details.Factory_Name = factory.FactoryFullName;
                                                    rRG_Factory_Details.Factory_Code = factory.FactoryCode;
                                                    rRG_Factory_Details.Manufacturer_Code = model.ManufacturerCode;
                                                    context.SaveChanges();
                                                }
                                                else
                                                {
                                                    context.DRRG_Factory_Details.Add(new DRRG_Factory_Details
                                                    {
                                                        Address = factory.FactoryAddress,
                                                        Country = !string.IsNullOrWhiteSpace(factory.FactoryCountry) ? factory.FactoryCountry : "United Arab Emirates",
                                                        EOL_PV_Module = factory.EOLPVModule,
                                                        Factory_Name = factory.FactoryFullName,
                                                        Factory_Code = factory.FactoryCode,
                                                        Manufacturer_Code = model.ManufacturerCode,
                                                        Manufacturer_ID = model.ManufacturerId
                                                    });
                                                    context.SaveChanges();
                                                }
                                                if ((factory.EnvironmentalFilemarker != null && factory.EnvironmentalFilemarker.Equals("empty")) || (factory.EnvironmentalFile != null && factory.EnvironmentalFile.ContentLength > 0))
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.EnvironmentalManagementSupport).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    if (factory.EnvironmentalFile != null && factory.EnvironmentalFile.ContentLength > 0)
                                                    {
                                                        using (MemoryStream memoryStream_8 = new MemoryStream())
                                                        {
                                                            factory.EnvironmentalFile.InputStream.CopyTo(memoryStream_8);
                                                            context.DRRG_Files.Add(new DRRG_Files
                                                            {
                                                                Name = factory.EnvironmentalFile.FileName,
                                                                ContentType = factory.EnvironmentalFile.ContentType,
                                                                Extension = factory.EnvironmentalFile.GetTrimmedFileExtension(),
                                                                File_Type = FileType.EnvironmentalManagementSupport,
                                                                Entity = FileEntity.Factory,
                                                                Content = memoryStream_8.ToArray() ?? new byte[0],
                                                                Size = factory.EnvironmentalFile.ContentLength.ToString(),
                                                                Reference_ID = factory.FactoryCode,
                                                                Manufacturer_Code = model.ManufacturerCode
                                                            });
                                                            context.SaveChanges();
                                                        }
                                                    }
                                                }
                                                if ((factory.EOLFilemarker != null && factory.EOLFilemarker.Equals("empty")) || (factory.EOLFile != null && factory.EOLFile.ContentLength > 0))
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.EOLPVModule).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    if(factory.EOLFile != null && factory.EOLFile.ContentLength > 0)
                                                    {
                                                        using (MemoryStream memoryStream_8 = new MemoryStream())
                                                        {
                                                            factory.EOLFile.InputStream.CopyTo(memoryStream_8);
                                                            context.DRRG_Files.Add(new DRRG_Files
                                                            {
                                                                Name = factory.EOLFile.FileName,
                                                                ContentType = factory.EOLFile.ContentType,
                                                                Extension = factory.EOLFile.GetTrimmedFileExtension(),
                                                                File_Type = FileType.EOLPVModule,
                                                                Entity = FileEntity.Factory,
                                                                Content = memoryStream_8.ToArray() ?? new byte[0],
                                                                Size = factory.EOLFile.ContentLength.ToString(),
                                                                Reference_ID = factory.FactoryCode,
                                                                Manufacturer_Code = model.ManufacturerCode
                                                            });
                                                            context.SaveChanges();
                                                        }
                                                    }
                                                }
                                                if ((factory.QMSFilemarker != null && factory.QMSFilemarker.Equals("empty")) || (factory.QMSFile != null && factory.QMSFile.ContentLength > 0))
                                                {
                                                    List<DRRG_Files> rRG_Files = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.FactoryCode.ToLower() && x.File_Type == FileType.QualityManagementSupport).ToList();
                                                    foreach (var item in rRG_Files)
                                                    {
                                                        context.DRRG_Files.Remove(item);
                                                        context.SaveChanges();
                                                    }
                                                    if(factory.QMSFile != null && factory.QMSFile.ContentLength > 0)
                                                    {
                                                        using (MemoryStream memoryStream_8 = new MemoryStream())
                                                        {
                                                            factory.QMSFile.InputStream.CopyTo(memoryStream_8);
                                                            context.DRRG_Files.Add(new DRRG_Files
                                                            {
                                                                Name = factory.QMSFile.FileName,
                                                                ContentType = factory.QMSFile.ContentType,
                                                                Extension = factory.QMSFile.GetTrimmedFileExtension(),
                                                                File_Type = FileType.QualityManagementSupport,
                                                                Entity = FileEntity.Factory,
                                                                Content = memoryStream_8.ToArray() ?? new byte[0],
                                                                Size = factory.QMSFile.ContentLength.ToString(),
                                                                Reference_ID = factory.FactoryCode,
                                                                Manufacturer_Code = model.ManufacturerCode
                                                            });
                                                            context.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                DRRG_Factory_Details rRG_Factory_Details = context.DRRG_Factory_Details.Where(x => x.Factory_Code.ToLower().Equals(factory.FactoryCode.ToLower())).FirstOrDefault();
                                                if (rRG_Factory_Details != null)
                                                {
                                                    context.DRRG_Factory_Details.Remove(rRG_Factory_Details);
                                                    context.SaveChanges();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogService.Error(ex, this);
                                        }
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, Translate.Text("Add Factory"));
                                }
                                #endregion

                                #region [Update Manufacturer Details]
                                DRRG_Manufacturer_Details dRRG_Manufacturer_Details = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower().Equals(model.ManufacturerCode)).FirstOrDefault();
                                if (dRRG_Manufacturer_Details != null)
                                {
                                    //model.UserEmail = dRRG_Manufacturer_Details.User_Email_Address;
                                    dRRG_Manufacturer_Details.Brand_Name = model.BrandName;
                                    dRRG_Manufacturer_Details.Manufacturer_Name = model.ManufacturerFullName;
                                    dRRG_Manufacturer_Details.Manufacturer_Country = model.Manufacturercountry;
                                    dRRG_Manufacturer_Details.Corporate_Email = model.ManufacturerEmailAddress;
                                    dRRG_Manufacturer_Details.Corporate_Phone_Number = model.ManufacturerPhoneNumber;
                                    dRRG_Manufacturer_Details.Corporate_Phone_Code = model.ManufacturerPhonecode;
                                    dRRG_Manufacturer_Details.Corporate_Fax_Number = model.ManufacturerFaxNumber;
                                    dRRG_Manufacturer_Details.Corporate_Fax_Code = model.ManufacturerFaxcode;
                                    dRRG_Manufacturer_Details.Local_Representative = model.UserLocalRepresentative;
                                    dRRG_Manufacturer_Details.User_Designation = model.UserDesignation;
                                    dRRG_Manufacturer_Details.User_Email_Address = model.UserEmail;//dRRG_Manufacturer_Details.User_Email_Address;
                                    dRRG_Manufacturer_Details.User_First_Name = model.Userfirstname;
                                    dRRG_Manufacturer_Details.User_Last_Name = model.UserLastName;
                                    dRRG_Manufacturer_Details.User_Gender = model.UserGender ? UserGender.Male : UserGender.Female;
                                    dRRG_Manufacturer_Details.User_Mobile_Number = model.UserPhoneNumber;
                                    dRRG_Manufacturer_Details.User_Mobile_Code = model.UserPhonecode;
                                    dRRG_Manufacturer_Details.User_Nationality = model.UserCountry;
                                    dRRG_Manufacturer_Details.Company_Full_Name = model.Userrepresentativename;
                                    dRRG_Manufacturer_Details.Website = model.Website;
                                    dRRG_Manufacturer_Details.Status = "Updated";
                                    dRRG_Manufacturer_Details.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                    dRRG_Manufacturer_Details.Manufacturer_Code = model.ManufacturerCode;
                                    dRRG_Manufacturer_Details.Enabled = false;
                                    context.SaveChanges();

                                }
                                #endregion

                                #region [Update Login Details]
                                DRRG_UserLogin dRRG_UserLogin = context.DRRG_UserLogin.Where(x => x.Login_username.ToLower().Equals(CurrentPrincipal.Username.ToString())).FirstOrDefault();
                                if (dRRG_UserLogin != null)
                                {
                                    dRRG_UserLogin.Login_username = model.UserEmail;
                                    dRRG_UserLogin.Status = "Updated";
                                    dRRG_UserLogin.Name = model.Userfirstname + " " + model.UserLastName;
                                    dRRG_UserLogin.UpdatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture);
                                    context.SaveChanges();
                                }
                                #endregion

                                transaction.Commit();

                                ViewBag.Email = model.UserEmail;
                                //SendDRRGModuleEmail(CurrentPrincipal.Name, CurrentPrincipal.UserId, string.Empty, DRRGStandardValues.AuthorizedLetterSubmitted);
                                string linktext = GetEncryptedLinkExpiryURL(model.UserEmail, DRRGStandardValues.Registration);
                                ObjectParameter insertlinkParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                context.SP_DRRG_Insertpasswordlink(model.UserEmail, linktext, insertlinkParamresponse);

                                ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                context.SP_DRRG_Logout(CurrentPrincipal.Username, CurrentPrincipal.SessionToken, myOutputParamresponse);
                                string myString = Convert.ToString(myOutputParamresponse.Value);
                                ClearCookiesSignOut();
                                SendDRRGRegistrationEmail(model.Userfirstname + " " + model.UserLastName, model.UserEmail, model.UserLocalRepresentative, linktext, DRRGStandardValues.UpdateProfile, model.ManufacturerFullName, model.ManufacturerCode);
                                //ViewBag.SuccessText = Translate.Text("DRRG.update profile description");
                                //return View("~/Views/Feature/DRRG/Dashboard/UpdateProfileSuccess.cshtml");
                                CacheProvider.Store("UpdateProfileSuccess", new AccessCountingCacheItem<bool>(true, Times.Exactly(1)));
                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_UPDATEPROFILESUCCESS);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }

            model.CountryMobileList = GetCountryMobilelist();
            model.NationalityList = GetCountrylist();

            return View("~/Views/Feature/DRRG/Registration/Registration.cshtml", model);
        }

        [HttpGet, TwoPhaseDRRGAuthorize]
        public ActionResult UpdateProfileWarning()
        {
            return View("~/Views/Feature/DRRG/Dashboard/UpdateProfileWarning.cshtml");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckDuplicateApplication(string equipmentType, string modelname)
        {
            string responseMessage = string.Empty;
            string responseCode = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(equipmentType) && !string.IsNullOrEmpty(modelname))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        DRRG_PVMODULE pvDetail = null;
                        DRRG_InverterModule invDetail = null;
                        DRRG_InterfaceModule ipDetail = null;

                        responseCode = "000";
                        responseMessage = "Success";

                        if (equipmentType == FileEntity.PVmodule)
                        {
                            pvDetail = context.DRRG_PVMODULE.Where(x => x.Model_Name.ToLower().Equals(modelname.ToLower())
                            && (x.Status.Equals("Approved") || x.Status.Equals("Updated") || x.Status.Equals("Rejected") || x.Status.Equals("Submitted") || x.Status.Equals("ReviewerApproved")
                            )).FirstOrDefault();
                        }
                        if (equipmentType == FileEntity.Invertermodule)
                        {
                            invDetail = context.DRRG_InverterModule.Where(x => x.Model_Name.ToLower().Equals(modelname.ToLower())
                            && (x.Status.Equals("Approved") || x.Status.Equals("Updated") || x.Status.Equals("Rejected") || x.Status.Equals("Submitted") || x.Status.Equals("ReviewerApproved")
                            )).FirstOrDefault();
                        }
                        if (equipmentType == FileEntity.Interfacemodule)
                        {
                            ipDetail = context.DRRG_InterfaceModule.Where(x => x.Model_Name.ToLower().Equals(modelname.ToLower())
                            && (x.Status.Equals("Approved") || x.Status.Equals("Updated") || x.Status.Equals("Rejected") || x.Status.Equals("Submitted") || x.Status.Equals("ReviewerApproved")
                            )).FirstOrDefault();
                        }

                        if ((equipmentType == FileEntity.PVmodule && pvDetail != null) ||
                            (equipmentType == FileEntity.Invertermodule && invDetail != null) ||
                            (equipmentType == FileEntity.Interfacemodule && ipDetail != null))
                        {
                            responseMessage = Translate.Text("DRRG_ExistModelName");
                            responseCode = "399";
                            return Json(new { Message = responseMessage, errorCode = responseCode });
                        }

                    }
                }
                else
                {
                    responseCode = "399";
                    responseMessage = Translate.Text("DRRG.require.ivtype");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { Message = responseMessage, errorCode = responseCode });
        }
        [HttpGet]
        public ActionResult UpdateProfileSuccess()
        {
            try
            {
                bool isSuccess = false;
                if (CacheProvider.TryGet("UpdateProfileSuccess", out isSuccess) && isSuccess)
                {
                    ViewBag.SuccessText = Translate.Text("DRRG.update profile description");

                    return View("~/Views/Feature/DRRG/Dashboard/UpdateProfileSuccess.cshtml");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG_DASHBOARD);
        }
    }
}
