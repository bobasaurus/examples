using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SDRplayAPIv3;

namespace sdrplay_api_v3_csharp_console_example
{
	class Program
	{
		//todo: un-static everything in production
		private static SDRplayAPI.sdrplay_api_DeviceT chosenDevice;

		static void Main(string[] args)
		{
			Console.WriteLine("Allen SDRplay RSPdx/RSP2 API V3 Test");

			SDRplayAPI.sdrplay_api_ErrT err;

			if ((err = SDRplayAPI.sdrplay_api_Open()) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
			{
				Console.WriteLine("sdrplay_api_Open failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
			}
			else
			{
				Console.WriteLine("API opened successfully");

				// Enable debug logging output
				if ((err = SDRplayAPI.sdrplay_api_DebugEnable(IntPtr.Zero, 1)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					Console.WriteLine("sdrplay_api_DebugEnable failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				}
				Console.WriteLine("debug messages enabled");

				// Check API versions match
				float ver;
				if ((err = SDRplayAPI.sdrplay_api_ApiVersion(out ver)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					Console.WriteLine("sdrplay_api_ApiVersion failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				}
				if (ver != SDRplayAPI.SDRPLAY_API_VERSION)
				{
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception(string.Format("API version don't match (expected={0:0.00} dll={0:0.00})", SDRplayAPI.SDRPLAY_API_VERSION, ver));
				}
				Console.WriteLine("API version: {0:0.000}", ver);

				// Lock API while device selection is performed
				SDRplayAPI.sdrplay_api_LockDeviceApi();

				// Fetch list of available devices

				SDRplayAPI.sdrplay_api_DeviceT[] devs = new SDRplayAPI.sdrplay_api_DeviceT[6];
				if ((err = SDRplayAPI.sdrplay_api_GetDevices(devs, out uint ndev, 6)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					SDRplayAPI.sdrplay_api_UnlockDeviceApi();
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception("sdrplay_api_GetDevices failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				}
				Console.WriteLine("num devs: {0:d}", ndev);


				chosenDevice = devs[0];//just use the first available device
				string serialNumber = (new string(chosenDevice.SerNo)).TrimEnd((char)0);
				Console.WriteLine("serial number: {0}", serialNumber);

				if ((chosenDevice.hwVer != SDRplayAPI.SDRPLAY_RSP2_ID) && (chosenDevice.hwVer != SDRplayAPI.SDRPLAY_RSPdx_ID))
				{
					SDRplayAPI.sdrplay_api_UnlockDeviceApi();
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception(string.Format("Unsupported RSP device: %02X", chosenDevice.hwVer));
				}

				// Select chosen device
				if ((err = SDRplayAPI.sdrplay_api_SelectDevice(ref chosenDevice)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					SDRplayAPI.sdrplay_api_UnlockDeviceApi();
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception("sdrplay_api_SelectDevice failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				}
				// Unlock API now that device is selected
				SDRplayAPI.sdrplay_api_UnlockDeviceApi();

				// Retrieve device parameters so they can be changed if wanted
				IntPtr deviceParams = IntPtr.Zero;
				if ((err = SDRplayAPI.sdrplay_api_GetDeviceParams(chosenDevice.dev, out deviceParams)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception("sdrplay_api_GetDeviceParams failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				}
				// Check for NULL pointers before changing settings
				if (deviceParams == IntPtr.Zero)
				{
					SDRplayAPI.sdrplay_api_Close();
					throw new Exception("sdrplay_api_GetDeviceParams returned NULL deviceParams pointer");
				}
				SDRplayAPI.sdrplay_api_DeviceParamsT myData2 = (SDRplayAPI.sdrplay_api_DeviceParamsT)Marshal.PtrToStructure(deviceParams, typeof(SDRplayAPI.sdrplay_api_DeviceParamsT));

				SDRplayAPI_Dev.sdrplay_api_DevParamsT blah = (SDRplayAPI_Dev.sdrplay_api_DevParamsT)Marshal.PtrToStructure(myData2.devParams, typeof(SDRplayAPI_Dev.sdrplay_api_DevParamsT));
				SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT blah2 = (SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT)Marshal.PtrToStructure(myData2.rxChannelA, typeof(SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT));
				SDRplayAPI_Dev.sdrplay_api_DevParamsT blah3 = (SDRplayAPI_Dev.sdrplay_api_DevParamsT)Marshal.PtrToStructure(myData2.devParams, typeof(SDRplayAPI_Dev.sdrplay_api_DevParamsT));

				int test = 1;

				//// Configure dev parameters
				//if (deviceParams->devParams != NULL)
				//{
				//	// Change from default Fs to 2MHz
				//	deviceParams->devParams->fsFreq.fsHz = 2000000.0;
				//	deviceParams->devParams->rspDxParams.antennaSel =SDRplayAPI.sdrplay_api_RspDx_ANTENNA_A;
				//	//etc
				//}
				//else
				//{
				//	sdrplay_api_Close();
				//	return 1;
				//}
				//if (deviceParams->rxChannelA != NULL)
				//{
				//	deviceParams->rxChannelA->tunerParams.rfFreq.rfHz = 402000000.0;
				//	deviceParams->rxChannelA->tunerParams.bwType =SDRplayAPI.sdrplay_api_BW_1_536;
				//	deviceParams->rxChannelA->tunerParams.ifType =SDRplayAPI.sdrplay_api_IF_Zero;
				//	deviceParams->rxChannelA->tunerParams.gain.gRdB = 40;
				//	deviceParams->rxChannelA->tunerParams.gain.LNAstate = 5;
				//	deviceParams->rxChannelA->ctrlParams.agc.enable =SDRplayAPI.sdrplay_api_AGC_DISABLE;
				//}
				//else
				//{
				//	sdrplay_api_Close();
				//	return 1;
				//}

				//// Assign callback functions to be passed toSDRplayAPI.sdrplay_api_Init()
				//sdrplay_api_CallbackFnsT cbFns;
				//cbFns.StreamACbFn = StreamACallback;
				//cbFns.EventCbFn = EventCallback;

				//// Now we're ready to start by calling the initialisation function
				//// This will configure the device and start streaming
				//if ((err =SDRplayAPI.sdrplay_api_Init(chosenDevice->dev, &cbFns, NULL)) !=SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				//{
				//	Console.WriteLine("sdrplay_api_Init failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				//	sdrplay_api_ErrorInfoT* errInfo =SDRplayAPI.sdrplay_api_GetLastError(NULL);
				//	if (errInfo != NULL) Console.WriteLine("Error in {0}: {1}(): line {2:d}: {3}", errInfo->file, errInfo->function, errInfo->line, errInfo->message);
				//	sdrplay_api_Close();
				//	return 1;
				//}

				//while (1) // Small loop allowing user to control gain reduction in +/-1dB steps using keyboard keys
				//{
				//	if (_kbhit())
				//	{
				//		char c = _getch();
				//		if (c == 'q')
				//			break;
				//	}
				//}

				//if ((err =SDRplayAPI.sdrplay_api_Uninit(chosenDevice->dev)) !=SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				//{
				//	Console.WriteLine("sdrplay_api_Uninit failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
				//	sdrplay_api_Close();
				//	return 1;
				//}
				//// Release device (make it available to other applications)
				//sdrplay_api_ReleaseDevice(chosenDevice);

				if ((err = SDRplayAPI.sdrplay_api_Close()) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
				{
					Console.WriteLine("Error closing sdrplay api: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
					//exception?
				}
			}

		}
	}
}
