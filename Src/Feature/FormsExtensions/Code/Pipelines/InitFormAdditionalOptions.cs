using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderForm;
using Sitecore.Mvc.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.FormsExtensions.Pipelines
{
    public class InitFormAdditionalOptions : MvcPipelineProcessor<RenderFormEventArgs>
    {
        public override void Process(RenderFormEventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            args.Attributes.Add("data-form-init", "true");
           
        }
    }
}                                                        