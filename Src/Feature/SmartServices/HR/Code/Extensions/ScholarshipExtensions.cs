using DEWAXP.Feature.HR.Models.Scholarship;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Integration.ScholarshipService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using CNST = DEWAXP.Feature.HR.Models.Scholarship.Constants.AttachmentType;

namespace DEWAXP.Feature.HR.Extensions
{
    public static class ScholarshipExtensions
    {
        public static ProfileUpdateModel MapToModel(this candidateDetails candidate, UpdateStage stage)
        {
            ProfileUpdateModel m = new ProfileUpdateModel();
            m.Stage = stage;
            //var dts = candidate.studentFormData;
            //Note: EmiratesID issue date is not provided by SAP service, message Manhoar later

            m.CandidateProgram = candidate.studentFormData.scholarshipProgram;
            m.CandidateEmirate = candidate.studentFormData.emirate;
            m.CandidateGender = candidate.studentFormData.gender;

            switch (stage)
            {
                default:
                case UpdateStage.PersonalInformation:
                    m.PersonalInformation = MapToPersonalDetail(candidate.studentFormData);
                    if (candidate.attachmentData == null) break;
                    foreach (var a in candidate.attachmentData)
                    {
                        #region Personal Information Attachments

                        if (a.mimetype.Equals(CNST.AT_PHOTO))
                        {
                            m.PersonalInformation.ExistingPhoto = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_EMIRATES_ID))
                        {
                            m.PersonalInformation.ExistingEmiratesID = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_PASSPORT))
                        {
                            m.PersonalInformation.ExistingPassport = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_FAMILY_BOOK))
                        {
                            m.PersonalInformation.ExistingFamilyBook = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }

                        #endregion Personal Information Attachments
                    }

                    break;

                case UpdateStage.ContactDetails:
                    m.ContactInformation = MapToContactDetail(candidate.studentFormData);

                    break;

