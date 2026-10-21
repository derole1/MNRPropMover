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
        const float MAX_STEP_MOVE = MAX_STEP * 10f;
        const float DURATION = 2f;
        const float QUARTER_TURN = MathF.PI / 2f;

        private MNRPropStamp _propStamp;
        private float stepProgress = 0;
        private PropControllerState state = PropControllerState.None;

        public MNRProp? Prop { get; protected set; }

        public PropController(MNRPropStamp propStamp)
        {
            _propStamp = propStamp;
        }

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

            if (Win32.GetAsyncKeyState(Win32.VK_Q) != 0) currentState |= PropControllerState.RotateLeft;
            if (Win32.GetAsyncKeyState(Win32.VK_E) != 0) currentState |= PropControllerState.RotateRight;

            if (Win32.GetAsyncKeyState(Win32.VK_U) != 0) currentState |= PropControllerState.RotateXNegative90;
            if (Win32.GetAsyncKeyState(Win32.VK_O) != 0) currentState |= PropControllerState.RotateXPositive90;
            if (Win32.GetAsyncKeyState(Win32.VK_J) != 0) currentState |= PropControllerState.RotateYNegative90;
            if (Win32.GetAsyncKeyState(Win32.VK_L) != 0) currentState |= PropControllerState.RotateYPositive90;
            if (Win32.GetAsyncKeyState(Win32.VK_I) != 0) currentState |= PropControllerState.RotateZNegative90;
            if (Win32.GetAsyncKeyState(Win32.VK_K) != 0) currentState |= PropControllerState.RotateZPositive90;

            if (Win32.GetAsyncKeyState(Win32.VK_PRIOR) != 0) currentState |= PropControllerState.ScaleUp;
            if (Win32.GetAsyncKeyState(Win32.VK_NEXT) != 0) currentState |= PropControllerState.ScaleDown;

            if (Win32.GetAsyncKeyState(Win32.VK_W) != 0) currentState |= PropControllerState.MoveForwards;
            if (Win32.GetAsyncKeyState(Win32.VK_S) != 0) currentState |= PropControllerState.MoveBackwards;
            if (Win32.GetAsyncKeyState(Win32.VK_A) != 0) currentState |= PropControllerState.MoveLeft;
            if (Win32.GetAsyncKeyState(Win32.VK_D) != 0) currentState |= PropControllerState.MoveRight;
            if (Win32.GetAsyncKeyState(Win32.VK_R) != 0) currentState |= PropControllerState.MoveUp;
            if (Win32.GetAsyncKeyState(Win32.VK_F) != 0) currentState |= PropControllerState.MoveDown;

            if (Win32.GetAsyncKeyState(Win32.VK_OEM_4) != 0) currentState |= PropControllerState.PreviousProp;
            if (Win32.GetAsyncKeyState(Win32.VK_OEM_6) != 0) currentState |= PropControllerState.NextProp;
#else
            // TODO: Implement backgground key detection for Linux (Equivalent of GetAsyncKeyState?)
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow) currentState |= PropControllerState.TiltForward;
                else if (key == ConsoleKey.DownArrow) currentState |= PropControllerState.TiltBackward;
                else if (key == ConsoleKey.LeftArrow) currentState |= PropControllerState.TiltLeft;
                else if (key == ConsoleKey.RightArrow) currentState |= PropControllerState.TiltRight;
                
                else if (key == ConsoleKey.Q) currentState |= PropControllerState.RotateLeft;
                else if (key == ConsoleKey.E) currentState |= PropControllerState.RotateRight;

                else if (key == ConsoleKey.U) currentState |= PropControllerState.RotateXNegative90;
                else if (key == ConsoleKey.O) currentState |= PropControllerState.RotateXPositive90;
                else if (key == ConsoleKey.J) currentState |= PropControllerState.RotateYNegative90;
                else if (key == ConsoleKey.L) currentState |= PropControllerState.RotateYPositive90;
                else if (key == ConsoleKey.I) currentState |= PropControllerState.RotateZNegative90;
                else if (key == ConsoleKey.K) currentState |= PropControllerState.RotateZPositive90;

                else if (key == ConsoleKey.PageUp) currentState |= PropControllerState.ScaleUp;
                else if (key == ConsoleKey.PageDown) currentState |= PropControllerState.ScaleDown;
                
                else if (key == ConsoleKey.W) currentState |= PropControllerState.MoveForwards;
                else if (key == ConsoleKey.S) currentState |= PropControllerState.MoveBackwards;
                else if (key == ConsoleKey.A) currentState |= PropControllerState.MoveLeft;
                else if (key == ConsoleKey.D) currentState |= PropControllerState.MoveRight;
                else if (key == ConsoleKey.R) currentState |= PropControllerState.MoveUp;
                else if (key == ConsoleKey.F) currentState |= PropControllerState.MoveDown;
                
                else if (key == ConsoleKey.Oem4) currentState |= PropControllerState.PreviousProp;
                else if (key == ConsoleKey.Oem6) currentState |= PropControllerState.NextProp;
            }
