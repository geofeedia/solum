﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace solum.core.http
{
    public abstract class HttpRequestHandler : Component, IHttpRequestHandler
    {
        public HttpRequestHandler()
        {

        }

        public abstract bool AsyncSupported { get; }

        public bool AcceptRequest(HttpListenerRequest request)
        {
            return OnAcceptRequest(request);
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                OnHandleRequest(request, response);
            }
            catch (FileNotFoundException ex)
            {
                response.StatusCode = 404;
                response.StatusDescription = ex.Message.RemoveControlCharacters();
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusDescription = "An error occurred processing your request: {0}".format(ex.Message).RemoveControlCharacters();
            }
        }
        public async Task HandleRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                await OnHandleRequestAsync(request, response);
            }
            catch (FileNotFoundException ex)
            {
                response.StatusCode = 404;
                response.StatusDescription = ex.Message.RemoveControlCharacters();
            }
            catch (Exception ex)
            {                
                response.StatusCode = 500;
                response.StatusDescription = "An error occurred processing your request: {0}".format(ex.Message).RemoveControlCharacters();
            }
        }

        protected abstract bool OnAcceptRequest(HttpListenerRequest request);
        protected abstract void OnHandleRequest(HttpListenerRequest request, HttpListenerResponse response);
        protected abstract Task OnHandleRequestAsync(HttpListenerRequest request, HttpListenerResponse response);
    }
}