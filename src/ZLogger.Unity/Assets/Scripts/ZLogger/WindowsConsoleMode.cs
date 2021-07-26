using System;
using System.Runtime.InteropServices;

namespace ZLogger
{
    internal static class WindowsConsoleMode
    {
        const string Kernel32 = "kernel32.dll";

        // https://docs.microsoft.com/en-us/windows/console/getstdhandle
        const int STD_OUTPUT_HANDLE = -11;

        // https://docs.microsoft.com/en-us/windows/console/setconsolemode
        const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport(Kernel32)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr handle, int mode);

        internal static bool TryEnableVirtualTerminalProcessing()
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(handle, out var mode))
            {
                if (SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING))
                {
                    // OK to configure.
                    return true;
                }
            }
            return false;
        }
    }
}
