/* 
 * TheAPI
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Org.OpenAPITools.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITopicApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>IdResponse</returns>
        IdResponse CreateTopic (CreateTopicModel createTopicModel = default(CreateTopicModel));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>ApiResponse of IdResponse</returns>
        ApiResponse<IdResponse> CreateTopicWithHttpInfo (CreateTopicModel createTopicModel = default(CreateTopicModel));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns></returns>
        void JoinTopic (JoinTopicModel joinTopicModel = default(JoinTopicModel));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> JoinTopicWithHttpInfo (JoinTopicModel joinTopicModel = default(JoinTopicModel));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>IdResponse</returns>
        IdResponse SendPost (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>ApiResponse of IdResponse</returns>
        ApiResponse<IdResponse> SendPostWithHttpInfo (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream));
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITopicApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>Task of IdResponse</returns>
        System.Threading.Tasks.Task<IdResponse> CreateTopicAsync (CreateTopicModel createTopicModel = default(CreateTopicModel));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>Task of ApiResponse (IdResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<IdResponse>> CreateTopicAsyncWithHttpInfo (CreateTopicModel createTopicModel = default(CreateTopicModel));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task JoinTopicAsync (JoinTopicModel joinTopicModel = default(JoinTopicModel));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> JoinTopicAsyncWithHttpInfo (JoinTopicModel joinTopicModel = default(JoinTopicModel));
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>Task of IdResponse</returns>
        System.Threading.Tasks.Task<IdResponse> SendPostAsync (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream));

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>Task of ApiResponse (IdResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<IdResponse>> SendPostAsyncWithHttpInfo (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITopicApi : ITopicApiSync, ITopicApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class TopicApi : ITopicApi
    {
        private Org.OpenAPITools.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TopicApi() : this((string) null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TopicApi(String basePath)
        {
            this.Configuration = Org.OpenAPITools.Client.Configuration.MergeConfigurations(
                Org.OpenAPITools.Client.GlobalConfiguration.Instance,
                new Org.OpenAPITools.Client.Configuration { BasePath = basePath }
            );
            this.Client = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public TopicApi(Org.OpenAPITools.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = Org.OpenAPITools.Client.Configuration.MergeConfigurations(
                Org.OpenAPITools.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public TopicApi(Org.OpenAPITools.Client.ISynchronousClient client,Org.OpenAPITools.Client.IAsynchronousClient asyncClient, Org.OpenAPITools.Client.IReadableConfiguration configuration)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(asyncClient == null) throw new ArgumentNullException("asyncClient");
            if(configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public Org.OpenAPITools.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public Org.OpenAPITools.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Org.OpenAPITools.Client.IReadableConfiguration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Org.OpenAPITools.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>IdResponse</returns>
        public IdResponse CreateTopic (CreateTopicModel createTopicModel = default(CreateTopicModel))
        {
             Org.OpenAPITools.Client.ApiResponse<IdResponse> localVarResponse = CreateTopicWithHttpInfo(createTopicModel);
             return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>ApiResponse of IdResponse</returns>
        public Org.OpenAPITools.Client.ApiResponse< IdResponse > CreateTopicWithHttpInfo (CreateTopicModel createTopicModel = default(CreateTopicModel))
        {
            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "application/json"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.Data = createTopicModel;


            // make the HTTP request
            var localVarResponse = this.Client.Post< IdResponse >("/api/topic/create", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("CreateTopic", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>Task of IdResponse</returns>
        public async System.Threading.Tasks.Task<IdResponse> CreateTopicAsync (CreateTopicModel createTopicModel = default(CreateTopicModel))
        {
             Org.OpenAPITools.Client.ApiResponse<IdResponse> localVarResponse = await CreateTopicAsyncWithHttpInfo(createTopicModel);
             return localVarResponse.Data;

        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="createTopicModel"> (optional)</param>
        /// <returns>Task of ApiResponse (IdResponse)</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<IdResponse>> CreateTopicAsyncWithHttpInfo (CreateTopicModel createTopicModel = default(CreateTopicModel))
        {

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "application/json"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };
            
            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);
            
            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);
            
            localVarRequestOptions.Data = createTopicModel;


            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.PostAsync<IdResponse>("/api/topic/create", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("CreateTopic", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns></returns>
        public void JoinTopic (JoinTopicModel joinTopicModel = default(JoinTopicModel))
        {
             JoinTopicWithHttpInfo(joinTopicModel);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public Org.OpenAPITools.Client.ApiResponse<Object> JoinTopicWithHttpInfo (JoinTopicModel joinTopicModel = default(JoinTopicModel))
        {
            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "application/json"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.Data = joinTopicModel;


            // make the HTTP request
            var localVarResponse = this.Client.Post<Object>("/api/topic/join-topic", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("JoinTopic", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task JoinTopicAsync (JoinTopicModel joinTopicModel = default(JoinTopicModel))
        {
             await JoinTopicAsyncWithHttpInfo(joinTopicModel);

        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="joinTopicModel"> (optional)</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<Object>> JoinTopicAsyncWithHttpInfo (JoinTopicModel joinTopicModel = default(JoinTopicModel))
        {

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "application/json"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
            };
            
            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);
            
            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);
            
            localVarRequestOptions.Data = joinTopicModel;


            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.PostAsync<Object>("/api/topic/join-topic", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("JoinTopic", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>IdResponse</returns>
        public IdResponse SendPost (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream))
        {
             Org.OpenAPITools.Client.ApiResponse<IdResponse> localVarResponse = SendPostWithHttpInfo(topicId, title, text, image);
             return localVarResponse.Data;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>ApiResponse of IdResponse</returns>
        public Org.OpenAPITools.Client.ApiResponse< IdResponse > SendPostWithHttpInfo (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream))
        {
            // verify the required parameter 'topicId' is set
            if (topicId == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'topicId' when calling TopicApi->SendPost");

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "multipart/form-data"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (topicId != null)
            {
                localVarRequestOptions.FormParameters.Add("TopicId", Org.OpenAPITools.Client.ClientUtils.ParameterToString(topicId)); // form parameter
            }
            if (title != null)
            {
                localVarRequestOptions.FormParameters.Add("Title", Org.OpenAPITools.Client.ClientUtils.ParameterToString(title)); // form parameter
            }
            if (text != null)
            {
                localVarRequestOptions.FormParameters.Add("Text", Org.OpenAPITools.Client.ClientUtils.ParameterToString(text)); // form parameter
            }
            if (image != null)
            {
                localVarRequestOptions.FileParameters.Add("Image", image);
            }


            // make the HTTP request
            var localVarResponse = this.Client.Post< IdResponse >("/api/topic/send-post", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("SendPost", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>Task of IdResponse</returns>
        public async System.Threading.Tasks.Task<IdResponse> SendPostAsync (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream))
        {
             Org.OpenAPITools.Client.ApiResponse<IdResponse> localVarResponse = await SendPostAsyncWithHttpInfo(topicId, title, text, image);
             return localVarResponse.Data;

        }

        /// <summary>
        ///  
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="topicId"></param>
        /// <param name="title"> (optional)</param>
        /// <param name="text"> (optional)</param>
        /// <param name="image"> (optional)</param>
        /// <returns>Task of ApiResponse (IdResponse)</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<IdResponse>> SendPostAsyncWithHttpInfo (string topicId, string title = default(string), string text = default(string), System.IO.Stream image = default(System.IO.Stream))
        {
            // verify the required parameter 'topicId' is set
            if (topicId == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'topicId' when calling TopicApi->SendPost");


            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
                "multipart/form-data"
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/plain",
                "application/json",
                "text/json"
            };
            
            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);
            
            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);
            
            if (topicId != null)
            {
                localVarRequestOptions.FormParameters.Add("TopicId", Org.OpenAPITools.Client.ClientUtils.ParameterToString(topicId)); // form parameter
            }
            if (title != null)
            {
                localVarRequestOptions.FormParameters.Add("Title", Org.OpenAPITools.Client.ClientUtils.ParameterToString(title)); // form parameter
            }
            if (text != null)
            {
                localVarRequestOptions.FormParameters.Add("Text", Org.OpenAPITools.Client.ClientUtils.ParameterToString(text)); // form parameter
            }
            if (image != null)
            {
                localVarRequestOptions.FileParameters.Add("Image", image);
            }


            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.PostAsync<IdResponse>("/api/topic/send-post", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("SendPost", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
