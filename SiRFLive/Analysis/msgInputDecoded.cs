﻿namespace SiRFLive.Analysis
{
    using SiRFLive.MessageHandling;
    using SiRFLive.Utilities;
    using System;
    using System.Configuration;

    public class msgInputDecoded
    {
        public static string[] DecodeInputMsg(string message)
        {
            if ((message.Length == 0) || (message == null))
            {
                string[] strArray = new string[0x33b];
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = "0";
                }
                return strArray;
            }
            byte[] comByte = HelperFunctions.HexToByte(message);
            MsgFactory factory = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols_F.xml");
            return factory.ConvertRawToFieldsForInput(comByte).Split(new char[] { ',' });
        }
    }
}

