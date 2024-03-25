using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes



namespace M24DriversDemo
{
    public partial class UserInterface
    {
        void _debug(string message)
        {
            CrestronConsole.PrintLine(message);
        }
    }
}