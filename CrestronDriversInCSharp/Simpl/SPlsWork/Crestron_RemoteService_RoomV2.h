namespace Crestron.RemoteService.Room;
        // class declarations
         class Item;
         class GroupItem;
         class EventItem;
         class AlexaRoom;
         class LevelChangeTypes;
         class OnOffStateTypes;
         class Images;
         class ItemEvent;
         class AlexaRoomManager;
         class GlobalMacro;
         class InvalidParameterResponse;
         class RemoteServiceClientType;
         class ImageSize;
         class Image;
         class DeviceItem;
         class SupportedFeature;
         class ClientVerification;
         class ItemSubtype;
         class ItemType;
         class ZoneItem;
         class ThermostatResponse;
         class SetpointLimitTypes;
         class Device;
         class SceneItem;
         class CurrentMeeting;
         class TemperatureUnits;
         class SetpointTypes;
         class SceneType;
         class ThermostatMode;
         class FanSpeed;
         class Result;
         class ReservedMacros;
         class ItemTree;
           class StreamingPlayerCapability;
           class PlayerCapability;
           class SpeakerCapability;
           class MediaGroupingCapability;
     class Item 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

     class GroupItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

     class EventItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

     class AlexaRoom 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION AddLightingScene ( STRING id , STRING sceneName );
        FUNCTION RemoveLightingScene ( STRING sceneId );
        FUNCTION RemoveAllLightingScenes ();
        FUNCTION RemoveShadeLoad ( STRING id );
        FUNCTION RemoveAllShadeLoads ();
        FUNCTION AddShadeScene ( STRING id , STRING sceneName );
        FUNCTION RemoveShadeScene ( STRING id );
        FUNCTION RemoveAllShadeScenes ();
        FUNCTION UpdateCurrentMeeting ( STRING meetingId , STRING organizer , STRING title , STRING startTime , STRING endTime );
        FUNCTION RemoveThermostatLoad ( STRING id );
        FUNCTION RemoveAllThermostatLoads ();
        FUNCTION UpdateThermostatLoadMode ( STRING id , ThermostatMode currentMode );
        FUNCTION UpdateLoadFanSpeed ( STRING id , FanSpeed speed );
        FUNCTION UpdateThermostatLoadUnits ( STRING id , TemperatureUnits unit );
        FUNCTION AddClimateScene ( STRING id , STRING sceneName );
        FUNCTION RemoveClimateScene ( STRING id );
        FUNCTION RemoveAllClimateScenes ();
        FUNCTION RemoveMediaDevice ( STRING id );
        FUNCTION RemoveGenericLoad ( STRING id );
        FUNCTION RemoveAllGenericLoads ();
        FUNCTION UpdateGenericLoadFeedback ( STRING id , SIGNED_LONG_INTEGER level );
        FUNCTION RemoveLightingLoad ( STRING id );
        FUNCTION RemoveAllLightingLoads ();
        FUNCTION UpdateLightingLoadFeedback ( STRING id , SIGNED_LONG_INTEGER level );
        FUNCTION Dispose ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING NormalizedRoomName[];
        STRING RoomName[];
        StreamingPlayerCapability StreamingPlayer;
        PlayerCapability Player;
        SpeakerCapability Speaker;
        MediaGroupingCapability MediaGrouping;
    };

    static class LevelChangeTypes // enum
    {
        static SIGNED_LONG_INTEGER Set;
        static SIGNED_LONG_INTEGER Increment;
        static SIGNED_LONG_INTEGER Decrement;
    };

    static class OnOffStateTypes // enum
    {
        static SIGNED_LONG_INTEGER On;
        static SIGNED_LONG_INTEGER Off;
    };

     class ItemEvent 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Room[];
        STRING Capability[];
        STRING Event[];

        // class properties
    };

    static class AlexaRoomManager 
    {
        // class delegates

        // class events
        static EventHandler RecallGlobalMacro ( GlobalMacro macro );

        // class functions
        static FUNCTION AddAlternateSpelling ( STRING originalSpelling , STRING alternateSpelling );
        static FUNCTION RemoveAlternateSpelling ( STRING alternateSpelling );
        static FUNCTION RemoveAlternateSpellings ( STRING originalSpelling );
        static FUNCTION RemoveAllAlternateSpellings ();
        static FUNCTION RemoveItem ( STRING path , Item item );
        static FUNCTION RemoveAllItems ( STRING path , ItemType type , ItemSubtype subtype );
        static FUNCTION ReportState ( DeviceItem device );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        RemoteServiceClientType ClientType;
    };

    static class GlobalMacro // enum
    {
        static SIGNED_LONG_INTEGER Home;
        static SIGNED_LONG_INTEGER Away;
        static SIGNED_LONG_INTEGER GoodMorning;
        static SIGNED_LONG_INTEGER GoodNight;
    };

     class InvalidParameterResponse 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING RequiredParameters[][];
        STRING InvalidParameter[];

        // class properties
    };

    static class RemoteServiceClientType // enum
    {
        static SIGNED_LONG_INTEGER Crestron;
        static SIGNED_LONG_INTEGER CrestronHome;
    };

    static class ImageSize // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER XSmall;
        static SIGNED_LONG_INTEGER Small;
        static SIGNED_LONG_INTEGER Medium;
        static SIGNED_LONG_INTEGER Large;
        static SIGNED_LONG_INTEGER XLarge;
    };

     class DeviceItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Manufacturer[];
        STRING Model[];
        STRING Version[];
        STRING Description[];
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

    static class SupportedFeature // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER OnOff;
        static SIGNED_LONG_INTEGER OpenClose;
        static SIGNED_LONG_INTEGER Levels;
        static SIGNED_LONG_INTEGER Temperature;
        static SIGNED_LONG_INTEGER FanSpeed;
        static SIGNED_LONG_INTEGER Feedback;
        static SIGNED_LONG_INTEGER JoinMeeting;
        static SIGNED_LONG_INTEGER LeaveMeeting;
        static SIGNED_LONG_INTEGER Calendar;
        static SIGNED_LONG_INTEGER ReportState;
        static SIGNED_LONG_INTEGER ReportEvent;
        static SIGNED_LONG_INTEGER VolumeSetting;
        static SIGNED_LONG_INTEGER VolumeAdjustment;
        static SIGNED_LONG_INTEGER Muting;
        static SIGNED_LONG_INTEGER AudioDucking;
        static SIGNED_LONG_INTEGER Play;
        static SIGNED_LONG_INTEGER Pause;
        static SIGNED_LONG_INTEGER Stop;
        static SIGNED_LONG_INTEGER StartOver;
        static SIGNED_LONG_INTEGER Previous;
        static SIGNED_LONG_INTEGER Next;
        static SIGNED_LONG_INTEGER Rewind;
        static SIGNED_LONG_INTEGER FastForward;
        static SIGNED_LONG_INTEGER StreamingPlayerAudioOnly;
        static SIGNED_LONG_INTEGER StreamingPlayerAudioVideo;
        static SIGNED_LONG_INTEGER StreamingPlayerQueue;
        static SIGNED_LONG_INTEGER StreamingPlayerDisplayMetadata;
        static SIGNED_LONG_INTEGER MediaGroupingDefault;
    };

     class ClientVerification 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ClientId[];
        STRING VerifyCode[];
    };

    static class ItemSubtype // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER Any;
        static SIGNED_LONG_INTEGER Light;
        static SIGNED_LONG_INTEGER Shade;
        static SIGNED_LONG_INTEGER Climate;
        static SIGNED_LONG_INTEGER Fan;
        static SIGNED_LONG_INTEGER Generic;
        static SIGNED_LONG_INTEGER Conference;
        static SIGNED_LONG_INTEGER Media;
    };

    static class ItemType // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER Any;
        static SIGNED_LONG_INTEGER Device;
        static SIGNED_LONG_INTEGER Zone;
        static SIGNED_LONG_INTEGER Scene;
        static SIGNED_LONG_INTEGER Event;
        static SIGNED_LONG_INTEGER Group;
    };

     class ZoneItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

     class ThermostatResponse 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        Crestron.RemoteService.Room.ThermostatMode PreviousDeviceMode;
        Crestron.RemoteService.Room.ThermostatMode CurrentDeviceMode;
        Crestron.RemoteService.Room.TemperatureUnits CurrentDeviceUnit;

        // class properties
    };

    static class SetpointLimitTypes // enum
    {
        static SIGNED_LONG_INTEGER MinCool;
        static SIGNED_LONG_INTEGER MaxCool;
        static SIGNED_LONG_INTEGER MinHeat;
        static SIGNED_LONG_INTEGER MaxHeat;
        static SIGNED_LONG_INTEGER MinAuto;
        static SIGNED_LONG_INTEGER MaxAuto;
    };

     class Device 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Id[];
        STRING Manufacturer[];
        STRING Model[];
        STRING Version[];
        STRING Name[];
        STRING LoadName[];
        STRING Room[];
        STRING Description[];
        ItemType Type;
    };

     class SceneItem 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Name[];
        Crestron.RemoteService.Room.ItemType Type;

        // class properties
    };

     class CurrentMeeting 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING MeetingId[];
        STRING OrganizerName[];
        STRING Title[];
        STRING StartTime[];
        STRING EndTime[];
    };

    static class TemperatureUnits // enum
    {
        static SIGNED_LONG_INTEGER celsius;
        static SIGNED_LONG_INTEGER fahrenheit;
        static SIGNED_LONG_INTEGER unknown;
    };

    static class SetpointTypes // enum
    {
        static SIGNED_LONG_INTEGER any;
        static SIGNED_LONG_INTEGER cool;
        static SIGNED_LONG_INTEGER heat;
        static SIGNED_LONG_INTEGER auto;
    };

    static class SceneType // enum
    {
        static SIGNED_LONG_INTEGER lights;
        static SIGNED_LONG_INTEGER shades;
        static SIGNED_LONG_INTEGER all;
        static SIGNED_LONG_INTEGER climate;
    };

    static class ThermostatMode // enum
    {
        static SIGNED_LONG_INTEGER off;
        static SIGNED_LONG_INTEGER cool;
        static SIGNED_LONG_INTEGER heat;
        static SIGNED_LONG_INTEGER auto;
    };

    static class FanSpeed // enum
    {
        static SIGNED_LONG_INTEGER auto;
        static SIGNED_LONG_INTEGER on;
        static SIGNED_LONG_INTEGER low;
        static SIGNED_LONG_INTEGER medium;
        static SIGNED_LONG_INTEGER high;
        static SIGNED_LONG_INTEGER off;
    };

     class Result 
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

    static class ReservedMacros // enum
    {
        static SIGNED_LONG_INTEGER TooCold;
        static SIGNED_LONG_INTEGER TooWarm;
        static SIGNED_LONG_INTEGER TooDark;
        static SIGNED_LONG_INTEGER TooBright;
        static SIGNED_LONG_INTEGER StartMeeting;
        static SIGNED_LONG_INTEGER StopMeeting;
    };

     class ItemTree 
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

