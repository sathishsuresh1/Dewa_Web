using Sitecore.Annotations;
using Sitecore.Pipelines;
using System.Collections.Generic;
using System.Web.Optimization;

namespace DEWAXP.Foundation.Content.Pipeline
{
    public class RegisterPlatformBundles
    {
        [UsedImplicitly]
        public virtual void Process(PipelineArgs args)
        {
            RegisterBundles(BundleTable.Bundles);
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            #region [Dewa]

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                       "~/scripts/External/underscore-min.js",
                        "~/scripts/External/handlebars.min.js",
                        "~/scripts/External/moment/moment.min.js",
                        "~/scripts/External/moment/moment-with-locales.min.js",
                        "~/scripts/External/moment/locale/ar.js",
                        "~/scripts/External/numeral.min.js",
                        "~/scripts/DEWA/static.js",
                        "~/scripts/External/jquery-observe.min.js",
                        "~/scripts/External/jquery.highlight.min.js",
                        "~/scripts/External/jquery.fancybox.js",
                        "~/scripts/External/nml/nmlhelpers.js",
                        "~/scripts/External/datepicker.js"));
            var headscriptBundle = new ScriptBundle("~/bundles/headjs")
                        .Include("~/scripts/External/jquery.min.js")
                        .Include("~/scripts/External/jquery-ui.min.js")
                        .Include("~/scripts/External/eventie.min.js")
                        .Include("~/scripts/External/doc-ready.min.js");
            headscriptBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(headscriptBundle);
            #region Custom Scripts

