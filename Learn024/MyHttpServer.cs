﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Learn024
{
    internal class MyHttpServer
    {
        private HttpListener listener;

        public MyHttpServer(string[] prefixes)
        {
            if(!HttpListener.IsSupported)
            {
                throw new Exception("HttpListener is not Supported");
            }
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            listener = new HttpListener();

            foreach(string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
        }

        public async Task StartAsync()
        {
            listener.Start();
            do
            {   
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                Console.WriteLine("Waiting for ...");
               
                HttpListenerContext context = await listener.GetContextAsync();
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                Console.WriteLine("Client has connected");
                await ProcessRequest(context);

            } while (listener.IsListening);
        }

        public async Task ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Console.WriteLine($"{request.HttpMethod} {request.RawUrl} {request?.Url?.AbsolutePath}");

            // Lấy stream / gửi dữ liệu về cho client
            var outputstream = response.OutputStream;


            switch (request?.Url?.AbsolutePath)
            {
                case "/requestinfo":
                    {
                        context.Response.Headers.Add("content-type", "text/html");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;

                        string responseString = GenerateHTML(request);
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;
                case "/":
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("Hello world!");
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;

                case "/stop":
                    {
                        listener.Stop();
                        Console.WriteLine("Stop Http");
                    }
                    break;

                case "/json":
                    {
                        response.Headers.Add("Content-Type", "application/json");
                        var product = new
                        {
                            Name = "Macbook Pro",
                            Price = 2000,
                            Manufacturer = "Apple"
                        };
                        string jsonstring = JsonConvert.SerializeObject(product);
                        byte[] buffer = Encoding.UTF8.GetBytes(jsonstring);
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);

                    }
                    break;
                case "/anh2.png":
                    {
                        response.Headers.Add("Content-Type", "image/png");
                        byte[] buffer = await File.ReadAllBytesAsync("anh2.png");
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);

                    }
                    break;

                default:
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        byte[] buffer = Encoding.UTF8.GetBytes("NOT FOUND!");
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;
            }

            // switch (request.Url.AbsolutePath)

            outputstream.Close();
        }

        public string GenerateHTML(HttpListenerRequest request)
        {
            string format = @"<!DOCTYPE html>
                            <html lang=""en""> 
                                <head>
                                    <meta charset=""UTF-8"">
                                    {0}
                                 </head> 
                                <body>
                                    {1}
                                </body> 
                            </html>";
            string head = "<title>Test WebServer</title>";
            var body = new StringBuilder();
            body.Append("<h1>Request Info</h1>");
            body.Append("<h2>Request Header:</h2>");

            // Header infomation
            var headers = from key in request.Headers.AllKeys
                          select $"<div>{key} : {string.Join(",", request.Headers.GetValues(key))}</div>";
            body.Append(string.Join("", headers));

            //Extract request properties
            body.Append("<h2>Request properties:</h2>");
            var properties = request.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property != null)
                {
                    var name_pro = property.Name;
                    string value_pro = "Default";
                    try
                    {
                        value_pro = property.GetValue(request).ToString();
                    }
                    catch (Exception e)
                    {
                        value_pro = e.Message;
                    }
                    body.Append($"<div>{name_pro} : {value_pro}</div>");
                }
            };
            return string.Format(format, head, body.ToString());
        }
    }
}
