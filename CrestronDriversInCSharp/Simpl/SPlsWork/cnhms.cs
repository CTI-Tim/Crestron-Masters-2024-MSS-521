using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;

namespace CrestronModule_CNHMS
{
    public class CrestronModuleClass_CNHMS : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        
        Crestron.Logos.SplusObjects.AnalogInput ROOM_MULTISOURCE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_1_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_2_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_3_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_4_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_5_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_6_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_7_USAGE;
        Crestron.Logos.SplusObjects.AnalogInput SOURCE_8_USAGE;
        Crestron.Logos.SplusObjects.AnalogOutput ROOM_SOURCE;
        ushort [] SOURCE_USAGE;
        ushort [] SOURCE;
        ushort I = 0;
        ushort BEST_SOURCE = 0;
        object ROOM_MULTISOURCE_OnChange_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 20;
                if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( ROOM_MULTISOURCE  .UshortValue > 8 ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 22;
                    SOURCE [ 1] = (ushort) ( ((ROOM_MULTISOURCE  .UshortValue & 61440) >> 12) ) ; 
                    __context__.SourceCodeLine = 23;
                    SOURCE [ 2] = (ushort) ( ((ROOM_MULTISOURCE  .UshortValue & 3840) >> 8) ) ; 
                    __context__.SourceCodeLine = 24;
                    SOURCE [ 3] = (ushort) ( ((ROOM_MULTISOURCE  .UshortValue & 240) >> 4) ) ; 
                    __context__.SourceCodeLine = 25;
                    SOURCE [ 4] = (ushort) ( (ROOM_MULTISOURCE  .UshortValue & 15) ) ; 
                    __context__.SourceCodeLine = 33;
                    ROOM_SOURCE  .Value = (ushort) ( 0 ) ; 
                    __context__.SourceCodeLine = 34;
                    Functions.ProcessLogic ( ) ; 
                    __context__.SourceCodeLine = 36;
                    ushort __FN_FORSTART_VAL__1 = (ushort) ( 1 ) ;
                    ushort __FN_FOREND_VAL__1 = (ushort)4; 
                    int __FN_FORSTEP_VAL__1 = (int)1; 
                    for ( I  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (I  >= __FN_FORSTART_VAL__1) && (I  <= __FN_FOREND_VAL__1) ) : ( (I  <= __FN_FORSTART_VAL__1) && (I  >= __FN_FOREND_VAL__1) ) ; I  += (ushort)__FN_FORSTEP_VAL__1) 
                        { 
                        __context__.SourceCodeLine = 38;
                        if ( Functions.TestForTrue  ( ( Functions.Not( SOURCE_USAGE[ SOURCE[ I ] ] ))  ) ) 
                            { 
                            __context__.SourceCodeLine = 40;
                            ROOM_SOURCE  .Value = (ushort) ( SOURCE[ I ] ) ; 
                            __context__.SourceCodeLine = 41;
                            break ; 
                            } 
                        
                        __context__.SourceCodeLine = 36;
                        } 
                    
