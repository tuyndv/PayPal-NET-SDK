//==============================================================================
//
//  This file was auto-generated by a tool using the PayPal REST API schema.
//  More information: https://developer.paypal.com/docs/api/
//
//==============================================================================
using System;
using Newtonsoft.Json;
using PayPal.Util;
using System.Collections.Specialized;
using System.Text;
using System.Security.Cryptography;

namespace PayPal.Api
{
    /// <summary>
    /// A REST API webhook event resource object.
    /// <para>
    /// See <a href="https://developer.paypal.com/docs/api/">PayPal Developer documentation</a> for more information.
    /// </para>
    /// </summary>
    public class WebhookEvent : PayPalRelationalObject
    {
        /// <summary>
        /// Identifier of the Webhooks event resource.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string id { get; set; }

        /// <summary>
        /// Time the resource was created.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "create_time")]
        public string create_time { get; set; }

        /// <summary>
        /// Name of the resource contained in resource element.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "resource_type")]
        public string resource_type { get; set; }

        /// <summary>
        /// Name of the event type that occurred on resource, identified by data_resource element, to trigger the Webhooks event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "event_type")]
        public string event_type { get; set; }

        /// <summary>
        /// A summary description of the event. E.g. A successful payment authorization was created for $$
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "summary")]
        public string summary { get; set; }

        /// <summary>
        /// This contains the resource that is identified by resource_type element.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "resource")]
        public object resource { get; set; }

        /// <summary>
        /// Retrieves the Webhooks event resource identified by event_id. Can be used to retrieve the payload for an event.
        /// </summary>
        /// <param name="apiContext">APIContext used for the API call.</param>
        /// <param name="eventId">Identifier of the Webhooks event resource.</param>
        /// <returns>WebhookEvent</returns>
        public static WebhookEvent Get(APIContext apiContext, string eventId)
        {
            // Validate the arguments to be used in the request
            ArgumentValidator.ValidateAndSetupAPIContext(apiContext);
            ArgumentValidator.Validate(eventId, "eventId");

            // Configure and send the request
            var pattern = "v1/notifications/webhooks-events/{0}";
            var resourcePath = SDKUtil.FormatURIPath(pattern, new object[] { eventId });
            return PayPalResource.ConfigureAndExecute<WebhookEvent>(apiContext, HttpMethod.GET, resourcePath);
        }

        /// <summary>
        /// Resends the Webhooks event resource identified by event_id.
        /// </summary>
        /// <param name="apiContext">APIContext used for the API call.</param>
        /// <returns>WebhookEvent</returns>
        public WebhookEvent Resend(APIContext apiContext)
        {
            return WebhookEvent.Resend(apiContext, this.id);
        }

        /// <summary>
        /// Resends the Webhooks event resource identified by event_id.
        /// </summary>
        /// <param name="apiContext">APIContext used for the API call.</param>
        /// <param name="webhookEventId">ID of the webhook event to resend.</param>
        /// <returns>WebhookEvent</returns>
        public static WebhookEvent Resend(APIContext apiContext, string webhookEventId)
        {
            // Validate the arguments to be used in the request
            ArgumentValidator.ValidateAndSetupAPIContext(apiContext);
            ArgumentValidator.Validate(webhookEventId, "webhookEventId");

            // Configure and send the request
            var pattern = "v1/notifications/webhooks-events/{0}/resend";
            var resourcePath = SDKUtil.FormatURIPath(pattern, new object[] { webhookEventId });
            return PayPalResource.ConfigureAndExecute<WebhookEvent>(apiContext, HttpMethod.POST, resourcePath);
        }

        /// <summary>
        /// Retrieves the list of Webhooks events resources for the application associated with token. The developers can use it to see list of past webhooks events.
        /// </summary>
        /// <param name="apiContext">APIContext used for the API call.</param>
        /// <param name="pageSize">Number of items to be returned by a GET operation</param>
        /// <param name="startTime">Resource creation time that indicates the start of a range of results.</param>
        /// <param name="endTime">Resource creation time that indicates the end of a range of results.</param>
        /// <returns>WebhookEventList</returns>
        public static WebhookEventList List(APIContext apiContext, int pageSize = 10, string startTime = "", string endTime = "")
        {
            // Validate the arguments to be used in the request
            ArgumentValidator.ValidateAndSetupAPIContext(apiContext);

            var queryParameters = new QueryParameters();
            queryParameters["page_size"] = pageSize.ToString();
            queryParameters["start_time"] = startTime;
            queryParameters["end_time"] = endTime;

            // Configure and send the request
            var resourcePath = "v1/notifications/webhooks-events" + queryParameters.ToUrlFormattedString();
            return PayPalResource.ConfigureAndExecute<WebhookEventList>(apiContext, HttpMethod.GET, resourcePath);
        }

        /// <summary>
        /// Validates a received webhook event by checking the signature of the event and verifying the event originated from PayPal.
        /// </summary>
        /// <param name="requestHeaders">A collection of HTTP request headers included with the received webhook event.</param>
        /// <param name="requestBody">The body of the received HTTP request.</param>
        /// <param name="webhookId">ID of the webhook resource associated with this webhook event.</param>
        /// <returns>True if the webhook event is valid and was sent from PayPal; false otherwise.</returns>
        public static bool ValidateReceivedEvent(NameValueCollection requestHeaders, string requestBody, string webhookId)
        {
            bool isValid = false;

            // Check the headers and ensure all the correct information is present.
            var transmissionId = requestHeaders[BaseConstants.PayPalTransmissionIdHeader];
            var transmissionTimestamp = requestHeaders[BaseConstants.PayPalTransmissionTimeHeader];
            var signature = requestHeaders[BaseConstants.PayPalTransmissionSignatureHeader];
            var certUrl = requestHeaders[BaseConstants.PayPalCertificateUrlHeader];
            var authAlgorithm = requestHeaders[BaseConstants.PayPalAuthAlgorithmHeader];

            ArgumentValidator.Validate(transmissionId, BaseConstants.PayPalTransmissionIdHeader);
            ArgumentValidator.Validate(transmissionTimestamp, BaseConstants.PayPalTransmissionTimeHeader);
            ArgumentValidator.Validate(signature, BaseConstants.PayPalTransmissionSignatureHeader);
            ArgumentValidator.Validate(certUrl, BaseConstants.PayPalCertificateUrlHeader);
            ArgumentValidator.Validate(authAlgorithm, BaseConstants.PayPalAuthAlgorithmHeader);

            try
            {
                // Convert the provided auth alrogithm header into a known hash alrogithm name.
                var hashAlgorithm = ConvertAuthAlgorithmHeaderToHashAlgorithmName(authAlgorithm);

                // Calculate a CRC32 checksum using the request body.
                var crc32 = Crc32.ComputeChecksum(requestBody);

                // Generate the expected signature.
                var expectedSignature = string.Format("{0}|{1}|{2}|{3}", transmissionId, transmissionTimestamp, webhookId, crc32);
                var expectedSignatureBytes = Encoding.UTF8.GetBytes(expectedSignature);

                // Get the cert from the cache.
                var x509Certificate = CertificateManager.Instance.GetCertificate(certUrl);

                // Verify the received signature matches the expected signature.
                var rsa = x509Certificate.PublicKey.Key as RSACryptoServiceProvider;
                var signatureBytes = Convert.FromBase64String(signature);
                isValid = rsa.VerifyData(expectedSignatureBytes, CryptoConfig.MapNameToOID(hashAlgorithm), signatureBytes);
            }
            catch (PayPalException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PayPalException("Encountered an error while attepting to validate a webhook event.", ex);
            }

            return isValid;
        }

        /// <summary>
        /// Converts the algorithm name specified by <paramref name="authAlgorithmHeader"/> into a hash algorithm name recognized by <seealso cref="System.Security.Cryptography.CryptoConfig"/>.
        /// </summary>
        /// <param name="authAlgorithmHeader">The PAYPAL-AUTH-ALGO header value included with a received Webhook event.</param>
        /// <returns>A mapped hash algorithm name.</returns>
        internal static string ConvertAuthAlgorithmHeaderToHashAlgorithmName(string authAlgorithmHeader)
        {
            // The PAYPAL-AUTH-ALGO header will be specified in a name recognized
            // by Java's java.security.Signature class.
            //
            // Currently, only RSA is supported, and the hashing algorithm will
            // be derived with the following assumption on the format:
            //   "<hash_algorithm>withRSA"
            var token = "withRSA";
            if (authAlgorithmHeader.EndsWith(token))
            {
                return authAlgorithmHeader.Split(new string[] { token }, StringSplitOptions.None)[0];
            }

            // At this point, we've encountered an unsupported algorithm.
            throw new AlgorithmNotSupportedException(string.Format("Unable to map {0} to a known hash algorithm.", authAlgorithmHeader));
        }
    }
}
