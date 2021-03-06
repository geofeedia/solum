﻿using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace solum.core.http
{
    public static class HttpExtensions
    {
        static ILogger Log = Serilog.Log.ForContext(typeof(HttpExtensions));

        public static async Task<string> HttpGetAsync(this string url, Dictionary<string, string> requestHeaders = null)
        {
            var client = new HttpClient();

            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public static async Task<string> HttpGetAsync(this string url, TimeSpan timeout)
        {
            using (var client = new HttpClient())
            {                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // ** Set the timeout
                client.Timeout = timeout;

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    Log.Debug("HttpClient: Request success... Reading content...");
                    var content = await response.Content.ReadAsStringAsync();
                    Log.Debug("HttpClient: Content read sucessfully...");
                    return content;
                }
                else
                    return null;
            }
        }

        public static void Write(this HttpListenerResponse response, string contentType, string content)
        {
            var encoding = SystemSettings.Encoding;
            var bytes = encoding.GetBytes(content);

            response.ContentType = contentType;
            response.ContentLength64 = bytes.Length;

            using (var writer = new BinaryWriter(response.OutputStream, encoding, leaveOpen: true))
            {
                writer.Write(bytes);
            }
        }

        public static void Write(this HttpListenerResponse response, string contentType, long length, Stream input)
        {
            response.ContentType = contentType;

            if (length >= 0)
                response.ContentLength64 = length;

            input.CopyTo(response.OutputStream);
        }
        public static void Write(this HttpListenerResponse response, string contentType, Stream input)
        {
            response.ContentType = contentType;

            // TODO: Dynamically set content length?

            input.CopyTo(response.OutputStream);
        }

        public static async Task WriteAsync(this HttpListenerResponse response, string contentType, long length, Stream input)
        {
            response.ContentType = contentType;
            response.ContentLength64 = length;

            await input.CopyToAsync(response.OutputStream);
        }
        public static async Task WriteAsync(this HttpListenerResponse response, string contentType, Stream input)
        {
            response.ContentType = contentType;

            // TODO: Dynamically set content length?
            await input.CopyToAsync(response.OutputStream);
        }

        public static string RemoveControlCharacters(this string inString)
        {
            if (inString == null) return null;
            StringBuilder newString = new StringBuilder();
            char ch;
            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if (!char.IsControl(ch))
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        }
    }
}