                    __context__.SourceCodeLine = 45;
                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt (ROOM_SOURCE  .Value == 0))  ) ) 
                        { 
                        __context__.SourceCodeLine = 47;
                        BEST_SOURCE = (ushort) ( SOURCE[ 1 ] ) ; 
                        __context__.SourceCodeLine = 48;
                        if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( SOURCE_USAGE[ SOURCE[ 2 ] ] < SOURCE_USAGE[ BEST_SOURCE ] ))  ) ) 
                            { 
                            __context__.SourceCodeLine = 50;
                            BEST_SOURCE = (ushort) ( SOURCE[ 2 ] ) ; 
                            } 
                        
                        __context__.SourceCodeLine = 52;
                        if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( SOURCE_USAGE[ SOURCE[ 3 ] ] < SOURCE_USAGE[ BEST_SOURCE ] ))  ) ) 
                            { 
                            __context__.SourceCodeLine = 54;
                            BEST_SOURCE = (ushort) ( SOURCE[ 3 ] ) ; 
                            } 
                        
                        __context__.SourceCodeLine = 56;
                        if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( SOURCE_USAGE[ SOURCE[ 4 ] ] < SOURCE_USAGE[ BEST_SOURCE ] ))  ) ) 
                            { 
                            __context__.SourceCodeLine = 58;
                            BEST_SOURCE = (ushort) ( SOURCE[ 4 ] ) ; 
                            } 
                        
                        __context__.SourceCodeLine = 60;
                        ROOM_SOURCE  .Value = (ushort) ( BEST_SOURCE ) ; 
                        } 
                    
                    } 
                
                else 
                    {
                    __context__.SourceCodeLine = 64;
                    ROOM_SOURCE  .Value = (ushort) ( ROOM_MULTISOURCE  .UshortValue ) ; 
                    }
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object SOURCE_1_USAGE_OnChange_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 69;
            SOURCE_USAGE [ 1] = (ushort) ( SOURCE_1_USAGE  .UshortValue ) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object SOURCE_2_USAGE_OnChange_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 74;
        SOURCE_USAGE [ 2] = (ushort) ( SOURCE_2_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_3_USAGE_OnChange_3 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 79;
        SOURCE_USAGE [ 3] = (ushort) ( SOURCE_3_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_4_USAGE_OnChange_4 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 84;
        SOURCE_USAGE [ 4] = (ushort) ( SOURCE_4_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_5_USAGE_OnChange_5 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 89;
        SOURCE_USAGE [ 5] = (ushort) ( SOURCE_5_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_6_USAGE_OnChange_6 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 94;
        SOURCE_USAGE [ 6] = (ushort) ( SOURCE_6_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_7_USAGE_OnChange_7 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 99;
        SOURCE_USAGE [ 7] = (ushort) ( SOURCE_7_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SOURCE_8_USAGE_OnChange_8 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 104;
        SOURCE_USAGE [ 8] = (ushort) ( SOURCE_8_USAGE  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 110;
        ushort __FN_FORSTART_VAL__1 = (ushort) ( 1 ) ;
        ushort __FN_FOREND_VAL__1 = (ushort)8; 
        int __FN_FORSTEP_VAL__1 = (int)1; 
        for ( I  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (I  >= __FN_FORSTART_VAL__1) && (I  <= __FN_FOREND_VAL__1) ) : ( (I  <= __FN_FORSTART_VAL__1) && (I  >= __FN_FOREND_VAL__1) ) ; I  += (ushort)__FN_FORSTEP_VAL__1) 
            {
            __context__.SourceCodeLine = 111;
            SOURCE_USAGE [ I] = (ushort) ( 0 ) ; 
            __context__.SourceCodeLine = 110;
            }
        
        __context__.SourceCodeLine = 112;
        SOURCE_USAGE [ 0] = (ushort) ( Functions.ToInteger( -( 1 ) ) ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    SocketInfo __socketinfo__ = new SocketInfo( 1, this );
    InitialParametersClass.ResolveHostName = __socketinfo__.ResolveHostName;
    _SplusNVRAM = new SplusNVRAM( this );
    SOURCE_USAGE  = new ushort[ 9 ];
    SOURCE  = new ushort[ 5 ];
    
    ROOM_MULTISOURCE = new Crestron.Logos.SplusObjects.AnalogInput( ROOM_MULTISOURCE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( ROOM_MULTISOURCE__AnalogSerialInput__, ROOM_MULTISOURCE );
    
    SOURCE_1_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_1_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_1_USAGE__AnalogSerialInput__, SOURCE_1_USAGE );
    
    SOURCE_2_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_2_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_2_USAGE__AnalogSerialInput__, SOURCE_2_USAGE );
    
    SOURCE_3_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_3_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_3_USAGE__AnalogSerialInput__, SOURCE_3_USAGE );
    
    SOURCE_4_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_4_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_4_USAGE__AnalogSerialInput__, SOURCE_4_USAGE );
    
    SOURCE_5_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_5_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_5_USAGE__AnalogSerialInput__, SOURCE_5_USAGE );
    
    SOURCE_6_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_6_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_6_USAGE__AnalogSerialInput__, SOURCE_6_USAGE );
    
    SOURCE_7_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_7_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_7_USAGE__AnalogSerialInput__, SOURCE_7_USAGE );
    
    SOURCE_8_USAGE = new Crestron.Logos.SplusObjects.AnalogInput( SOURCE_8_USAGE__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SOURCE_8_USAGE__AnalogSerialInput__, SOURCE_8_USAGE );
    
    ROOM_SOURCE = new Crestron.Logos.SplusObjects.AnalogOutput( ROOM_SOURCE__AnalogSerialOutput__, this );
    m_AnalogOutputList.Add( ROOM_SOURCE__AnalogSerialOutput__, ROOM_SOURCE );
    
    
    ROOM_MULTISOURCE.OnAnalogChange.Add( new InputChangeHandlerWrapper( ROOM_MULTISOURCE_OnChange_0, false ) );
    SOURCE_1_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_1_USAGE_OnChange_1, false ) );
    SOURCE_2_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_2_USAGE_OnChange_2, false ) );
    SOURCE_3_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_3_USAGE_OnChange_3, false ) );
    SOURCE_4_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_4_USAGE_OnChange_4, false ) );
    SOURCE_5_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_5_USAGE_OnChange_5, false ) );
    SOURCE_6_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_6_USAGE_OnChange_6, false ) );
    SOURCE_7_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_7_USAGE_OnChange_7, false ) );
    SOURCE_8_USAGE.OnAnalogChange.Add( new InputChangeHandlerWrapper( SOURCE_8_USAGE_OnChange_8, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    
    
}

public CrestronModuleClass_CNHMS ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}




const uint ROOM_MULTISOURCE__AnalogSerialInput__ = 0;
const uint SOURCE_1_USAGE__AnalogSerialInput__ = 1;
const uint SOURCE_2_USAGE__AnalogSerialInput__ = 2;
const uint SOURCE_3_USAGE__AnalogSerialInput__ = 3;
const uint SOURCE_4_USAGE__AnalogSerialInput__ = 4;
const uint SOURCE_5_USAGE__AnalogSerialInput__ = 5;
const uint SOURCE_6_USAGE__AnalogSerialInput__ = 6;
const uint SOURCE_7_USAGE__AnalogSerialInput__ = 7;
const uint SOURCE_8_USAGE__AnalogSerialInput__ = 8;
const uint ROOM_SOURCE__AnalogSerialOutput__ = 0;

[SplusStructAttribute(-1, true, false)]
public class SplusNVRAM : SplusStructureBase
{

    public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
    
    
}

SplusNVRAM _SplusNVRAM = null;

public class __CEvent__ : CEvent
{
    public __CEvent__() {}
    public void Close() { base.Close(); }
    public int Reset() { return base.Reset() ? 1 : 0; }
    public int Set() { return base.Set() ? 1 : 0; }
    public int Wait( int timeOutInMs ) { return base.Wait( timeOutInMs ) ? 1 : 0; }
}
public class __CMutex__ : CMutex
{
    public __CMutex__() {}
    public void Close() { base.Close(); }
    public void ReleaseMutex() { base.ReleaseMutex(); }
    public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
}
 public int IsNull( object obj ){ return (obj == null) ? 1 : 0; }
}


}
