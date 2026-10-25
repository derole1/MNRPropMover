using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNRPropMover.Types
{
    internal class RPCS3Constants
    {
        public const ulong RPCS3_EMU_BASE_ADDRESS = 0x300000000;
        // MNR 1.11 US
        public const ulong WORLD_CHUNKS_ADDRESS = 0x01468150;
        public const ulong PROP_STAMP_ADDRESS = 0x00F03688;
        public const ulong PITROW_STRING_ADDRESS = 0x0140d738;
    }
}
