using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace SetNewContext
{
    public class SetNewContext
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetThreadContext(IntPtr hThread, ref GetContext.GetContext.CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public const uint MEM_RELEASE = 0x00008000;

        public static void ReturnNothing(PROCESS_INFORMATION procInfo, IntPtr hThread, GetContext.GetContext.CONTEXT ctx, IntPtr nemM)
        {
            // Set new thread context; RCX points to current position in instruction set:
            if (!SetThreadContext(hThread, ref ctx))
            {
                Console.WriteLine("Failed to set updated thread context: " + GetLastError());
                bool fRes = VirtualFreeEx(procInfo.hProcess, nemM, 0, MEM_RELEASE);
                Console.WriteLine("Free worked?: " + fRes);
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }

            Console.WriteLine("SetThreadContext succeeded; new address: " + ctx.Rcx.ToString("X"));
            return;
        }
    }
}
