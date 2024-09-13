using DEWAXP.Feature.Account.Models.PrimaryAccount;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Models;
using DEWAXP.Foundation.Content.Models.MoveOut;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
    public class PrimaryAccountController : BaseWorkflowController<PrimaryAccountWorkflowState>
    {
        [HttpGet]
        public ActionResult SetPrimaryAccount()
        {
            Clear();

            bool registrationSuccessful;
            if (CacheProvider.TryGet(CacheKeys.REGISTRATION_SUCCESSFUL_STATE, out registrationSuccessful))
            {
                if (registrationSuccessful)
                {
                    ViewData.Add("RegistrationSuccessful", true);
                }
                CacheProvider.Remove(CacheKeys.REGISTRATION_SUCCESSFUL_STATE);
            }

            if (!CurrentPrincipal.HasPrimaryAccount)
            {
                //var accountListResponse = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
                var accountListResponse = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                if (!accountListResponse.Succeeded && accountListResponse.Message == "NoneActive")
                {
                    //var _authService = DependencyResolver.Current.GetService<IDewaAuthStateService>();
                    var profile = AuthStateService.GetActiveProfile();
                    if (profile != null)
                    {
                        profile.HasActiveAccounts = false;
                        profile.IsFirstRegistration = true;
                    }
                    AuthStateService.Save(profile);
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                }
                // If only one account is available, try to automatically set the primary account.
                if (accountListResponse.Succeeded && accountListResponse.Payload.Count() == 1)
                {
                    var response = DewaApiClient.SetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, accountListResponse.Payload[0].AccountNumber, CurrentPrincipal.TermsAndConditions, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        CurrentPrincipal.PrimaryAccount = accountListResponse.Payload[0].AccountNumber;
                        CurrentPrincipal.IsFirstRegistration = true;
                        AuthStateService.Save(CurrentPrincipal);

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J69_CUSTOMER_DASHBOARD);
                    }
                    ModelState.AddModelError(string.Empty, accountListResponse.Message);
                }

                CurrentPrincipal.IsFirstRegistration = false;
            }
            return PartialView("~/Views/Feature/Account/PrimaryAccount/_SetPrimaryAccount.cshtml");
        }

        [HttpGet]
        public ActionResult Confirm()
        {
            if (!InProgress())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J98_START);
            }

            return PartialView("~/Views/Feature/Account/PrimaryAccount/_Confirm.cshtml", new ConfirmModel
            {
                Account = State.Account,
                IsSuccess = State.Succeeded,
                Message = State.Message
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetPrimaryAccount(SelectedAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.SelectedAccountNumber) || "-".Equals(model.SelectedAccountNumber))
            {
                ModelState.AddModelError(string.Empty, Translate.Text("No accounts available"));
            }
            else
            {
                //var accounts = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
                var accounts = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

                var matched = accounts.Payload.FirstOrDefault(x => string.Equals(x.AccountNumber, model.SelectedAccountNumber));
                var account = SharedAccount.CreateFrom(matched);

                var validator = new Validator(State);
                foreach (var error in validator.ValidateSelectedAccount(account))
                {
                    this.ModelState.AddModelError(error.Field, error.Message);
                }

                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Feature/Account/PrimaryAccount/_SetPrimaryAccount.cshtml", model);
                }

                var response = DewaApiClient.SetPrimaryAccount(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, model.SelectedAccountNumber, CurrentPrincipal.TermsAndConditions, RequestLanguage, Request.Segment());

                State.Account = account;
                State.Succeeded = response.Succeeded;
                State.Message = response.Message;

                Save();

                CurrentPrincipal.PrimaryAccount = model.SelectedAccountNumber;

                AuthStateService.Save(CurrentPrincipal);

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J98_CONFIRM);
            }
            return PartialView("~/Views/Feature/Account/PrimaryAccount/_SetPrimaryAccount.cshtml", model);
        }

        protected override string Name
        {
            get { return CacheKeys.SET_PRIMARY_ACCOUNT_WORKFLOW_STATE; }
        }

        public class Validator
        {
            private readonly PrimaryAccountWorkflowState _workflowState;

            public Validator(PrimaryAccountWorkflowState workflowState)
            {
                _workflowState = workflowState;
            }

            public IEnumerable<Result> ValidateSelectedAccount(SharedAccount selectedAccount)
            {
                if (selectedAccount == null)
                {
                    yield return new Result { Field = string.Empty, Message = Translate.Text("No accounts available") };
                }
                //todo: put it back?
                //if (selectedAccount != null && !selectedAccount.Active)
                //{
                //	yield return new Result { Field = string.Empty, Message = "Account is not active." };
                //}
            }

            public class Result
            {
                public string Field { get; set; }
                public string Message { get; set; }
            }
        }
    }
}