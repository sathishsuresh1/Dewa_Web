using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    public class ConservationFromHelpers
    {
        public ConservationFromHelpers()
        {
            LecturList = new List<SelectListItem>();
            AcademicList = new List<SelectListItem>();
            LectureTypeList = new List<SelectListItem>();
        }
        public List<SelectListItem> LecturList { get; set; }
        public List<SelectListItem> AcademicList { get; set; }
        public List<SelectListItem> LectureTypeList { get; set; }
    }

    public enum ConservationFromType
    {
        FormEducationalInstitution = 1,
        FormLeader = 2,
        FormTeam = 3,
        FormProject = 4,
        FormLecture = 5
    }

    public enum ConservationAcademics
    {
        [Description("Nursery")]
        Nursery = 1,
        [Description("Kindergarten/ Nursery (KG1-KG2)")]
        Kindergarten = 2,
        [Description("Primary (Grade 1-5)")]
        Primary = 3,
        [Description("Elementary (Grade 6-8)")]
        Elementary = 4,
        [Description("Intermediate")]
        Intermediate = 5,
        [Description("Secondary")]
        Secondary = 6,
        [Description("High School (Grade 9-13)")]
        HighSchool = 7,
        [Description("University /College")]
        UniversityCollege = 8,
        [Description("Centers for people with determination / Special Needs Center")]
        SpecialNeedsCenter = 9,
        [Description("Adult Education Centers")]
        AdultEducationCenters = 10

    }
    public enum ConservationEduAcademics
    {
        [Description("Kindergarten/ Nursery (KG1-KG2)")]
        Kindergarten = 3,
        [Description("Primary (Grade 1-5)")]
        Primary = 4,
        [Description("Elementary (Grade 6-8)")]
        Elementary = 5,
        [Description("High School (Grade 9-13)")]
        HighSchool = 6,
        [Description("University /College")]
        UniversityCollege = 7,
        [Description("Centers for people with determination / Special Needs Center")]
        SpecialNeedsCenter = 8,
    }

    public enum ConservationEduMethodOfEducation
    {
        [Description("100% virtual learning")]
        VirutalLearning100 = 1,
        [Description("50% virtual learning")]
        VirutalLearning50 = 2,
    }

    public enum ConservationTeamAcademics
    {
        [Description("Kindergarten/ Nursery (KG1-KG2)")]
        Kindergarten = 3,
        [Description("Primary (Grade 1-5)")]
        Primary = 4,
        [Description("Elementary (Grade 6-8)")]
        Elementary = 5,
        [Description("Centers for people with determination / Special Needs Center")]
        SpecialNeedsCenter = 6,
    }
}