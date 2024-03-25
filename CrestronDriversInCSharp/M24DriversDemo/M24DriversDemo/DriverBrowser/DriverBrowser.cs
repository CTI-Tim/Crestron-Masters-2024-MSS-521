using System;
using System.Collections.Generic;
using Crestron.SimplSharp.CrestronIO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.RAD.Common.ExtensionMethods;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common;
using Crestron.RAD.Common.BasicDriver;
using System.Net;

namespace M24DriversDemo
{
    class DriverBrowser
    {
        //Build paths needed. Using GetApplicationDirectory and GetApplicationRootDirectory return correct path based on the type of processor being used.
        //Path.DirectorySeparatorChar returns \\ or / depending on the same. This ensures this will work regardless of the type of processor it is running on. 
        static string _tempPath = string.Format("{0}{1}_browserTemp", Directory.GetApplicationDirectory(), Path.DirectorySeparatorChar.ToString());        
        static string _appDriverPath = string.Format("{0}{1}Drivers{1}", Directory.GetApplicationDirectory(), Path.DirectorySeparatorChar.ToString());                
        static string _userDriverPath = string.Format("{0}{1}USER{1}Drivers{1}", Directory.GetApplicationRootDirectory(), Path.DirectorySeparatorChar.ToString());

        //Main driver list. This is populated with all the drivers found in the system.
        public List<DriverFile> DriverList;

        //Driver that has been marked as "Active" and the associated IP address
        public string DriverIPAddress;
        
        //Enum for driver types
        public enum DriverType
        {
            All,
            Display,
            Receiver,
            Source
        }

        /// <summary>
        /// Constructor, passes in the UI that will be used.
        /// </summary>
        /// <param name="BrowserUI"></param>
        public DriverBrowser()
        {
            //Set IP addresses just to show the RX traffic from a driver in the UI.
            DriverIPAddress = "172.0.0.1";
            Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.DriverIpAddress, "127.0.0.1");
        }

        /// <summary>
        /// Refreshes the list of drivers based on the driverType that is passed in.
        /// </summary>
        /// <param name="driverType">enum indicating what type of drivers to filter by.</param>
        public void RefreshList(DriverType driverType)
        {

            //If the temp directory for storing the files already exists, delete and recreate it            
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
                Directory.CreateDirectory(_tempPath);
                Console.SendLine("Directory Created");
            }
            //Calls to extract all *.pkg files in any of the locations we assume they might be stored. After this step there is a folder full of all the extracted files
            //_extractAllDrivers(_appPath, _tempPath);
            //_extractAllDrivers(_userPath, _tempPath);
            _extractAllDrivers(_userDriverPath, _tempPath);
            _extractAllDrivers(_appDriverPath, _tempPath);

            //Build a string list of all of the *.dat files
            string[] _driverFiles = Directory.GetFiles(_tempPath, "*.dat");            

            //Clear the list incase there is existing data in it.
            if (DriverList != null)
            {
                DriverList.Clear();
            }
            DriverList = new List<DriverFile>();

