using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public enum UpdateStage
    {
        PersonalInformation = 0, ContactDetails = 1, AcademicInformation = 2, Questionair = 3, Completed = 4, Invalid=5
    }
    public class ProfileUpdateModel
    {
        public string CandidateProgram { get; set; }
        public string CandidateEmirate { get; set; }
        public string CandidateGender { get; set; }
        public UpdateStage Stage { get; set; }

        public PersonalDetail PersonalInformation { get; set; }

        public ContactDetail ContactInformation { get; set; }

        public AcademicDetail AcademicInformation { get; set; }

        public Questionnaire Questionnaire { get; set; }
        public string StepTitle
        {
            get
            {
                switch (this.Stage)
                {
                    default:
                    case UpdateStage.PersonalInformation:
                        return Translate.Text("Scholarship Personal Information");
                    case UpdateStage.ContactDetails:
                        return Translate.Text("Scholarship Contact Details");
                    case UpdateStage.AcademicInformation:
                        return Translate.Text("Scholarship Academic Information");
                    case UpdateStage.Questionair:
                        return Translate.Text("Scholarship Questionair");
                    case UpdateStage.Completed:
                        return Translate.Text("Scholarship Profile Completed");
                    case UpdateStage.Invalid:
                        return Translate.Text("Scholarship Invalid Context");

                }
            }
        }

        public string StepNumber
        {
            get
            {
                switch (this.Stage)
                {
                    default:
                    case UpdateStage.PersonalInformation:
                        return "1";
                    case UpdateStage.ContactDetails:
                        return "2";
                    case UpdateStage.AcademicInformation:
                        return "3";
                    case UpdateStage.Questionair:
                        return "4";
                    case UpdateStage.Completed:
                        return "5";

                }
            }
        }

    }
}