/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Net;

namespace Cliver
{
    /// <summary>
    /// Enhanced System.Net.WebClient
    /// </summary>
    public class WebClient : System.Net.WebClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult asyncResult)
        {
            Exception = null;
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request, asyncResult);
            }
            catch (WebException e)
            {
                Exception = e;
                response = e.Response;
            }
            Response = response as HttpWebResponse;
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            Exception = null;
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request);
            }
            catch (WebException e)
            {
                Exception = e;
                response = e.Response;
            }
            Response = response as HttpWebResponse;
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        public HttpWebResponse Response { get; private set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public WebException Exception { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            Request = request as HttpWebRequest;
            Request.CookieContainer = CookieContainer;
            return request;
        }
        /// <summary>
        /// 
        /// </summary>
        public HttpWebRequest Request { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public readonly CookieContainer CookieContainer = new CookieContainer();
    }
}