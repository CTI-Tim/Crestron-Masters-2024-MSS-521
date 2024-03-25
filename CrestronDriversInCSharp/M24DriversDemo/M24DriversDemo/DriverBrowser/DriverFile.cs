using Crestron.RAD.Common;
using Newtonsoft.Json;
using System;
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M24DriversDemo
{
    class DriverFile
    {
        public string DatFile;
        public string DllFile;
        public string FilePath;
        public GeneralInformation ApiData;

        public DriverFile(string driverFile, string filePath)
        {

            DatFile = filePath + Path.DirectorySeparatorChar.ToString() + driverFile + ".dat";
            DllFile = filePath + Path.DirectorySeparatorChar.ToString() + driverFile + ".dll";
            FilePath = filePath;
            Console.SendLine("Driver Object Creation Started " + driverFile + " " + filePath);
            Console.SendLine(DatFile);
            string _apiData;
            ApiData = new GeneralInformation();
            using (FileStream _apiFile = new FileStream(DatFile, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    Console.SendLine("Opening File Stream");
                    StreamReader _sr = new StreamReader(_apiFile);
                    Console.SendLine("Read String");
                    _apiData = _sr.ReadToEnd();
                    ApiData = JsonConvert.DeserializeObject<GeneralInformation>(_apiData);
                    Console.SendLine(ApiData.Manufacturer);
                }
                catch (Exception e)
                {
                    Console.SendLine(e.Message);
                }
            }
        }
    }
}
