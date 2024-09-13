using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.Conservation
{
    [Bind(Include = "LectureDetails,LectureTypeDetails,AcademiLevel,NumberOfAttendees,DateOfLecture,SuitableTime,CoordinateDetails")]
    public class FormLectureModel
    {
        public FormLectureModel() {
            FromHelpers = new ConservationFromHelpers();
            CoordinateDetails = new OtherDetails();
        }
        public string LectureDetails { get; set; }
        public string LectureTypeDetails { get; set; }

        public  ConservationFromHelpers FromHelpers { get; set; }

        public string AcademiLevel { get; set; }
        public string NumberOfAttendees { get; set; }
        public string DateOfLecture { get; set; }
        public string SuitableTime { get; set; }
        public OtherDetails  CoordinateDetails { get; set; }
    }
}