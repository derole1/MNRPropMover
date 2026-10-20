using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MNRPropMover.Types
{
    internal class EmulatorProcess
    {
        public string ProcessName { get; }

        public string WindowTitle => _process?.MainWindowTitle ?? string.Empty;
#if _WINDOWS
        public bool IsRunning => _process != null && !_process.HasExited && _processHandle != IntPtr.Zero;
#else
        public bool IsRunning => _process != null && !_process.HasExited;
#endif

        private Process? _process;
        private ulong _baseAddress;
#if _WINDOWS
        private IntPtr _processHandle;
#endif

        public EmulatorProcess(string processName)
        {
            ProcessName = processName;
        }

        public bool Attach()
        {
#if _LINUX
            _process = FindProcess();
#else
            _process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
#endif
            
            if (_process == null || _process.MainModule == null)
                return false;

            _baseAddress = (ulong)_process.MainModule.BaseAddress;

#if _WINDOWS
            _processHandle = Win32.OpenProcess(Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_READ | Win32.PROCESS_VM_WRITE, false, _process.Id);

            return _processHandle != IntPtr.Zero;
#else
            return true;
#endif
        }

#if _LINUX
        // TODO: Can we improve this?
        private static Process? FindProcess()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName != "AppRun.wrapped")
                    continue;

                try
                {
                    var status = File.ReadAllLines($"/proc/{process.Id}/status");
                    var parentLine = status.FirstOrDefault(line => line.StartsWith("PPid:"));
                    if (parentLine == null)
                        continue;

                    var parentId = parentLine.Split(':')[1].Trim();
                    var parentCommand = File.ReadAllText($"/proc/{parentId}/cmdline");
                    if (parentCommand.Contains("rpcs3", StringComparison.OrdinalIgnoreCase))
                        return process;
                }
                catch (Exception)
                {
                }
            }

            return null;
        }
#endif

        public uint ReadPointer32(ulong address)
        {
            if (!IsRunning)
                return 0;

            byte[] buffer = new byte[sizeof(ulong)];
            if (!ReadMemory(address, buffer))
                return 0;

            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }

        public float ReadFloat32(ulong address)
        {
            if (!IsRunning)
                return 0;

            byte[] buffer = new byte[sizeof(float)];
            if (!ReadMemory(address, buffer))
                return 0;

            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }

        public bool WriteFloat32(ulong address, float value)
        {
            if (!IsRunning)
                return false;

            byte[] buffer = new byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);
            if (!WriteMemory(address, buffer))
                return false;

            return true;
        }

        private bool ReadMemory(ulong address, byte[] buffer)
        {
#if _WINDOWS
            int bytesRead = 0;
            return Win32.ReadProcessMemory(_processHandle, address, buffer, buffer.Length, ref bytesRead);
#elif _LINUX
            unsafe
            {
                fixed (byte* localBuffer = buffer)
                {
                    var localIov = new LinuxKernel.iovec { iov_base = localBuffer, iov_len = buffer.Length };
                    var remoteIov = new LinuxKernel.iovec { iov_base = (void*)address, iov_len = buffer.Length };
                    return LinuxKernel.process_vm_readv(_process.Id, &localIov, 1, &remoteIov, 1, 0) == buffer.Length;
                }
            }
#else
            return false;
#endif
        }

        private bool WriteMemory(ulong address, byte[] buffer)
        {
#if _WINDOWS
            int bytesWritten = 0;
            return Win32.WriteProcessMemory(_processHandle, address, buffer, buffer.Length, ref bytesWritten);
#elif _LINUX
            unsafe
            {
                fixed (byte* localBuffer = buffer)
                {
                    var localIov = new LinuxKernel.iovec { iov_base = localBuffer, iov_len = buffer.Length };
                    var remoteIov = new LinuxKernel.iovec { iov_base = (void*)address, iov_len = buffer.Length };
                    return LinuxKernel.process_vm_writev(_process.Id, &localIov, 1, &remoteIov, 1, 0) == buffer.Length;
                }
            }
#else
            return false;
#endif
        }
    }
}
