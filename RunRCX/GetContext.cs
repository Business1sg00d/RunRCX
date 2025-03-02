using System;
using System.Runtime.InteropServices;
using MakeProc;

namespace GetContext
{
    public static class GetContext
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public ulong P1Home;
            public ulong P2Home;
            public ulong P3Home;
            public ulong P4Home;
            public ulong P5Home;
            public ulong P6Home;

            public uint ContextFlags;
            public uint MxCsr;

            public ushort SegCs;
            public ushort SegDs;
            public ushort SegEs;
            public ushort SegFs;
            public ushort SegGs;
            public ushort SegSs;
            public uint EFlags;

            public ulong Dr0;
            public ulong Dr1;
            public ulong Dr2;
            public ulong Dr3;
            public ulong Dr6;
            public ulong Dr7;

            public ulong Rax;
            public ulong Rcx;
            public ulong Rdx;
            public ulong Rbx;
            public ulong Rsp;
            public ulong Rbp;
            public ulong Rsi;
            public ulong Rdi;
            public ulong R8;
            public ulong R9;
            public ulong R10;
            public ulong R11;
            public ulong R12;
            public ulong R13;
            public ulong R14;
            public ulong R15;
            public ulong Rip;

            // The x64 equivalent of FLOATING_SAVE_AREA
            public XMM_SAVE_AREA32 FltSave;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public M128A[] VectorRegister;

            public ulong VectorControl;

            public ulong DebugControl;
            public ulong LastBranchToRip;
            public ulong LastBranchFromRip;
            public ulong LastExceptionToRip;
            public ulong LastExceptionFromRip;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct M128A
        {
            public ulong Low;
            public ulong High;
        }

        // Floating point save area for x64 (replaces FLOATING_SAVE_AREA)
        [StructLayout(LayoutKind.Sequential)]
        public struct XMM_SAVE_AREA32
        {
            public ushort ControlWord;
            public ushort StatusWord;
            public byte TagWord;
            public byte Reserved1;
            public ushort ErrorOpcode;
            public uint ErrorOffset;
            public ushort ErrorSelector;
            public ushort Reserved2;
            public uint DataOffset;
            public ushort DataSelector;
            public ushort Reserved3;
            public uint MxCsr;
            public uint MxCsrMask;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public M128A[] FloatRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public M128A[] XmmRegisters;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
            public byte[] Reserved4;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool TerminateProcess(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        public const uint CONTEXT_FULL = 0x00010007;
        public const uint MEM_RELEASE = 0x00008000;

        public static CONTEXT ReturnNewContext(PROCESS_INFORMATION procInfo, IntPtr hThread, IntPtr nemM) 
        {
            // Get Context data from process:
            CONTEXT ctx = new CONTEXT();
            ctx.ContextFlags = CONTEXT_FULL;
            if (!GetThreadContext(hThread, ref ctx))
            {
                Console.WriteLine("Unable to call GetThreadContext correctly: " + Marshal.GetLastWin32Error());
                bool fRes = VirtualFreeEx(procInfo.hProcess, nemM, 0, MEM_RELEASE);
                Console.WriteLine("Free worked?: " + fRes);
                TerminateProcess(procInfo.hProcess);
                Environment.Exit(1);
            }

            // The RCX register looks like an offset into the ImageBaseAddress
            Console.WriteLine("What is the current RCX value in the thread??: " + ctx.Rcx.ToString("x"));
            return ctx;
        }
    }
}
