//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DEWAXP.Foundation.CustomDB.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class PODEventRegistration
    {
        public System.Guid EventID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string LinkUrl { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Passcode { get; set; }
    }
}
