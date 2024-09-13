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
    
    public partial class DRRG_Factory_Details
    {
        public long Factory_ID { get; set; }
        public long Manufacturer_ID { get; set; }
        public string Factory_Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string EOL_PV_Module { get; set; }
        public string Factory_Code { get; set; }
        public string Manufacturer_Code { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Status { get; set; }
        public string Reviewed_By { get; set; }
        public string Approved_By { get; set; }
        public string Published_By { get; set; }
    
        public virtual DRRG_Manufacturer_Details DRRG_Manufacturer_Details { get; set; }
    }
}
