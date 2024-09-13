using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Feature.ChatBot.Models.Rammas
{
    public class RammasModel
    {
       public virtual Link RammasLink { get; set; }
        public virtual string Title { get; set; }
        public virtual string Close { get; set; }
    }
}