using MNRPropMover.Types;

namespace MNRPropMover
{
    internal class Program
    {
        private static EmulatorProcess _process;
        private static MNRPropStamp _propStamp;
        private static MNRWorldChunkList _worldChunks;
        private static PropController _propController;

        private static bool _debug = false;

        static void Main(string[] args)
        {
            if (args.Contains("--debug") || args.Contains("-d"))
                _debug = true;

#if _LINUX
            int ptraceScope = GetPtraceScope();
            if (ptraceScope != 0)
            {
                Console.WriteLine($"Ptrace scope is {ptraceScope}."
                    + "\nRun: sudo sysctl -w kernel.yama.ptrace_scope=0 or run this program as superuser");
                return;
            }
#endif

            _process = new EmulatorProcess("rpcs3");
            if (!_process.Attach())
            {
                Console.WriteLine("Unable to attach to rpcs3.exe!");
                return;
            }

            _propStamp = new MNRPropStamp(_process);
            _worldChunks = new MNRWorldChunkList(_process);
            _propController = new PropController(_propStamp);

            while (true)
            {
                Console.Clear();

                var windowTitle = _process.WindowTitle;
                var deltaTime = 0.3333f;

                if (windowTitle.StartsWith("FPS: "))
                    deltaTime = 1f / float.Parse(windowTitle.Substring(5, windowTitle.IndexOf(" | ") - 5));

                Update(deltaTime);

                // The below is to fix a memory leak, this shouldnt really be needed and is bad practise.
                GC.Collect();

                Thread.Sleep((int)(deltaTime * 1000f));
            }
        }

        static void Update(float deltaTime)
        {
            var currentProp = _propStamp.CurrentProp;

            if (currentProp != null)
            {
                if (currentProp.Address != _propController.Prop?.Address)
                    _propController.SetProp(currentProp);

                _propController.Update(deltaTime);

                Console.Write($"Current Prop Information:\n\n" +
                                $"Type UID: 0x{currentProp.TypeUID:x}\n" +
                                $"Position: X={currentProp.PositionX}, Y={currentProp.PositionY}, Z={currentProp.PositionZ}\n" +
                                $"Rotation: X={currentProp.RotationX}, Y={currentProp.RotationY}, Z={currentProp.RotationZ}\n" +
                                $"Scale: {currentProp.Scale}");

                if (_debug)
                {
                    Console.Write("\n\nDebug Information:\n\n" +
                                      $"Prop Stamp Address: 0x{RPCS3Constants.RPCS3_EMU_BASE_ADDRESS + currentProp.Address:x}\n" +
                                      $"Prop Stamp Address (VM): 0x{currentProp.Address:x}");

                    Console.Write($"\n\nFlags:\n\n{((int)currentProp.Flags).ToBinDump()}");
                    Console.Write($"\n\nSnap Flags:\n\n{((int)currentProp.SnapFlags).ToBinDump()}");

                    Console.Write($"\n\nProp Buffer:\n\n{currentProp.ToBuffer().ToHexDump()}");
                }
            }
            else
                Console.Write("No prop is currently selected.");

            // Disable the check for all _pitrow_ props that causes them to be deleted
            _process.WriteString(RPCS3Constants.RPCS3_EMU_BASE_ADDRESS + RPCS3Constants.PITROW_STRING_ADDRESS, "");
        }

#if _LINUX
        static int GetPtraceScope()
        {
            using (var reader = new StreamReader("/proc/sys/kernel/yama/ptrace_scope"))
            {
                return int.Parse(reader.ReadToEnd());
            }
        }
#endif

    }
}
