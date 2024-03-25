using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher.Extender;                          				

namespace AudioVideoSwitcher_Crestron_SampleDriverModel_IP
{
    public class SampleAudioVideoSwitcherProtocol : AAudioVideoSwitcherProtocol
    {
        public SampleAudioVideoSwitcherProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
        }

        protected override void SendDataToComPort(AudioVideoExtenderComPort port, string data, object[] parameters)
        {
            base.SendDataToComPort(port, data, parameters);

            // Fake a response
            DeConstructComPort(port, "ACK");
        }

        protected override void SendDataToCecPort(AudioVideoExtenderCecPort port, string data, object[] parameters)
        {
            base.SendDataToCecPort(port, data, parameters);

            // Fake a response
            DeConstructCecPort(port, "ACK");
        }

        protected override ValidatedRxData ResponseValidator(string response, CommonCommandGroupType commandGroup)
        {
            // This functions the same way other device types handle responses, where an instance of ResponseValidation
            // has a method handles the current buffer of data received from the device.

            // If this method returns an instance of ValidatedRxData that has Ready or Ignore set to true
            // then the buffer will be cleared. In all other cases the buffer will not clear.
            return base.ResponseValidator(response, commandGroup);
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            // This is called with the instance of ValidatedRxData where Ready was set to true.
            // By default, this method chooses to call DeConstruct<Feature> based on the command group assigned 
            // to ValidatedRxData.
            base.ChooseDeconstructMethod(validatedData);
        }

        protected override void DeConstructSwitcherRoute(string response)
        {
            // Receiving: ROUTED=OUTPUT#1:INPUT#1
            //            ROUTED=<output ID>:<input ID>
            //            ROUTED= is stripped out of response before this is called. 

            var routePath = response.Split(':');
            AudioVideoExtender inputExtender = null;
            AudioVideoExtender outputExtender = null;

            // We can get the extender objects here using the API identifier set
            // in the embedded file.
            // We can also get the extender objects by their unique ID using GetExtenderById
            outputExtender = GetExtenderByApiIdentifier(routePath[0]);
            inputExtender = routePath.Length > 1 ? GetExtenderByApiIdentifier(routePath[1]) : null;
            
            // Figured out which input is routed to the specified output
            // Now update the output extender with the current source routed to it
            // The framework will figure out if this was a real change or not if it is not done here.
            if (outputExtender != null)
            {
                outputExtender.VideoSourceExtenderId = inputExtender == null ? 
                    null : inputExtender.Id;
            }
        }

        public override void RouteAudioInput(string inputId, string outputId)
        {
            // This will be called when an application attempts to route an audio input to an output.
            // This command can be built manually by sending a new instance of CommandSet using
            // the method SendCommand.
            
            // The CommandSet will need to be set to use:
            //      StandardCommandsEnum.AudioVideoSwitcherRoute as the standard command
            //      CommonCommandGroupType.AudioVideoSwitcher as the command group
            base.RouteAudioInput(inputId, outputId);
        }

        public override void RouteVideoInput(string inputId, string outputId)
        {
            // This will be called when an application attempts to route a video input to an output.
            // This command can be built manually by sending a new instance of CommandSet using
            // the method SendCommand.

            // The CommandSet will need to be set to use:
            //      StandardCommandsEnum.AudioVideoSwitcherRoute as the standard command
            //      CommonCommandGroupType.AudioVideoSwitcher as the command group
            base.RouteVideoInput(inputId, outputId);
        }
    }
}
