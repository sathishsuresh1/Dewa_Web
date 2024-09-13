using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DEWAXP.Foundation.Integration.Impl
{
	public class CustomAuthenticationBehavior : IEndpointBehavior
	{
		/// <summary>
		///     The sap API key.
		/// </summary>
		private string sapApiKey;
		private string token;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomAuthenticationBehavior"/> class. 
		/// Initializes a new instance of the
		///     <see cref="CustomAuthenticationBehavior"/> class.
		/// </summary>
		/// <param name="sapKey">
		/// The authentication token.
		/// </param>
		public CustomAuthenticationBehavior(string sapKey, string token)
		{
			this.sapApiKey = sapKey;
			this.token = token;
		}

		/// <summary>
		/// The add binding parameters.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint.
		/// </param>
		/// <param name="bindingParameters">
		/// The binding parameters.
		/// </param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		/// <summary>
		/// The apply client behavior.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint.
		/// </param>
		/// <param name="clientRuntime">
		/// The client runtime.
		/// </param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			if (clientRuntime != null)
			{
				clientRuntime.ClientMessageInspectors.Add(new CustomMessageInspector(sapApiKey, token));
			}
		}

		/// <summary>
		/// The apply dispatch behavior.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint.
		/// </param>
		/// <param name="endpointDispatcher">
		/// The <paramref name="endpoint"/> dispatcher.
		/// </param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		/// <summary>
		/// The validate.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint.
		/// </param>
		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}

	public class CustomMessageInspector : IClientMessageInspector
	{
		/// <summary>
		///     The sap API key.
		/// </summary>
		private readonly string sapApiKey;
		private readonly string token;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomMessageInspector"/> class. 
		/// Initializes a new instance of the
		///     <see cref="CustomMessageInspector"/> class.
		/// </summary>
		/// <param name="sapKey">
		/// The authentication token.
		/// </param>
		public CustomMessageInspector(string sapKey, string token)
		{
			this.sapApiKey = sapKey;
			this.token = token;
		}

		/// <summary>
		/// The after receive reply.
		/// </summary>
		/// <param name="reply">
		/// The reply.
		/// </param>
		/// <param name="correlationState">
		/// The correlation state.
		/// </param>
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		/// <summary>
		/// The before send request.
		/// </summary>
		/// <param name="request">
		/// The request.
		/// </param>
		/// <param name="channel">
		/// The channel.
		/// </param>
		/// <returns>
		/// The <see cref="System.Object"/> .
		/// </returns>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var reqMsgProperty = new HttpRequestMessageProperty();
			reqMsgProperty.Headers.Add("apikey", this.sapApiKey);
			reqMsgProperty.Headers.Add("Authorization", this.token);
			if (request != null)
			{
				request.Properties[HttpRequestMessageProperty.Name] = reqMsgProperty;
			}

			return null;
		}
	}
}
