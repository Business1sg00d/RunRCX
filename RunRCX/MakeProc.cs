using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MakeProc
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFO
    {
        uint cb;
        IntPtr lpReserved;
        IntPtr lpDesktop;
        IntPtr lpTitle;
        uint dwX;
        uint dwY;
        uint dwXSize;
        uint dwYSize;
        uint dwXCountChars;
        uint dwYCountChars;
        uint dwFillAttributes;
        uint dwFlags;
        ushort wShowWindow;
        ushort cbReserved;
        IntPtr lpReserved2;
        IntPtr hStdInput;
        IntPtr hStdOutput;
        IntPtr hStdErr;
    }

    internal class GetProc
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcAttribs, IntPtr lpThreadAttribs, bool bInheritHandles, uint dwCreateFlags, IntPtr lpEnvironment, IntPtr lpCurrentDir, [In] ref STARTUPINFO lpStartinfo, out PROCESS_INFORMATION lpProcInformation);

        public const uint CREATE_SUSPENDED = 0x04;

        public static PROCESS_INFORMATION ReturnProcInfo()
        {
            // Verify target binary exists on disk
            if (!File.Exists("C:\\Windows\\System32\\mstsc.exe"))
            {
                Console.WriteLine("Couldn't find target binary on file system.");
                Environment.Exit(1);
            }

            // Create the target process
            STARTUPINFO startInfo = new STARTUPINFO();
            PROCESS_INFORMATION procInfo = new PROCESS_INFORMATION();
            uint flags = CREATE_SUSPENDED;
            CreateProcess(null, "C:\\Windows\\System32\\mstsc.exe", IntPtr.Zero, IntPtr.Zero, false, flags, IntPtr.Zero, IntPtr.Zero, ref startInfo, out procInfo);
            return procInfo;
        }
    }
}
