using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDRplayAPIv3
{
    public class SDRplayAPI_Dev
    {
        // Dev parameter public enums
        public enum sdrplay_api_TransferModeT
        {
            sdrplay_api_ISOCH = 0,
            sdrplay_api_BULK = 1
        }

        // Dev parameter public structs
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_FsFreqT
        {
            public double fsHz;                        // default: 2000000.0
            public byte syncUpdate;           // default: 0
            public byte reCal;                // default: 0
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_SyncUpdateT
        {
            public uint sampleNum;             // default: 0
            public uint period;                // default: 0
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_ResetFlagsT
        {
            public byte resetGainUpdate;      // default: 0
            public byte resetRfUpdate;        // default: 0
            public byte resetFsUpdate;        // default: 0
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_DevParamsT
        {
            public double ppm;                         // default: 0.0
            public sdrplay_api_FsFreqT fsFreq;
            public sdrplay_api_SyncUpdateT syncUpdate;
            public sdrplay_api_ResetFlagsT resetFlags;
            public sdrplay_api_TransferModeT mode;     // default: sdrplay_api_ISOCH
            public uint samplesPerPkt;         // default: 0 (output param)
            public SDRplayAPI_RSP1a.sdrplay_api_Rsp1aParamsT rsp1aParams;
            public SDRplayAPI_RSP2.sdrplay_api_Rsp2ParamsT rsp2Params;
            public SDRplayAPI_RSPduo.sdrplay_api_RspDuoParamsT rspDuoParams;
            public SDRplayAPI_RSPdx.sdrplay_api_RspDxParamsT rspDxParams;
        }

    }
}
