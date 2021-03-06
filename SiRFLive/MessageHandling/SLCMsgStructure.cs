﻿namespace SiRFLive.MessageHandling
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SLCMsgStructure
    {
        public int messageType;
        public int messageID;
        public string messageName;
        public int fieldNumber;
        public string fieldName;
        public int bytes;
        public string datatype;
        public string units;
        public double scale;
        public int startByte;
        public int endByte;
        public string defaultValue;
    }
}