            //Build a list of drivers based on the list of files.
            foreach (string driverFile in _driverFiles)
            {
                DriverList.Add(new DriverFile(Path.GetFileNameWithoutExtension(driverFile),_tempPath));
            }
            //take the full list and filter it by type. Pass that filtered list to _drawList to get it displayed on the screen
            _drawList(_filterDrivers(DriverList, driverType));    
        }

        /// <summary>
        /// internal method extracts all the zip files that end in *.pkg in the provided path into a destination path. 
        /// </summary>
        /// <param name="originPath">Path that the files to be extracted exist in</param>
        /// <param name="destinationPath">Path that all file will be extracted to </param>
        internal void _extractAllDrivers(string originPath, string destinationPath)
        {
            try
            {
                string[] _driverFiles;
                if (Directory.GetFiles(originPath, "*.pkg").Length > 0)
                {
                    _driverFiles = Directory.GetFiles(originPath, "*.pkg");                    
                    foreach (string _driverFile in _driverFiles)
                    {
                        Crestron.SimplSharp.CrestronZIP.Unzip(_driverFile, destinationPath);
                    }
                }
            }
            catch (Exception e)
            {
                Console.SendLine("Exception in _extractAllDrivers() : " + e.Message);
            }
        }

        /// <summary>
        /// Internal function to filter the drivers and return a list of only the ones of the selected type.
        /// </summary>
        /// <param name="fullDriversList">List of drivers to be filtered.</param>
        /// <param name="sortType">Type of driver to filter by.</param>
        /// <returns>Filtered list of drivers</returns>
        internal List<DriverFile> _filterDrivers(List<DriverFile> fullDriversList,DriverType sortType)
        {
            List<DriverFile> _drivers = new List<DriverFile>(); //Create list for sorted infomation
            foreach (DriverFile _driverFile in fullDriversList) //Switch on the curent selected sort value
            {                
                switch (sortType)
                {
                    case DriverType.All://For each option only add the drivers that match the DeviceType as defined in the API documentation. Using ToLower becase there is inconsistency in drivers.
                        {
                            _drivers.Add(_driverFile);
                            break;
                        }
                    case DriverType.Display:
                        {
                            if (_driverFile.ApiData.DeviceType.ToLower() == "flat panel display" || _driverFile.ApiData.DeviceType.ToLower() == "projector")
                            {
                                _drivers.Add(_driverFile);
                            }
                            break;
                        }
                    case DriverType.Receiver:
                        {
                            if (_driverFile.ApiData.DeviceType.ToLower() == "av receiver")
                            {
                                _drivers.Add(_driverFile);
                            }
                            break;                            
                        }
                    case DriverType.Source:
                        {
                            if (_driverFile.ApiData.DeviceType.ToLower() == "video server" || _driverFile.ApiData.DeviceType.ToLower() == "bluray player" || _driverFile.ApiData.DeviceType.ToLower() == "cable box")
                            {
                                _drivers.Add(_driverFile);
                            }
                            break;
                        }
                    default:
                        {
                            _drivers.Add(_driverFile);
                            break;
                        }
                }
            }
            return _drivers;
        }

        /// <summary>
        /// Internal Method to draw the UI list based on the list object of drivers
        /// </summary>
        /// <param name="driverList">Driver list to display.</param>
        internal void _drawList(List<DriverFile> driverList)
        {
            try
            {
                _clearList();//Clear the list first                
                Global.M24DemoUI.SetJoin("DriverList", "Set Number of Items", (ushort)driverList.Count);//Set the number of lines in the list based on the driverList count                
                int i = 1; //incrementing value used for Smartobject join names
                foreach (DriverFile _driver in driverList) //Step through each object in the list
                {
                    //use UI framework to add an anonymous action object into the object location in the join. This calls _selectDriver when a line is pressed.  
                    Global.M24DemoUI.SetOutputObject("DriverList", "Item " + i + " Pressed", UserInterface.JoinType.Digital, new Action<bool>(_press => { if (_press) { _selectDriver(_driver); } }));
                    //Set the text on the list buttons
                    Global.M24DemoUI.SetJoin("DriverList", "Set Item " + i + " Text", _driver.ApiData.Manufacturer + " " + _driver.ApiData.BaseModel);
                    i++;
                }
            }
            catch(Exception e)
            {
                Console.SendLine("Exception in _drawList : " + e.Message);
            }
        }

        /// <summary>
        /// Internal method to clear the UI list
        /// </summary>
        internal void _clearList()
        {            
            try
            {
                //Set list size to 0
                Global.M24DemoUI.SetJoin("DriverList", "Set Number of Items", (ushort)0);
                for (int x = 1; x < 31; x++) //List has a maximum of 30 lines, Ones based. Loop through every button. 
                {
                    //Clear the object used for the press action
                    Global.M24DemoUI.SetOutputObject("DriverList", "Item " + x + " Pressed", UserInterface.JoinType.Digital, null);
                    //Xlear the text on the button
                    Global.M24DemoUI.SetJoin("DriverList", "Set Item " + x + " Text", "");
                }                
            }
            catch(Exception e)
            {
                Console.SendLine("Exception in _clearList" + e.Message);                
            }
        }

        /// <summary>
        /// Internal method called when a driver is selected from the list. 
        /// </summary>
        /// <param name="driver">Driver selected that should be displayed</param>
        internal void _selectDriver(DriverFile driver)
        {
            if (driver.ApiData.DeviceType == "Flat Panel Display" || driver.ApiData.DeviceType == "Projector") //If we have selected a Display
            {
                //Set all the infomation on the sected driver on the UI
                Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.ErrorMessage, "");                
                Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.SelectedDeviceType, driver.ApiData.DeviceType);
                Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.SelectedManufacture, driver.ApiData.Manufacturer);
                Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.SelectedBaseModel, driver.ApiData.BaseModel);

                //Add an anonymous method to the Load Driver button that calls _loadDriver after IP validation
                Global.M24DemoUI.SetOutputObject((uint)JoinNumbers.Digital.LoadDriver, UserInterface.JoinType.Digital, new Action<bool>(_press =>
                {
                    if (_press)
                    {
                        IPAddress _address = new IPAddress(0);
                        if (IPAddress.TryParse(DriverIPAddress, out _address)) //Validate if an IP address has been entered or not and if it is a real(ish) IP address. 
                        {
                            _loadDriver(driver, DriverIPAddress);
                        }
                        else
                        {
                            Global.M24DemoUI.SetJoin((uint)JoinNumbers.Serial.ErrorMessage, "Invalid IP Address");
                        }

                    }
                }));
            }
            else
            {
                Global.M24DemoUI.SetJoin(20, "Only display drivers can be loaded.");
            }
        }

        /// <summary>
        /// Internal method to Load the driver in for use.
        /// </summary>
        /// <param name="driver">Driver to load</param>
        /// <param name="ipAddress">Ip Address to pass to driver</param>
        internal void _loadDriver(DriverFile driver, string ipAddress)
        {
            try
            {
                //Check if the directory exists.
                if (!Directory.Exists(_userDriverPath))
                {
                    Directory.Create(_userDriverPath);
                }

                //For this exapmple only loads one driver at a time.
                  

               File.Copy(driver.DllFile, _userDriverPath + Path.GetFileName(driver.DllFile), true);  //Copy the driver file to our driver folder             
               CreateDisplayDrivers.LoadDrivers(Path.GetFileName(driver.DllFile), ipAddress); //Load Driver
            }
            catch (Exception e)
            {
                Console.SendLine("Exception in _loadDriver" + e.Message);
            }            
        }
    }
}
