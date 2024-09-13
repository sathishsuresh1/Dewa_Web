using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.IdealHome.Models.IdealHome
{
     [SitecoreType(TemplateId = "{A318B424-665D-4533-ABA7-D69EFE016C38}", AutoMap = true)]
    public class SurveyEntities : GlassBase
    {
          
        [SitecoreField(FieldName = "DisplayName")]
        public virtual string EntityName { get; set; }

        [SitecoreField(FieldName = "UserName")]
        public virtual string UserName { get; set; }

        [SitecoreField(FieldName = "AssignedSurvey")]
        public virtual string AssignedSurvey { get; set; }

    }
}