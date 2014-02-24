//using System;
//using System.Collections;
//using System.IO;
//using JsonLDNet.Core;
//using JsonLDNet.Util;

//namespace JsonLDNet.Core
//{
//    public class DocumentLoader
//    {
//        /// <exception cref="JsonLDNet.Core.JsonLdError"></exception>
//        public virtual RemoteDocument LoadDocument(string url)
//        {
//            RemoteDocument doc = new RemoteDocument(url, null);
//            try
//            {
//                doc.Document = FromURL(new URL(url));
//            }
//            catch (Exception)
//            {
//                new JsonLdError(JsonLdError.Error.LoadingRemoteContextFailed, url);
//            }
//            return doc;
//        }

//        /// <summary>An HTTP Accept header that prefers JSONLD.</summary>
//        /// <remarks>An HTTP Accept header that prefers JSONLD.</remarks>
//        public const string AcceptHeader = "application/ld+json, application/json;q=0.9, application/javascript;q=0.5, text/javascript;q=0.5, text/plain;q=0.2, */*;q=0.1";

//        private static volatile IHttpClient httpClient;

//        /// <summary>
//        /// Returns a Map, List, or String containing the contents of the JSON
//        /// resource resolved from the URL.
//        /// </summary>
//        /// <remarks>
//        /// Returns a Map, List, or String containing the contents of the JSON
//        /// resource resolved from the URL.
//        /// </remarks>
//        /// <param name="url">The URL to resolve</param>
//        /// <returns>
//        /// The Map, List, or String that represent the JSON resource
//        /// resolved from the URL
//        /// </returns>
//        /// <exception cref="Com.Fasterxml.Jackson.Core.JsonParseException">If the JSON was not valid.
//        /// 	</exception>
//        /// <exception cref="System.IO.IOException">If there was an error resolving the resource.
//        /// 	</exception>
//        public static object FromURL(URL url)
//        {
//            MappingJsonFactory jsonFactory = new MappingJsonFactory();
//            InputStream @in = OpenStreamFromURL(url);
//            try
//            {
//                JsonParser parser = jsonFactory.CreateParser(@in);
//                try
//                {
//                    JsonToken token = parser.NextToken();
//                    Type type;
//                    if (token == JsonToken.StartObject)
//                    {
//                        type = typeof(IDictionary);
//                    }
//                    else
//                    {
//                        if (token == JsonToken.StartArray)
//                        {
//                            type = typeof(IList);
//                        }
//                        else
//                        {
//                            type = typeof(string);
//                        }
//                    }
//                    return parser.ReadValueAs(type);
//                }
//                finally
//                {
//                    parser.Close();
//                }
//            }
//            finally
//            {
//                @in.Close();
//            }
//        }

//        /// <summary>
//        /// Opens an
//        /// <see cref="Java.IO.InputStream">Java.IO.InputStream</see>
//        /// for the given
//        /// <see cref="Java.Net.URL">Java.Net.URL</see>
//        /// , including support
//        /// for http and https URLs that are requested using Content Negotiation with
//        /// application/ld+json as the preferred content type.
//        /// </summary>
//        /// <param name="url">The URL identifying the source.</param>
//        /// <returns>An InputStream containing the contents of the source.</returns>
//        /// <exception cref="System.IO.IOException">If there was an error resolving the URL.</exception>
//        public static InputStream OpenStreamFromURL(URL url)
//        {
//            string protocol = url.GetProtocol();
//            if (!JsonLDNet.Shims.EqualsIgnoreCase(protocol, "http") && !JsonLDNet.Shims.EqualsIgnoreCase
//                (protocol, "https"))
//            {
//                // Can't use the HTTP client for those!
//                // Fallback to Java's built-in URL handler. No need for
//                // Accept headers as it's likely to be file: or jar:
//                return url.OpenStream();
//            }
//            IHttpUriRequest request = new HttpGet(url.ToExternalForm());
//            // We prefer application/ld+json, but fallback to application/json
//            // or whatever is available
//            request.AddHeader("Accept", AcceptHeader);
//            IHttpResponse response = GetHttpClient().Execute(request);
//            int status = response.GetStatusLine().GetStatusCode();
//            if (status != 200 && status != 203)
//            {
//                throw new IOException("Can't retrieve " + url + ", status code: " + status);
//            }
//            return response.GetEntity().GetContent();
//        }

//        public static IHttpClient GetHttpClient()
//        {
//            IHttpClient result = httpClient;
//            if (result == null)
//            {
//                lock (typeof(JSONUtils))
//                {
//                    result = httpClient;
//                    if (result == null)
//                    {
//                        // Uses Apache SystemDefaultHttpClient rather than
//                        // DefaultHttpClient, thus the normal proxy settings for the
//                        // JVM will be used
//                        DefaultHttpClient client = new SystemDefaultHttpClient();
//                        // Support compressed data
//                        // http://hc.apache.org/httpcomponents-client-ga/tutorial/html/httpagent.html#d5e1238
//                        client.AddRequestInterceptor(new RequestAcceptEncoding());
//                        client.AddResponseInterceptor(new ResponseContentEncoding());
//                        CacheConfig cacheConfig = new CacheConfig();
//                        cacheConfig.SetMaxObjectSize(1024 * 128);
//                        // 128 kB
//                        cacheConfig.SetMaxCacheEntries(1000);
//                        // and allow caching
//                        httpClient = new CachingHttpClient(client, cacheConfig);
//                        result = httpClient;
//                    }
//                }
//            }
//            return result;
//        }

//        public static void SetHttpClient(IHttpClient nextHttpClient)
//        {
//            lock (typeof(JSONUtils))
//            {
//                httpClient = nextHttpClient;
//            }
//        }
//    }
//}