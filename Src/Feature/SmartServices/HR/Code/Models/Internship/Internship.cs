using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.HR.Models.Internship
{
    public class Internship
    {
        public string CountryCode { get; set; }
        public string National { get; set; }
        public string Program { get; set; }

        public string SQ { get; set; }

        public string Work_Dewa_Year { get; set; }

        public List<SelectListItem> RelationList { get; set; }
        public List<SelectListItem> CountryCallingCodesList { get; set; }

        public List<SelectListItem> DepartmentList { get; set; }

        public List<SelectListItem> DivisionList { get; set; }

        public string Name { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string University { get; set; }
        public string GradePointAverage { get; set; }
        public string Project { get; set; }
        public string Major { get; set; }
        public string Email_Address { get; set; }
        public string Mobile_Number { get; set; }

        public string Address { get; set; }

        public string Age { get; set; }

        public string Nationality { get; set; }

        public IEnumerable<SelectListItem> EmirateList { get; set; }

        public IEnumerable<SelectListItem> GradeList { get; set; }

        public IEnumerable<SelectListItem> SectionList { get; set; }

        public string Emirates { get; set; }

        public string WorkPlacement_From_Date { get; set; }

        public string WorkPlacement_To_Date { get; set; }

        public string WorkPlacement_Coordinator_Name { get; set; }

        public string WorkPlacement_Coordinator_Email { get; set; }

        public string DEWA_Scholarship { get; set; }

        public string DEWA_Scholarship_ID { get; set; }

        public string PassportNumber { get; set; }

        public string FamilyBookNumber { get; set; }

        public string Work_In_DEWA { get; set; }

        public string Work_Dewa_From_Date { get; set; }

        public string Work_Dewa_To_Date { get; set; }

        public string Department { get; set; }

        public string Division { get; set; }

        public string EducationLevel_High_School { get; set; }

        public string Semester { get; set; }

        public string Section { get; set; }

        public string EducationLevel_University { get; set; }

        public string Parent_Name { get; set; }
        public string Parent_RelationShip { get; set; }

        public string Parent_Home_Phone { get; set; }

        public string Parent_Home_Phone_Country_Code { get; set; }

        public string Parent_Mobile_Number { get; set; }

        public string Parent_Mobile_Number_Country_Code { get; set; }

        public string Relative_In_DEWA { get; set; }

        public string Relative_Name { get; set; }

        public string Relative_Relationship { get; set; }

        public string Relative_Department { get; set; }

        public string Relative_Division { get; set; }

        public string Relative_Mobile_Number { get; set; }

        public string Relative_Mobile_Country_Code { get; set; }

        public string DateOfBirth { get; set; }

        public HttpPostedFileBase File_Official_Letter { get; set; }
        public HttpPostedFileBase File_Survey_Questions { get; set; }
        public HttpPostedFileBase File_Interview_Questions { get; set; }
        public HttpPostedFileBase File_Project_Description { get; set; }
        public HttpPostedFileBase File_Passport { get; set; }

        public HttpPostedFileBase File_Picture { get; set; }

        public HttpPostedFileBase File_FamilyBook { get; set; }

        public HttpPostedFileBase File_CV { get; set; }
        public HttpPostedFileBase File_Transcript { get; set; }
    }
}