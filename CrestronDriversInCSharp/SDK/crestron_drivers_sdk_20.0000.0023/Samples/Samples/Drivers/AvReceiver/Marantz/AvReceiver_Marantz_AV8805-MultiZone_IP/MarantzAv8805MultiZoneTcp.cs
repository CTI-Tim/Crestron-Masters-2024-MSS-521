// Copyright (C) 2018 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.RADAVReceiver;
using Crestron.SimplSharp; 

namespace Crestron.RAD.Drivers.AVReceivers
{
    public class MarantzAv8805Tcp : ABasicAVReceiver, ITcp
    {
        public MarantzAv8805Tcp() { }
        public void Initialize(IPAddress ipAddress, int port)
        {
            var tcpTransport = new TcpTransport
            {
                EnableAutoReconnect = EnableAutoReconnect,
                EnableLogging = EnableLogging,
                EnableRxDebug = EnableRxDebug,
                EnableTxDebug = EnableTxDebug
            };

            tcpTransport.Initialize(ipAddress, port);
            ConnectionTransport = tcpTransport;

            ReceiverProtocol = new SoundUnitedAvrProtocol(ConnectionTransport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            ReceiverProtocol.StateChange += StateChange;
            ReceiverProtocol.RxOut += SendRxOut;
            ReceiverProtocol.Initialize(AvrData);
        }
    }
}
