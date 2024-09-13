using DEWAXP.Foundation.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Dashboard.Models.ConsumptionStatistics
{
    public class ConsumtionResponseData
    {
        public ConsumtionResponseData() {
            Months =DateHelper.GetStaticMonthList().Select(x => x.Text).ToList();
            //Months.Add();
        }
        public TypeDataSeries dataSeries { get; set; }
        public List<string> Months { get; set; }
    }

    public class TypeDataSeries
    {
        public TypeDataSeries()
        {
            electricity = new List<DataPointV1>();
            water = new List<DataPointV1>();
        }
        public List<DataPointV1> electricity { get; set; }

        public List<DataPointV1> water { get; set; }
        public DataPointV1 carbonFootprint { get; set; }


    }

    public class DataPointV1
    {
        public DataPointV1(int year, int month, decimal value)
        {

            this.year = year;
            this.month = month;
            this.value = value;
        }
        public decimal value { get; private set; }

        public int year { get; private set; }

        public int month { get; private set; }
    }




}