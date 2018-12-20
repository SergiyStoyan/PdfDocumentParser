//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Net;

namespace Cliver
{
    class WebClient : System.Net.WebClient
    {
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult asyncResult)
        {
            WebResponse response = null;
            try
            {
                response = base.GetWebResponse(request, asyncResult);
            }
            catch (WebException e)
            {
                response = e.Response;
            }
            Response = response as HttpWebResponse;
            return response;
        }
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = null;
            try
            {
                response = base.GetWebResponse(request);
            }
            catch (WebException e)
            {
                response = e.Response;
            }
            Response = response as HttpWebResponse;
            return response;
        }
        public HttpWebResponse Response { get; private set; } = null;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            Request = request as HttpWebRequest;
            Request.CookieContainer = cookieContainer;
            return request;
        }
        private readonly CookieContainer cookieContainer = new CookieContainer();
        public HttpWebRequest Request { get; private set; } = null;
    }
}