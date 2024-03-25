using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Linq;

namespace M24DriversDemo
{
    internal static class GetDriverAssembly
    {
        internal static objectType getAssembly<objectType>(string driverFileName, string iName, string tName)
        {
            string _filename = "";

            if (driverFileName.EndsWith(".dll"))
            {
                _filename = driverFileName;
            }
            else if (driverFileName.EndsWith(".pkg"))
            {
                _filename = extractFiles(driverFileName);
            }

            var myDll = System.Reflection.Assembly.LoadFrom(_filename);
            var types = myDll.GetTypes();

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();

                    // Look for the device interface we are after
                    if (interfaces.FirstOrDefault(x => x.Name == iName) == null)
                        continue;

                    // Look for the Transport interface that is not null
                    if (interfaces.FirstOrDefault(x => x.Name == tName) != null)
                        return (objectType)myDll.CreateInstance(type.FullName);
                }
            return default(objectType);
        }
        private static string extractFiles(string filename)
        {
            var path = string.Empty;

            if (File.Exists(filename))  // Check if file has been uploaded to processor
            {
                try
                {
                    CrestronZIP.Unzip(filename, Global.GetDriverPath());              // UnZip the Crestron driver .pkg file
                    path = filename.Replace(".pkg", ".dll");                          // get the .dll file from the package                                        
                }
                catch (Exception e)
                {
                    CrestronConsole.PrintLine($"Exception thrown extracting file {filename} for {e.Message}");
                }
            }
            return path;
        }
    }
}
