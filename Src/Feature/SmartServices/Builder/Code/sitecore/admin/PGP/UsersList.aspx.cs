using DEWAXP.Feature.Builder.Models.ProjectGeneration;
using DEWAXP.Foundation.Content;
using Glass.Mapper.Sc;
using Sitecore.Security.Authentication;
using Sitecore.sitecore.admin;
using System;
using System.Linq;

namespace DEWAXP.Feature.Builder.sitecore.admin.PGP
{
    public partial class UsersList : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new SitecoreService("master");

            var users = context.GetItem<ProjectUsers>(SitecoreItemPaths.PROJECTGENERATION_USERS);
            if (users != null && users.Users != null && users.Users.Any())
            {
                this.rptProjects.DataSource = users.Users;
                this.rptProjects.DataBind();
            }
            else
            {
                this.rptProjects.Visible = false;
                this.lblNoprojects.Visible = true;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            CheckSecurity(true); //Required!

            base.OnInit(e);
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            AuthenticationManager.Logout();
            Response.Redirect("UsersList.aspx");
        }
    }
}