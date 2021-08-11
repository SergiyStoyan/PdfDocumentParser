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
    public class WebClient : System.Net.WebClient
    {
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
        public HttpWebResponse Response { get; private set; } = null;
        public WebException Exception { get; private set; } = null;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            Request = request as HttpWebRequest;
            Request.CookieContainer = CookieContainer;
            return request;
        }
        public HttpWebRequest Request { get; private set; } = null;

        public readonly CookieContainer CookieContainer = new CookieContainer();
    }
}