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
        
        RotateLeft45 = (1 << 16),
        RotateRight45 = (1 << 17),
        TiltForward45 = (1 << 18),
        TiltBackward45 = (1 << 19),
        TiltLeft45 = (1 << 20),
        TiltRight45 = (1 << 21),

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
