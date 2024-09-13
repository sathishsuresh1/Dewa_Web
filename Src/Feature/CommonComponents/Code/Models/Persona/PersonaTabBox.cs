using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Persona
{
    [SitecoreType(TemplateName = "PersonaTabBox", TemplateId = "{FD449878-7577-4D2A-88DD-34D15B700519}", AutoMap = true)]
    public class PersonaTabBox : GlassBase
    {
        [SitecoreField("Persona List")]
        public virtual IEnumerable<PersonaCategory> PersonaList { get; set; }

        /// <summary>
        /// Active Persona
        /// </summary>
        [SitecoreField("Active Persona")]
        public virtual PersonaCategory ActivePersona {get;set;}
    }
}