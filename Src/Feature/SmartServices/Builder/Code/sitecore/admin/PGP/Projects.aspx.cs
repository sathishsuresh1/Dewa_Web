using DEWAXP.Feature.Builder.Models.ProjectGeneration;
using DEWAXP.Foundation.Content;
using Glass.Mapper.Sc;
using Sitecore.Security.Authentication;
using Sitecore.sitecore.admin;
using System;
using System.Linq;

namespace DEWAXP.Feature.Builder.sitecore.admin.PGP
{
    public partial class Projects : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var sitecoreService = new SitecoreService();
            var context = new SitecoreService("master");

            var projects = context.GetItem<ProjectsFolder>(SitecoreItemPaths.PROJECTGENERATION_PROJECTS);
            if (projects != null && projects.Projects != null && projects.Projects.Any())
            {
                this.rptProjects.DataSource = projects.Projects;
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
            Response.Redirect("Projects.aspx");
        }
    }
}