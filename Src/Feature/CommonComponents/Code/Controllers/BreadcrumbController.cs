using DEWAXP.Feature.CommonComponents.Models.Breadcrumbs;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.CommonComponents.Controllers
{
    public class BreadcrumbController : BaseController
    {
        //private IContentRepository _contentRepository;
        //private IContextRepository _contextRepository;

        //public BreadcrumbController(IContentRepository contentRepository, IContextRepository contextRepository)
        //{
        //    _contentRepository = contentRepository;
        //    _contextRepository = contextRepository;
        //}

        public ActionResult Breadcrumb()
        {
            var viewModel = new List<BreadCrumbItemViewModel>();
            BreadCrumbModel currentItem = ContextRepository.GetCurrentItem<BreadCrumbModel>();

            if (Guid.Parse(SitecoreItemIdentifiers.HOME) != currentItem.Id)
            {
                viewModel.Add(new BreadCrumbItemViewModel
                {
                    Url = currentItem.Url,
                    Text = GetTitle(currentItem),
                    IsCurrent = true
                });
            }
            else
            {
                viewModel.Add(new BreadCrumbItemViewModel
                {
                    Url = currentItem.Url,
                    Text = GetTitle(currentItem),
                    IsCurrent = true
                });
                //  return new ContentResult();
            }

            var parent = currentItem.Parent;

            if (parent != null)
            {
                while (parent.ContentPath != "/sitecore/content" && parent.ContentPath != string.Empty)
                {
                    if (DoesItemHasPresentationDetails(parent.Item))
                    {
                        viewModel.Add(new BreadCrumbItemViewModel
                        {
                            Url = parent.Url,
                            Text = GetTitle(parent)
                        });

                        if (parent.Parent == null)
                        {
                            break;
                        }
                    }
                    parent = parent.Parent;
                }

                viewModel.Reverse(); // reverse order of parents to get in correct order
            }
            return View("~/Views/Feature/CommonComponents/Breadcrumb/Breadcrumb.cshtml", viewModel);
        }

        public ActionResult HeroBreadcrumb()
        {
            var viewModel = new List<BreadCrumbItemViewModel>();
            BreadCrumbModel currentItem = ContextRepository.GetCurrentItem<BreadCrumbModel>();

            if (Guid.Parse(SitecoreItemIdentifiers.HOME) != currentItem.Id)
            {
                viewModel.Add(new BreadCrumbItemViewModel
                {
                    Url = currentItem.Url,
                    Text = GetTitle(currentItem),
                    IsCurrent = true
                });
            }
            else
            {
                viewModel.Add(new BreadCrumbItemViewModel
                {
                    Url = currentItem.Url,
                    Text = GetTitle(currentItem),
                    IsCurrent = true
                });
                //  return new ContentResult();
            }

            var parent = currentItem.Parent;

            if (parent != null)
            {
                while (parent.ContentPath != "/sitecore/content" && parent.ContentPath != string.Empty)
                {
                    if (DoesItemHasPresentationDetails(parent.Item))
                    {
                        viewModel.Add(new BreadCrumbItemViewModel
                        {
                            Url = parent.Url,
                            Text = GetTitle(parent)
                        });

                        if (parent.Parent == null)
                        {
                            break;
                        }
                    }
                    parent = parent.Parent;
                }

                viewModel.Reverse(); // reverse order of parents to get in correct order
            }
            return View("~/Views/Feature/CommonComponents/Breadcrumb/HeroBreadcrumb.cshtml", viewModel);
        }

        private static string GetTitle(BreadCrumbModel parent)
        {
            return !string.IsNullOrEmpty(parent.MenuLabel) ? parent.MenuLabel : GetAlternativeTitle(parent);
        }

        private static string GetAlternativeTitle(BreadCrumbModel parent)
        {
            return !string.IsNullOrEmpty(parent.DisplayName) ? parent.DisplayName : parent.Name;
        }

        public bool DoesItemHasPresentationDetails(Item item)
        {
            return item.Fields[FieldIDs.LayoutField] != null && !String.IsNullOrEmpty(item.Fields[FieldIDs.LayoutField].Value);
        }
    }
}