namespace Crestron.RemoteServiceSimpl;
        // class declarations
         class DeviceTypes;
         class DeveloperSettings;
         class AlexaGlobalRoomSimpl;
         class SessionManagerSimpl;
    static class DeviceTypes // enum
    {
        static SIGNED_LONG_INTEGER Light;
        static SIGNED_LONG_INTEGER Shade;
        static SIGNED_LONG_INTEGER Thermostat;
        static SIGNED_LONG_INTEGER Other;
        static SIGNED_LONG_INTEGER Conferencing;
        static SIGNED_LONG_INTEGER AvDevice;
    };

     class DeveloperSettings 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING CustomDeviceUrl[];
        STRING CustomDialogUrl[];

        // class properties
    };

     class AlexaGlobalRoomSimpl 
    {
        // class delegates
        delegate FUNCTION DelegateNoParams ( );

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateNoParams RecallGlobalHomeSimpl;
        DelegateProperty DelegateNoParams RecallGlobalAwaySimpl;
        DelegateProperty DelegateNoParams RecallGlobalGoodMorningSimpl;
        DelegateProperty DelegateNoParams RecallGlobalGoodNightSimpl;
    };

     class SessionManagerSimpl 
    {
        // class delegates
        delegate FUNCTION DelegateUshort ( INTEGER state );
        delegate FUNCTION DelegateSimplString ( SIMPLSHARPSTRING value );

        // class events

        // class functions
        FUNCTION Initialize ( SIGNED_LONG_INTEGER serverFromSimpl );
        FUNCTION SendRegister ( STRING registrationCode );
        FUNCTION SendVerificationRequest ( STRING verificationRequest );
        FUNCTION EnableLogging ();
        FUNCTION DisableLogging ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateUshort ConnectionStateChangedSimpl;
        DelegateProperty DelegateUshort RegisterStateChangedSimpl;
        DelegateProperty DelegateSimplString IDChangedSimpl;
        DelegateProperty DelegateSimplString DialogUrlChangedSimpl;
        DelegateProperty DelegateUshort SessionStateChangedSimpl;
    };

