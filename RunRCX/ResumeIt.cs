using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace ResumeIt
{
    public class ResumeIt
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        public const uint MEM_RELEASE = 0x00008000;
        public static void ReturnNothing(PROCESS_INFORMATION procInfo, IntPtr hThread, IntPtr nemM)
        {
            int resTReturn = ResumeThread(hThread);
            if (resTReturn == -1)
            {
                Console.WriteLine("ResumeThread failed.");
                bool fRes = VirtualFreeEx(procInfo.hProcess, nemM, 0, MEM_RELEASE);
                Console.WriteLine("Free worked?: " + fRes);
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }
        }
    }
}
