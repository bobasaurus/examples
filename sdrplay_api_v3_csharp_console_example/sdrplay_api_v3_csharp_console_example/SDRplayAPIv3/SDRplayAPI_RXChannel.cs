using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDRplayAPIv3
{
    public class SDRplayAPI_RXChannel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_RxChannelParamsT
        {
            public SDRplayAPI_Tuner.sdrplay_api_TunerParamsT tunerParams;
            public SDRplayAPI_Control.sdrplay_api_ControlParamsT ctrlParams;
            public SDRplayAPI_RSP1a.sdrplay_api_Rsp1aTunerParamsT rsp1aTunerParams;
            public SDRplayAPI_RSP2.sdrplay_api_Rsp2TunerParamsT rsp2TunerParams;
            public SDRplayAPI_RSPduo.sdrplay_api_RspDuoTunerParamsT rspDuoTunerParams;
            public SDRplayAPI_RSPdx.sdrplay_api_RspDxTunerParamsT rspDxTunerParams;
        }




    }
}
