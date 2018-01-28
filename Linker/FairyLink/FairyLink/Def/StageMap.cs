using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Def
{
    static class StageMap
    {
        public static readonly int XMax = 240;
        public static readonly int YMax = 12;
        public static readonly int BlockSize = 64;

        public static readonly int Width = StageMap.XMax * StageMap.BlockSize;
        public static readonly int Height = StageMap.YMax * StageMap.BlockSize;
    }
}
