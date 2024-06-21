using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory;

namespace MemCleaner
{
    public partial class Form1 : Form
    {
        private readonly Mem MemLib = new Mem();

        public Form1()
        {
            InitializeComponent();
        }

        private IntPtr GetProcessID()
        {
            IntPtr processHandle = IntPtr.Zero;
            uint maxThreads = 0;

            IntPtr snapshot = CreateToolhelp32Snapshot(2U, 0U);
            if (snapshot.ToInt32() > 0)
            {
                ProcessEntry32 processEntry = new ProcessEntry32();
                processEntry.dwSize = (uint)Marshal.SizeOf(typeof(ProcessEntry32));

                if (Process32First(snapshot, ref processEntry) == 1)
                {
                    do
                    {
                        if (processEntry.szExeFile.Contains("lsass.exe") && processEntry.cntThreads > maxThreads)
                        {
                            maxThreads = processEntry.cntThreads;
                            processHandle = (IntPtr)processEntry.th32ProcessID;
                        }
                    } while (Process32Next(snapshot, ref processEntry) == 1);
                }

                CloseHandle(snapshot);
            }

            return processHandle;
        }

        private async Task CleanMemory(string originalBytes, string replacementBytes)
        {
            try
            {
                IntPtr processHandle = GetProcessID();
                if (processHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Processo lsass.exe não encontrado.");
                    return;
                }

                MemLib.OpenProcess(processHandle.ToInt32());

                IEnumerable<long> scanResults = await MemLib.AoBScan(0L, 0x00007fffffffffff, originalBytes, true, true);
                long firstScanResult = scanResults.FirstOrDefault();

                if (firstScanResult == 0)
                {
                    Console.WriteLine("Nenhum resultado encontrado na varredura inicial.");
                }
                else
                {
                    Console.WriteLine("Varredura inicial concluída. Modificando memória...");
                }

                foreach (long address in scanResults)
                {
                    Mem.MemoryProtection oldProtection;
                    MemLib.ChangeProtection(address.ToString("X"), Mem.MemoryProtection.ReadWrite, out oldProtection);

                    MemLib.WriteMemory(address.ToString("X"), "bytes", replacementBytes);

                    Console.WriteLine($"Memória modificada em 0x{address:X}");
                }

                if (firstScanResult == 0)
                {
                    Console.WriteLine("Nenhum resultado encontrado na varredura.");
                }
                else
                {
                    Console.WriteLine("Strings deletadas com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar operações de memória: {ex.Message}");
            }
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string originalBytes = InputTextBytes.Text;
            string replacementBytes = "63 68 72 6f 6d 65 2e 65 78 65"; // "chrome.exe" em formato hexadecimal

            await CleanMemory(originalBytes, replacementBytes);
        }

        private void guna2ControlBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [DllImport("KERNEL32.DLL")]
        private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

        [DllImport("KERNEL32.DLL")]
        private static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);

        [DllImport("KERNEL32.DLL")]
        private static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);
    }
}
