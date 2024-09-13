// <copyright file="SurveyQuestionandAnswers.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.Integration.DewaSvc;
using System.Collections.Generic;

namespace DEWAXP.Feature.GeneralServices.Models.CCSurvey
{

    public class SurveyQuestionandAnswers
    {
        public string Transactioncode { get; set; }

        public enquiryInput[] enquiryInputs { get; set; }

        public InquiryType InquiryType { get; set; }

        public string datasource { get; set; }

    }

    public class FavoriteGroup
    {
        public string GroupName { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Options { get; set; }
        public int SelectedAnswer { set; get; }
    }
}
