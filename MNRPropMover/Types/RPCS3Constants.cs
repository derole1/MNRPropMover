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
        // MNR 1.11 US - This will differ for other game versions and regions
        public const ulong PROP_STAMP_ADDRESS = 0x00F03688;
    }
}
