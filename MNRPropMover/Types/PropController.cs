using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNRPropMover.Types
{
    internal class PropController
    {
        const float MIN_STEP = 0f;
        const float MAX_STEP = 0.05f;
        const float DURATION = 2f;

        private float stepProgress = 0;
        private PropControllerState state = PropControllerState.None;

        public MNRProp? Prop { get; protected set; }

        public void SetProp(MNRProp prop)
        {
            Prop = prop;
        }

        public void Update(float deltaTime)
        {
            if (Prop == null)
                return;

            var currentState = PropControllerState.None;

#if _WINDOWS
            if (Win32.GetAsyncKeyState(Win32.VK_UP) != 0) currentState |= PropControllerState.TiltForward;
            if (Win32.GetAsyncKeyState(Win32.VK_DOWN) != 0) currentState |= PropControllerState.TiltBackward;
            if (Win32.GetAsyncKeyState(Win32.VK_LEFT) != 0) currentState |= PropControllerState.TiltLeft;
            if (Win32.GetAsyncKeyState(Win32.VK_RIGHT) != 0) currentState |= PropControllerState.TiltRight;
#else
            // TODO: Linux key detection outside of console window like in Windows (What is the linux equivalent of GetAsyncKeyState?)
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow) currentState |= PropControllerState.TiltForward;
                else if (key == ConsoleKey.DownArrow) currentState |= PropControllerState.TiltBackward;
                else if (key == ConsoleKey.LeftArrow) currentState |= PropControllerState.TiltLeft;
                else if (key == ConsoleKey.RightArrow) currentState |= PropControllerState.TiltRight;
            }
#endif

            if (((currentState | state) & PropControllerState.TiltForwardBackward) == PropControllerState.TiltForwardBackward)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.TiltLeftRight) == PropControllerState.TiltLeftRight)
                stepProgress = 0;
            else if (currentState != PropControllerState.None)
            {
                if (stepProgress < 1f)
                    stepProgress += deltaTime / DURATION;
            }
            else
                stepProgress = 0;

            if ((currentState & PropControllerState.TiltForward) == PropControllerState.TiltForward)
                Prop.RotationY -= Lerp(MIN_STEP, MAX_STEP, stepProgress);
            if ((currentState & PropControllerState.TiltBackward) == PropControllerState.TiltBackward)
                Prop.RotationY += Lerp(MIN_STEP, MAX_STEP, stepProgress);
            if ((currentState & PropControllerState.TiltLeft) == PropControllerState.TiltLeft)
                Prop.RotationZ -= Lerp(MIN_STEP, MAX_STEP, stepProgress);
            if ((currentState & PropControllerState.TiltRight) == PropControllerState.TiltRight)
                Prop.RotationZ += Lerp(MIN_STEP, MAX_STEP, stepProgress);

            state = currentState;
        }

        private float Lerp(float start, float end, float progress) => start + (end - start) * progress;
    }
}
