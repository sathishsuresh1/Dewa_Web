//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DEWAXP.Foundation.CustomDB.DRRGDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class DRRG_PVModule_Nominal
    {
        public long id { get; set; }
        public string PV_ID { get; set; }
        public string wp1 { get; set; }
        public string wp2 { get; set; }
        public string wp3 { get; set; }
        public string mpv1 { get; set; }
        public string mpc1 { get; set; }
        public string ocv1 { get; set; }
        public string scc1 { get; set; }
        public string tci1 { get; set; }
        public string tcv1 { get; set; }
        public string noct1 { get; set; }
        public string npnoct1 { get; set; }
    
        public virtual DRRG_PVModule_Nominal DRRG_PVModule_Nominal1 { get; set; }
        public virtual DRRG_PVModule_Nominal DRRG_PVModule_Nominal2 { get; set; }
    }
}
