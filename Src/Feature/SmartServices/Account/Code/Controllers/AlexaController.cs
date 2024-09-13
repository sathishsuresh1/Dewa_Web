using DEWAXP.Feature.Account.Models;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.APIHandler.Clients;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Alexa;
using DEWAXP.Foundation.Logger;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    public class AlexaController : BaseController
    {
        private readonly IAlexaClient _alexaClient;
        protected IAlexaClient AlexaClient => _alexaClient;
        public AlexaController() : base()
        {
            _alexaClient = DependencyResolver.Current.GetService<IAlexaClient>();
        }
        // GET: Alexa
        public ActionResult Login(oAuthreq model)
        {
            if (model.scope == null)
            {
                model.scope = "";
            }
            if (ModelState.IsValid)
            {
                CacheProvider.Store("Alexa_RequestModel", new CacheItem<oAuthreq>(model));
            }

            return View("~/Views/Feature/Account/Alexa/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ValidateCredential(model);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
            }

            return View("~/Views/Feature/Account/Alexa/Login.cshtml");
        }

        private bool ValidateCredential(LoginModel model)
        {
            try
            {
                var response = AlexaClient.ValidateCredential(model.Username, model.Password, RequestLanguageCode);
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.developerStatus.Equals("000"))
                {
                    SubmitLoginValues(response.Payload);
                    return true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
            }
            return false;
        }

        protected ActionResult SubmitLoginValues(Login_Res response)
        {
            try
            {
                CacheProvider.TryGet("Alexa_RequestModel", out oAuthreq oAuthreq);

                var url = string.Format(ConfigurationManager.AppSettings[ConfigKeys.ALEXA_URL], oAuthreq.client_id, oAuthreq.response_type, oAuthreq.scope, oAuthreq.state, response.userid, response.token, oAuthreq.redirect_uri);
                Response.Redirect(url, false);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                return null;
            }
        }
    }
}