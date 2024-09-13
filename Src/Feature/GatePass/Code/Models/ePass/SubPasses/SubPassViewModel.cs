using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GatePass.Models.ePass.SubPasses
{
    public class SubPassViewModel
    {
        public string subPassVisitorEmail { get; set; }
        public string subPassDeptApprovalDate { get; set; }
        public string subPassSecApprovedBy { get; set; }
        public string subPassSecurityApprovers { get; set; }
        public string subPassDepartmentApprovalRemarks { get; set; }
        public string subPassSecurityApprovalRemarks { get; set; }
        public string subPassValidTo { get; set; }
        public string subPassDepartmentApprover { get; set; }
        public string subPassSecApprovalDate { get; set; }
        public string subPassMainPassValidity { get; set; }
        public string subPassValidFrom { get; set; }
        public int ID { get; set; }
        public string subPassCreatedOn { get; set; }
        public string subPassJustification { get; set; }
        public string subPassDeptApprovedBy { get; set; }
        public string subPassDeviceModel { get; set; }
        public string subPassCreatedByEmail { get; set; }
        public string subPassDeviceSerialNo { get; set; }
        public string subPassDeviceType { get; set; }
        public string subPassStatus { get; set; }
        public string subPassNewPassType { get; set; }
        public string subPassRequestID { get; set; }
        public string subPassDeviceImage { get; set; }
        public string subPassMainPassNo { get; set; }
        public string subPassLocation { get; set; }
       
    }


    public class SubpassDetails: SubPassViewModel
    {
        public string status { get; set; }
    }
    /* public class SubPassViewModel
     {
         public string subPassDeptApprovalDate { get; set; }
         public string subPassSecApprovedBy { get; set; }
         public string subPassSecurityApprovers { get; set; }
         public string subPassDepartmentApprovalRemarks { get; set; }
         public string subPassSecurityApprovalRemarks { get; set; }
         public string subPassSecApprovalDate { get; set; }
         public string subPassID { get; set; }
         public string subPassJustification { get; set; }
         public string subPassDepartmentApprover { get; set; }
         public string subPassMainPassNo { get; set; }
         public string subPassDeptApprovedBy { get; set; }
         public string subPassDeviceModel { get; set; }
         public string subPassCreatedByEmail { get; set; }
         public string subPassDeviceSerialNo { get; set; }
         public string subPassDeviceType { get; set; }
         public string subPassStatus { get; set; }
         public string subPassNewPassType { get; set; }
         public string subPassDeviceImage { get; set; }
         public string subPassLocation { get; set; }
         public string subPassCreatedOn { get; set; }
         public string subPassValidFrom { get; set; }
         public string subPassValidTo { get; set; }
     }*/
}

/*
 {
    "executionTime": 30385,
    "values": [
        {
            "typeName": "T000121_ePass_SubRequest",
            "attribute": [
                {
                    "type": "text",
                    "name": "VisitorEmail",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "DeptApprovalDate",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "ValidTo",
                    "value": "30 September 2020 17:30"
                },
                {
                    "type": "text",
                    "name": "SecApprovedBy",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "SecurityApprovers",
                    "value": "manbahadur.thapa@dewa.gov.ae;"
                },
                {
                    "type": "text",
                    "name": "DepartmentApprovalRemarks",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "SecurityApprovalRemarks",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "SecApprovalDate",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "ValidFrom",
                    "value": "1 September 2020 05:30"
                },
                {
                    "type": "integer",
                    "name": "ID",
                    "value": "2"
                },
                {
                    "type": "text",
                    "name": "Justification",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "CreatedOn",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "DepartmentApprover",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "MainPassNo",
                    "value": "ST0830NKLK1F8B"
                },
                {
                    "type": "text",
                    "name": "InitiatoinDate",
                    "value": "2020-09-01 09:39:25.472"
                },
                {
                    "type": "text",
                    "name": "DeptApprovedBy",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "DeviceModel",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "CreatedByEmail",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "DeviceSerialNo",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "DeviceType",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "Status",
                    "value": "Initiated"
                },
                {
                    "type": "text",
                    "name": "NewPassType",
                    "value": "Holiday"
                },
                {
                    "type": "text",
                    "name": "RequestID",
                    "value": "DEWAePass_H2"
                },
                {
                    "type": "text",
                    "name": "MainPassValidity",
                    "value": "30 September 2020"
                },
                {
                    "type": "text",
                    "name": "DeviceImage",
                    "value": ""
                },
                {
                    "type": "text",
                    "name": "Location",
                    "value": ""
                }
            ]
        }
    ]
}
     */
