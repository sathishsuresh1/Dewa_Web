using DEWAXP.Foundation.Integration;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;
using System.Collections;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class ApprovedEmailAction
    {
        private UserProfile _userProfile;

        public void Process(WorkflowPipelineArgs args)
        {
            if (args == null) return;
            ID iD = MainUtil.GetID(args.CommandItem["next state"], null);
            Database masterDb = Factory.GetDatabase("master");
            var workflowstate = masterDb.GetItem(iD);
            var emailtable = new Hashtable();
            if (workflowstate["Final"] == "1")
            {
                this.GetSubmitter(args);
                if (this._userProfile != null && !string.IsNullOrEmpty(this._userProfile.Email)) emailtable.Add(this._userProfile.Email, this._userProfile.Email);
            }

            this.ProcessEmail(emailtable, args);
        }

        private void ProcessEmail(Hashtable emailtable, WorkflowPipelineArgs args)
        {
            ProcessorItem processorItem = args.ProcessorItem;
            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();

            if (processorItem != null)
            {
                Item innerItem = processorItem.InnerItem;
                string from = GetText(innerItem, "from", args, false);
                string subject = GetText(innerItem, "subject", args, false);
                string body = GetText(innerItem, "message", args, true);
                string useEmail = GetText(innerItem, "Use Dewa Email Service", args, false);
                bool useEmailService = !string.IsNullOrEmpty(useEmail);
                foreach (DictionaryEntry emailuser in emailtable)
                {
                    if (!useEmailService)
                    {
                        var message = new MailMessage(from, emailuser.Key.ToString())
                        {
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };

                        //var nc = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["DEWA_SMPT_USER"],
                        //    ConfigurationManager.AppSettings["DEWA_SMPT_PASSWORD"]);// if u are using Gmail

                        var client = new SmtpClient
                        {
                            Port = 25,
                            Host = ConfigurationManager.AppSettings["DEWA_SMTP_HOST"],
                            EnableSsl = true,
                            UseDefaultCredentials = false
                            //Credentials = nc
                        };
                        client.Send(message);
                    }
                    else
                    {
                        emailserviceclient.SendEmail(from, emailuser.Key.ToString(), subject, body);
                    }
                }
            }
        }

        private string GetText(Item commandItem, string field, WorkflowPipelineArgs args, bool replacevariables)
        {
            string text = commandItem[field];
            if (!string.IsNullOrEmpty(text))
            {
                if (replacevariables)
                {
                    return ReplaceVariables(text, args);
                }
                return text;
            }
            return string.Empty;
        }

        private string ReplaceVariables(string text, WorkflowPipelineArgs args)
        {
            Item workflowItem = args.DataItem;

            text = text.Replace("$itemPath$", this.GetItemPath(args));
            text = text.Replace("$itemName$", workflowItem.DisplayName);
            text = text.Replace("$itemLanguage$", workflowItem.Language.ToString());
            text = text.Replace("$itemVersion$", workflowItem.Version.ToString());
            text = text.Replace("$itemSubmittedBy$", this._userProfile != null ? this._userProfile.FullName : string.Empty);
            text = text.Replace("$itemRejectedBy$", Context.GetUserName());
            if (args.CommentFields != null) text = text.Replace("$itemComments$", args.CommentFields.ToString());
            return text;
        }

        private void GetSubmitter(WorkflowPipelineArgs args)
        {
            Item contentItem = args.DataItem;
            IWorkflow contentWorkflow = contentItem.Database.WorkflowProvider.GetWorkflow(contentItem);
            WorkflowEvent[] contentHistory = contentWorkflow.GetHistory(contentItem);

            if (contentHistory.Length > 0)
            {
                string lastUser = contentHistory[contentHistory.Length - 1].User;
                User user = User.FromName(lastUser, false);
                this._userProfile = user.Profile;
            }
        }

        private string GetItemPath(WorkflowPipelineArgs args)
        {
            Item contentItem = args.DataItem;
            string domainName = HttpContext.Current.Request.Url.Host;
            string scheme = HttpContext.Current.Request.Url.Scheme;
            return scheme + "://" + domainName + "/sitecore/shell/Applications/Content Editor?id=" +
                   contentItem.ID.Guid + "&amp;vs=" + contentItem.Version + "&amp;la=" +
                   contentItem.Language + "&amp;sc_content=master&amp;fo=" + contentItem.ID.Guid +
                   "&amp;ic=People%2f16x16%2fcubes_blue.png&amp;he=Content+Editor&cl=0 ";
        }
    }
}