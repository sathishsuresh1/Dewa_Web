// <copyright file="DewaStoreController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Dashboard.Controllers
{
    using DEWAXP.Feature.Dashboard.Models.DewaStore;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Models.DewaStore;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Integration.Responses.DewaStoreSvc;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Mvc.Presentation;
    using System;
    using System.Linq;
    using System.ServiceModel.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the <see cref="DewaStoreController" />.
    /// </summary>
    public class DewaStoreController : BaseController
    {
        /// <summary>
        /// The AllWebOffers.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult AllWebOffers()
        {
            if (RenderingRepository.HasDataSource && !DewastoreglobalConfiguration.ServiceOutage())
            {
                OfferConfig offerConfigdatasource = RenderingRepository.GetDataSourceItem<OfferConfig>();
                if (offerConfigdatasource != null && offerConfigdatasource.ModuleParameter != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleParameter.Text))
                {
                    if (offerConfigdatasource.ModuleParameter.Text.Equals("carousel"))
                    {
                        return View("~/Views/Feature/Dashboard/DewaStore/OffersCarousel.cshtml", offerConfigdatasource);
                    }
                    else if (offerConfigdatasource.ModuleParameter.Text.Equals("teaser"))
                    {
                        return View("~/Views/Feature/Dashboard/DewaStore/OffersTeaser.cshtml", offerConfigdatasource);
                    }
                }
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The GetWebOffers.
        /// </summary>
        /// <param name="account">The account<see cref="string"/>.</param>
        /// <param name="renderingid">The renderingid<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetWebOffers(string account, string renderingid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(renderingid))
                {
                    OfferConfig offerConfigdatasource = ContentRepository.GetItem<OfferConfig>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(renderingid)));
                    if (offerConfigdatasource != null)
                    {
                        var Taskresponse = fnofferresponse(account, renderingid, offerConfigdatasource);
                        if (Taskresponse != null && Taskresponse.Result != null)
                        {
                            var response = Taskresponse.Result;
                            if (response != null && response.Succeeded && response.Payload != null && response.Payload.Data != null && response.Payload.Data.Count > 0)
                            {
                                System.Collections.Generic.List<OfferDataViewModel> datamodel = response.Payload.Data.ToList().Select(x => new OfferDataViewModel
                                {
                                    CompanyName = x.CompanyName,
                                    DiscountPercentage = x.DiscountPercentage,
                                    LogoUrl = DewastoreglobalConfiguration.LogoURL() + x.LogoUrl,
                                    OfferBenefits = x.OfferBenefits,
                                    OfferDetails = x.OfferDetails,
                                    OfferName = x.OfferName,
                                    Row_Id = x.Row_Id,
                                    ThumbnailUrl = DewastoreglobalConfiguration.ThumbnailsURL() + x.ThumbnailUrl,
                                    Imagevalue = !string.IsNullOrWhiteSpace(x.ImageJsonValue) && CustomJsonConvertor.DeserializeObject<ImageJsonValue>(x.ImageJsonValue).OfferImages.Any() ?
                                     DewastoreglobalConfiguration.OffersURL() + CustomJsonConvertor.DeserializeObject<ImageJsonValue>(x.ImageJsonValue).OfferImages.FirstOrDefault() : string.Empty,
                                }).ToList();

                                DewaStoreOfferViewModel dewaStoreOfferViewModel = new DewaStoreOfferViewModel
                                {
                                    offerDataViewModels = datamodel,
                                    BackgroundImage = offerConfigdatasource.BackgroundImage != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.BackgroundImage.Src) ? offerConfigdatasource.BackgroundImage.Src : string.Empty,
                                    ModuleLinkUrl = offerConfigdatasource.ModuleLink != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleLink.Url) ? offerConfigdatasource.ModuleLink.Url : string.Empty,
                                    ModuleLinkdesc = offerConfigdatasource.ModuleLink != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleLink.Text) ? offerConfigdatasource.ModuleLink.Text : string.Empty,
                                    ModuleTitle = offerConfigdatasource.ModuleTitle,
                                };
                                if (offerConfigdatasource.ModuleParameter.Text.Equals("carousel"))
                                {
                                    return PartialView("~/Views/Feature/Dashboard/DewaStore/Partials/_WebOffersCarousel.cshtml", dewaStoreOfferViewModel);
                                }
                                else if (offerConfigdatasource.ModuleParameter.Text.Equals("teaser"))
                                {
                                    return PartialView("~/Views/Feature/Dashboard/DewaStore/Partials/_WebOffersTeaser.cshtml", dewaStoreOfferViewModel);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The GetGuestWebOffers.
        /// </summary>
        /// <param name="account">The account<see cref="string"/>.</param>
        /// <param name="renderingid">The renderingid<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetGuestWebOffers(string account, string renderingid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(renderingid))
                {
                    OfferConfig offerConfigdatasource = ContentRepository.GetItem<OfferConfig>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(renderingid)));
                    if (offerConfigdatasource != null)
                    {
                        var Taskresponse = fnguestofferresponse(account, renderingid, offerConfigdatasource);
                        if (Taskresponse != null && Taskresponse.Result != null)
                        {
                            var response = Taskresponse.Result;
                            if (response != null && response.Succeeded && response.Payload != null && response.Payload.Data != null && response.Payload.Data.Count > 0)
                            {
                                System.Collections.Generic.List<OfferDataViewModel> datamodel = response.Payload.Data.ToList().Select(x => new OfferDataViewModel
                                {
                                    CompanyName = x.CompanyName,
                                    DiscountPercentage = x.DiscountPercentage,
                                    LogoUrl = DewastoreglobalConfiguration.LogoURL() + x.LogoUrl,
                                    OfferBenefits = x.OfferBenefits,
                                    OfferDetails = x.OfferDetails,
                                    OfferName = x.OfferName,
                                    Row_Id = x.Row_Id,
                                    ThumbnailUrl = DewastoreglobalConfiguration.ThumbnailsURL() + x.ThumbnailUrl,
                                    Imagevalue = !string.IsNullOrWhiteSpace(x.ImageJsonValue) && CustomJsonConvertor.DeserializeObject<ImageJsonValue>(x.ImageJsonValue).OfferImages.Any() ?
                                     DewastoreglobalConfiguration.OffersURL() + CustomJsonConvertor.DeserializeObject<ImageJsonValue>(x.ImageJsonValue).OfferImages.FirstOrDefault() : string.Empty,
                                }).ToList();

                                DewaStoreOfferViewModel dewaStoreOfferViewModel = new DewaStoreOfferViewModel
                                {
                                    offerDataViewModels = datamodel,
                                    BackgroundImage = offerConfigdatasource.BackgroundImage != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.BackgroundImage.Src) ? offerConfigdatasource.BackgroundImage.Src : string.Empty,
                                    ModuleLinkUrl = offerConfigdatasource.ModuleLink != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleLink.Url) ? offerConfigdatasource.ModuleLink.Url : string.Empty,
                                    ModuleLinkdesc = offerConfigdatasource.ModuleLink != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleLink.Text) ? offerConfigdatasource.ModuleLink.Text : string.Empty,
                                    ModuleTitle = offerConfigdatasource.ModuleTitle,
                                };
                                if (offerConfigdatasource.ModuleParameter.Text.Equals("carousel"))
                                {
                                    return PartialView("~/Views/Feature/Dashboard/DewaStore/Partials/_WebOffersCarousel.cshtml", dewaStoreOfferViewModel);
                                }
                                else if (offerConfigdatasource.ModuleParameter.Text.Equals("teaser"))
                                {
                                    return PartialView("~/Views/Feature/Dashboard/DewaStore/Partials/_WebOffersTeaser.cshtml", dewaStoreOfferViewModel);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new EmptyResult();
        }

        public async Task<DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse>> fnofferresponse(string account, string renderingid, OfferConfig offerConfigdatasource)
        {
            return await Task.FromResult(((Func<DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse>>)(() =>
                          {
                              DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse> response1 = null;
                              try
                              {
                                  response1 = DewaStoreClient.GetAllWebOffers(
                                                                IsHotOffer: Convert.ToInt32(offerConfigdatasource.IsHotOffer),
                                                            IsFeaturedOffer: Convert.ToInt32(offerConfigdatasource.IsFeaturedOffer),
                                                            IsNewOffer: Convert.ToInt32(offerConfigdatasource.IsNewOffer),
                                                            CompanyUno: offerConfigdatasource.CompanyUno,
                                                            CategoryUno: offerConfigdatasource.CategoryUno,
                                                            PageSize: Convert.ToInt32(offerConfigdatasource.PageSize),
                                                            PageNumber: Convert.ToInt32(offerConfigdatasource.PageNumber),
                                                            Condition: Convert.ToInt32(offerConfigdatasource.Condition),
                                                            ContractAccNumber: account,
                                                            language: RequestLanguage
                                                            );
                              }
                              catch (Exception ex)
                              {
                                  LogService.Fatal(ex, this);
                              }
                              return response1;
                          }))());
        }

        public async Task<DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse>> fnguestofferresponse(string account, string renderingid, OfferConfig offerConfigdatasource)
        {
            return await Task.FromResult(((Func<DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse>>)(() =>
            {
                DEWAXP.Foundation.Integration.Responses.ServiceResponse<OfferBaseResponse> response1 = null;
                try
                {
                    response1 = DewaStoreClient.GetAllGuestOffers(
                                                  IsHotOffer: Convert.ToInt32(offerConfigdatasource.IsHotOffer),
                                              IsFeaturedOffer: Convert.ToInt32(offerConfigdatasource.IsFeaturedOffer),
                                              IsNewOffer: Convert.ToInt32(offerConfigdatasource.IsNewOffer),
                                              CompanyUno: offerConfigdatasource.CompanyUno,
                                              CategoryUno: offerConfigdatasource.CategoryUno,
                                              PageSize: Convert.ToInt32(offerConfigdatasource.PageSize),
                                              PageNumber: Convert.ToInt32(offerConfigdatasource.PageNumber),
                                              Condition: Convert.ToInt32(offerConfigdatasource.Condition),
                                              //ContractAccNumber: account,
                                              language: RequestLanguage
                                              );
                }
                catch (Exception ex)
                {
                    LogService.Fatal(ex, this);
                }
                return response1;
            }))());
        }
    }
}