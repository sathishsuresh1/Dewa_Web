using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using DEWAXP.Foundation.DI;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.RecruitmentSvc;
using DEWAXP.Foundation.Integration.Responses.RecruitmentSvc;

namespace DEWAXP.Foundation.Integration.Impl.RecruitmentSvc
{
    [Service(typeof(IRecruitmentServiceClient), Lifetime = Lifetime.Transient)]
    public class RecruitmentSoapClient : BaseDewaGateway, IRecruitmentServiceClient
	{
		public IEnumerable<Vacancy> GetVacancies(SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
		{
			using (var proxy = CreateProxy())
			{
				var response = proxy.JobSearch(new DT_JobSearch_Req()
				{
					Language = language.Code(),
					SearchTask = string.Empty,
					ObjectID = string.Empty,
					WithDetailsearch = "X",
					HotJobs = "X"
				});

				if (response.JobList != null && response.JobList.Any())
				{
					return response.JobList.Select(Vacancy.From);
				}
				return new Vacancy[0];
			}
		}

		public Vacancy GetVacancy(string identifier, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
		{
			using (var proxy = CreateProxy())
			{
				var response = proxy.JobSearch(new DT_JobSearch_Req()
				{
					Language = language.Code(),
					ObjectID = identifier,
					SearchTask = string.Empty,
					WithDetailsearch = "X",
					HotJobs = "X"
				});

				if (response.JobList != null && response.JobList.Any())
				{
					return response.JobList.Select(Vacancy.From).First();
				}
				return null;
			}
		}
		
		private SI_O_ERecruitmentServiceClient CreateProxy()
		{
			var binding = CreateBinding();
			var endpointAddress = GetEndpointAddress("RecruitmentHttpsSoap");

            var proxy = new SI_O_ERecruitmentServiceClient(binding, endpointAddress);
			proxy.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings[ConfigKeys.RECRUITMENT_UN];
			proxy.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings[ConfigKeys.RECRUITMENT_PWD];

			return proxy;
		}

		private Binding CreateBinding()
		{
			var binding = new BasicHttpBinding()
			{
				ReceiveTimeout = TimeSpan.FromMinutes(2),
				SendTimeout = TimeSpan.FromMinutes(1)
			};
			
			binding.Security.Mode = BasicHttpSecurityMode.Transport;
			binding.Security.Transport.Realm = "XISOAPApps";
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
			binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
			binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;

			return binding;
		}
	}
}
