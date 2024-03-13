namespace Crestron.RemoteService.SessionManager;
        // class declarations
         class SessionManager;
         class FailureCodes;
         class AppFeatures;
         class ConnectionSecurity;
         class UrlManager;
         class Server;
         class ConnectionManager;
     class SessionManager 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION UpdateConnectionURL ( Server server );
        FUNCTION Shutdown ();
        FUNCTION GetReportStateSettings ();
        FUNCTION SendClientVerification ( STRING verifyMessage );
        FUNCTION SendConnectionCheck ();
        FUNCTION SendRegister ( STRING registrationCode );
        FUNCTION SendUnregister ();
        FUNCTION SendCreateSession ();
        FUNCTION SendStartSession ();
        FUNCTION SendEndSession ();
        FUNCTION SendHeartbeat ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ID[];
    };

    static class FailureCodes // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER InvalidCode;
        static SIGNED_LONG_INTEGER Other;
        static SIGNED_LONG_INTEGER TimedOut;
        static SIGNED_LONG_INTEGER NotRegistered;
    };

     class AppFeatures 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING AppVersion[];

        // class properties
    };

     class ConnectionSecurity 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION GetPublicKey ();
        FUNCTION GenerateKeys ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

    static class UrlManager 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING DialogUrl[];
        STRING AmazonVoiceRegistrationUrl[];
        STRING GoogleVoiceRegistrationUrl[];
    };

    static class Server // enum
    {
        static SIGNED_LONG_INTEGER UsDev;
        static SIGNED_LONG_INTEGER UsPreProduction;
        static SIGNED_LONG_INTEGER UsRelease;
        static SIGNED_LONG_INTEGER EuPreProduction;
        static SIGNED_LONG_INTEGER EuRelease;
        static SIGNED_LONG_INTEGER UsQe;
        static SIGNED_LONG_INTEGER ApacPreProduction;
        static SIGNED_LONG_INTEGER ApacRelease;
        static SIGNED_LONG_INTEGER UsReleaseTest;
        static SIGNED_LONG_INTEGER EuReleaseTest;
        static SIGNED_LONG_INTEGER ApacReleaseTest;
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER Custom;
    };

     class ConnectionManager 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