#endif

            if (((currentState | state) & PropControllerState.TiltForwardBackward) == PropControllerState.TiltForwardBackward)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.TiltLeftRight) == PropControllerState.TiltLeftRight)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.RotateLeftRight) == PropControllerState.RotateLeftRight)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.ScaleUpDown) == PropControllerState.ScaleUpDown)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.MoveForwardsBackwards) == PropControllerState.MoveForwardsBackwards)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.MoveLeftRight) == PropControllerState.MoveLeftRight)
                stepProgress = 0;
            else if (((currentState | state) & PropControllerState.MoveUpDown) == PropControllerState.MoveUpDown)
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

            if ((currentState & PropControllerState.RotateLeft) == PropControllerState.RotateLeft)
                Prop.RotationX -= Lerp(MIN_STEP, MAX_STEP, stepProgress);
            if ((currentState & PropControllerState.RotateRight) == PropControllerState.RotateRight)
                Prop.RotationX += Lerp(MIN_STEP, MAX_STEP, stepProgress);

            if ((currentState & PropControllerState.RotateXNegative90) != 0 && (state & PropControllerState.RotateXNegative90) == 0)
                Prop.RotationX -= QUARTER_TURN;
            if ((currentState & PropControllerState.RotateXPositive90) != 0 && (state & PropControllerState.RotateXPositive90) == 0)
                Prop.RotationX += QUARTER_TURN;
            if ((currentState & PropControllerState.RotateYNegative90) != 0 && (state & PropControllerState.RotateYNegative90) == 0)
                Prop.RotationY -= QUARTER_TURN;
            if ((currentState & PropControllerState.RotateYPositive90) != 0 && (state & PropControllerState.RotateYPositive90) == 0)
                Prop.RotationY += QUARTER_TURN;
            if ((currentState & PropControllerState.RotateZNegative90) != 0 && (state & PropControllerState.RotateZNegative90) == 0)
                Prop.RotationZ -= QUARTER_TURN;
            if ((currentState & PropControllerState.RotateZPositive90) != 0 && (state & PropControllerState.RotateZPositive90) == 0)
                Prop.RotationZ += QUARTER_TURN;

            if ((currentState & PropControllerState.ScaleUp) == PropControllerState.ScaleUp)
                Prop.Scale += Lerp(MIN_STEP, MAX_STEP, stepProgress);
            if ((currentState & PropControllerState.ScaleDown) == PropControllerState.ScaleDown)
                Prop.Scale -= Lerp(MIN_STEP, MAX_STEP, stepProgress);

            if ((currentState & PropControllerState.MoveForwards) == PropControllerState.MoveForwards)
                Prop.PositionY += Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);
            if ((currentState & PropControllerState.MoveBackwards) == PropControllerState.MoveBackwards)
                Prop.PositionY -= Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);
            if ((currentState & PropControllerState.MoveLeft) == PropControllerState.MoveLeft)
                Prop.PositionX -= Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);
            if ((currentState & PropControllerState.MoveRight) == PropControllerState.MoveRight)
                Prop.PositionX += Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);
            if ((currentState & PropControllerState.MoveUp) == PropControllerState.MoveUp)
                Prop.PositionZ += Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);
            if ((currentState & PropControllerState.MoveDown) == PropControllerState.MoveDown)
                Prop.PositionZ -= Lerp(MIN_STEP, MAX_STEP_MOVE, stepProgress);

            if ((currentState & PropControllerState.MoveUpDown) != 0)
                Prop.SnapFlags &= ~MNRPropSnapFlags.IsSnapped;

            if (((currentState & state) & PropControllerState.PreviousNextProp) == 0)
            {
                if ((currentState & PropControllerState.PreviousProp) == PropControllerState.PreviousProp)
                    _propStamp.CurrentProp = Prop.PreviousProp;
                if ((currentState & PropControllerState.NextProp) == PropControllerState.NextProp)
                    _propStamp.CurrentProp = Prop.NextProp;
            }

            state = currentState;
        }

        private float Lerp(float start, float end, float progress) => start + (end - start) * progress;
    }
}
