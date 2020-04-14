using SDRplayAPIv3;
using System;
using System.Runtime.InteropServices;

namespace sdrplay_api_v3_csharp_console_example
{
	unsafe class Program
	{
		private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		private static long totalSampleCount = 0;
		//public unsafe delegate void sdrplay_api_StreamCallback_t(short* xi, short* xq, ref sdrplay_api_StreamCbParamsT Params, uint numSamples, uint reset, IntPtr cbContext);
		private static void StreamACallback(short* xi, short* xq, ref SDRplayAPI_Callback.sdrplay_api_StreamCbParamsT Params, uint numSamples, uint reset, IntPtr cbContext)
		{
			if (reset != 0)
			{
				sw.Start();
				Console.WriteLine("sdrplay_api_StreamACallback: numSamples={0:d}", numSamples);
			}

			totalSampleCount += numSamples;
			// Process stream callback data here
			return;
		}

		private static void EventCallback(SDRplayAPI_Callback.sdrplay_api_EventT eventId, SDRplayAPI_Tuner.sdrplay_api_TunerSelectT tuner, ref SDRplayAPI_Callback.sdrplay_api_EventParamsT Params, IntPtr cbContext)
		{
			switch (eventId)
			{
				case SDRplayAPI_Callback.sdrplay_api_EventT.sdrplay_api_GainChange:
					Console.WriteLine("sdrplay_api_EventCb: {0}, tuner={1} gRdB={2:d} lnaGRdB={3:d} systemGain={4:0.00}",
						"sdrplay_api_GainChange", (tuner == SDRplayAPI_Tuner.sdrplay_api_TunerSelectT.sdrplay_api_Tuner_A) ? "sdrplay_api_Tuner_A" :
						"sdrplay_api_Tuner_B", Params.gainParams.gRdB, Params.gainParams.lnaGRdB,
						Params.gainParams.currGain);
					break;
				case SDRplayAPI_Callback.sdrplay_api_EventT.sdrplay_api_PowerOverloadChange:
					Console.WriteLine("sdrplay_api_PowerOverloadChange: tuner={0} powerOverloadChangeType={1}",
						(tuner == SDRplayAPI_Tuner.sdrplay_api_TunerSelectT.sdrplay_api_Tuner_A) ? "sdrplay_api_Tuner_A" : "sdrplay_api_Tuner_B",
						(Params.powerOverloadParams.powerOverloadChangeType == SDRplayAPI_Callback.sdrplay_api_PowerOverloadCbEventIdT.sdrplay_api_Overload_Detected) 
						? "sdrplay_api_Overload_Detected" : "sdrplay_api_Overload_Corrected");
					// Send update message to acknowledge power overload message received
					SDRplayAPI.sdrplay_api_Update(chosenDevice.dev, tuner, SDRplayAPI.sdrplay_api_ReasonForUpdateT.sdrplay_api_Update_Ctrl_OverloadMsgAck,
						SDRplayAPI.sdrplay_api_ReasonForUpdateExtension1T.sdrplay_api_Update_Ext1_None);
					break;
				case SDRplayAPI_Callback.sdrplay_api_EventT.sdrplay_api_RspDuoModeChange:
					Console.WriteLine("sdrplay_api_EventCb: {0}, tuner={1} modeChangeType={2}",
						"sdrplay_api_RspDuoModeChange", (tuner == SDRplayAPI_Tuner.sdrplay_api_TunerSelectT.sdrplay_api_Tuner_A) ?
						"sdrplay_api_Tuner_A" : "sdrplay_api_Tuner_B",
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_MasterInitialised) ?
						"sdrplay_api_MasterInitialised" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_SlaveAttached) ?
						"sdrplay_api_SlaveAttached" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_SlaveDetached) ?
						"sdrplay_api_SlaveDetached" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_SlaveInitialised) ?
						"sdrplay_api_SlaveInitialised" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_SlaveUninitialised) ?
						"sdrplay_api_SlaveUninitialised" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_MasterDllDisappeared) ?
						"sdrplay_api_MasterDllDisappeared" :
						(Params.rspDuoModeParams.modeChangeType == SDRplayAPI_Callback.sdrplay_api_RspDuoModeCbEventIdT.sdrplay_api_SlaveDllDisappeared) ?
						"sdrplay_api_SlaveDllDisappeared" : "unknown type");
					//if (Params.rspDuoModeParams.modeChangeType == sdrplay_api_MasterInitialised)
					//	masterInitialised = 1;
					//if (Params.rspDuoModeParams.modeChangeType == sdrplay_api_SlaveUninitialised)
					//	slaveUninitialised = 1;
					break;
				case SDRplayAPI_Callback.sdrplay_api_EventT.sdrplay_api_DeviceRemoved:
					Console.WriteLine("sdrplay_api_EventCb: {0}", "sdrplay_api_DeviceRemoved");
					break;
				default:
					Console.WriteLine("sdrplay_api_EventCb: {0}, unknown event", eventId);
					break;
			}
		}

		//todo: un-static everything in production
		private static SDRplayAPI.sdrplay_api_DeviceT chosenDevice;

		private static string CStrNullTermToString(char[] cStrNullTerm)
		{
			return (new string(cStrNullTerm)).TrimEnd((char)0);
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Allen SDRplay RSPdx/RSP2 API V3 Test");

			try
			{
				//these sizes must match the C API or random memory will get overwritten causing corruption
				if (Marshal.SizeOf(typeof(SDRplayAPI.sdrplay_api_DeviceParamsT)) != 12)
					throw new Exception("sdrplay_api_DeviceParamsT size mismatch to C API expected size, check struct packing");
				if (Marshal.SizeOf(typeof(SDRplayAPI_Dev.sdrplay_api_DevParamsT)) != 64)
					throw new Exception("sdrplay_api_DevParamsT size mismatch to C API expected size, check struct packing");
				if (Marshal.SizeOf(typeof(SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT)) != 144)
					throw new Exception("sdrplay_api_RxChannelParamsT size mismatch to C API expected size, check struct packing");

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
					string serialNumber = CStrNullTermToString(chosenDevice.SerNo);
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
					IntPtr deviceParamsPtr = IntPtr.Zero;
					if ((err = SDRplayAPI.sdrplay_api_GetDeviceParams(chosenDevice.dev, out deviceParamsPtr)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
					{
						SDRplayAPI.sdrplay_api_Close();
						throw new Exception("sdrplay_api_GetDeviceParams failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
					}
					// Check for NULL pointers before changing settings
					if (deviceParamsPtr == IntPtr.Zero)
					{
						SDRplayAPI.sdrplay_api_Close();
						throw new Exception("sdrplay_api_GetDeviceParams returned NULL deviceParams pointer");
					}
					SDRplayAPI.sdrplay_api_DeviceParamsT deviceParams = Marshal.PtrToStructure<SDRplayAPI.sdrplay_api_DeviceParamsT>(deviceParamsPtr);




					// Configure dev parameters
					if (deviceParams.devParams != IntPtr.Zero)
					{
						//this makes a copy of the unmanaged structure allocated in the API
						SDRplayAPI_Dev.sdrplay_api_DevParamsT devParams = Marshal.PtrToStructure<SDRplayAPI_Dev.sdrplay_api_DevParamsT>(deviceParams.devParams);
						devParams.fsFreq.fsHz = 2000000;
						devParams.rspDxParams.antennaSel = SDRplayAPI_RSPdx.sdrplay_api_RspDx_AntennaSelectT.sdrplay_api_RspDx_ANTENNA_B;
						devParams.rspDxParams.biasTEnable = 1;

						//to apply these changes, overwrite the unmanaged structure memory with the updated managed structure
						Marshal.StructureToPtr<SDRplayAPI_Dev.sdrplay_api_DevParamsT>(devParams, deviceParams.devParams, false);
					}
					else
					{
						SDRplayAPI.sdrplay_api_Close();
						throw new Exception("NULL dev params structure");
					}
					if (deviceParams.rxChannelA != IntPtr.Zero)
					{
						//this makes a copy of the unmanaged structure allocated in the API
						SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT rxParamsA = Marshal.PtrToStructure<SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT>(deviceParams.rxChannelA);

						rxParamsA.tunerParams.rfFreq.rfHz = 402000000.0;
						rxParamsA.tunerParams.bwType = SDRplayAPI_Tuner.sdrplay_api_Bw_MHzT.sdrplay_api_BW_1_536;
						rxParamsA.tunerParams.ifType = SDRplayAPI_Tuner.sdrplay_api_If_kHzT.sdrplay_api_IF_Zero;
						rxParamsA.tunerParams.gain.gRdB = 40;
						rxParamsA.tunerParams.gain.LNAstate = 5;
						rxParamsA.ctrlParams.agc.enable = SDRplayAPI_Control.sdrplay_api_AgcControlT.sdrplay_api_AGC_DISABLE;

						//to apply these changes, overwrite the unmanaged structure memory with the updated managed structure
						Marshal.StructureToPtr<SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT>(rxParamsA, deviceParams.rxChannelA, false);
					}
					else
					{
						SDRplayAPI.sdrplay_api_Close();
						throw new Exception("NULL rx channel A structure");
					}

					// Assign callback functions to be passed toSDRplayAPI.sdrplay_api_Init()
					SDRplayAPI_Callback.sdrplay_api_CallbackFnsT cbFns = new SDRplayAPI_Callback.sdrplay_api_CallbackFnsT();
					cbFns.StreamACbFn = StreamACallback;
					cbFns.EventCbFn = EventCallback;

					// Now we're ready to start by calling the initialisation function
					// This will configure the device and start streaming
					if ((err = SDRplayAPI.sdrplay_api_Init(chosenDevice.dev, ref cbFns, IntPtr.Zero)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
					{
						Console.WriteLine("sdrplay_api_Init failed {0}", Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));

						IntPtr lastErrorPtr = SDRplayAPI.sdrplay_api_GetLastError(IntPtr.Zero);
						if (lastErrorPtr != IntPtr.Zero)
						{
							SDRplayAPI.sdrplay_api_ErrorInfoT errInfo = Marshal.PtrToStructure<SDRplayAPI.sdrplay_api_ErrorInfoT>(lastErrorPtr);
							Console.WriteLine("Error in {0}: {1}(): line {2:d}: {3}",
								CStrNullTermToString(errInfo.file),
								CStrNullTermToString(errInfo.function),
								errInfo.line,
								CStrNullTermToString(errInfo.message));
						}

						SDRplayAPI.sdrplay_api_Close();
						return;
					}



					//read the device parameters again as a test just to make sure the C# managed struct to C unmanaged struct marshalling worked, check in debug watch
					//{
					//	IntPtr deviceParamsPtr2 = IntPtr.Zero;
					//	if ((err = SDRplayAPI.sdrplay_api_GetDeviceParams(chosenDevice.dev, out deviceParamsPtr2)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
					//	{
					//		SDRplayAPI.sdrplay_api_Close();
					//		throw new Exception("sdrplay_api_GetDeviceParams failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
					//	}
					//	SDRplayAPI.sdrplay_api_DeviceParamsT deviceParams2 = Marshal.PtrToStructure<SDRplayAPI.sdrplay_api_DeviceParamsT>(deviceParamsPtr2);
					//	SDRplayAPI_Dev.sdrplay_api_DevParamsT devParams2 = Marshal.PtrToStructure<SDRplayAPI_Dev.sdrplay_api_DevParamsT>(deviceParams2.devParams);
					//	SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT rxParamsA2 = Marshal.PtrToStructure<SDRplayAPI_RXChannel.sdrplay_api_RxChannelParamsT>(deviceParams2.rxChannelA);
					//}



					while (sw.Elapsed.TotalSeconds < 6)
					{
						Console.WriteLine("Waiting while receiving data...");
						System.Threading.Thread.Sleep(500);
					}

					if ((err = SDRplayAPI.sdrplay_api_Uninit(chosenDevice.dev)) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
					{
						SDRplayAPI.sdrplay_api_Close();
						throw new Exception("sdrplay_api_Uninit failed: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
					}

					sw.Stop();

					// Release device (make it available to other applications)
					SDRplayAPI.sdrplay_api_ReleaseDevice(ref chosenDevice);

					if ((err = SDRplayAPI.sdrplay_api_Close()) != SDRplayAPI.sdrplay_api_ErrT.sdrplay_api_Success)
					{
						throw new Exception("Error closing sdrplay api: " + Marshal.PtrToStringAnsi(SDRplayAPI.sdrplay_api_GetErrorString(err)));
					}



					Console.WriteLine("total samples: {0:d}, total seconds: {1:0.00}, approx sps {2:0.}", totalSampleCount, sw.Elapsed.TotalSeconds, ((double)totalSampleCount) / sw.Elapsed.TotalSeconds);

				}

			}//end of try
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.ToString());
			}

		}//end of main
	}
}
