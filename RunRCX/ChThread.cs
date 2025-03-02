using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace ThreadWork
{
    public static class ChThread
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        public const int TOKEN_ALL_ACCESS = 0x000f01ff;

        public static IntPtr ReturnThread(PROCESS_INFORMATION procInfo) 
        {
            Console.WriteLine("Changing thread context...");
            uint u = Convert.ToUInt32(procInfo.dwThreadId);
            IntPtr hThread = OpenThread(TOKEN_ALL_ACCESS, false, u);
            if (hThread == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open thread in ChThread Function: " + Marshal.GetLastWin32Error());
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }
            return hThread;
        }
    }
}
