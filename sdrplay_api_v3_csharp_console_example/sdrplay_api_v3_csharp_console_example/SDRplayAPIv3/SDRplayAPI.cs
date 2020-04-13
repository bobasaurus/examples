using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDRplayAPIv3
{
    public class SDRplayAPI
    {
        private const string DLL_FILENAME = "sdrplay_api.dll";

        // Application code should check that it is compiled against the same API version
        // sdrplay_api_ApiVersion() returns the API version 
        public const float SDRPLAY_API_VERSION = 3.06f;

        // API Constants
        public const int SDRPLAY_MAX_DEVICES = 16;
        public const int SDRPLAY_MAX_TUNERS_PER_DEVICE = 2;

        public const int SDRPLAY_MAX_SER_NO_LEN = 64;
        public const int SDRPLAY_MAX_ROOT_NM_LEN = 128;

        public const int SDRPLAY_RSP1_ID = 1;
        public const int SDRPLAY_RSP1A_ID = 255;
        public const int SDRPLAY_RSP2_ID = 2;
        public const int SDRPLAY_RSPduo_ID = 3;
        public const int SDRPLAY_RSPdx_ID = 4;

        // Enum types
        public enum sdrplay_api_ErrT
        {
            sdrplay_api_Success = 0,
            sdrplay_api_Fail = 1,
            sdrplay_api_InvalidParam = 2,
            sdrplay_api_OutOfRange = 3,
            sdrplay_api_GainUpdateError = 4,
            sdrplay_api_RfUpdateError = 5,
            sdrplay_api_FsUpdateError = 6,
            sdrplay_api_HwError = 7,
            sdrplay_api_AliasingError = 8,
            sdrplay_api_AlreadyInitialised = 9,
            sdrplay_api_NotInitialised = 10,
            sdrplay_api_NotEnabled = 11,
            sdrplay_api_HwVerError = 12,
            sdrplay_api_OutOfMemError = 13,
            sdrplay_api_ServiceNotResponding = 14,
            sdrplay_api_StartPending = 15,
            sdrplay_api_StopPending = 16,
            sdrplay_api_InvalidMode = 17,
            sdrplay_api_FailedVerification1 = 18,
            sdrplay_api_FailedVerification2 = 19,
            sdrplay_api_FailedVerification3 = 20,
            sdrplay_api_FailedVerification4 = 21,
            sdrplay_api_FailedVerification5 = 22,
            sdrplay_api_FailedVerification6 = 23,
            sdrplay_api_InvalidServiceVersion = 24
        }

        public enum sdrplay_api_ReasonForUpdateT
        {
            sdrplay_api_Update_None = 0x00000000,

            // Reasons for master only mode 
            sdrplay_api_Update_Dev_Fs = 0x00000001,
            sdrplay_api_Update_Dev_Ppm = 0x00000002,
            sdrplay_api_Update_Dev_SyncUpdate = 0x00000004,
            sdrplay_api_Update_Dev_ResetFlags = 0x00000008,

            sdrplay_api_Update_Rsp1a_BiasTControl = 0x00000010,
            sdrplay_api_Update_Rsp1a_RfNotchControl = 0x00000020,
            sdrplay_api_Update_Rsp1a_RfDabNotchControl = 0x00000040,

            sdrplay_api_Update_Rsp2_BiasTControl = 0x00000080,
            sdrplay_api_Update_Rsp2_AmPortSelect = 0x00000100,
            sdrplay_api_Update_Rsp2_AntennaControl = 0x00000200,
            sdrplay_api_Update_Rsp2_RfNotchControl = 0x00000400,
            sdrplay_api_Update_Rsp2_ExtRefControl = 0x00000800,

            sdrplay_api_Update_RspDuo_ExtRefControl = 0x00001000,

            sdrplay_api_Update_Master_Spare_1 = 0x00002000,
            sdrplay_api_Update_Master_Spare_2 = 0x00004000,

            // Reasons for master and slave mode
            // Note: sdrplay_api_Update_Tuner_Gr MUST be the first value defined in this section!
            sdrplay_api_Update_Tuner_Gr = 0x00008000,
            sdrplay_api_Update_Tuner_GrLimits = 0x00010000,
            sdrplay_api_Update_Tuner_Frf = 0x00020000,
            sdrplay_api_Update_Tuner_BwType = 0x00040000,
            sdrplay_api_Update_Tuner_IfType = 0x00080000,
            sdrplay_api_Update_Tuner_DcOffset = 0x00100000,
            sdrplay_api_Update_Tuner_LoMode = 0x00200000,

            sdrplay_api_Update_Ctrl_DCoffsetIQimbalance = 0x00400000,
            sdrplay_api_Update_Ctrl_Decimation = 0x00800000,
            sdrplay_api_Update_Ctrl_Agc = 0x01000000,
            sdrplay_api_Update_Ctrl_AdsbMode = 0x02000000,
            sdrplay_api_Update_Ctrl_OverloadMsgAck = 0x04000000,

            sdrplay_api_Update_RspDuo_BiasTControl = 0x08000000,
            sdrplay_api_Update_RspDuo_AmPortSelect = 0x10000000,
            sdrplay_api_Update_RspDuo_Tuner1AmNotchControl = 0x20000000,
            sdrplay_api_Update_RspDuo_RfNotchControl = 0x40000000,
            sdrplay_api_Update_RspDuo_RfDabNotchControl = unchecked((int)0x80000000)//TODO: see if this actually works
        }


        public enum sdrplay_api_ReasonForUpdateExtension1T
        {
            sdrplay_api_Update_Ext1_None = 0x00000000,

            // Reasons for master only mode 
            sdrplay_api_Update_RspDx_HdrEnable = 0x00000001,
            sdrplay_api_Update_RspDx_BiasTControl = 0x00000002,
            sdrplay_api_Update_RspDx_AntennaControl = 0x00000004,
            sdrplay_api_Update_RspDx_RfNotchControl = 0x00000008,
            sdrplay_api_Update_RspDx_RfDabNotchControl = 0x00000010,
            sdrplay_api_Update_RspDx_HdrBw = 0x00000020,

            // Reasons for master and slave mode
        }

        // Device public structure 
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_DeviceT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SDRPLAY_MAX_SER_NO_LEN)]
            public char[] SerNo;
            public byte hwVer;
            public SDRplayAPI_Tuner.sdrplay_api_TunerSelectT tuner;
            public SDRplayAPI_RSPduo.sdrplay_api_RspDuoModeT rspDuoMode;
            public double rspDuoSampleFreq;
            public /*HANDLE*/ IntPtr dev;
        }


        // Device parameter public structure
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_DeviceParamsT
        {
            public /*sdrplay_api_DevParamsT**/ IntPtr devParams;
            public /*sdrplay_api_RxChannelParamsT**/ IntPtr rxChannelA;
            public /*sdrplay_api_RxChannelParamsT**/ IntPtr rxChannelB;
        }


        // Extended error message public structure
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct sdrplay_api_ErrorInfoT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] file;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] function;

            public int line;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public char[] message;
        }



        // Comman API function definitions
        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_Open(void);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_Open();

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_Close(void);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_Close();

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_ApiVersion(float* apiVer);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_ApiVersion(out float apiVer);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_LockDeviceApi(void);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_LockDeviceApi();

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_UnlockDeviceApi(void);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_UnlockDeviceApi();

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_GetDevices(sdrplay_api_DeviceT* devices, unsigned int* numDevs, unsigned int maxDevs);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_GetDevices([Out] sdrplay_api_DeviceT[] devices, out uint numDevs, uint maxDevs);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_SelectDevice(sdrplay_api_DeviceT* device);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_SelectDevice(ref sdrplay_api_DeviceT device);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_ReleaseDevice(sdrplay_api_DeviceT* device);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_ReleaseDevice(IntPtr device);

        //_SDRPLAY_DLL_QUALIFIER const char* sdrplay_api_GetErrorString(sdrplay_api_ErrT err);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sdrplay_api_GetErrorString(sdrplay_api_ErrT err);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrorInfoT* sdrplay_api_GetLastError(sdrplay_api_DeviceT* device);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sdrplay_api_GetLastError(IntPtr device);

        // Device API function definitions
        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_DebugEnable(HANDLE dev, unsigned int enable);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_DebugEnable(IntPtr dev, uint enable);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_GetDeviceParams(HANDLE dev, sdrplay_api_DeviceParamsT** deviceParams);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_GetDeviceParams(IntPtr dev, out IntPtr deviceParams);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_Init(HANDLE dev, sdrplay_api_CallbackFnsT* callbackFns, void* cbContext);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_Init(IntPtr dev, ref SDRplayAPI_Callback.sdrplay_api_CallbackFnsT callbackFns, IntPtr cbContext);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_Uninit(HANDLE dev);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_Uninit(IntPtr dev);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_Update(HANDLE dev, sdrplay_api_TunerSelectT tuner, sdrplay_api_ReasonForUpdateT reasonForUpdate, sdrplay_api_ReasonForUpdateExtension1T reasonForUpdateExt1);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_Update(IntPtr dev, SDRplayAPI_Tuner.sdrplay_api_TunerSelectT tuner, sdrplay_api_ReasonForUpdateT reasonForUpdate, sdrplay_api_ReasonForUpdateExtension1T reasonForUpdateExt1);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_SwapRspDuoActiveTuner(HANDLE dev, sdrplay_api_TunerSelectT* currentTuner, sdrplay_api_RspDuo_AmPortSelectT tuner1AmPortSel);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_SwapRspDuoActiveTuner(IntPtr dev, ref SDRplayAPI_Tuner.sdrplay_api_TunerSelectT currentTuner, SDRplayAPI_RSPduo.sdrplay_api_RspDuo_AmPortSelectT tuner1AmPortSel);

        //_SDRPLAY_DLL_QUALIFIER sdrplay_api_ErrT        sdrplay_api_SwapRspDuoDualTunerModeSampleRate(HANDLE dev, double* currentSampleRate);
        [DllImport(DLL_FILENAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern sdrplay_api_ErrT sdrplay_api_SwapRspDuoDualTunerModeSampleRate(IntPtr dev, ref double currentSampleRate);

    }
}
