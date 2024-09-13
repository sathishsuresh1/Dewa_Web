using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests.SmartCustomer.EVDashboard;
using DEWAXP.Foundation.Integration.Responses;
using Sitecore.Globalization;


namespace DEWAXP.Foundation.Content.Models.ConsumptionStatistics
{
	public class DataSeries
	{
		public string Legend { get; set; }

		public MunicipalService Utility { get; set; }

		public DataPoint[] DataPoints { get; set; }

		public DataSeries(string legend, MunicipalService utility, params DataPoint[] points)
		{
			Legend = legend;
			Utility = utility;
			DataPoints = points ?? new DataPoint[0];
		}

		public static IList<DataSeries> Create(YearlyConsumptionDataResponse response)
		{
			if (response.Utilities == null)
			{
				return new List<DataSeries>();
			}

			var @return = new List<DataSeries>();
			if (response.Utilities.Electricity != null)
			{
				if (response.Utilities.Electricity.FirstYear != null)
				{
					var dataPoints = response.Utilities.Electricity.FirstYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Electricity.FirstYear.PeriodStart.Year.ToString(), MunicipalService.Electricity, dataPoints));
				}

				if (response.Utilities.Electricity.SecondYear != null)
				{
					var dataPoints = response.Utilities.Electricity.SecondYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Electricity.SecondYear.PeriodStart.Year.ToString(), MunicipalService.Electricity, dataPoints));
				}

				if (response.Utilities.Electricity.ThirdYear != null)
				{
					var dataPoints = response.Utilities.Electricity.ThirdYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Electricity.ThirdYear.PeriodStart.Year.ToString(), MunicipalService.Electricity, dataPoints));
				}
                if (response.Utilities.Electricity.FourthYear != null)
                {
                    var dataPoints = response.Utilities.Electricity.FourthYear.DataPoints.Select(DataPoint.From).ToArray();

                    @return.Add(new DataSeries(response.Utilities.Electricity.FourthYear.PeriodStart.Year.ToString(), MunicipalService.Electricity, dataPoints));
                }
                if (response.Utilities.Electricity.FifthYear != null)
                {
                    var dataPoints = response.Utilities.Electricity.FifthYear.DataPoints.Select(DataPoint.From).ToArray();

                    @return.Add(new DataSeries(response.Utilities.Electricity.FifthYear.PeriodStart.Year.ToString(), MunicipalService.Electricity, dataPoints));
                }
            }

			if (response.Utilities.Water != null)
			{
				if (response.Utilities.Water.FirstYear != null)
				{
					var dataPoints = response.Utilities.Water.FirstYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Water.FirstYear.PeriodStart.Year.ToString(), MunicipalService.Water, dataPoints));
				}

				if (response.Utilities.Water.SecondYear != null)
				{
					var dataPoints = response.Utilities.Water.SecondYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Water.SecondYear.PeriodStart.Year.ToString(), MunicipalService.Water, dataPoints));
				}

				if (response.Utilities.Water.ThirdYear != null)
				{
					var dataPoints = response.Utilities.Water.ThirdYear.DataPoints.Select(DataPoint.From).ToArray();

					@return.Add(new DataSeries(response.Utilities.Water.ThirdYear.PeriodStart.Year.ToString(), MunicipalService.Water, dataPoints));
				}
                if (response.Utilities.Water.FourthYear != null)
                {
                    var dataPoints = response.Utilities.Water.FourthYear.DataPoints.Select(DataPoint.From).ToArray();

                    @return.Add(new DataSeries(response.Utilities.Water.FourthYear.PeriodStart.Year.ToString(), MunicipalService.Water, dataPoints));
                }
                if (response.Utilities.Water.FifthYear != null)
                {
                    var dataPoints = response.Utilities.Water.FifthYear.DataPoints.Select(DataPoint.From).ToArray();

                    @return.Add(new DataSeries(response.Utilities.Water.FifthYear.PeriodStart.Year.ToString(), MunicipalService.Water, dataPoints));
                }
            }
			return @return;
		}

		public static IList<DataSeries> Create(YearlyAverageConsumptionDataResponse response)
		{
			var @return = new List<DataSeries>();

			if (response.Utilities.Electricity != null && response.Utilities.Electricity.Averages.Count > 0)
			{
				var accountNumber = response.Utilities.Electricity.Averages.First().ComparisonAccountNumber;
                var accountDataPoints = response.Utilities.Electricity.AllDataPoints.Select(dp => new DataPoint(dp.Period, dp.Value)).ToArray();

				@return.Add(new DataSeries(accountNumber, MunicipalService.Electricity, accountDataPoints));
				
				var averageDataPoints = response.Utilities.Electricity.Averages.Select(dp => new DataPoint(dp.Period,Convert.ToInt32(dp.Average))).ToArray();

				@return.Add(new DataSeries(accountNumber, MunicipalService.Electricity, averageDataPoints));
			}

			if (response.Utilities.Water != null && response.Utilities.Water.Averages.Count > 0)
			{
                var accountDataPoints = response.Utilities.Water.AllDataPoints.Select(dp => new DataPoint(dp.Period, dp.Value)).ToArray();

				@return.Add(new DataSeries(Translate.Text("Neighbourhood"), MunicipalService.Water, accountDataPoints));

				var averageDataPoints = response.Utilities.Water.Averages.Select(dp => new DataPoint(dp.Period, dp.Value)).ToArray();

				@return.Add(new DataSeries(Translate.Text("Neighbourhood"), MunicipalService.Water, averageDataPoints));
			}
			return @return;
		}

		public static IList<DataSeries> Create(ComparativeConsumptionResponse response)
		{
			var @return = new List<DataSeries>();

			foreach (var account in response.Accounts)
			{
				var grouped = account.DataPoints.GroupBy(dp => dp.Utility);
				foreach (var group in grouped)
				{
					var dataPoints = group.Select(DataPoint.From);
					var series = new DataSeries(account.AccountNumber, group.Key, dataPoints.ToArray());

                    @return.Add(series);
				}
			}
			return @return;
		}

      
		public static DataSeries Null(MunicipalService service)
		{
			return new DataSeries(string.Empty, service);
		}

        public static IList<DataSeries> Create(EVConsumptionResponse response)
        {
            if (response == null)
            {
                return new List<DataSeries>();
            }

            var @return = new List<DataSeries>();
            if (response.Dailyconsumption != null && response.Dailyconsumption.Any() && response.Dailyconsumption.FirstOrDefault() != null 
                && response.Dailyconsumption.FirstOrDefault().Consumption != null
                && response.Dailyconsumption.FirstOrDefault().Consumption.Any())
            {
                EVDataPointsDaily(response.Dailyconsumption.FirstOrDefault().Consumption, @return);
            }
            else
            {
                var orderedlist =  response.Electricityconsumption.Where(x => !string.IsNullOrWhiteSpace(x.Year)).ToList().OrderByDescending(y => Convert.ToInt32(y.Year));
                orderedlist.ToList().ForEach(x => EVDataPoints(x.Consumption, @return));
                //EVDataPoints(response.Consumption1, @return);
                //EVDataPoints(response.Consumption2, @return);
                //EVDataPoints(response.Consumption3, @return);
                //EVDataPoints(response.Consumption4, @return);
                //EVDataPoints(response.Consumption5, @return);
            }
            return @return;
        }

        private static void EVDataPoints(List<Consumptions> response, List<DataSeries> @return)
        {
            if (response != null)
            {
                var dataPoints = response.Select(DataPoint.EVFrom).ToArray();

                @return.Add(new DataSeries(PeriodStart(response).Year.ToString(), MunicipalService.EV, dataPoints));
            }
        }
        private static void EVDataPointsDaily(List<Consumptions> response, List<DataSeries> @return)
        {
            List<double?> listArr = new List<double?>();
            for (int i=1;i<=31;i++)
            {
                if(response.Where(x=>x.TransactiondatePeriod.Day.Equals(i)).Any())
                {
                    listArr.Add(Convert.ToDouble(response.Where(x => x.TransactiondatePeriod.Day.Equals(i)).FirstOrDefault().Consumption));
                }
                else
                {
                    listArr.Add(null);
                }
            }
            @return.Add(new DataSeries(PeriodStart(response).Month.ToString(), MunicipalService.EV, new DataPoint(listArr)));
        }

        public static DateTime PeriodStart(List<Consumptions> dailyconsumptions) => dailyconsumptions.Min(dp => dp.Period);

    }

	public class DataPoint
	{
        public DataPoint(int year,int month,int value) {

            Year = year;
            Month = month;
            Value = value;
        }
        public DataPoint(DateTime period, int value)
		{
			Year = period.Year;
			Month = period.Month;
			Value = value;
		}
        public DataPoint(DateTime period, double value)
        {
            Year = period.Year;
            Month = period.Month;
            dValue = value;
        }
        public DataPoint(List<double?> values)
        {
            dValues = values;
        }
        public List<int?> Values { get; set; }
        public List<double?> dValues { get; set; }
        public int Value { get; private set; }
        public double dValue { get; private set; }

		public int Year { get; private set; }

		public int Month { get; private set; }

		public static DataPoint From(DEWAXP.Foundation.Integration.Responses.ConsumptionDataPoint response)
		{
			return new DataPoint(response.Period, response.Value);
		}
        public static DataPoint EVFrom(Consumptions response)
        {
            return new DataPoint(response.Period,Convert.ToDouble(response.Consumption));
        }
    }
}