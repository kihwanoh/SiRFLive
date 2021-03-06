﻿namespace SiRFLive.Communication
{
    using OpenNETCF.IO.Serial;
    using System;

    public class UART_Properties
    {
        public int BaudRate = 0xe100;
        public int DataBits = 8;
        public OpenNETCF.IO.Serial.Parity Parity;
        public string PortName = string.Empty;
        public OpenNETCF.IO.Serial.StopBits StopBits;
    }
}

