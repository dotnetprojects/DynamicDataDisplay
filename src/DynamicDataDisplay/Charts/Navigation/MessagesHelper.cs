using System;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Helper class to parse Windows messages</summary>
	internal static class MessagesHelper {
		internal static int GetXFromLParam(IntPtr lParam) {
			return LOWORD(lParam.ToInt32());
		}

		internal static int GetYFromLParam(IntPtr lParam) {
			return HIWORD(lParam.ToInt32());
		}

		internal static Point GetMousePosFromLParam(IntPtr lParam) {
			return new Point(GetXFromLParam(lParam), GetYFromLParam(lParam));
		}

		internal static int GetWheelDataFromWParam(IntPtr wParam) {
			return HIWORD(wParam.ToInt32());
		}

		internal static short HIWORD(int i) {
			return (short)((i & 0xFFFF0000) >> 16);
		}

		internal static short LOWORD(int i) {
			return (short)(i & 0x0000FFFF);
		}
	}
}
