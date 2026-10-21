using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNRPropMover.Types
{
    internal enum PropControllerState
    {
        None = 0,

        TiltForward = (1 << 0),
        TiltBackward = (1 << 1),
        TiltLeft = (1 << 2),
        TiltRight = (1 << 3),

        RotateLeft = (1 << 4),
        RotateRight = (1 << 5),

        ScaleUp = (1 << 6),
        ScaleDown = (1 << 7),

        MoveForwards = (1 << 8),
        MoveBackwards = (1 << 9),
        MoveLeft = (1 << 10),
        MoveRight = (1 << 11),
        MoveUp = (1 << 12),
        MoveDown = (1 << 13),

        PreviousProp = (1 << 14),
        NextProp = (1 << 15),
        
        RotateXNegative90 = (1 << 16),
        RotateXPositive90 = (1 << 17),
        RotateYNegative90 = (1 << 18),
        RotateYPositive90 = (1 << 19),
        RotateZNegative90 = (1 << 20),
        RotateZPositive90 = (1 << 21),

        TiltForwardBackward = TiltForward | TiltBackward,
        TiltLeftRight = TiltLeft | TiltRight,

        RotateLeftRight = RotateLeft | RotateRight,

        ScaleUpDown = ScaleUp | ScaleDown,

        MoveForwardsBackwards = MoveForwards | MoveBackwards,
        MoveLeftRight = MoveLeft | MoveRight,
        MoveUpDown = MoveUp | MoveDown,

        PreviousNextProp = PreviousProp | NextProp
    }
}
