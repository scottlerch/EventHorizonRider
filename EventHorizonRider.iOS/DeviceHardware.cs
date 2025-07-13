using ObjCRuntime;
using System;
using System.Runtime.InteropServices;
using UIKit;

namespace EventHorizonRider.iOS;

public partial class DeviceHardware
{
    private const string HardwareProperty = "hw.machine";

    [LibraryImport(Constants.SystemLibrary)]
    private static partial int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

    public static HardwareType Version
    {
        get
        {
            var pLen = Marshal.AllocHGlobal(sizeof(int));
            sysctlbyname(HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

            var length = Marshal.ReadInt32(pLen);

            if (length == 0)
            {
                Marshal.FreeHGlobal(pLen);

                return HardwareType.Unknown;
            }

            var pStr = Marshal.AllocHGlobal(length);
            sysctlbyname(HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

            var hardwareStr = Marshal.PtrToStringAnsi(pStr);

            Marshal.FreeHGlobal(pLen);
            Marshal.FreeHGlobal(pStr);

            if (hardwareStr == "iPhone1,1") return HardwareType.iPhone;
            if (hardwareStr == "iPhone1,2") return HardwareType.iPhone3G;
            if (hardwareStr == "iPhone2,1") return HardwareType.iPhone3GS;
            if (hardwareStr == "iPhone3,1") return HardwareType.iPhone4;
            if (hardwareStr == "iPhone3,2") return HardwareType.iPhone4RevA;
            if (hardwareStr == "iPhone3,3") return HardwareType.iPhone4CDMA;
            if (hardwareStr == "iPhone4,1") return HardwareType.iPhone4S;
            if (hardwareStr == "iPhone5,1") return HardwareType.iPhone5GSM;
            if (hardwareStr == "iPhone5,2") return HardwareType.iPhone5CDMAGSM;

            if (hardwareStr == "iPad1,1") return HardwareType.iPad;
            if (hardwareStr == "iPad1,2") return HardwareType.iPad3G;
            if (hardwareStr == "iPad2,1") return HardwareType.iPad2;
            if (hardwareStr == "iPad2,2") return HardwareType.iPad2GSM;
            if (hardwareStr == "iPad2,3") return HardwareType.iPad2CDMA;
            if (hardwareStr == "iPad2,4") return HardwareType.iPad2RevA;
            if (hardwareStr == "iPad2,5") return HardwareType.iPadMini;
            if (hardwareStr == "iPad2,6") return HardwareType.iPadMiniGSM;
            if (hardwareStr == "iPad2,7") return HardwareType.iPadMiniCDMAGSM;
            if (hardwareStr == "iPad3,1") return HardwareType.iPad3;
            if (hardwareStr == "iPad3,2") return HardwareType.iPad3CDMA;
            if (hardwareStr == "iPad3,3") return HardwareType.iPad3GSM;
            if (hardwareStr == "iPad3,4") return HardwareType.iPad4;
            if (hardwareStr == "iPad3,5") return HardwareType.iPad4GSM;
            if (hardwareStr == "iPad3,6") return HardwareType.iPad4CDMAGSM;

            if (hardwareStr == "iPod1,1") return HardwareType.iPodTouch1G;
            if (hardwareStr == "iPod2,1") return HardwareType.iPodTouch2G;
            if (hardwareStr == "iPod3,1") return HardwareType.iPodTouch3G;
            if (hardwareStr == "iPod4,1") return HardwareType.iPodTouch4G;
            if (hardwareStr == "iPod5,1") return HardwareType.iPodTouch5G;

            if (hardwareStr == "i386" || hardwareStr == "x86_64")
            {
                if (UIDevice.CurrentDevice.Model.Contains("iPhone"))
                {
                    if (UIScreen.MainScreen.Scale > 1.5f)
                        return HardwareType.iPhoneRetinaSimulator;

                    return HardwareType.iPhoneSimulator;
                }

                if (UIScreen.MainScreen.Scale > 1.5f)
                    return HardwareType.iPadRetinaSimulator;

                return HardwareType.iPadSimulator;
            }

            return HardwareType.Unknown;
        }
    }
}