            bundles.Add(new ScriptBundle("~/bundles/bes").Include("~/scripts/DEWA/bes.js"));
            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include("~/scripts/DEWA/dashboard.js"));
            bundles.Add(new ScriptBundle("~/bundles/manageaccountinfo").Include("~/scripts/DEWA/manageaccountinfo.js"));
            bundles.Add(new ScriptBundle("~/bundles/standard").Include("~/scripts/DEWA/standard.js"));
            bundles.Add(new ScriptBundle("~/bundles/totalbillamount").Include("~/scripts/DEWA/totalbillamount.js"));
            bundles.Add(new ScriptBundle("~/bundles/m87gaugealert").Include("~/scripts/DEWA/m87gaugealert.js"));
            bundles.Add(new ScriptBundle("~/bundles/amigraph").Include("~/scripts/DEWA/amigraph.js"));
            bundles.Add(new ScriptBundle("~/bundles/dtmc_track_map").Include("~/scripts/DEWA/dtmc.consumptiontracking.map.js"));
            bundles.Add(new ScriptBundle("~/bundles/evtotalbillamount").Include("~/scripts/DEWA/evtotalbillamount.js"));
            bundles.Add(new ScriptBundle("~/bundles/evbillcompare").Include("~/scripts/DEWA/evbillcompare.js"));
            bundles.Add(new ScriptBundle("~/bundles/evtransactions").Include("~/scripts/DEWA/evtransactions.js"));
            bundles.Add(new ScriptBundle("~/bundles/evconsumption").Include("~/scripts/DEWA/evconsumption.js"));
            bundles.Add(new ScriptBundle("~/bundles/listofevcards").Include("~/scripts/DEWA/listofevcards.js"));
            bundles.Add(new ScriptBundle("~/bundles/epassscript").Include("~/scripts/Epass/epass.js"));

            #endregion Custom Scripts

            bundles.Add(new StyleBundle("~/Styles/en").Include(
                        "~/styles/DEWA/static.en.css",
                        "~/styles/DEWA/typography.en.css",
                        "~/styles/External/jquery.fancybox.css",
                        "~/styles/DEWA/print.css"
            ));
            bundles.Add(new StyleBundle("~/Styles/ar").Include(
                        "~/styles/DEWA/static.ar.css",
                        "~/styles/DEWA/typography.ar.css",
                        "~/styles/External/jquery.fancybox.css",
                        "~/styles/DEWA/print.css"
            ));

            #endregion [Dewa]

            #region [Smart Communications]

            bundles.Add(new StyleBundle("~/Styles/SmartCommunication/en").Include(
                        "~/styles/SmartCommunication/static.en.css",
                        "~/styles/SmartCommunication/typography.en.css"
            ));
            bundles.Add(new StyleBundle("~/Styles/SmartCommunication/ar").Include(
                        "~/styles/SmartCommunication/static.ar.css",
                        "~/styles/SmartCommunication/typography.ar.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/smartcummunicationbodyjs").Include("~/Scripts/SmartCommunication/static.js"));
            bundles.Add(new ScriptBundle("~/bundles/smartcummunicationheadjs").Include("~/Scripts/External/jquery.min.js"));

            #endregion [Smart Communications]

            #region Career Portal
            var CareerheadenstyleBundle = new StyleBundle("~/Styles/Careers/en")
                        .Include("~/styles/CareerPortal/static.en.css")
                        .Include("~/styles/CareerPortal/typography.en.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/CareerPortal/print.css");
            CareerheadenstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(CareerheadenstyleBundle);
            var CareerheadarstyleBundle = new StyleBundle("~/Styles/Careers/ar")
                        .Include("~/styles/CareerPortal/static.ar.css")
                        .Include("~/styles/CareerPortal/typography.ar.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/CareerPortal/print.css");
            CareerheadarstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(CareerheadarstyleBundle);
            bundles.Add(new ScriptBundle("~/bundles/Careers/js").Include(
                       "~/scripts/External/underscore-min.js",
                        "~/scripts/External/handlebars.min.js",
                        "~/scripts/External/moment/moment.min.js",
                        "~/scripts/External/moment/moment-with-locales.min.js",
                        "~/scripts/External/moment/locale/ar.js",
                        "~/scripts/External/numeral.min.js",
                        "~/scripts/CareerPortal/static.js",
                        "~/scripts/External/jquery-observe.min.js",
                        "~/scripts/External/jquery.highlight.min.js",
                        "~/scripts/External/jquery.fancybox.js",
                        "~/scripts/External/nml/nmlhelpers.js",
                        "~/scripts/External/datepicker.js"));
            #endregion

            #region Emirati Women
            var EWheadenstyleBundle = new StyleBundle("~/Styles/EW/en")
                        .Include("~/styles/EW/static.en.css")
                        .Include("~/styles/EW/typography.en.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/EW/print.css");
            EWheadenstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(EWheadenstyleBundle);
            var EWheadarstyleBundle = new StyleBundle("~/Styles/EW/ar")
                        .Include("~/styles/EW/static.ar.css")
                        .Include("~/styles/EW/typography.ar.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/EW/print.css");
            EWheadarstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(EWheadarstyleBundle);
            bundles.Add(new ScriptBundle("~/bundles/EW/js").Include(
                        "~/scripts/External/jquery-observe.min.js",
                        "~/scripts/External/jquery.highlight.min.js",
                        "~/scripts/External/jquery.fancybox.js",
                        "~/scripts/EW/static.js"));
            #endregion


            #region CP Portal
            var CPPortalheadenstyleBundle = new StyleBundle("~/Styles/CPPortal/en")
                        .Include("~/styles/CPPortal/static.en.css")
                        .Include("~/styles/CPPortal/typography.en.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/CPPortal/print.css");
            CPPortalheadenstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(CPPortalheadenstyleBundle);
            var CPPortalheadarstyleBundle = new StyleBundle("~/Styles/CPPortal/ar")
                        .Include("~/styles/CPPortal/static.ar.css")
                        .Include("~/styles/CPPortal/typography.ar.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/CPPortal/print.css");
            CPPortalheadarstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(CPPortalheadarstyleBundle);
            bundles.Add(new ScriptBundle("~/bundles/CPPortal/js").Include(
                       "~/scripts/External/underscore-min.js",
                        "~/scripts/External/handlebars.min.js",
                        "~/scripts/External/moment/moment.min.js",
                        "~/scripts/External/moment/moment-with-locales.min.js",
                        "~/scripts/External/moment/locale/ar.js",
                        "~/scripts/External/numeral.min.js",
                        "~/scripts/CPPortal/static.js",
                        "~/scripts/External/jquery-observe.min.js",
                        "~/scripts/External/jquery.highlight.min.js",
                        "~/scripts/External/jquery.fancybox.js",
                        "~/scripts/External/nml/nmlhelpers.js",
                        "~/scripts/External/datepicker.js"));
            #endregion

            #region CP Portal
            var EpassheadenstyleBundle = new StyleBundle("~/Styles/Epass/en")
                        .Include("~/styles/Epass/static.en.css")
                        .Include("~/styles/Epass/typography.en.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/Epass/print.css");
            EpassheadenstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(EpassheadenstyleBundle);
            var EpassheadarstyleBundle = new StyleBundle("~/Styles/Epass/ar")
                        .Include("~/styles/Epass/static.ar.css")
                        .Include("~/styles/Epass/typography.ar.css")
                        .Include("~/styles/External/jquery.fancybox.css")
                        .Include("~/styles/Epass/print.css");
            EpassheadarstyleBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(EpassheadarstyleBundle);
            bundles.Add(new ScriptBundle("~/bundles/bodyepassjs").Include(
                       "~/scripts/External/underscore-min.js",
                        "~/scripts/External/handlebars.min.js",
                        "~/scripts/External/moment/moment.min.js",
                        "~/scripts/External/moment/moment-with-locales.min.js",
                        "~/scripts/External/moment/locale/ar.js",
                        "~/scripts/External/numeral.min.js",
                        "~/scripts/Epass/static.js",
                        "~/scripts/External/jquery-observe.min.js",
                        "~/scripts/External/jquery.highlight.min.js",
                        "~/scripts/External/jquery.fancybox.js",
                        "~/scripts/External/nml/nmlhelpers.js",
                        "~/scripts/External/datepicker.js"));
            #endregion

            #region [DRRG]
            bundles.Add(new StyleBundle("~/styles/DRRG/en").Include(
                        "~/styles/DRRG/static.en.css",
                        "~/styles/DRRG/typography.en.css"
            ));
            bundles.Add(new StyleBundle("~/styles/DRRG/ar").Include(
                        "~/styles/DRRG/static.ar.css",
                        "~/styles/DRRG/typography.ar.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/DRRGbodyjs").Include("" +
                "~/scripts/DRRG/static.js"
                ));
            var scriptBundle = new ScriptBundle("~/bundles/DRRGheadjs")
                        .Include("~/scripts/External/jquery.min.js")
                        .Include("~/scripts/External/jquery-ui.min.js")
                        .Include("~/scripts/External/eventie.min.js")
                        .Include("~/scripts/External/doc-ready.min.js")
                        .Include("~/scripts/External/handlebars.min.js");
            scriptBundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(scriptBundle);
            #endregion

        }
    }
    public sealed class AsIsBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}