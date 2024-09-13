using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.MoveOut
{
	public class ConfirmModel
	{
		public SharedAccount Account { get; set; }

        public SharedAccount[] Accounts { get; set; }

        public ConfirmNotificationModel[] Notifications { get; set; }
		
		public bool IsSuccess { get; set; }

		public string Message { get; set; }
		public string CardNumber { get; set; }

        public string Notification { get; set; }

        public string ErrorMessage { get; set; }
        public string AdditionalInformation { get; set; }

    }

    public class ConfirmNotificationModel
    {
        public string ContractAccountNumber { get; set; }

        public string Message { get; set; }

        public string NotificationNumber { get; set; }
    }
}