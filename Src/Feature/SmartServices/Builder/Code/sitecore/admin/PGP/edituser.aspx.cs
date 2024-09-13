using DEWAXP.Feature.Builder.Models.ProjectGeneration;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using Glass.Mapper.Sc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using Sitecore.sitecore.admin;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace DEWAXP.Feature.Builder.sitecore.admin.PGP
{
    public partial class EditUser : AdminPage
    {
        private Database masterDB = Factory.GetDatabase("master");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                var context = new SitecoreService("master");
                var UserId = Request.QueryString["id"];
                this.FillProjects();
                if (string.IsNullOrEmpty(UserId))
                {
                    this.btnReset.Visible = false;
                    this.btnDel.Visible = false;
                    return;
                }

                var user = context.GetItem<ProjectUser>(UserId);
                if (user != null)
                {
                    this.txtusername.Text = user.UserName;
                    this.txtEmailaddress.Text = user.Emailaddress;
                    this.txtcompanyname.Text = user.CompanyName;
                    this.txtCity.Text = user.City;
                    this.txtPobox.Text = user.POBox;
                    this.txtTelephone.Text = user.Telephone;
                    this.txtMobile.Text = user.Mobile;
                    this.txtFax.Text = user.Fax;
                    this.txtLocation.Text = user.CompanyLocation;

                    foreach (var project in user.Projects)
                    {
                        if (this.ddlProjects.Items.FindByValue(project.Id.ToString()) != null)
                        {
                            this.ddlProjects.Items.FindByValue(project.Id.ToString()).Selected = true;
                        }
                    }
                }
            }
        }

        private void FillProjects()
        {
            var context = new SitecoreService("master");
            var projects = context.GetItem<ProjectsFolder>(SitecoreItemPaths.PROJECTGENERATION_PROJECTS);
            this.ddlProjects.DataSource = projects.Projects;
            this.ddlProjects.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            CheckSecurity(true); //Required!

            base.OnInit(e);
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (!this.Page.IsValid) return;
            var context = new SitecoreService("master");
            var UserId = Request.QueryString["id"];
            Item myitem;
            if (!string.IsNullOrEmpty(UserId))
            {
                myitem = masterDB.GetItem(new ID(Guid.Parse(UserId)));
            }
            else
            {
                if (this.UserExists(this.txtusername.Text))
                {
                    this.lblerror.Text = "User with this username already exists. Please choose a different username";
                    return;
                }
                TemplateItem template = masterDB.GetItem(new ID(Guid.Parse("{BBAFD552-DE4B-44D0-8BDD-A30AD6A421FD}")));
                Item parentItem = masterDB.GetItem(SitecoreItemPaths.PROJECTGENERATION_USERS);

                // Add the item to the site tree
                myitem = parentItem.Add(this.txtusername.Text, template);
            }
            using (new SecurityDisabler())
            {
                string sprojects = string.Empty;
                myitem.Editing.BeginEdit();

                myitem["Emailaddress"] = this.txtEmailaddress.Text;
                myitem["CompanyName"] = this.txtcompanyname.Text;
                myitem["City"] = this.txtCity.Text;
                myitem["POBox"] = this.txtPobox.Text;
                myitem["Telephone"] = this.txtTelephone.Text;
                myitem["Mobile"] = this.txtMobile.Text;
                myitem["Fax"] = this.txtFax.Text;
                myitem["CompanyLocation"] = this.txtLocation.Text;
                foreach (ListItem optitem in this.ddlProjects.Items)
                {
                    if (optitem.Selected)
                    {
                        if (!string.IsNullOrEmpty(sprojects)) sprojects += "|";
                        sprojects += optitem.Value.ToUpper();
                    }
                }
                myitem["AssignedProjects"] = sprojects;
                myitem.Editing.EndEdit();
            }
            if (string.IsNullOrEmpty(UserId))
            {
                this.SetPassword(myitem.ID.Guid.ToString());
            }
            UtilExtensions.PublishItem(myitem);

            Response.Redirect("UsersList.aspx");
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("UsersList.aspx");
        }

        private bool UserExists(string username)
        {
            var context = new SitecoreService("master");

            var projectUsers = context.GetItem<ProjectUsers>(SitecoreItemPaths.PROJECTGENERATION_USERS);

            var currentUser =
                projectUsers.Users.FirstOrDefault(
                    c =>
                        c.UserName.ToLower() == username.ToLower());
            return currentUser != null;
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            AuthenticationManager.Logout();
            Response.Redirect("UsersList.aspx");
        }

        protected void btnReset_OnClick(object sender, EventArgs e)
        {
            var UserId = Request.QueryString["id"];
            this.SetPassword(UserId);
        }

        private void SetPassword(string UserId)
        {
            var context = new SitecoreService("master");
            var user = context.GetItem<ProjectUser>(UserId);
            string password = Membership.GeneratePassword(12, 1);

            Database masterDB = Factory.GetDatabase("master");
            Item myitem = masterDB.GetItem(new ID((UserId)));
            using (new SecurityDisabler())
            {
                myitem.Editing.BeginEdit();
                myitem["Password"] = password;
                myitem.Editing.EndEdit();
            }
            UtilExtensions.PublishItem(myitem);
            this.lblerror.Text = "Password reset successfully";
            this.SendPasswordToUser(password, user.Emailaddress, user.UserName);
        }

        protected void btnDel_OnClick(object sender, EventArgs e)
        {
            var UserId = Request.QueryString["id"];
            Database masterDB = Factory.GetDatabase("master");
            Item myitem = masterDB.GetItem(new ID((UserId)));
            using (new SecurityDisabler())
            {
                myitem.Editing.BeginEdit();
                myitem["__Never publish"] = "1";
                myitem.Editing.EndEdit();
                UtilExtensions.PublishItem(myitem);
                myitem.Recycle();
            }
            Response.Redirect("UsersList.aspx");
        }

        private void SendPasswordToUser(string password, string email, string username)
        {
            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
            var smtpuser = ConfigurationManager.AppSettings["DEWA_SMPT_USER"];
            string from = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["DEWA_FROMEMAIL"]) ? ConfigurationManager.AppSettings["DEWA_FROMEMAIL"] : "no-reply@dewa.gov.ae";
            var context = new SitecoreService("master");
            var emailtemplate = context.GetItem<FormattedText>(SitecoreItemIdentifiers.J86_EMAILTEMPlATE);
            var LoginItem = context.GetItem<Item>(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            string subject = "DEWA Partner User Information";
            var LOGINLINK = LinkHelper.GetItemUrl(LoginItem, false, setSite: true);
            string virtualFolder = LinkManager.GetDefaultUrlBuilderOptions().Site.VirtualFolder.TrimEnd('/');
            LOGINLINK = LOGINLINK.Replace(virtualFolder, "");
            var adminCMSHost = HttpContext.Current.Request.Url.Authority;
            var CDhost = Factory.GetSite("website").Properties["cdhostname"];
            LOGINLINK = LOGINLINK.Replace(adminCMSHost, CDhost);
            string body = emailtemplate != null ? emailtemplate.RichText : @"Dear Sir / Madam,
                        Your password has been reseted.
                        Your User ID: *USERNAME*
                        Your Password: *PASSWORD*
                    Please login at *LOGINLINK* to submit your documents.
                    Note: You need Internet Explorer version 8 or above to use the system.";
            body = body.Replace("*USERNAME*", username);
            body = body.Replace("*PASSWORD*", password);
            body = body.Replace("*LOGINLINK*", LOGINLINK);

            if (!string.IsNullOrEmpty(smtpuser))
            {
                var message = new MailMessage(from, email)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var nc = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["DEWA_SMPT_USER"],
                    ConfigurationManager.AppSettings["DEWA_SMPT_PASSWORD"]);// if u are using Gmail
                var client = new SmtpClient
                {
                    Port = 587,
                    Host = ConfigurationManager.AppSettings["DEWA_SMTP_HOST"],
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = nc,
                };
                client.Send(message);
            }
            else
            {
                emailserviceclient.SendEmail(from, email, subject, body);
            }
        }
    }
}