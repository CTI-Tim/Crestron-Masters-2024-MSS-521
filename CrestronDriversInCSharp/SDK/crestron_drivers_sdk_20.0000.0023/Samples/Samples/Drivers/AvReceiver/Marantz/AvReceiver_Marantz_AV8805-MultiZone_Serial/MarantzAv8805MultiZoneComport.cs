// Copyright (C) 2018 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

using System;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.RADAVReceiver;
using Crestron.RAD.ProTransports;
using Crestron.SimplSharp;                    				// For Basic SIMPL# Classes

namespace Crestron.RAD.Drivers.AVReceivers
{
    public class MarantzAv8805Comport : ABasicAVReceiver, ISerialComport, ISimpl
    {
        private SimplTransport _transport;

        public void Initialize(IComPort comPort)
        {
            ConnectionTransport = new CommonSerialComport(comPort)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug
            };

            ReceiverProtocol = new SoundUnitedAvrProtocol(ConnectionTransport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            ReceiverProtocol.StateChange += StateChange;
            ReceiverProtocol.RxOut += SendRxOut;
            ReceiverProtocol.Initialize(AvrData);
        }

        public SimplTransport Initialize(Action<string, object[]> send)
        {
            _transport = new SimplTransport { Send = send};
            ConnectionTransport = _transport;
            ConnectionTransport.LogTxAndRxAsBytes = false;

            ReceiverProtocol = new SoundUnitedAvrProtocol(ConnectionTransport, Id);
            ReceiverProtocol.StateChange += StateChange;
            ReceiverProtocol.RxOut += SendRxOut;
            ReceiverProtocol.Initialize(AvrData);
            return _transport;
        }
    }
}
