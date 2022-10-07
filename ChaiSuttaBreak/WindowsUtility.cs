using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ChaiSuttaBreak
{
    [FlagsAttribute]
    internal enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }

    internal class WindowsUtility
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static internal extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
