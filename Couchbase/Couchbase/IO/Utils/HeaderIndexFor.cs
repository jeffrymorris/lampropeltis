﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.IO.Utils
{
    public struct HeaderIndexFor
    {
        public const byte MagicValue = 0x81;
        public const int HeaderLength = 24;

        public const int Magic = 0;
        public const int Opcode = 1;
        public const int KeyLength = 2; // 2-3
        public const int ExtrasLength = 4;
        public const int Datatype = 5;
        public const int Status = 6; // 6-7
        public const int Body = 8; // 8-11
        public const int Opaque = 12; // 12-15
        public const int Cas = 16; // 16-23
    }
}
