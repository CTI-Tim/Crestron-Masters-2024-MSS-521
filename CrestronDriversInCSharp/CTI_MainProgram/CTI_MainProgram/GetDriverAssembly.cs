using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Linq;

namespace CTI_MainProgram
{
    internal static class GetDriverAssembly
    {
        internal static objectType getAssembly<objectType>(string filename, string iName, string tName , string packagetype)
        {

            if (packagetype == ".dll")
            {
                var myDll = System.Reflection.Assembly.LoadFrom(filename);
                var myType = myDll.GetTypes();

                foreach (var type in myType)
                {
                    var interfaces = type.GetInterfaces();

                    // Look for the device interface we are after
                    if (interfaces.FirstOrDefault(x => x.Name == iName) == null)
                        continue;

                    // Look for the Transport interface that is not null
                    if (interfaces.FirstOrDefault(x => x.Name == tName) != null)
                        return (objectType)myDll.CreateInstance(type.FullName);
                }
                
            }
            else if (packagetype == ".pkg")
            {
                var myDll = System.Reflection.Assembly.LoadFrom(getFileName(filename));
                var myType = myDll.GetTypes();

                foreach (var type in myType)
                {
                    var interfaces = type.GetInterfaces();

                    // Look for the device interface we are after
                    if (interfaces.FirstOrDefault(x => x.Name == iName) == null)
                        continue;

                    // Look for the Transport interface that is not null
                    if (interfaces.FirstOrDefault(x => x.Name == tName) != null)
                        return (objectType)myDll.CreateInstance(type.FullName);
                }

            }

            return default(objectType);
        }
        private static string getFileName(string filename)
        {
            var path = string.Empty;

            if (File.Exists(filename))  // Check if file has been uploaded to processor
            {
                try
                {
                    CrestronZIP.Unzip(filename, Global.GetDriverPath());              // UnZip the Crestron driver .pkg file
                    path = filename.Replace(".pkg", ".dll");                          // get the .dll file from the package
                    File.Delete(filename);                                            // Delete the original .pkg
                    File.Delete(filename.Replace(".pkg",".dat"));                     // Delete the unneeded .dat file
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
