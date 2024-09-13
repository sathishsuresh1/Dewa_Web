namespace DEWAXP.Feature.HappinessIndicator.Models.HappinessIndicator
{
	public class HappinessIndicatorViewModel
	{
		public HappinessIndicatorViewModel(string type, string transactionId, bool autoShow, string serviceCode,string serviceDescription,bool isLocal)
		{
			TransactionId = transactionId; 
			AutoShow = autoShow;
			Type = type;
            ServiceCode = serviceCode;
            ServiceDescription = serviceDescription;
            islocal = isLocal;
		}

		public string Type { get; private set; }

		public string TransactionId { get; private set; }

		public bool AutoShow { get; private set; }

        public string ServiceCode { get; private set; }

        public string ServiceDescription { get; private set; }

        public bool islocal { get; private set; }

	}
}