namespace Crestron.RemoteService.RoomV2.Capabilities;
        // class declarations
         class SpeakerCapability;
         class SpeakerFeatures;
         class SpeakerActions;
         class SpeakerAction;
         class SpeakerState;
         class PlayerControl;
         class PlayerControlType;
         class PlayerControlName;
         class StreamingPlayerCapability;
         class StreamingFeatures;
         class StreamingActions;
         class PlayBehavior;
         class PlayRequestor;
         class CaptionType;
         class InterruptedBehavior;
         class PlaybackStatus;
         class ClearBehavior;
         class StreamingState;
         class StreamPlaybackErrorType;
         class QueueStatus;
         class ConfirmedAction;
         class MediaGroupingCapability;
         class MediaGroupingFeatures;
         class MediaGroupingActions;
         class DeviceGroupError;
         class DeviceGroupErrorType;
         class PlayerCapability;
         class PlayerFeatures;
         class PlayerActions;
         class PlaybackCommand;
         class PlayerState;
         class PlayerSupportedAction;
         class PlayerAction;
         class PlayerActionState;
         class ShuffleState;
         class RepeatState;
         class Rating;
         class MediaType;
     class SpeakerCapability 
    {
        // class delegates
        delegate FUNCTION SetDeviceVolumeAction ( STRING id , STRING room , SIGNED_LONG_INTEGER level );
        delegate FUNCTION AdjustDeviceVolumeAction ( STRING id , STRING room , SIGNED_LONG_INTEGER levelAdjustment );

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static SIGNED_LONG_INTEGER RecommendedDuckingTimeout;

        // class properties
        DelegateProperty SetDeviceVolumeAction SetDeviceVolumeHandler;
        DelegateProperty AdjustDeviceVolumeAction AdjustDeviceVolumeHandler;
    };

    static class SpeakerFeatures 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING VolumeSetting[];
        static STRING VolumeAdjustment[];
        static STRING Muting[];
        static STRING AudioDucking[];

        // class properties
    };

    static class SpeakerActions 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING SetDeviceVolume[];
        static STRING AdjustDeviceVolume[];
        static STRING SetDeviceVolumeMute[];
        static STRING SetDeviceAudioDucking[];

        // class properties
    };

    static class SpeakerAction // enum
    {
        static SIGNED_LONG_INTEGER SetVolume;
        static SIGNED_LONG_INTEGER AdjustVolume;
        static SIGNED_LONG_INTEGER SetMute;
        static SIGNED_LONG_INTEGER SetAudioDucking;
    };

    static class SpeakerState // enum
    {
        static SIGNED_LONG_INTEGER Volume;
        static SIGNED_LONG_INTEGER Muted;
        static SIGNED_LONG_INTEGER AudioDucked;
    };

    static class PlayerControlType // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Button;
        static SIGNED_LONG_INTEGER Toggle;
    };

    static class PlayerControlName // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER PlayPause;
        static SIGNED_LONG_INTEGER Next;
        static SIGNED_LONG_INTEGER Previous;
        static SIGNED_LONG_INTEGER SkipForward;
        static SIGNED_LONG_INTEGER SkipBackward;
        static SIGNED_LONG_INTEGER Shuffle;
        static SIGNED_LONG_INTEGER Repeat;
        static SIGNED_LONG_INTEGER Like;
        static SIGNED_LONG_INTEGER Dislike;
    };

     class StreamingPlayerCapability 
    {
        // class delegates
        delegate FUNCTION PlayDeviceStreamAction ( STRING id , STRING room , PlayBehavior playBehavior , PlayRequestor playRequestorType , STRING playRequestorId , STRING audioItemId , STRING streamUrl , SIGNED_LONG_INTEGER streamOffset , STRING streamExpiryTime , STRING streamToken , STRING streamPreviousToken , CaptionType streamCaptionType , STRING streamCaption , SIGNED_LONG_INTEGER streamProgressReportDelay , SIGNED_LONG_INTEGER streamProgressReportInterval , InterruptedBehavior streamInterruptedBehavior );
        delegate FUNCTION StopDeviceStreamAction ( STRING id , STRING room );
        delegate FUNCTION ClearDeviceStreamAction ( STRING id , STRING room , ClearBehavior clearBehavior );
        delegate FUNCTION SetDeviceStreamProgressIntervalAction ( STRING id , STRING room , SIGNED_LONG_INTEGER progressInterval );
        delegate FUNCTION DisplayDeviceStreamMetadataAction ( STRING id , STRING room , STRING audioItemId , STRING title , STRING titleSubtext1 , STRING titleSubtext2 , STRING header , STRING headerSubtext1 , SIGNED_LONG_INTEGER duration , Images art , STRING providerName , Images providerLogo , PlayerControl controls[] );

        // class events

        // class functions
        FUNCTION UpdateState ( STRING deviceId , STRING streamToken , StreamingState streamingState , SIGNED_LONG_INTEGER streamOffset );
        FUNCTION NotifyStreamPlaybackStatusChanged ( STRING deviceId , STRING streamToken , PlaybackStatus playbackStatus , StreamingState streamingState , SIGNED_LONG_INTEGER streamOffset );
        FUNCTION NotifyStreamPlaybackFailed ( STRING deviceId , STRING failedStreamToken , STRING currentStreamToken , SIGNED_LONG_INTEGER currentStreamOffset , StreamPlaybackErrorType errorType , STRING errorMessage , StreamingState currentState );
        FUNCTION NotifyStreamingPlayerQueueChanged ( STRING deviceId , QueueStatus status , STRING currentStreamToken , SIGNED_LONG_INTEGER currentStreamOffset , StreamingState currentState );
        FUNCTION NotifyStreamingPlayerConfirmation ( STRING deviceId , ConfirmedAction confirmedAction , STRING currentStreamToken , SIGNED_LONG_INTEGER currentStreamOffset , StreamingState currentState );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty PlayDeviceStreamAction PlayDeviceStreamActionHandler;
        DelegateProperty StopDeviceStreamAction StopDeviceStreamActionHandler;
        DelegateProperty ClearDeviceStreamAction ClearDeviceStreamActionHandler;
        DelegateProperty SetDeviceStreamProgressIntervalAction SetDeviceStreamProgressIntervalActionHandler;
        DelegateProperty DisplayDeviceStreamMetadataAction DisplayDeviceStreamMetadataActionHandler;
    };

    static class StreamingFeatures 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING AudioOnly[];
        static STRING AudioVideo[];
        static STRING Queue[];
        static STRING DisplayMetadata[];

        // class properties
    };

    static class StreamingActions 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING PlayDeviceStream[];
        static STRING StopDeviceStream[];
        static STRING SetDeviceStreamProgressInterval[];
        static STRING ClearDeviceStream[];
        static STRING DisplayDeviceStreamMetadata[];

        // class properties
    };

    static class PlayBehavior // enum
    {
        static SIGNED_LONG_INTEGER ReplaceAll;
        static SIGNED_LONG_INTEGER Enqueue;
        static SIGNED_LONG_INTEGER ReplaceEnqueued;
    };

    static class PlayRequestor // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Alert;
    };

    static class CaptionType // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER WebVTT;
    };

    static class InterruptedBehavior // enum
    {
        static SIGNED_LONG_INTEGER Attenuate;
        static SIGNED_LONG_INTEGER Pause;
    };

    static class PlaybackStatus // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Started;
        static SIGNED_LONG_INTEGER InitialProgressReport;
        static SIGNED_LONG_INTEGER ProgressReport;
        static SIGNED_LONG_INTEGER Paused;
        static SIGNED_LONG_INTEGER Resumed;
        static SIGNED_LONG_INTEGER Stopped;
        static SIGNED_LONG_INTEGER AlmostFinished;
        static SIGNED_LONG_INTEGER StutterStarted;
        static SIGNED_LONG_INTEGER StutterFinished;
        static SIGNED_LONG_INTEGER Finished;
    };

    static class ClearBehavior // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Enqueued;
        static SIGNED_LONG_INTEGER All;
    };

    static class StreamingState // enum
    {
        static SIGNED_LONG_INTEGER Idle;
        static SIGNED_LONG_INTEGER Playing;
        static SIGNED_LONG_INTEGER Paused;
        static SIGNED_LONG_INTEGER BufferUnderrun;
        static SIGNED_LONG_INTEGER Stopped;
        static SIGNED_LONG_INTEGER Finished;
    };

    static class StreamPlaybackErrorType // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
    };

    static class QueueStatus // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Cleared;
    };

    static class ConfirmedAction // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER SetDeviceStreamProgressInterval;
    };

     class MediaGroupingCapability 
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

    static class MediaGroupingFeatures 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING Default[];

        // class properties
    };

    static class MediaGroupingActions 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING ChangeDeviceGroup[];
        static STRING SyncDeviceGroupRequest[];

        // class properties
    };

    static class DeviceGroupErrorType // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER DifferentNetworks;
        static SIGNED_LONG_INTEGER DeviceUnreachable;
        static SIGNED_LONG_INTEGER NoSuchDevice;
    };

     class PlayerCapability 
    {
        // class delegates
        delegate FUNCTION SetDevicePlaybackAction ( STRING id , PlaybackCommand playbackCommand );

        // class events

        // class functions
        FUNCTION UpdateState ( STRING deviceId , PlayerState playerState , PlayerSupportedAction supportedActions[] , SIGNED_LONG_INTEGER position , ShuffleState shuffleState , RepeatState repeatState , Rating rating , STRING playbackSource , STRING playbackSourceId , STRING trackName , STRING trackId , STRING trackNumber , STRING artist , STRING artistId , STRING album , STRING albumId , STRING coverUrlTiny , STRING coverUrlSmall , STRING coverUrlMedium , STRING coverUrlLarge , STRING coverUrlFull , STRING coverId , STRING mediaProvider , MediaType mediaType , SIGNED_LONG_INTEGER duration );
        FUNCTION NotifyPlayerActionOccurred ( STRING deviceId , PlayerAction playerAction , PlayerActionState playerActionState );
        FUNCTION NotifyPlayerStateChanged ( STRING deviceId );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty SetDevicePlaybackAction SetDevicePlaybackActionHandler;
    };

    static class PlayerFeatures 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING Play[];
        static STRING Pause[];
        static STRING Stop[];
        static STRING StartOver[];
        static STRING Previous[];
        static STRING Next[];
        static STRING Rewind[];
        static STRING FastForward[];

        // class properties
    };

    static class PlayerActions 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING SetDevicePlayback[];

        // class properties
    };

    static class PlaybackCommand // enum
    {
        static SIGNED_LONG_INTEGER Play;
        static SIGNED_LONG_INTEGER Pause;
        static SIGNED_LONG_INTEGER Stop;
        static SIGNED_LONG_INTEGER StartOver;
        static SIGNED_LONG_INTEGER Previous;
        static SIGNED_LONG_INTEGER Next;
        static SIGNED_LONG_INTEGER Rewind;
        static SIGNED_LONG_INTEGER FastForward;
    };

    static class PlayerState // enum
    {
        static SIGNED_LONG_INTEGER Idle;
        static SIGNED_LONG_INTEGER Playing;
        static SIGNED_LONG_INTEGER Paused;
        static SIGNED_LONG_INTEGER Stopped;
        static SIGNED_LONG_INTEGER Finished;
    };

    static class PlayerSupportedAction // enum
    {
        static SIGNED_LONG_INTEGER Play;
        static SIGNED_LONG_INTEGER Pause;
        static SIGNED_LONG_INTEGER Stop;
        static SIGNED_LONG_INTEGER StartOver;
        static SIGNED_LONG_INTEGER Previous;
        static SIGNED_LONG_INTEGER Next;
        static SIGNED_LONG_INTEGER Rewind;
        static SIGNED_LONG_INTEGER FastForward;
        static SIGNED_LONG_INTEGER AdjustSeekPosition;
        static SIGNED_LONG_INTEGER SetSeekPosition;
        static SIGNED_LONG_INTEGER Like;
        static SIGNED_LONG_INTEGER Dislike;
        static SIGNED_LONG_INTEGER EnableShuffle;
        static SIGNED_LONG_INTEGER DisableShuffle;
        static SIGNED_LONG_INTEGER EnableRepeat;
        static SIGNED_LONG_INTEGER EnableRepeatOne;
        static SIGNED_LONG_INTEGER DisableRepeat;
    };

    static class PlayerAction // enum
    {
        static SIGNED_LONG_INTEGER Unknown;
        static SIGNED_LONG_INTEGER Play;
        static SIGNED_LONG_INTEGER Pause;
        static SIGNED_LONG_INTEGER Previous;
        static SIGNED_LONG_INTEGER Next;
        static SIGNED_LONG_INTEGER SkipForward;
        static SIGNED_LONG_INTEGER SkipBackward;
        static SIGNED_LONG_INTEGER Shuffle;
        static SIGNED_LONG_INTEGER Loop;
        static SIGNED_LONG_INTEGER Repeat;
        static SIGNED_LONG_INTEGER Like;
        static SIGNED_LONG_INTEGER Dislike;
    };

    static class PlayerActionState // enum
    {
        static SIGNED_LONG_INTEGER NotApplicable;
        static SIGNED_LONG_INTEGER On;
        static SIGNED_LONG_INTEGER Off;
    };

    static class ShuffleState // enum
    {
        static SIGNED_LONG_INTEGER Off;
        static SIGNED_LONG_INTEGER Shuffle;
    };

    static class RepeatState // enum
    {
        static SIGNED_LONG_INTEGER Off;
        static SIGNED_LONG_INTEGER Repeated;
        static SIGNED_LONG_INTEGER OneRepeated;
    };

    static class Rating // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER Liked;
        static SIGNED_LONG_INTEGER Disliked;
    };

    static class MediaType // enum
    {
        static SIGNED_LONG_INTEGER Other;
        static SIGNED_LONG_INTEGER Track;
        static SIGNED_LONG_INTEGER Podcast;
        static SIGNED_LONG_INTEGER Station;
        static SIGNED_LONG_INTEGER Ad;
        static SIGNED_LONG_INTEGER Sample;
    };

namespace Crestron.RemoteService.RoomV2.Room.New_Datastructures;
        // class declarations
         class DeviceItemEvent;
     class DeviceItemEvent 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        STRING Id[];
        STRING Room[];
        STRING Capability[];
        STRING Event[];

        // class properties
    };