                case UpdateStage.AcademicInformation:
                    m.AcademicInformation = MapToAcademicDetail(candidate.studentFormData, candidate.educationDataList);
                    if (candidate.educationDataList != null && candidate.educationDataList.Length > 0)
                    {
                        m.AcademicInformation.Eduction = candidate.educationDataList.Select(x => new Eduction()
                        {
                            // ID = x.level,
                            Country = x.country,
                            EndDate = x.endDate,
                            GradeOrPercentage = x.grade,
                            Level = x.level,
                            Major = x.major,
                            Region = x.region,
                            School = x.institute,
                            StartDate = x.startDate
                        }).ToList();
                        m.AcademicInformation.EducationJson = JsonConvert.SerializeObject(m.AcademicInformation.Eduction);
                    }
                    if (candidate.attachmentData == null) break;
                    foreach (var a in candidate.attachmentData)
                    {
                        #region Academic Information Attachments

                        if (a.mimetype.Equals(CNST.AT_ACADEMIC_CERTIFICATE))
                        {
                            m.AcademicInformation.ExistingAcademicCertificate = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }

                        if (a.mimetype.Equals(CNST.AT_GRADE12_FINAL_CERTIFICATE))
                        {
                            m.AcademicInformation.ExistingGrade12FinalCertificate = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_CO_GOOD_CONDUCT))
                        {
                            m.AcademicInformation.ExistingCertificateOfGoodConduct = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_POLICE_CLEARANCE))
                        {
                            m.AcademicInformation.ExistingPoliceCertificate = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_PEOPLE_OF_DETERMINATION))
                        {
                            m.AcademicInformation.ExistingPODCard = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }
                        if (a.mimetype.Equals(CNST.AT_UNIVERSITY_TRANSCRIPT))
                        {
                            m.AcademicInformation.ExistingUniversityTranscript = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }

                        #endregion Academic Information Attachments
                    }
                    break;

                case UpdateStage.Questionair:
                    m.Questionnaire = MapToQuestionnaire(candidate.studentFormData);
                    if (candidate.attachmentData == null) break;
                    foreach (var a in candidate.attachmentData)
                    {
                        #region Questionnaire

                        if (a.mimetype.Equals(CNST.AT_MILITARY__SERVICE_STATUS))
                        {
                            m.Questionnaire.ExistingMilitaryServiceUpload = new Attachment() { FileName = a.filename, DownloadUrl = a.url }; continue;
                        }

                        #endregion Questionnaire
                    }

                    break;

                case UpdateStage.Completed:
                    break;
            }

            return m;
        }

        public static studentFormData MapToModel(this PersonalDetail model)
        {
            studentFormData st = new studentFormData()
            {
                applicationSource = model.SourceOfApplication,
                firstName = model.FirstName,
                middleName = model.MiddleName,
                lastName = model.LastName,
                fullName = model.FullNameInArabic,
                nationality = model.Nationality,
                dateOfBirth = model.DateOfBirth,
                gender = model.Gender,
                birthPlace = model.PlaceOfBirth,
                passportNumber = model.PassportNumber,
                passportIssueDate = model.PassportIssueDate,
                passportExpiryDate = model.PassportExpiryDate,
                passportPlaceIssue = model.PassportPlaceOfIssue,
                emiratesId = model.EmiratesID,
                emiratesIdExpiryDate = model.EIDExpiryDate,
                fathersFirstName = model.FatherFirstName,
                fathersMiddleName = model.FatherMiddleName,
                fathersLastName = model.FatherLastName,
                mothersFirstName = model.MotherFirstName,
                mothersMiddleName = model.MotherMiddleName,
                mothersLastName = model.MotherLastName,
                familyBookNo = model.FamilyBookNumber
            };
            return st;
        }

        public static studentFormData MapToModel(this ContactDetail model)
        {
            studentFormData fd = new studentFormData()
            {
                studentEmail = model.EmailAddress,
                studentMobile = model.CandidateMobileNo,
                parentMobile = model.FatherMobileNo,
                mothersMobileNo = model.MotherMobileNo,
                parentEmail = model.FatherEamilAddress,
                mothersEmail = model.MotherEmailAddress,
                street = model.StreetAddress,
                emirate = model.Emirate,
                area = model.AreaOfEmirate,
                poBox = model.POBox,
                postalCode = model.PostalCode
            };

            return fd;
        }

        public static studentFormData MapToModel(this AcademicDetail model)
        {
            studentFormData fd = new studentFormData()
            {
                scholarshipProgram = model.Program,
                university = model.University,
                otherUniversity = model.OtherUniversity,
                major = model.Major,
                otherMajor = model.OtherMajor
            };

            model.Eduction = string.IsNullOrEmpty(model.EducationJson) ? new List<Eduction>() : CustomJsonConvertor.DeserializeObject<List<Eduction>>(model.EducationJson);
            /*foreach(var e in model.Eduction)
            {
                e.Level =e.ID;
            }*/

            return fd;
        }

        public static studentEducationData[] MapToEducationData(this AcademicDetail model)
        {
            //model.Eduction = JsonConvert.DeserializeObject<List<Eduction>>(model.EducationJson);

            return model.Eduction.Select(x => new studentEducationData()
            {
                country = x.Country,
                endDate = x.EndDate,
                grade = x.GradeOrPercentage,
                institute = x.School,
                level = x.Level,
                major = x.Major,
                region = x.Region,
                startDate = x.StartDate
            }).ToArray();
        }

        public static studentFormData MapToModel(this Questionnaire model)
        {
            return new studentFormData()
            {
                milatryStatus = model.MilitaryStatus,
                medicalCondition = model.AnyMedicalCondition,
                medicalReason = model.MedicalConditionDetail
            };
        }

        #region Private methods

        private static PersonalDetail MapToPersonalDetail(studentFormData dts)
        {
            return new PersonalDetail()
            {
                FirstName = dts.firstName,
                MiddleName = dts.middleName,
                LastName = dts.lastName,
                FullNameInArabic = dts.fullName,
                Nationality = dts.nationality,
                DateOfBirth = dts.dateOfBirth,
                Gender = dts.gender,
                PlaceOfBirth = dts.birthPlace,
                PassportNumber = dts.passportNumber,
                PassportIssueDate = dts.passportIssueDate,
                PassportExpiryDate = dts.passportExpiryDate,
                PassportPlaceOfIssue = dts.passportPlaceIssue,
                EmiratesID = dts.emiratesId, /*EIDIssueDate= ?*/
                EIDExpiryDate = dts.emiratesIdExpiryDate,
                FatherFirstName = dts.fathersFirstName,
                FatherMiddleName = dts.fathersMiddleName,
                FatherLastName = dts.fathersLastName,
                MotherFirstName = dts.mothersFirstName,
                MotherMiddleName = dts.mothersMiddleName,
                MotherLastName = dts.mothersLastName,
                FamilyBookNumber = dts.familyBookNo,
                SourceOfApplication = dts.applicationSource
                //Attachements = attachments
            };
        }

        private static ContactDetail MapToContactDetail(studentFormData dts)
        {
            return new ContactDetail()
            {
                CandidateMobileNo = dts.studentMobile,
                EmailAddress = dts.studentEmail,
                FatherMobileNo = dts.parentMobile,
                FatherEamilAddress = dts.parentEmail,
                MotherMobileNo = dts.mothersMobileNo,
                MotherEmailAddress = dts.mothersEmail,
                StreetAddress = dts.street,
                Emirate = dts.emirate,
                AreaOfEmirate = dts.area,
                POBox = dts.poBox,
                PostalCode = dts.postalCode
            };
        }

        private static AcademicDetail MapToAcademicDetail(studentFormData dts, studentEducationData[] edu)
        {
            var ad = new AcademicDetail()
            {
                Program = dts.scholarshipProgram,
                University = dts.university,
                OtherUniversity = dts.otherUniversity,
                Major = dts.major,
                OtherMajor = dts.otherMajor
            };

            if (edu != null && edu.Count() > 0)
            {
                ad.Eduction = edu.Select(ed => new Eduction()
                {
                    Country = ed.country,
                    EndDate = ed.endDate,
                    GradeOrPercentage = ed.grade,
                    Level = ed.level,
                    Major = ed.major,
                    Region = ed.region,
                    School = ed.institute,
                    StartDate = ed.startDate
                }).ToList();
            }

            return ad;
        }

        private static Questionnaire MapToQuestionnaire(studentFormData dts)
        {
            return new Questionnaire()
            {
                AnyMedicalCondition = dts.medicalCondition,
                MedicalConditionDetail = dts.medicalReason,
                MilitaryStatus = dts.milatryStatus
            };
        }

        #endregion Private methods
    }
}