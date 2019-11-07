using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace NetFrameWork.Core.WebService
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthHeaderBehavior : IEndpointBehavior
    {
        private readonly AuthHeaderInserter _authHeaderInserter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerInserter"></param>
        public AuthHeaderBehavior(AuthHeaderInserter headerInserter)
        {
            _authHeaderInserter = headerInserter;
        }

        #region IEndpointBehavior Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(_authHeaderInserter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}