namespace Crestron.RemoteServiceSimpl.Room;
        // class declarations
         class AlexaRoomBase;
         class AlexaRoomSimplShades;
         class AlexaRoomSimplEvents;
         class AlexaRoomSimplClimate;
         class AlexaRoomSimplGenericDevices;
         class AlexaRoomSimplMeeting;
         class AlexaRoomSimplLights;
         class ConfigurationItem;
           class DelegateInt;
           class DelegateIntInt;
           class DelegateNoParams;
           class DelegateString;
           class DelegateAdHoc;
     class AlexaRoomBase 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class AlexaRoomSimplShades 
    {
        // class delegates
        delegate FUNCTION DelegateInt ( INTEGER index );
        delegate FUNCTION DelegateIntInt ( INTEGER index , INTEGER value );

        // class events

        // class functions
        FUNCTION AddShadeLoad ( INTEGER index , STRING name , STRING description , STRING manufacturer , STRING model , INTEGER supportsLevels , INTEGER supportsOnOff , INTEGER isOfflineNumber , INTEGER supportsReportState , INTEGER prependRoomName );
        FUNCTION AddDefaultShadeLoad ( INTEGER supportsReportState );
        FUNCTION AddShadeScene ( INTEGER index , STRING name );
        FUNCTION RemoveAllShadeScenes ();
        FUNCTION SetShadeDeviceFeedback ( INTEGER index , INTEGER level );
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateInt ShadeDeviceOnSimpl;
        DelegateProperty DelegateInt ShadeDeviceOffSimpl;
        DelegateProperty DelegateIntInt ShadeDeviceSetPositionSimpl;
        DelegateProperty DelegateInt RecallShadeSceneSimpl;
    };

     class AlexaRoomSimplEvents 
    {
        // class delegates
        delegate FUNCTION DelegateNoParams ( );

        // class events

        // class functions
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateNoParams RecallTooColdSimpl;
        DelegateProperty DelegateNoParams RecallTooWarmSimpl;
        DelegateProperty DelegateNoParams RecallTooDarkSimpl;
        DelegateProperty DelegateNoParams RecallTooBrightSimpl;
    };

     class AlexaRoomSimplClimate 
    {
        // class delegates
        delegate FUNCTION DelegateInt ( INTEGER index );

        // class events

        // class functions
        FUNCTION AddThermostatLoad ( INTEGER index , STRING name , STRING description , STRING manufacturer , STRING model , INTEGER isOfflineNumber , INTEGER supportsReportState , INTEGER prependRoomName );
        FUNCTION AddDefaultThermostatLoad ( INTEGER supportsReportState );
        FUNCTION SetThermostatLoadHeatSetpointFeedback ( INTEGER setpoint );
        FUNCTION SetThermostatLoadCoolSetpointFeedback ( INTEGER setpoint );
        FUNCTION SetThermostatLoadAutoSetpointFeedback ( INTEGER setpoint );
        FUNCTION SetThermostatLoadModeFeedback ( INTEGER mode );
        FUNCTION SetThermostatTemperatureUnitFeedback ( INTEGER mode );
        FUNCTION SetThermostatAutoModeIsSingleSetpoint ( INTEGER autoModeIsSingleSetpoint );
        FUNCTION SetThermostatLoadFanSpeed ( INTEGER value );
        FUNCTION SetThermostatLoadMinCoolSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadMaxCoolSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadMinHeatSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadMaxHeatSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadMinAutoSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadMaxAutoSetpoint ( INTEGER value );
        FUNCTION SetThermostatLoadCurrentTemperature ( INTEGER value );
        FUNCTION SetThermostatLoadHeatModeAvailable ( INTEGER heatModeAvailable );
        FUNCTION SetThermostatLoadCoolModeAvailable ( INTEGER coolModeAvailable );
        FUNCTION SetThermostatLoadAutoModeAvailable ( INTEGER autoModeAvailable );
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateInt ThermostatLoadSetHeatSetpointSimpl;
        DelegateProperty DelegateInt ThermostatLoadSetCoolSetpointSimpl;
        DelegateProperty DelegateInt ThermostatLoadSetAutoSetpointSimpl;
        DelegateProperty DelegateInt ThermostatLoadSetModeSimpl;
        DelegateProperty DelegateInt ThermostatLoadSetFanSpeedSimpl;
    };

     class AlexaRoomSimplGenericDevices 
    {
        // class delegates
        delegate FUNCTION DelegateInt ( INTEGER index );
        delegate FUNCTION DelegateIntInt ( INTEGER index , INTEGER value );

        // class events

        // class functions
        FUNCTION AddGenericLoad ( INTEGER index , STRING name , STRING description , STRING manufacturer , STRING model , INTEGER supportsLevels , INTEGER supportsOnOff , INTEGER isOfflineNumber , INTEGER supportsReportState , INTEGER prependRoomName );
        FUNCTION SetGenericDeviceFeedback ( INTEGER index , INTEGER level );
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateInt GenericDeviceOnSimpl;
        DelegateProperty DelegateInt GenericDeviceOffSimpl;
        DelegateProperty DelegateIntInt GenericDeviceSetLevelSimpl;
    };

     class AlexaRoomSimplMeeting 
    {
        // class delegates
        delegate FUNCTION DelegateNoParams ( );
        delegate FUNCTION DelegateString ( SIMPLSHARPSTRING value );
        delegate FUNCTION DelegateAdHoc ( SIMPLSHARPSTRING id , SIMPLSHARPSTRING provider , SIMPLSHARPSTRING protocol , SIMPLSHARPSTRING endpoint , SIMPLSHARPSTRING pin );
        delegate FUNCTION DelegateInt ( INTEGER index );

        // class events

        // class functions
        FUNCTION AddConferencingDevice ( STRING deviceName , STRING description , INTEGER isCalendarEnabled , INTEGER isCalendarAvailable , INTEGER isEnabled , INTEGER isOnline , INTEGER prependRoomNameToLoadName );
        FUNCTION UpdateConferencingDevice ( STRING deviceName , STRING description , INTEGER isCalendarEnabled , INTEGER isCalendarAvailable , INTEGER isEnabled , INTEGER isOnline , INTEGER prependRoomNameToLoadName );
        FUNCTION SetCurrentMeeting ( STRING meetingId , STRING organizer , STRING title , STRING startTime , STRING endTime );
        FUNCTION RequestToJoinMeeting ();
        FUNCTION RequestToLeaveMeeting ();
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateNoParams RecallStartMeetingSimpl;
        DelegateProperty DelegateNoParams RecallStopMeetingSimpl;
        DelegateProperty DelegateNoParams JoinScheduledMeetingSimpl;
        DelegateProperty DelegateNoParams JoinAdHocMeetingSimpl;
        DelegateProperty DelegateNoParams LeaveMeetingSimpl;
        DelegateProperty DelegateString ScheduledMeetingIdSimpl;
        DelegateProperty DelegateAdHoc AdHocMeetingSimpl;
        DelegateProperty DelegateInt MeetingStatusSimpl;
    };

     class AlexaRoomSimplLights 
    {
        // class delegates
        delegate FUNCTION DelegateInt ( INTEGER index );
        delegate FUNCTION DelegateIntInt ( INTEGER index , INTEGER value );

        // class events

        // class functions
        FUNCTION AddLightingLoad ( INTEGER index , STRING name , STRING description , STRING manufacturer , STRING model , INTEGER supportsLevels , INTEGER supportsOnOff , INTEGER isOfflineNumber , INTEGER supportsReportState , INTEGER prependRoomName );
        FUNCTION AddDefaultLightingLoad ( INTEGER supportsReportState );
        FUNCTION AddLightingScene ( INTEGER index , STRING name );
        FUNCTION RemoveAllLightingScenes ();
        FUNCTION SetLightingDeviceFeedback ( INTEGER index , INTEGER level );
        FUNCTION Initialize ( STRING roomName );
        FUNCTION UpdateOfflineStatus ( INTEGER index , INTEGER deviceTypeNumber , INTEGER isOfflineNumber );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty DelegateInt LightingDeviceOnSimpl;
        DelegateProperty DelegateInt LightingDeviceOffSimpl;
        DelegateProperty DelegateIntInt LightingDeviceSetLevelSimpl;
        DelegateProperty DelegateInt RecallLightSceneSimpl;
    };

     class ConfigurationItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING RoomName[];
        STRING RoomId[];

        // class properties
    };

namespace Crestron.RemoteServiceSimpl.Common;
        // class declarations
         class Logger;
     class Logger 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

