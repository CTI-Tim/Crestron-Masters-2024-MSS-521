namespace Crestron.RemoteService.MessageManagement;
        // class declarations
         class MessageManager;
         class Message;
         class Request;
         class Response;
         class ResponseResultCode;
         class ResponseResult;
         class MessageTypes;
    static class MessageManager 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING SenderID[];
    };

     class Message 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Type[];
        STRING SenderId[];

        // class properties
    };

     class Request 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Action[];

        // class properties
    };

     class Response 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Action[];
        Crestron.RemoteService.MessageManagement.ResponseResult Result;

        // class properties
    };

    static class ResponseResultCode // enum
    {
        static SIGNED_LONG_INTEGER Success;
        static SIGNED_LONG_INTEGER Fail;
        static SIGNED_LONG_INTEGER NotCurrentlyAvailable;
        static SIGNED_LONG_INTEGER NotSupported;
        static SIGNED_LONG_INTEGER InvalidParameter;
        static SIGNED_LONG_INTEGER ParameterOutOfRange;
        static SIGNED_LONG_INTEGER Offline;
        static SIGNED_LONG_INTEGER BadVersion;
        static SIGNED_LONG_INTEGER NoncompatibleFeatures;
        static SIGNED_LONG_INTEGER InvalidCode;
        static SIGNED_LONG_INTEGER UnsupportedAlgorithm;
        static SIGNED_LONG_INTEGER BadSession;
        static SIGNED_LONG_INTEGER NotRegistered;
        static SIGNED_LONG_INTEGER RsaHashFailure;
        static SIGNED_LONG_INTEGER Busy;
        static SIGNED_LONG_INTEGER AlreadyRegistered;
        static SIGNED_LONG_INTEGER InternalError;
        static SIGNED_LONG_INTEGER TimedOutLocal;
    };

     class ResponseResult 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        Crestron.RemoteService.MessageManagement.ResponseResultCode ResultCode;

        // class properties
    };

    static class MessageTypes 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING Request[];
        static STRING Response[];

        // class properties
    };

