using DEWAXP.Foundation.Content.Controllers;
using System.Web.Mvc;

namespace DEWAXP.Feature.Events.Controllers
{
    public class PartnershipEventController : BaseController
    {       
        public const string REGISTERATION_PAGE = "{480E34F8-36F2-4AEA-9835-DEDE159BBCE3}";
        public ActionResult EventsPage()
        {
            if (Session["emailsubmitted"] == null)
            {
                //ViewBag.Error = "Invalid Context";
                return RedirectToSitecoreItem(REGISTERATION_PAGE);
            }

            return new EmptyResult();           
        }        
    }
}