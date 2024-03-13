using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.WebScripting;

namespace CTI_MainProgram.CWS
{
    internal class DisplayDriverRequestHandler : IHttpCwsHandler
    {
        public void ProcessRequest(HttpCwsContext context)
        {
            var method = context.Request.HttpMethod;

            switch (method)
            {
              
                case "POST":
                    {
                        try
                        {
                            string fileName = null;
                            string ip = null;

                            context.Response.ContentType = "application/json";

                            // Get the FileName
                            if (context.Request.Headers["X-File-Name"] != null)
                            {
                                CrestronConsole.PrintLine(context.Request.Headers["X-File-Name"]);
                                fileName = context.Request.Headers["X-File-Name"];
                            }
                            //Get the IP assigned to the Driver
                            if (context.Request.Headers["X-File-IP"] != null)
                            {
                                CrestronConsole.PrintLine(context.Request.Headers["X-File-IP"]);
                                ip = context.Request.Headers["X-File-IP"];
                            }

                            if (fileName != null && ip != null)
                            {
                                //Copy the file the Correct Driver Directory
                                using (var fileStream = File.Create(Global.GetDriverPath() + fileName))
                                {
                                    var buffer = new byte[20000];
                                    int bytesRead;
                                    using (var inputStream = context.Request.InputStream)
                                    {
                                        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            fileStream.Write(buffer, 0, bytesRead);
                                        }
                                    }
                                }
                                //Load the new Driver
                                CreateDisplayDrivers.LoadDrivers(fileName, ip);
                                context.Response.Write("{ \"status\":\"OK\"} ", true);
                            }
                            context.Response.Write("{ \"status\":\"ERROR\"} ", true);
                        }
                        catch (System.Exception e)
                        {
                            CrestronConsole.PrintLine($"Error in Post Data {e.Message}");
                        }                    
                        break;
                    }
            }
        }
    }
}