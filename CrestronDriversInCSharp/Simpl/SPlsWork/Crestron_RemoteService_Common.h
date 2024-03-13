namespace Crestron.RemoteService.Common;
        // class declarations
         class LogLevel;
         class LogVisibility;
         class ExtensionMethods;
    static class LogLevel // enum
    {
        static SIGNED_LONG_INTEGER Off;
        static SIGNED_LONG_INTEGER Error;
        static SIGNED_LONG_INTEGER Warning;
        static SIGNED_LONG_INTEGER Info;
        static SIGNED_LONG_INTEGER Verbose;
    };

    static class LogVisibility // enum
    {
        static SIGNED_LONG_INTEGER Off;
        static SIGNED_LONG_INTEGER Developer;
        static SIGNED_LONG_INTEGER Programmer;
        static SIGNED_LONG_INTEGER User;
    };

    static class ExtensionMethods 
    {
        // class delegates

        // class events

        // class functions
        static STRING_FUNCTION NullToEmpty ( STRING str );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

