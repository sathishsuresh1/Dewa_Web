using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.sitecore.admin;
using Glass.Mapper.Sc;
using DEWAXP.Feature.Builder.Models.ProjectGeneration;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Content;
using Sitecore.ContentSearch.Utilities;

namespace DEWAXP.Feature.Builder.sitecore.admin.PGP
{
    public partial class EditProject : AdminPage
    {
        private Database masterDB = Factory.GetDatabase("master");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                var context = new SitecoreService("master");
                var ProjectId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(ProjectId))
                {
                    this.btnDel.Visible = false;
                    return;
                }
                var project = context.GetItem<Project>(ProjectId);
                if (project != null)
                {
                    this.txtProject.Text = project.ProjectName;
                    this.txtContract.Text = project.ContractNumber;
                    this.txtDMSFolder.Text = project.DMSFolder;
                    this.txtAclName.Text = project.AclName;
                    this.txtAclDomain.Text = project.AclDomain;
                }
            }
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
            var ProjectId = Request.QueryString["id"];

            if (!string.IsNullOrEmpty(ProjectId))
            {
                var item = masterDB.GetItem(new ID(Guid.Parse(ProjectId)));
                using (new SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    item["ProjectName"] = this.txtProject.Text;
                    item["ContractNumber"] = this.txtContract.Text;
                    item["DMSFolder"] = this.txtDMSFolder.Text;
                    item["AclName"] = this.txtAclName.Text;
                    item["AclDomain"] = this.txtAclDomain.Text;
                    item.Editing.EndEdit();
                }

                UtilExtensions.PublishItem(item);
            }
            else
            {
                TemplateItem template = masterDB.GetItem(new ID(Guid.Parse("{6EA199B3-EEDE-4402-8671-FC9B59928475}")));
                Item parentItem = masterDB.GetItem(SitecoreItemPaths.PROJECTGENERATION_PROJECTS);

                // Add the item to the site tree
                Item newItem = parentItem.Add(this.txtProject.Text, template);
                newItem.Editing.BeginEdit();
                newItem["ProjectName"] = this.txtProject.Text;
                newItem["ContractNumber"] = this.txtContract.Text;
                newItem["DMSFolder"] = this.txtDMSFolder.Text;
                newItem["AclName"] = this.txtAclName.Text;
                newItem["AclDomain"] = this.txtAclDomain.Text;
                newItem.Editing.EndEdit();
                UtilExtensions.PublishItem(newItem);
            }

            Response.Redirect("Projects.aspx");
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projects.aspx");
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            AuthenticationManager.Logout();
            Response.Redirect("Projects.aspx");
        }

        protected void btnDel_OnClick(object sender, EventArgs e)
        {
            var ProjectId = Request.QueryString["id"];
            Database masterDB = Factory.GetDatabase("master");
            Item myitem = masterDB.GetItem(new ID((ProjectId)));
            this.DetachProjectFromUser(ProjectId);
            using (new SecurityDisabler())
            {
                myitem.Editing.BeginEdit();
                myitem["__Never publish"] = "1";
                myitem.Editing.EndEdit();
                UtilExtensions.PublishItem(myitem);
                myitem.Recycle();
            }
            Response.Redirect("Projects.aspx");
        }

        private void DetachProjectFromUser(string ProjectId)
        {
            var context = new SitecoreService("master");
            var users = context.GetItem<ProjectUsers>(SitecoreItemPaths.PROJECTGENERATION_USERS);
            if (users != null && users.Users != null && users.Users.Any())
            {
                var affectedusers = users.Users.Where(c => c.Projects.Any(d => d.Id == Guid.Parse(ProjectId)));
                foreach (ProjectUser affecteduser in affectedusers)
                {
                    List<Project> projects = affecteduser.Projects.RemoveWhere(c => c.Id == Guid.Parse(ProjectId)).ToList();
                    string assignedprojs = string.Join<string>("|", projects.Select(x => x.Id.ToString()).ToArray());
                    Item myitem = masterDB.GetItem(new ID((affecteduser.Id)));
                    using (new SecurityDisabler())
                    {
                        myitem.Editing.BeginEdit();
                        myitem["AssignedProjects"] = assignedprojs;
                        myitem.Editing.EndEdit();
                        UtilExtensions.PublishItem(myitem);
                    }
                }
            }
        }
    }
}