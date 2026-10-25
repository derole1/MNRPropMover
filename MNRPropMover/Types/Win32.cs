using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MNRPropMover.Types
{
    internal class Win32
    {
#if _WINDOWS
        public static readonly int PROCESS_VM_OPERATION = 0x0008;
        public static readonly int PROCESS_VM_READ = 0x0010;
        public static readonly int PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(nint hProcess, ulong lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(nint hProcess, ulong lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static readonly int VK_PRIOR = 0x21;
        public static readonly int VK_NEXT = 0x22;
        public static readonly int VK_LEFT = 0x25;
        public static readonly int VK_UP = 0x26;
        public static readonly int VK_RIGHT = 0x27;
        public static readonly int VK_DOWN = 0x28;
        public static readonly int VK_W = 0x57;
        public static readonly int VK_A = 0x41;
        public static readonly int VK_S = 0x53;
        public static readonly int VK_D = 0x44;
        public static readonly int VK_Q = 0x51;
        public static readonly int VK_E = 0x45;
        public static readonly int VK_R = 0x52;
        public static readonly int VK_F = 0x46;
        public static readonly int VK_U = 0x55;
        public static readonly int VK_O = 0x4F;
        public static readonly int VK_J = 0x4A;
        public static readonly int VK_L = 0x4C;
        public static readonly int VK_I = 0x49;
        public static readonly int VK_K = 0x4B;
        public static readonly int VK_OEM_4 = 0xDB; // [
        public static readonly int VK_OEM_6 = 0xDD; // ]

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
#endif
    }
}
