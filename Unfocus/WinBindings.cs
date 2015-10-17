using System;
using System.Runtime.InteropServices;

namespace Unfocus
{
    public static class WinBindings
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        // TODO Take tick wraparound (~50 days) into account
        public static TimeSpan TimeSinceLastInput()
        {
            var lastInput = new LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);

            if (!GetLastInputInfo(ref lastInput))
            {
                throw new Exception("Failed to fetch last input time.");
            }
            
            return TimeSpan.FromMilliseconds(Environment.TickCount - lastInput.dwTime);
        }
    }
}
