using ModernHttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xamtastic.Patterns.Azure.EventHub
{
    public class EventHubSasClient
    {
        private string _sharedAccessSignature;
        /// <summary>
        /// The Windows Azure Service Bus Shared Access Signature that grants Send rights to publish to the Windows Azure Event Hub that is defined by the values of the property ServiceNamespace and HubName, for the device id that is supplied in the property DeviceName (or the corresponding ctor overloads).
        /// </summary>
        public string SharedAccessSignature
        {
            get { return SharedAccessSignature; }
            set { SharedAccessSignature = value; _httpClient = null; }
        }

        private string _serviceNamespace;
        /// <summary>
        /// The Namespace of the Service Bus that is associated with the Event Hub that is identified in the property HubName (or corresponding ctor overloads).
        /// </summary>
        public string ServiceNamespace
        {
            get { return _serviceNamespace; }
            set { _serviceNamespace = value; _httpClient = null; }
        }

        private string _hubName;
        /// <summary>
        /// The name of the Event Hub that is associated with the Serice Bus namespace that is identified in the ServiceNamespace property, or associated overload.
        /// </summary>
        public string HubName
        {
            get { return _hubName; }
            set { _hubName = value; _httpClient = null; }
        }

        private string _deviceName;
        /// <summary>
        /// The name of the device that is publishing the message to the Event Hub, and which Device name was declared as part of the creation of the Shared Access Signature.
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; _httpClient = null; }
        }

        private HttpClient _httpClient;

        public EventHubSasClient()
        {

        }

        public EventHubSasClient(string sharedAccessSignature, string serviceNamespace, string hubName, string deviceName)
        {
            _sharedAccessSignature = sharedAccessSignature;
            _serviceNamespace = serviceNamespace;
            _hubName = hubName;
            _deviceName = deviceName;
        }

        private HttpClient GetClient()
        {
            HttpClient httpClient = new HttpClient(new NativeMessageHandler());
            httpClient.BaseAddress = new Uri(String.Format("https://{0}.servicebus.windows.net/", _serviceNamespace));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", _sharedAccessSignature);

            return httpClient;
        }

        /// <summary>
        /// Usage: var httpResponse = await sasClient.SendMessageAsync(message). Publishes a message, using the Shared Access Signature for authorisation as is supplied in the property SharedAccessSignature, to the Windows Azure Event Hub that is defined by the values of the following properties: ServiceNamespace, HubName; or in the corresponding ctor overloads. 
        /// The HttpClient undertakes a PostAsync to the following address: https://{ServiceNamespace}.servicebus.windows.net/{HubName}/publishers/{DeviceName}/messages". It passes the message as HttpContent which is contrived as HttpContent content = new StringContent(message).
        /// </summary>
        /// <param name="message">The message that is to be published to the Event Hub.</param>
        /// <returns>An HttpResponseMessage which allows for the HttpStatusCode to be determined.</returns>
        public async Task<HttpResponseMessage> SendMessageAsync(string message)
        {
            if (_httpClient == null) _httpClient = GetClient();

            HttpContent content = new StringContent(message);

            return await _httpClient.PostAsync(String.Format("{0}/publishers/{1}/messages", _hubName, _deviceName), content);
        }
    }
}
