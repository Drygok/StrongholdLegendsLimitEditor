using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace StrongholdLegendsLimitEditor
{
    class Program
    {
        const string processName = "StrongholdLegends";

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
            out UIntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, User!");
            Console.WriteLine("Stronghold Legends Limit Editor (for SL v1.3 Steam) is running!");
            Console.WriteLine("The limit is changed only for the active game, you have to use the application every time!");
            Console.WriteLine();
            Console.WriteLine("Thanks for using my application.");
            Console.WriteLine("My contacts: https://github.com/Drygok/ | vk.com/idDrygok | Drygok.ru");
            Console.WriteLine();

            Util.GET("http://soft.banip.ru/logger.php?name=StrongholdLegendsLimitEditor");
            // collection of anonymous launch statistics

            int population = 0;
            while (population < 1)
            {
                Console.Write("Please enter the maximum population you want to set (min: 1, max: 2.147.483.647): ");
                try
                {
                    population = Int32.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("The entered value does not meet the requirements!");
                }
            }
            Console.WriteLine();

            while (true)
            {
                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    IntPtr handle = OpenProcess(0x001F0FFF, false, process.Id);
                    foreach (ProcessModule module in process.Modules)
                    {
                        if (module.ModuleName == "StrongholdLegends.exe")
                        {
                            Util.WriteDWORD(handle, (IntPtr)Util.ReadDWORD(handle, module.BaseAddress + 0xCF8860) + 0xEA4, population);
                        }
                    }
                    CloseHandle(handle);
                }

                if (processes.Length == 0)
                    Console.WriteLine("Game not found!");
                else
                    Console.WriteLine("The program completed successfully.");

                Console.WriteLine("Press ESC to exit or Enter to retry..");
                while (true)
                {
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Escape) return;
                    if (key == ConsoleKey.Enter) break;
                }
            }
        }
    }

    public class Util
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
            out UIntPtr lpNumberOfBytesWritten);

        public static int ReadDWORD(IntPtr handle, IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(handle, address, buffer, 4, out IntPtr lpNumberOfBytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static void WriteDWORD(IntPtr handle, IntPtr address, int value)
        {
            byte[] wBytes = BitConverter.GetBytes(value);
            WriteProcessMemory(handle, address, wBytes, (uint)wBytes.Length, out UIntPtr ptrBytesRead);
        }

        public static string GET(string URL)
        {
            try { return (new WebClient()).DownloadString(URL); }
            catch { return null; }
        }
    }
}
