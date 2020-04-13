using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDRplayAPIv3
{
    public class SDRplayAPI_Control
    {
        // Control parameter public enums
        public enum sdrplay_api_AgcControlT
        {
            sdrplay_api_AGC_DISABLE = 0,
            sdrplay_api_AGC_100HZ = 1,
            sdrplay_api_AGC_50HZ = 2,
            sdrplay_api_AGC_5HZ = 3,
            sdrplay_api_AGC_CTRL_EN = 4
        }

        public enum sdrplay_api_AdsbModeT
        {
            sdrplay_api_ADSB_DECIMATION = 0,
            sdrplay_api_ADSB_NO_DECIMATION_LOWPASS = 1,
            sdrplay_api_ADSB_NO_DECIMATION_BANDPASS_2MHZ = 2,
            sdrplay_api_ADSB_NO_DECIMATION_BANDPASS_3MHZ = 3
        }

        // Control parameter public structs
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_DcOffsetT
        {
            public byte DCenable;          // default: 1
            public byte IQenable;          // default: 1
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_DecimationT
        {
            public byte enable;            // default: 0
            public byte decimationFactor;  // default: 1
            public byte wideBandSignal;    // default: 0
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_AgcT
        {
            public sdrplay_api_AgcControlT enable;    // default: sdrplay_api_AGC_50HZ
            public int setPoint_dBfs;                 // default: -60
            public ushort attack_ms;          // default: 0
            public ushort decay_ms;           // default: 0
            public ushort decay_delay_ms;     // default: 0
            public ushort decay_threshold_dB; // default: 0
            public int syncUpdate;                    // default: 0
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_ControlParamsT
        {
            public sdrplay_api_DcOffsetT dcOffset;
            public sdrplay_api_DecimationT decimation;
            public sdrplay_api_AgcT agc;
            public sdrplay_api_AdsbModeT adsbMode;  //default: sdrplay_api_ADSB_DECIMATION
        }
    }
}
