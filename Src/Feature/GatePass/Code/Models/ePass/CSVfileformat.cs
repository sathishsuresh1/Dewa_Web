// <copyright file="CSVfileformat.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Models.ePass
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.Helpers;
    using FileHelpers;
    using global::Sitecore.Globalization;
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines the <see cref="CSVfileformat" />
    /// </summary>
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class CSVfileformat
    {
        /// <summary>
        /// Defines the ID
        /// </summary>
        [FieldHidden]
        public int ID;

        /// <summary>
        /// Defines the CustomerName
        /// </summary>
        [FieldNotEmpty]
        //[FieldNullValue(typeof(string), "nobody")]
        [FieldConverter(typeof(CustomFieldNotemptyConverter), "Customer Name")]
        public string CustomerName;

        /// <summary>
        /// Defines the Nationality
        /// </summary>
        [FieldNotEmpty]
        [FieldConverter(typeof(CustomFieldNotemptyConverter), "Nationality")]
        //[FieldTrim(TrimMode.Both)]
        public string Nationality;

        /// <summary>
        /// Defines the Profession
        /// </summary>
        [FieldNotEmpty]
        [FieldConverter(typeof(CustomFieldNotemptyConverter), "Profession")]
        [FieldTrim(TrimMode.Both)]
        public string Profession;

        /// <summary>
        /// Defines the Phonenumber
        /// </summary>
        /// 
       
        [FieldConverter(typeof(UAEPhonenumberConverter))]
        [FieldTrim(TrimMode.Both)]
        public string Phonenumber;

        /// <summary>
        /// Defines the Emailid
        /// </summary>
        /// 
       
        [FieldConverter(typeof(EmailConverter))]
        [FieldTrim(TrimMode.Both)]
        public string Emailid;

        /// <summary>
        /// Defines the EmiratesID
        /// </summary>
     
        [FieldNotEmpty]
        [FieldConverter(typeof(EmiratesIDConverter))]
        [FieldTrim(TrimMode.Both)]
        public string EmiratesID;

        /// <summary>
        /// Defines the EidDate
        /// </summary>
        [FieldOrder(7)]
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(ExpiryDateConverter),"Emirates ID Expiry date should be 15 days greater than current date","")]
        public DateTime? EidDate;

        /// <summary>
        /// Defines the Visanumber
        /// </summary>
        [FieldNotEmpty]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Visanumber;

        /// <summary>
        /// Defines the VisaexpDate
        /// </summary>
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(ExpiryDateConverter),"Visa Expiry date should be 15 days greater than current date", "")]
        public DateTime? VisaexpDate;

        /// <summary>
        /// Defines the Passportnumber
        /// </summary>
        [FieldNotEmpty]
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Passportnumber;

        /// <summary>
        /// Defines the PassportexpDate
        /// </summary>
     
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(ExpiryDateConverter),"Passport Expiry date should be 15 days greater than current date", "")]
        public DateTime PassportexpDate;

        /// <summary>
        /// Defines the Passissuedate
        /// </summary>
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(DateConverter),"Pass Issue Date should be in proper format")]
        public DateTime Passissuedate;

        /// <summary>
        /// Defines the Passexpirydate
        /// </summary>
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(ExpiryDateConverter),"Pass Expiry date should be {0} day(s) greater than current date","Expiry")]
        public DateTime Passexpirydate;

        /// <summary>
        /// Defines the FromTime
        /// </summary>
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(TimeConverter),"Fromtime is in unsupported format")]
        public string FromTime;

        /// <summary>
        /// Defines the ToTime
        /// </summary>
        [FieldNotEmpty]
        [FieldTrim(TrimMode.Both)]
        [FieldConverter(typeof(TimeConverter),"Totime is in unsupported format")]
        public string ToTime;

        /// <summary>
        /// Defines the registeredefolderid
        /// </summary>
        [FieldHidden]
        public string registeredefolderid;
    }

    /// <summary>
    /// Defines the <see cref="EmiratesIDConverter" />
    /// </summary>
    public class EmiratesIDConverter : ConverterBase
    {
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(from))
                {
                    return from;
                }
                if (EmiratesIDValidator.IsValid(from))
                {
                    return from;
                }
                throw new InvalidOperationException(Translate.Text("Emirates id Unsupported format"));
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(Translate.Text("Emirates id Unsupported format"));
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }

    /// <summary>
    /// Defines the <see cref="TimeConverter" />
    /// </summary>
    public class TimeConverter : ConverterBase
    {
        /// <summary>
        /// Defines the _parameter
        /// </summary>
        private readonly string _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldNotemptyConverter"/> class.
        /// </summary>
        /// <param name="paramter">The paramter<see cref="string"/></param>
        public TimeConverter(string paramter)
        {
            _parameter = paramter;
        }
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="time">The time<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string time)
        {
            try
            {
                time = time.Replace(@"\", "").Replace("\"", "");
                if (string.IsNullOrWhiteSpace(time))
                {
                    return time;
                }
                if (!string.IsNullOrWhiteSpace(time))
                {
                    return DateTime.Parse(time).ToShortTimeString();
                }
                throw new InvalidOperationException(_parameter);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(_parameter);
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }

    /// <summary>
    /// Defines the <see cref="CustomFieldNotemptyConverter" />
    /// </summary>
    public class CustomFieldNotemptyConverter : ConverterBase
    {
        /// <summary>
        /// Defines the _parameter
        /// </summary>
        private readonly string _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldNotemptyConverter"/> class.
        /// </summary>
        /// <param name="paramter">The paramter<see cref="string"/></param>
        public CustomFieldNotemptyConverter(string paramter)
        {
            _parameter = paramter;
        }

        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {
                if (_parameter.Equals("Visanumber") || _parameter.Equals("Passportnumber"))
                {
                    return from;
                }
                else {
                   
                    if (string.IsNullOrWhiteSpace(from) || from.Equals("nobody"))
                    {
                        string msg = _parameter + Translate.Text("Field cannot be empty");
                        throw new InvalidOperationException(msg);
                    }
                    if (!string.IsNullOrWhiteSpace(from))
                    {
                        return from;
                    }
                    throw new InvalidOperationException(_parameter + Translate.Text("Field cannot be empty"));
                }
                
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(Translate.Text("Field cannot be empty"));
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }

    /// <summary>
    /// Defines the <see cref="UAEPhonenumberConverter" />
    /// </summary>
    public class UAEPhonenumberConverter : ConverterBase
    {
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {
                if (UAEPhonenumberValidator.IsValid(from))
                {
                    if (from.StartsWith("+"))
                    {
                        from = from.Substring(1);
                    }
                    if (from.StartsWith("971"))
                    {
                        from = from.Substring(3);
                    }
                    if (from.StartsWith("0"))
                    {
                        from = from.Substring(1);
                    }
                    return from;
                }
                throw new InvalidOperationException(Translate.Text("Phone number is invalid"));
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(Translate.Text("Phone number is invalid"));
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }

    /// <summary>
    /// Defines the <see cref="EmailConverter" />
    /// </summary>
    public class EmailConverter : ConverterBase
    {
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {
                if (EmailValidator.IsValid(from))
                {
                    return from;
                }
                throw new InvalidOperationException(Translate.Text("Emailid is invalid"));
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(Translate.Text("Emailid is invalid"));
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }

    /// <summary>
    /// Defines the <see cref="DateConverter" />
    /// </summary>
    public class DateConverter : ConverterBase
    {
        public DateConverter(string format1)
        {
            _expirydatetype = format1;
        }

        private string _expirydatetype;
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(from))
                {
                    return null;
                }
                if (DateTime.TryParse(from, out DateTime dateResult))
                {
                    return dateResult;
                }

                throw new InvalidOperationException(_expirydatetype);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(_expirydatetype);
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }
    public static class CSVFilepassType
    {
        public static int expirydatecount=15;
    }

    /// <summary>
    /// Defines the <see cref="ExpiryDateConverter" />
    /// </summary>
    public class ExpiryDateConverter : ConverterBase
    {
        public ExpiryDateConverter(string format1,string type)
        {
            _expirydatetype = format1;
            _format = format1;
            _type = type;
        }

        private string _expirydatetype;
        private string _type;
        private string _format;
        /// <summary>
        /// The StringToField
        /// </summary>
        /// <param name="from">The from<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        public override object StringToField(string from)
        {
            try
            {
                from = from.Split()[0];
                if (string.IsNullOrWhiteSpace(from))
                {
                    return null;
                }

                if (DateTime.TryParseExact(from, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateResult))
                {
                    int expirydatecount = 15;
                    _expirydatetype = _format;
                    if (_type.Equals("Expiry"))
                    {
                        expirydatecount = CSVFilepassType.expirydatecount;
                        var _temp = string.Format(_format, CSVFilepassType.expirydatecount + 1);
                        _expirydatetype = _temp;
                    }
                    if (dateResult.Date > DateTime.Now.AddDays(expirydatecount).Date)
                    {
                        return dateResult;
                    }
                }
                else
                {
                    _expirydatetype = _format = "Date format provided not in correct format expected MM/dd/yyyy";
                }
                throw new InvalidOperationException(_expirydatetype);
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
                throw new InvalidOperationException(_expirydatetype);
            }
        }

        /// <summary>
        /// The FieldToString
        /// </summary>
        /// <param name="fieldValue">The fieldValue<see cref="object"/></param>
        /// <returns>The <see cref="string"/></returns>
        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }

        public bool CheckDateFormat(string datetime)
        {
            bool status = false;
            DateTime d;
            if (DateTime.TryParseExact(datetime, new string[] { "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }
    }
}
