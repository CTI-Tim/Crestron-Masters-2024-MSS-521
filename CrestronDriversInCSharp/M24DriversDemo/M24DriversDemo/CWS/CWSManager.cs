
using Crestron.SimplSharp;
using Crestron.SimplSharp.WebScripting;
using System;

namespace M24DriversDemo
{
    internal static class CWSManager
    {
        internal static HttpCwsServer HttpCwsServer;
        internal static void InitializeCWSServer()
        {
            try
            {
                HttpCwsServer = new HttpCwsServer("/api");

                var displayDrivers = new HttpCwsRoute("drivers")
                {
                    RouteHandler = new DisplayDriverRequestHandler()
                };

                HttpCwsServer.Routes.Add(displayDrivers);

                if (HttpCwsServer.Register())
                {
                    CrestronConsole.PrintLine("Web Server Running");
                }
            }
            catch (Exception)
            {
                ErrorLog.Error("Error creating the CWS Server");

            }
        }
    }
}
