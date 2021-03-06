﻿namespace SiRFLive.MessageHandling
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct OutputMsg
    {
        public int messageID;
        public int fieldNumber;
        public string fieldName;
        public int bytes;
        public string datatype;
        public string units;
        public double scale;
        public int startByte;
        public int endByte;
        public string referenceField;
    }
}

