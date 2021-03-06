﻿namespace SiRFLive.GUI
{
    using CommonUtilsClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.Configuration;
    using SiRFLive.General;
    using SiRFLive.MessageHandling;
    using SiRFLive.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public class frmFileConversion : Form
    {
        private bool _abort;
        private bool _conversionStatus;
        private ConversionType _conversionType;
        private int _convertedFilesCount;
        private string _fileType = ".gpx";
        private RadioButton autoDetectRadioBtn;
        private Button automationTestAbortBtn;
        private Button autoTestAddAllBtn;
        private Button autoTestAddBtn;
        private Label autoTestAvailableScriptsLabel;
        private Button autoTestClearAvailableListBtn;
        private Button autoTestDirBrowser;
        private TextBox autoTestDirVal;
        private Button autoTestExitBtn;
        private Label autoTestFilePathLabel;
        private Button autoTestRefreshBtn;
        private Button autoTestRemoveAllBtn;
        private Button autoTestRemoveBtn;
        private Button autoTestRunBtn;
        private Label autoTestRunScriptsLabel;
        private IContainer components;
        private int conversionThreadID;
        private Label convertedFilesLabel;
        private ObjectInterface crossThreadUpdateIntf = new ObjectInterface();
        private EmailHandler emailHelper = new EmailHandler();
        private ListBox fileConversionAvailableFilesListVal;
        private string fileConversionIniPath = (clsGlobal.InstalledDirectory + @"\Config\SiRFLiveFileConversion.cfg");
        private Label fileConversionStatusLabel;
        private ListBox fileConversionToConvertFilesListVal;
        private static int fileIdx;
        private string[] filesArray = new string[0];
        private List<string> filesFullPathLists = new List<string>();
        private Label frmFileConversionFilesCntLabel;
        private const double GPS_DOP_LSB = 0.2;
        private const int GPS_MODE_DGPS_USED = 0x80;
        private const int GPS_MODE_MASK = 7;
        private const int GPS_NUM_CHANNELS = 12;
        private CheckBox includeDateTimeChkBox;
        private static frmFileConversion m_SChildform;
        private float[] Power10 = new float[] { 1f, 10f, 100f, 1000f, 10000f, 100000f, 1000000f, 1E+07f, 1E+08f, 1E+09f, 1E+10f, 1E+11f, 1E+12f };
        private ProgressBar progressBar1;
        private string testResults = string.Empty;
        private List<string> toRunList = new List<string>();
        private string[] toRunListArray = new string[0];
        private RadioButton useOspMsg69RadioBtn;
        private RadioButton useSSBMsg41RadioBtn;

        internal event updateParentEventHandler updateParent;

        public frmFileConversion(ConversionType conversionType)
        {
            this.InitializeComponent();
            IniHelper helper = new IniHelper(this.fileConversionIniPath);
            string str = string.Empty;
            this._conversionType = conversionType;
            switch (conversionType)
            {
                case ConversionType.GP2ToGPS:
                    this._fileType = ".gpx";
                    break;

                case ConversionType.BinToGPS_GP2:
                    this._fileType = ".bin";
                    break;

                case ConversionType.GPSToNMEA:
                case ConversionType.GPSToKML:
                    this._fileType = ".gps";
                    break;

                case ConversionType.NMEAToGPS:
                    this._fileType = ".nmea";
                    break;

                default:
                    this._fileType = ".gpx";
                    break;
            }
            try
            {
                if (File.Exists(this.fileConversionIniPath))
                {
                    foreach (string str2 in helper.GetKeys("EMAIL"))
                    {
                        if (!str2.Contains("#"))
                        {
                            str = helper.GetIniFileString("EMAIL", str2, "");
                            if (str.Length != 0)
                            {
                                str = str.Replace(" ", "").TrimEnd(new char[] { '\n' }).TrimEnd(new char[] { '\r' });
                                ConfigurationManager.AppSettings[str2] = str;
                            }
                        }
                    }
                    str = helper.GetIniFileString("SETUP", "AVAILABLE_SCRIPTS", "");
                    if (str.Length != 0)
                    {
                        this.filesFullPathLists.Clear();
                        foreach (string str3 in str.Split(new char[] { ',' }))
                        {
                            if (str3.Length != 0)
                            {
                                this.addAvailableFiles(str3);
                            }
                        }
                    }
                    this.filesArray = this.filesFullPathLists.ToArray();
                    this.autoTestDirVal.Text = helper.GetIniFileString("SETUP", "SCRIPTS_DIR", "");
                    str = helper.GetIniFileString("SETUP", "SEND_EMAIL", "");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ERROR!");
            }
        }

        private void addAvailableFiles(string fileStr)
        {
            string[] strArray = new string[] { "" };
            int length = 0;
            this.filesFullPathLists.Add(fileStr);
            strArray = fileStr.Split(new char[] { '\\' });
            length = strArray.Length;
            this.fileConversionAvailableFilesListVal.Items.Add(strArray[length - 1]);
        }

        private void conversionAbort()
        {
            if (MessageBox.Show("Abort conversion?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                this._abort = true;
                this._conversionStatus = false;
            }
        }

        private void convertFiles()
        {
            EventHandler method = null;
            EventHandler handler3 = null;
            EventHandler handler4 = null;
            EventHandler handler5 = null;
            EventHandler handler6 = null;
            EventHandler handler7 = null;
            EventHandler handler8 = null;
            EventHandler handler9 = null;
            EventHandler handler10 = null;
            EventHandler handler11 = null;
            EventHandler handler12 = null;
            CommunicationManager manager = new CommunicationManager();
            manager.RxCtrl = new OSPReceiver();
            manager.RxCtrl.ResetCtrl = new OSPReset();
            StreamWriter writer = null;
            StreamWriter writer2 = null;
            StreamReader reader = null;
            LargeFileHandler handler = null;
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            try
            {
                foreach (string str in this.toRunList)
                {
                    if (!File.Exists(str))
                    {
                        this._convertedFilesCount++;
                        if (method == null)
                        {
                            method = delegate {
                                this.convertedFilesLabel.Text = string.Format("Files converted: {0}", this._convertedFilesCount);
                            };
                        }
                        this.convertedFilesLabel.BeginInvoke(method);
                        continue;
                    }
                    string str2 = string.Empty;
                    string path = string.Empty;
                    string str4 = string.Empty;
                    if (str.EndsWith(".gp2"))
                    {
                        path = str.Replace(".gp2", ".gps");
                    }
                    else if (str.EndsWith(".gpx"))
                    {
                        path = str.Replace(".gpx", ".gps");
                    }
                    else if (str.EndsWith(".bin"))
                    {
                        path = str.Replace(".bin", ".gps");
                        str4 = str.Replace(".bin", ".gp2");
                    }
                    else if (str.EndsWith(".gps"))
                    {
                        if (this._conversionType == ConversionType.GPSToNMEA)
                        {
                            if (this.useSSBMsg41RadioBtn.Checked)
                            {
                                path = str.Replace(".gps", "_msg41.nmea");
                            }
                            else if (this.useOspMsg69RadioBtn.Checked)
                            {
                                path = str.Replace(".gps", "_msg69.nmea");
                            }
                            else if (this.autoDetectRadioBtn.Checked)
                            {
                                path = str.Replace(".gps", "_auto.nmea");
                                str4 = str.Replace(".gps", ".msg69parse");
                            }
                            else
                            {
                                path = str.Replace(".gps", ".nmea");
                            }
                        }
                        else if (this._conversionType == ConversionType.GPSToKML)
                        {
                            path = str.Replace(".gps", ".kml");
                        }
                        else
                        {
                            path = str.Replace(".gps", ".convert");
                        }
                    }
                    else if (str.EndsWith(".nmea"))
                    {
                        path = str.Replace(".nmea", ".gps");
                    }
                    else
                    {
                        path = str.Substring(0, str.Length - 4) + ".gps";
                    }
                    writer = new StreamWriter(path);
                    reader = new StreamReader(str);
                    FileInfo info = new FileInfo(str);
                    double length = info.Length;
                    if (length == 0.0)
                    {
                        this._convertedFilesCount++;
                        if (handler3 == null)
                        {
                            handler3 = delegate {
                                this.convertedFilesLabel.Text = string.Format("Files converted: {0}", this._convertedFilesCount);
                            };
                        }
                        this.convertedFilesLabel.BeginInvoke(handler3);
                        continue;
                    }
                    if (handler4 == null)
                    {
                        handler4 = delegate {
                            this.progressBar1.Value = 0;
                            this.progressBar1.Maximum = 100;
                            this.progressBar1.Minimum = 0;
                        };
                    }
                    this.progressBar1.BeginInvoke(handler4);
                    long num2 = 0L;
                    switch (this._conversionType)
                    {
                        case ConversionType.GP2ToGPS:
                            if (handler6 == null)
                            {
                                handler6 = delegate {
                                    this.fileConversionStatusLabel.Text = "Status: converting...";
                                };
                            }
                            this.fileConversionStatusLabel.BeginInvoke(handler6);
                            while ((str2 = reader.ReadLine()) != null)
                            {
                                num2 += str2.Length;
                                int percent = (int) ((((double) num2) / length) * 100.0);
                                if (percent > 100)
                                {
                                    percent = 100;
                                }
								this.progressBar1.BeginInvoke((MethodInvoker)delegate
								{
                                    this.progressBar1.Value = percent;
                                });
                                try
                                {
                                    if (str2.Contains("A0 A2"))
                                    {
                                        int index = str2.IndexOf("A0 A2");
                                        byte[] comByte = HelperFunctions.HexToByte(str2.Substring(index));
                                        string str9 = manager.m_Protocols.ConvertRawToFields(comByte);
                                        if (this.includeDateTimeChkBox.Checked)
                                        {
                                            writer.WriteLine(str2.Substring(0, index) + " " + str9);
                                        }
                                        else
                                        {
                                            writer.WriteLine(str9);
                                        }
                                    }
                                    else
                                    {
                                        writer.WriteLine(str2);
                                    }
                                }
                                catch
                                {
                                    writer.WriteLine(str2);
                                    continue;
                                }
                                if (this._abort)
                                {
                                    break;
                                }
                            }
                            goto Label_1731;

                        case ConversionType.BinToGPS_GP2:
                            break;

                        case ConversionType.GPSToNMEA:
                        {
                            reader.Close();
                            Queue<uint> queue = new Queue<uint>();
                            Queue<long> queue2 = new Queue<long>();
                            handler = new LargeFileHandler(str);
                            long item = 0L;
                            if (this.autoDetectRadioBtn.Checked)
                            {
                                writer2 = new StreamWriter(str4);
                                if (handler7 == null)
                                {
                                    handler7 = delegate {
                                        this.fileConversionStatusLabel.Text = "Status: scanning...";
                                    };
                                }
                                this.fileConversionStatusLabel.BeginInvoke(handler7);
                                str2 = handler[item];
                                while (str2 != "EOF")
                                {
                                    str2 = str2.Replace(" ", "");
                                    if (str2.StartsWith("69,1"))
                                    {
                                        try
                                        {
                                            string[] strArray3 = str2.Split(new char[] { ',' });
                                            if (strArray3.Length > 9)
                                            {
                                                uint num8 = Convert.ToUInt32(strArray3[9]);
                                                writer2.WriteLine(string.Format("{0} -- {1}", num8, strArray3[5]));
                                                if ((strArray3[3] != "0") && (strArray3[5] == "1"))
                                                {
                                                    queue.Enqueue(num8);
                                                    queue2.Enqueue(item);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    item = handler.Index + 1L;
                                    str2 = handler[item];
                                }
                                writer2.Close();
                                item = 0L;
                                str2 = handler[item];
                            }
                            if (handler8 == null)
                            {
                                handler8 = delegate {
                                    this.fileConversionStatusLabel.Text = "Status: converting...";
                                };
                            }
                            this.fileConversionStatusLabel.BeginInvoke(handler8);
                            while (str2 != "EOF")
                            {
                                string str11;
                                num2 += str2.Length;
                                int percent = (int) ((((double) num2) / length) * 100.0);
                                if (percent > 100)
                                {
                                    percent = 100;
                                }
								this.progressBar1.BeginInvoke((MethodInvoker)delegate
								{
                                    this.progressBar1.Value = percent;
                                });
                                str2 = str2.TrimEnd(new char[] { '\n' }).TrimEnd(new char[] { '\r' }).Replace(" ", "");
                                if (str2.StartsWith("4,"))
                                {
                                    string[] strArray4 = str2.Split(new char[] { ',' });
                                    string str10 = this.GPS_NMEA_OutputGSV(strArray4);
                                    if (str10 != string.Empty)
                                    {
                                        writer.Write(str10);
                                    }
                                    goto Label_0AFE;
                                }
                                if (!str2.StartsWith("41,"))
                                {
                                    goto Label_0A08;
                                }
                                if (!this.useSSBMsg41RadioBtn.Checked && !this.autoDetectRadioBtn.Checked)
                                {
                                    goto Label_0AFE;
                                }
                                string[] msgArray = str2.Split(new char[] { ',' });
                                if (msgArray.Length < 0x22)
                                {
                                    item = handler.Index + 1L;
                                    str2 = handler[item];
                                    continue;
                                }
                                if ((queue.Count > 0) && this.autoDetectRadioBtn.Checked)
                                {
                                    uint num9 = Convert.ToUInt32(msgArray[4]);
                                    uint num10 = queue.Peek();
                                    long num11 = queue2.Peek();
                                    if (num9 == num10)
                                    {
                                        queue.Dequeue();
                                        queue2.Dequeue();
                                    }
                                    else
                                    {
                                        if (num9 > num10)
                                        {
                                            if (item > num11)
                                            {
                                                while (item > num11)
                                                {
                                                    queue.Dequeue();
                                                    queue2.Dequeue();
                                                    num10 = queue.Peek();
                                                    num11 = queue2.Peek();
                                                    if (num9 == num10)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (item > num11)
                                                {
                                                    goto Label_0960;
                                                }
                                                continue;
                                            }
                                            item = handler.Index + 1L;
                                            str2 = handler[item];
                                            continue;
                                        }
                                        item = handler.Index + 1L;
                                        str2 = handler[item];
                                        continue;
                                    }
                                }
                            Label_0960:
                                str11 = this.GPS_NMEA_OutputGGA(msgArray);
                                if (str11 != string.Empty)
                                {
                                    writer.Write(str11);
                                }
                                str11 = this.GPS_NMEA_OutputRMC(msgArray);
                                if (str11 != string.Empty)
                                {
                                    writer.Write(str11);
                                }
                                str11 = this.GPS_NMEA_OutputGLL(msgArray);
                                if (str11 != string.Empty)
                                {
                                    writer.Write(str11);
                                }
                                str11 = this.GPS_NMEA_OutputGSA(msgArray);
                                if (str11 != string.Empty)
                                {
                                    writer.Write(str11);
                                }
                                str11 = this.GPS_NMEA_OutputVTG(msgArray);
                                if (str11 != string.Empty)
                                {
                                    writer.Write(str11);
                                }
                                goto Label_0AFE;
                            Label_0A08:
                                if (str2.StartsWith("69,1") && this.useOspMsg69RadioBtn.Checked)
                                {
                                    string[] strArray6 = new string[0x24];
                                    if (this.msg69ToMsg4AndMsg41Format(str2, ref strArray6) != 0)
                                    {
                                        item = handler.Index + 1L;
                                        str2 = handler[item];
                                        continue;
                                    }
                                    string str12 = this.GPS_NMEA_OutputGGA(strArray6);
                                    if (str12 != string.Empty)
                                    {
                                        writer.Write(str12);
                                    }
                                    str12 = this.GPS_NMEA_OutputRMC(strArray6);
                                    if (str12 != string.Empty)
                                    {
                                        writer.Write(str12);
                                    }
                                    str12 = this.GPS_NMEA_OutputGLL(strArray6);
                                    if (str12 != string.Empty)
                                    {
                                        writer.Write(str12);
                                    }
                                    str12 = this.GPS_NMEA_OutputGSA(strArray6);
                                    if (str12 != string.Empty)
                                    {
                                        writer.Write(str12);
                                    }
                                    str12 = this.GPS_NMEA_OutputVTG(strArray6);
                                    if (str12 != string.Empty)
                                    {
                                        writer.Write(str12);
                                    }
                                }
                            Label_0AFE:
                                if (this._abort)
                                {
                                    break;
                                }
                                item = handler.Index + 1L;
                                str2 = handler[item];
                            }
                            goto Label_1731;
                        }
                        case ConversionType.GPSToKML:
                        {
                            if (handler10 == null)
                            {
                                handler10 = delegate {
                                    this.fileConversionStatusLabel.Text = "Status: converting...";
                                };
                            }
                            this.fileConversionStatusLabel.BeginInvoke(handler10);
                            PortManager manager2 = new PortManager();
                            manager.MessageProtocol = "OSP";
                            manager2.comm.SetupRxCtrl();
                            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                            writer.WriteLine("<kml xmlns=\"http://earth.google.com/kml/2.0\">");
                            writer.WriteLine("\t<Document>");
                            writer.WriteLine("\t\t<Style id=\"red\">");
                            writer.WriteLine("\t\t\t<IconStyle>");
                            writer.WriteLine("\t\t\t\t<Icon><href>http://maps.google.com/mapfiles/kml/pal3/icon63.png</href></Icon>");
                            writer.WriteLine("\t\t\t\t<color>ff0000ff</color>");
                            writer.WriteLine("\t\t\t\t<scale>.3</scale>");
                            writer.WriteLine("\t\t\t</IconStyle>");
                            writer.WriteLine("\t\t</Style>");
                            writer.WriteLine("\t\t<Style id=\"green\">");
                            writer.WriteLine("\t\t\t<IconStyle>");
                            writer.WriteLine("\t\t\t\t<Icon><href>http://maps.google.com/mapfiles/kml/pal3/icon63.png</href></Icon>");
                            writer.WriteLine("\t\t\t\t<color>ff00ff00</color>");
                            writer.WriteLine("\t\t\t\t<scale>.3</scale>");
                            writer.WriteLine("\t\t\t</IconStyle>");
                            writer.WriteLine("\t\t</Style>");
                            writer.WriteLine("\t\t<Style id=\"blue\">");
                            writer.WriteLine("\t\t\t<IconStyle>");
                            writer.WriteLine("\t\t\t\t<Icon><href>http://maps.google.com/mapfiles/kml/pal3/icon63.png</href></Icon>");
                            writer.WriteLine("\t\t\t\t<color>ffff0000</color>");
                            writer.WriteLine("\t\t\t\t<scale>.3</scale>");
                            writer.WriteLine("\t\t\t</IconStyle>");
                            writer.WriteLine("\t\t</Style>");
                            writer.WriteLine("\t\t<Style id=\"cyan\">");
                            writer.WriteLine("\t\t\t<IconStyle>");
                            writer.WriteLine("\t\t\t\t<Icon><href>http://maps.google.com/mapfiles/kml/pal3/icon63.png</href></Icon>");
                            writer.WriteLine("\t\t\t\t<color>ffff41b0</color>");
                            writer.WriteLine("\t\t\t\t<scale>.3</scale>");
                            writer.WriteLine("\t\t\t</IconStyle>");
                            writer.WriteLine("\t\t</Style>");
                            bool flag4 = false;
                            double num21 = 0.0;
                            while ((str2 = reader.ReadLine()) != null)
                            {
                                num2 += str2.Length;
                                num21++;
                                int percent = (int) ((((double) num2) / length) * 100.0);
                                if (percent > 100)
                                {
                                    percent = 100;
                                }
								this.progressBar1.BeginInvoke((MethodInvoker)delegate
								{
                                    this.progressBar1.Value = percent;
                                });
                                if (str2.StartsWith("41,"))
                                {
                                    Hashtable hashtable = manager2.comm.getSatellitesDataForGUIFromCSV(0x29, 0, "SSB", str2);
                                    if (hashtable != null)
                                    {
                                        double num22 = 0.0;
                                        int num23 = 0;
                                        double num24 = 0.0;
                                        double num25 = 0.0;
                                        double num26 = 0.0;
                                        double num27 = 0.0;
                                        double num28 = 0.0;
                                        double num29 = 0.0;
                                        double num30 = 0.0;
                                        int num31 = 0;
                                        uint num32 = 0;
                                        int num33 = 0;
                                        int num34 = 0;
                                        int num35 = 0;
                                        int num36 = 0;
                                        int num37 = 0;
                                        int num38 = 0;
                                        double num39 = 0.0;
                                        try
                                        {
                                            if (hashtable.ContainsKey("NAV Type"))
                                            {
                                                num23 = Convert.ToUInt16((string) hashtable["NAV Type"]);
                                            }
                                            if (num23 > 0)
                                            {
                                                if (hashtable.ContainsKey("TOW"))
                                                {
                                                    num22 = Convert.ToDouble((string) hashtable["TOW"]) / 1000.0;
                                                }
                                                if (hashtable.ContainsKey("Latitude"))
                                                {
                                                    num24 = Convert.ToDouble((string) hashtable["Latitude"]) / 10000000.0;
                                                }
                                                if (hashtable.ContainsKey("Longitude"))
                                                {
                                                    num25 = Convert.ToDouble((string) hashtable["Longitude"]) / 10000000.0;
                                                }
                                                if (hashtable.ContainsKey("Altitude from Ellipsoid"))
                                                {
                                                    double num1 = Convert.ToDouble((string) hashtable["Altitude from Ellipsoid"]) / 100.0;
                                                }
                                                if (hashtable.ContainsKey("Altitude from MSL"))
                                                {
                                                    num26 = Convert.ToDouble((string) hashtable["Altitude from MSL"]) / 100.0;
                                                }
                                                if (hashtable.ContainsKey("UTC Hour"))
                                                {
                                                    num37 = Convert.ToInt32((string) hashtable["UTC Hour"]);
                                                }
                                                if (hashtable.ContainsKey("UTC Minute"))
                                                {
                                                    num38 = Convert.ToInt32((string) hashtable["UTC Minute"]);
                                                }
                                                if (hashtable.ContainsKey("UTC Second"))
                                                {
                                                    num39 = Convert.ToDouble((string) hashtable["UTC Second"]) / 1000.0;
                                                }
                                                if (hashtable.ContainsKey("UTC Year"))
                                                {
                                                    num34 = Convert.ToInt32((string) hashtable["UTC Year"]);
                                                }
                                                if (hashtable.ContainsKey("UTC Month"))
                                                {
                                                    num36 = Convert.ToInt32((string) hashtable["UTC Month"]);
                                                }
                                                if (hashtable.ContainsKey("UTC Day"))
                                                {
                                                    num35 = Convert.ToInt32((string) hashtable["UTC Day"]);
                                                }
                                                if (hashtable.ContainsKey("HDOP"))
                                                {
                                                    num30 = Convert.ToDouble((string) hashtable["HDOP"]) / 5.0;
                                                }
                                                if (hashtable.ContainsKey("Speed Over Ground (SOG)"))
                                                {
                                                    num27 = Convert.ToDouble((string) hashtable["Speed Over Ground (SOG)"]) / 100.0;
                                                }
                                                if (hashtable.ContainsKey("Course Over Ground (COG True)"))
                                                {
                                                    num28 = Convert.ToDouble((string) hashtable["Course Over Ground (COG True)"]) / 100.0;
                                                }
                                                if (hashtable.ContainsKey("Extended Week Number"))
                                                {
                                                    num32 = Convert.ToUInt16((string) hashtable["Extended Week Number"]);
                                                }
                                                if (hashtable.ContainsKey("Number of SVs in Fix"))
                                                {
                                                    num33 = Convert.ToInt32((string) hashtable["Number of SVs in Fix"]);
                                                }
                                                if (hashtable.ContainsKey("AdditionalModelInfor"))
                                                {
                                                    num31 = Convert.ToInt32((string) hashtable["AdditionalModelInfor"]);
                                                }
                                                if (hashtable.ContainsKey("Estimated Horizontal Position Error"))
                                                {
                                                    num29 = Convert.ToDouble((string) hashtable["Estimated Horizontal Position Error"]) / 100.0;
                                                }
                                                writer.WriteLine("\t\t<Placemark>");
                                                if (!flag4)
                                                {
                                                    writer.WriteLine("\t\t\t<name>Start</name>");
                                                    flag4 = true;
                                                }
                                                writer.WriteLine("\t\t\t<description>Nav mode = 0x{0}<br/> GPS Week = {1}<br/> TOW = {2:F3} s<br/>UTC = {3}/{4}/{5} {6}:{7}:{8:F3}<br/> SV Cnt = {9}<br/> Lat = {10:F7} deg<br/> Lon = {11:F7} deg<br/> Alt (msl)= {12:F2} m<br/> Sog = {13:F2} m/s<br/> Hdg = {14:F2} deg<br/> Ehpe = {15:F2} m<br/> Hdop = {16:F2}<br/> Additional Mode Info = 0x{17}<br/></description>", new object[] { 
                                                    num23.ToString("X"), num32, num22, num36, num35, num34, num37, num38, num39, num33, num24, num25, num26, num27, num28, num29, 
                                                    num30, num31.ToString("X")
                                                 });
                                                writer.WriteLine("\t\t\t<styleUrl>red</styleUrl>");
                                                writer.WriteLine("\t\t\t<Point><coordinates>{0:F7},{1:F7},0</coordinates></Point>", num25, num24);
                                                writer.WriteLine("\t\t</Placemark>");
                                            }
                                            continue;
                                        }
                                        catch
                                        {
                                            writer.WriteLine("Error encounters! {0}", num21);
                                            continue;
                                        }
                                    }
                                }
                            }
                            manager2 = null;
                            writer.WriteLine("\t</Document>");
                            writer.WriteLine("</kml>");
                            goto Label_1731;
                        }
                        case ConversionType.NMEAToGPS:
                        {
                            if (handler9 == null)
                            {
                                handler9 = delegate {
                                    this.fileConversionStatusLabel.Text = "Status: converting...";
                                };
                            }
                            this.fileConversionStatusLabel.BeginInvoke(handler9);
                            string str16 = "0";
                            string str17 = string.Empty;
                            string str18 = string.Empty;
                            string str19 = string.Empty;
                            string nS = string.Empty;
                            string eW = string.Empty;
                            string str22 = string.Empty;
                            string str23 = string.Empty;
                            double num13 = 0.0;
                            double num14 = 0.0;
                            int fixPosIndicator = 0;
                            int lat = 0;
                            int lon = 0;
                            int num18 = 0;
                            int gpsWeek = 0;
                            double gpsTOW = 0.0;
                            bool flag = false;
                            bool flag2 = false;
                            bool flag3 = false;
                            while ((str2 = reader.ReadLine()) != null)
                            {
                                num2 += str2.Length;
                                int percent = (int) ((((double) num2) / length) * 100.0);
                                if (percent > 100)
                                {
                                    percent = 100;
                                }
								this.progressBar1.BeginInvoke((MethodInvoker)delegate
								{
                                    this.progressBar1.Value = percent;
                                });
                                if (str2.Contains("GPRMC"))
                                {
                                    string str24 = string.Empty;
                                    try
                                    {
                                        if (flag && flag2)
                                        {
                                            if (flag3)
                                            {
                                                str24 = string.Format("41,0,{0},{1},{2},{3},{4},{5},{6},{7},{8},0,{9},{10},0,0,0,{11:F2},{12:F2},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0", new object[] { num18, gpsWeek, gpsTOW * 1000.0, str19.Substring(4, 2), str19.Substring(2, 2), str19.Substring(0, 2), str16.Substring(0, 2), str16.Substring(2, 2), str16.Substring(4, 2), lat, lon, num14, num13 });
                                            }
                                            else
                                            {
                                                str24 = string.Format("41,0,{0},{1},{2},{3},{4},{5},{6},{7},{8},0,{9},{10},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0", new object[] { num18, gpsWeek, gpsTOW * 1000.0, str19.Substring(4, 2), str19.Substring(2, 2), str19.Substring(0, 2), str16.Substring(0, 2), str16.Substring(2, 2), str16.Substring(4, 2), lat, lon });
                                            }
                                        }
                                        writer.WriteLine(str24);
                                        flag = false;
                                        flag2 = false;
                                        flag3 = false;
                                    }
                                    catch (Exception exception)
                                    {
                                        MessageBox.Show("Exception in file conversion NMEA to SB: " + exception.ToString());
                                    }
                                    string[] strArray7 = str2.Split(new char[] { ',' });
                                    str16 = strArray7[1];
                                    str17 = strArray7[3];
                                    nS = strArray7[4];
                                    str18 = strArray7[5];
                                    eW = strArray7[6];
                                    str19 = strArray7[9];
                                    try
                                    {
                                        this.ConvertRMCDateToWeekAndSeconds(str16, str19, ref gpsWeek, ref gpsTOW);
                                        this.ConvertRMCLatLontoSBmsg41LatLon(str17, nS, str18, eW, ref lat, ref lon);
                                    }
                                    catch (Exception exception2)
                                    {
                                        MessageBox.Show("Exception in file conversion NMEA to SB: " + exception2.ToString());
                                    }
                                    flag = true;
                                }
                                else if (str2.Contains("GPGGA"))
                                {
                                    string[] strArray8 = str2.Split(new char[] { ',' });
                                    string text1 = strArray8[1];
                                    try
                                    {
                                        fixPosIndicator = Convert.ToInt32(strArray8[6]);
                                        num18 = this.ConvertGGAPosFixIndiToSBmsg41NavType(fixPosIndicator);
                                    }
                                    catch (Exception exception3)
                                    {
                                        MessageBox.Show("Exception in file conversion NMEA to SB: " + exception3.ToString());
                                    }
                                    flag2 = true;
                                }
                                else if (str2.Contains("GPVTG"))
                                {
                                    string[] strArray9 = str2.Split(new char[] { ',' });
                                    str22 = strArray9[1];
                                    str23 = strArray9[7];
                                    try
                                    {
                                        num13 = Convert.ToDouble(str22);
                                        num14 = Convert.ToDouble(str23);
                                    }
                                    catch (Exception exception4)
                                    {
                                        MessageBox.Show("Exception in file conversion NMEA to SB: " + exception4.ToString());
                                    }
                                    num13 *= 100.0;
                                    num14 = (num14 * 1000.0) / 36.0;
                                    flag3 = true;
                                }
                                else
                                {
                                    writer.WriteLine(str2);
                                }
                                if (this._abort)
                                {
                                    break;
                                }
                            }
                            goto Label_1731;
                        }
                        default:
                            goto Label_1731;
                    }
                    reader.Close();
                    FileStream stream = File.OpenRead(str);
                    writer2 = new StreamWriter(str4);
                    if (handler5 == null)
                    {
                        handler5 = delegate {
                            this.fileConversionStatusLabel.Text = "Status: converting...";
                        };
                    }
                    this.fileConversionStatusLabel.BeginInvoke(handler5);
                    int offset = 0;
                    int count = 0x1000;
                    byte[] buffer = new byte[count];
                    int len = stream.Read(buffer, offset, count);
                    manager.SetupRxCtrl();
                    while (len > 0)
                    {
                        manager.PopulateData(buffer, len);
                        string str5 = manager.ByteToMsgQueue(new byte[1]);
                        string[] separator = new string[] { "\r\n" };
                        foreach (string str6 in str5.Split(separator, StringSplitOptions.None))
                        {
                            string str7 = manager.m_Protocols.ConvertRawToFields(HelperFunctions.HexToByte(str6));
                            writer.WriteLine(str7);
                            writer2.WriteLine(CommonUtilsClass.LogToGP2(str6, string.Empty));
                        }
                        offset += len;
                        int percent = (int) ((((double) offset) / length) * 100.0);
                        if (percent > 100)
                        {
                            percent = 100;
                        }
						this.progressBar1.BeginInvoke((MethodInvoker)delegate
						{
                            this.progressBar1.Value = percent;
                        });
                        len = stream.Read(buffer, 0, count);
                        if (this._abort)
                        {
                            break;
                        }
                    }
                    stream.Close();
                    writer2.Close();
                Label_1731:
                    if (this._abort)
                    {
                        break;
                    }
                    reader.Close();
                    writer.Close();
                    if (handler != null)
                    {
                        handler.Close();
                    }
                    this._convertedFilesCount++;
                    if (handler11 == null)
                    {
                        handler11 = delegate {
                            this.convertedFilesLabel.Text = string.Format("Files converted: {0}", this._convertedFilesCount);
                        };
                    }
                    this.convertedFilesLabel.BeginInvoke(handler11);
                }
                if (this._abort)
                {
                    MessageBox.Show("Conversion Aborted", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (writer != null)
                    {
                        writer.Close();
                    }
                    if (writer2 != null)
                    {
                        writer2.Close();
                    }
                    if (handler != null)
                    {
                        handler.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Conversion Done", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                this._conversionStatus = false;
            }
            catch (Exception exception5)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (writer2 != null)
                {
                    writer2.Close();
                }
                if (handler != null)
                {
                    handler.Close();
                }
                this._conversionStatus = false;
                this._abort = false;
                MessageBox.Show("Error: " + exception5.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            try
            {
                if (handler12 == null)
                {
                    handler12 = delegate {
                        this.fileConversionStatusLabel.Text = "Status: idle";
                    };
                }
                this.fileConversionStatusLabel.BeginInvoke(handler12);
            }
            catch
            {
            }
            manager.Dispose();
            manager = null;
        }

        private int ConvertGGAPosFixIndiToSBmsg41NavType(int fixPosIndicator)
        {
            switch (fixPosIndicator)
            {
                case 0:
                    return 0;

                case 1:
                    return 6;

                case 2:
                    return 6;

                case 3:
                    return 6;

                case 4:
                    return 6;

                case 5:
                    return 6;

                case 6:
                    return 7;

                case 7:
                    return 6;

                case 8:
                    return 6;
            }
            return 0;
        }

        private void ConvertRMCDateToWeekAndSeconds(string RMCStr_utcTime, string RMCStr_Date, ref int gpsWeek, ref double gpsTOW)
        {
            int num = 14;
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            try
            {
                hour = Convert.ToInt32(RMCStr_utcTime.Substring(0, 2));
                minute = Convert.ToInt32(RMCStr_utcTime.Substring(2, 2));
                second = Convert.ToInt32(RMCStr_utcTime.Substring(4, 2));
                day = Convert.ToInt32(RMCStr_Date.Substring(0, 2));
                month = Convert.ToInt32(RMCStr_Date.Substring(2, 2));
                year = 0x7d0 + Convert.ToInt32(RMCStr_Date.Substring(4, 2));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in file conversion NMEA to SB: " + exception.ToString());
            }
            DateTime time = new DateTime(year, month, day, hour, minute, second);
            DateTime time2 = new DateTime(0x7bc, 1, 6);
            TimeSpan span = (TimeSpan) (time - time2);
            gpsWeek = span.Days / 7;
            gpsTOW = (span.TotalSeconds - ((double) ((gpsWeek * 7) * 0x15180))) + num;
        }

        private void ConvertRMCLatLontoSBmsg41LatLon(string RMC_lat, string NS, string RMC_lon, string EW, ref int lat, ref int lon)
        {
            int length = RMC_lat.Length;
            int num2 = RMC_lon.Length;
            int num3 = 0;
            int num4 = 0;
            double num5 = 0.0;
            double num6 = 0.0;
            try
            {
                num3 = Convert.ToInt32(RMC_lat.Substring(0, length - 6));
                num4 = Convert.ToInt32(RMC_lon.Substring(0, num2 - 6));
                num5 = Convert.ToDouble(RMC_lat.Substring(length - 6));
                num6 = Convert.ToDouble(RMC_lon.Substring(num2 - 6));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in file conversion NMEA to SB: " + exception.ToString());
            }
            lat = (int) ((num3 + (num5 / 60.0)) * 10000000.0);
            lon = (int) ((num4 + (num6 / 60.0)) * 10000000.0);
            if (NS == "S")
            {
                lat = -lat;
            }
            if (EW == "W")
            {
                lon = -lon;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fileConversionAddAllBtn_Click(object sender, EventArgs e)
        {
            this.fileConversionToConvertFilesListVal.Items.Clear();
            this.toRunList.Clear();
            foreach (string str in this.fileConversionAvailableFilesListVal.Items)
            {
                this.fileConversionToConvertFilesListVal.Items.Add(str);
            }
            foreach (string str2 in this.filesFullPathLists)
            {
                this.toRunList.Add(str2);
            }
            this.updateFiles2ConvertCnt();
        }

        private void fileConversionAddBtn_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.fileConversionAvailableFilesListVal.SelectedIndex;
            if (selectedIndex >= 0)
            {
                this.fileConversionToConvertFilesListVal.Items.Add(this.fileConversionAvailableFilesListVal.SelectedItem);
                this.toRunList.Add(this.filesArray[selectedIndex]);
                this.updateFiles2ConvertCnt();
            }
        }

        private void fileConversionAvailableFilesListVal_DoubleClick(object sender, EventArgs e)
        {
            int selectedIndex = this.fileConversionAvailableFilesListVal.SelectedIndex;
            if (selectedIndex >= 0)
            {
                this.fileConversionToConvertFilesListVal.Items.Add(this.fileConversionAvailableFilesListVal.SelectedItem);
                this.toRunList.Add(this.filesArray[selectedIndex]);
                this.updateFiles2ConvertCnt();
            }
        }

        private void fileConversionAvailableFilesListVal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.filesFullPathLists.Count != 0)
            {
                string text = string.Empty;
                foreach (string str2 in this.filesFullPathLists)
                {
                    text = text + str2 + "\n";
                }
                MessageBox.Show(text, "Information");
            }
        }

        private void fileConversionClearAvailableListBtn_Click(object sender, EventArgs e)
        {
            this.fileConversionAvailableFilesListVal.Items.Clear();
            this.filesFullPathLists.Clear();
        }

        private void fileConversionClosing()
        {
            this._abort = true;
            this._conversionStatus = false;
            m_SChildform = null;
            this.saveNExit();
        }

        private void fileConversionDirBrowser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = ConfigurationManager.AppSettings["InstalledDirectory"] + @"\scripts";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.autoTestDirVal.Text = dialog.SelectedPath;
                foreach (string str in Directory.GetFiles(dialog.SelectedPath))
                {
                    if (this._fileType == ".gpx")
                    {
                        if (str.EndsWith(this._fileType) || str.EndsWith(".gp2"))
                        {
                            this.addAvailableFiles(str);
                        }
                    }
                    else if (str.EndsWith(this._fileType))
                    {
                        this.addAvailableFiles(str);
                    }
                }
            }
            this.filesArray = this.filesFullPathLists.ToArray();
        }

        private void fileConversionExitBtn_Click(object sender, EventArgs e)
        {
            this._abort = false;
            this._conversionStatus = false;
            if (this.updateParent != null)
            {
                this.updateParent();
            }
            base.Close();
        }

        private void fileConversionRefreshBtn_Click(object sender, EventArgs e)
        {
            this.updateAvailableFiles();
        }

        private void fileConversionRemoveAllBtn_Click(object sender, EventArgs e)
        {
            this.fileConversionToConvertFilesListVal.Items.Clear();
            this.toRunList.Clear();
            fileIdx = 0;
            this.updateFiles2ConvertCnt();
        }

        private void fileConversionRemoveBtn_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.fileConversionToConvertFilesListVal.SelectedIndex;
            string[] strArray = this.toRunList.ToArray();
            if (selectedIndex >= 0)
            {
                this.fileConversionToConvertFilesListVal.Items.Remove(this.fileConversionToConvertFilesListVal.SelectedItem);
                this.toRunList.Remove(strArray[selectedIndex]);
                if ((fileIdx > 0) && (fileIdx > selectedIndex))
                {
                    fileIdx--;
                }
                this.updateFiles2ConvertCnt();
            }
        }

        private void fileConversionRunBtn_Click(object sender, EventArgs e)
        {
            this._abort = false;
            if (!this._conversionStatus)
            {
                this._conversionStatus = false;
                this._convertedFilesCount = 0;
                this.convertedFilesLabel.Text = string.Format("Files converted: 0", new object[0]);
                this.progressBar1.Value = 0;
                Thread.Sleep(100);
                try
                {
                    this._conversionStatus = true;
                    Thread thread = new Thread(new ThreadStart(this.convertFiles));
                    this.conversionThreadID = thread.ManagedThreadId;
                    thread.Start();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void fileConversionToConvertFilesListVal_DoubleClick(object sender, EventArgs e)
        {
            int selectedIndex = this.fileConversionToConvertFilesListVal.SelectedIndex;
            string[] strArray = this.toRunList.ToArray();
            if (selectedIndex >= 0)
            {
                this.fileConversionToConvertFilesListVal.Items.Remove(this.fileConversionToConvertFilesListVal.SelectedItem);
                this.toRunList.Remove(strArray[selectedIndex]);
                if ((fileIdx > 0) && (fileIdx > selectedIndex))
                {
                    fileIdx--;
                }
                this.updateFiles2ConvertCnt();
            }
        }

        private void fileConvserionAbortBtn_Click(object sender, EventArgs e)
        {
            this.conversionAbort();
        }

        private string Float2AsciiPad(float FValue, int left, int right)
        {
            char[] chArray = new char[20];
            int index = 0x13;
            int num4 = index - right;
            int num5 = num4 - left;
            StringBuilder builder = new StringBuilder();
            long num = (long) (FValue * this.Power10[right + 1]);
            if (num < 0L)
            {
                builder.Append('-');
                num = -num;
                num5++;
            }
            chArray[num4] = '.';
            num /= 10L;
            while (index >= num5)
            {
                if (index != num4)
                {
                    if (num != 0L)
                    {
                        long num2 = num / 10L;
                        char ch = (char) ((ushort) (num - (10L * num2)));
                        chArray[index] = (char) ('0' + ch);
                        num = num2;
                    }
                    else
                    {
                        chArray[index] = '0';
                    }
                }
                index--;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                builder.Append(chArray[i]);
            }
            return builder.ToString().Replace("\0", "");
        }

        private void frmFileConversion_Load(object sender, EventArgs e)
        {
            this.includeDateTimeChkBox.Enabled = false;
            this.includeDateTimeChkBox.Visible = false;
            this.useOspMsg69RadioBtn.Enabled = false;
            this.useOspMsg69RadioBtn.Visible = false;
            this.useSSBMsg41RadioBtn.Enabled = false;
            this.useSSBMsg41RadioBtn.Visible = false;
            this.autoDetectRadioBtn.Visible = false;
            this.autoDetectRadioBtn.Enabled = false;
            this.autoDetectRadioBtn.Checked = false;
            switch (this._conversionType)
            {
                case ConversionType.GP2ToGPS:
                    this.Text = "GP2 To GPS";
                    this.includeDateTimeChkBox.Enabled = true;
                    this.includeDateTimeChkBox.Visible = true;
                    return;

                case ConversionType.BinToGPS_GP2:
                    this.Text = "Bin To GP2/GPS";
                    return;

                case ConversionType.GPSToNMEA:
                    this.Text = "GPS To NMEA";
                    this.useOspMsg69RadioBtn.Enabled = true;
                    this.useOspMsg69RadioBtn.Visible = true;
                    this.useSSBMsg41RadioBtn.Enabled = true;
                    this.useSSBMsg41RadioBtn.Visible = true;
                    this.autoDetectRadioBtn.Visible = true;
                    this.autoDetectRadioBtn.Enabled = true;
                    this.autoDetectRadioBtn.Checked = true;
                    return;

                case ConversionType.GPSToKML:
                    this.Text = "GPS To KML";
                    return;

                case ConversionType.NMEAToGPS:
                    this.Text = "NMEA To GPS";
                    return;
            }
            this.Text = "General File Conversion";
            this.includeDateTimeChkBox.Enabled = true;
            this.includeDateTimeChkBox.Visible = true;
            this.useOspMsg69RadioBtn.Enabled = true;
            this.useOspMsg69RadioBtn.Visible = true;
            this.useSSBMsg41RadioBtn.Enabled = true;
            this.useSSBMsg41RadioBtn.Visible = true;
            this.autoDetectRadioBtn.Visible = true;
        }

        public static frmFileConversion GetChildInstance(ConversionType fileType)
        {
            if (m_SChildform == null)
            {
                m_SChildform = new frmFileConversion(fileType);
            }
            return m_SChildform;
        }

        private string GPS_NMEA_OutputGGA(string[] msgArray)
        {
            if (msgArray.Length <= 0)
            {
                return string.Empty;
            }
            double num = 0.0;
            short num2 = 0;
            double num3 = 0.0;
            double num4 = 0.0;
            short num5 = 0;
            double num6 = 0.0;
            int num7 = 0;
            string str = "";
            string str2 = "";
            try
            {
                int num8 = Convert.ToInt32(msgArray[2]);
                if ((num8 & 7) == 0)
                {
                    num7 = 0;
                }
                else if ((num8 & 7) == 7)
                {
                    num7 = 6;
                }
                else if ((num8 & 0x80) == 0x80)
                {
                    num7 = 2;
                }
                else
                {
                    num7 = 1;
                }
                num = Convert.ToDouble(msgArray[12]) * 1E-07;
                num4 = Convert.ToDouble(msgArray[13]) * 1E-07;
                num2 = (short) num;
                num3 = Math.Abs((double) ((num - num2) * 60.0));
                num5 = (short) num4;
                num6 = Math.Abs((double) ((num4 - num5) * 60.0));
                str = this.Float2AsciiPad((float) num3, 2, 4);
                str2 = this.Float2AsciiPad((float) num6, 2, 4);
                string inputString = string.Format("$GPGGA,{0:00}{1:00}{2:00.000},{3:00}{4},{5},{6:000}{7},{8},{9},{10},{11:F1},{12:F1},M,{13:F1},,,", new object[] { Convert.ToByte(msgArray[8]), Convert.ToByte(msgArray[9]), Convert.ToDouble(msgArray[10]) * 0.001, Math.Abs(num2), str.ToString(), (num >= 0.0) ? 'N' : 'S', Math.Abs(num5), str2.ToString(), (num4 < 0.0) ? 'W' : 'E', num7, Convert.ToByte(msgArray[0x21]).ToString().PadLeft(2, '0'), Convert.ToDouble(msgArray[0x22]) * 0.2, Convert.ToDouble(msgArray[15]) * 0.01, (Convert.ToDouble(msgArray[14]) - Convert.ToDouble(msgArray[15])) / 100.0 });
                return this.NMEA_AddCheckSum(inputString);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GPS_NMEA_OutputGLL(string[] msgArray)
        {
            double num = 0.0;
            short num2 = 0;
            double num3 = 0.0;
            double num4 = 0.0;
            short num5 = 0;
            double num6 = 0.0;
            string str = "";
            string str2 = "";
            try
            {
                char ch;
                char ch2;
                int num7 = Convert.ToInt32(msgArray[2]);
                if ((num7 & 7) == 0)
                {
                    ch = 'N';
                    ch2 = 'V';
                }
                else if ((num7 & 7) == 7)
                {
                    ch = 'E';
                    ch2 = 'A';
                }
                else if ((num7 & 0x80) == 0x80)
                {
                    ch = 'D';
                    ch2 = 'A';
                }
                else
                {
                    ch = 'A';
                    ch2 = 'A';
                }
                num = Convert.ToDouble(msgArray[12]) * 1E-07;
                num4 = Convert.ToDouble(msgArray[13]) * 1E-07;
                num2 = (short) num;
                num3 = Math.Abs((double) ((num - num2) * 60.0));
                num5 = (short) num4;
                num6 = Math.Abs((double) ((num4 - num5) * 60.0));
                str = this.Float2AsciiPad((float) num3, 2, 4);
                str2 = this.Float2AsciiPad((float) num6, 2, 4);
                string inputString = string.Format("$GPGLL,{0:00}{1},{2},{3:000}{4},{5},{6:00}{7:00}{8:00.000},{9},{10}", new object[] { Math.Abs(num2), str.ToString(), (num >= 0.0) ? 'N' : 'S', Math.Abs(num5), str2.ToString(), (num4 < 0.0) ? 'W' : 'E', Convert.ToByte(msgArray[8]), Convert.ToByte(msgArray[9]), Convert.ToDouble(msgArray[10]) * 0.001, ch2, ch });
                return this.NMEA_AddCheckSum(inputString);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GPS_NMEA_OutputGSA(string[] msgArray)
        {
            try
            {
                int num;
                int num2;
                switch ((Convert.ToInt32(msgArray[2]) & 7))
                {
                    case 4:
                    case 6:
                        num2 = 3;
                        break;

                    case 0:
                        num2 = 1;
                        break;

                    default:
                        num2 = 2;
                        break;
                }
                uint num5 = Convert.ToUInt32(msgArray[11]);
                StringBuilder builder = new StringBuilder();
                for (num = 0; num < 0x20; num++)
                {
                    uint num6 = ((uint) 1) << num;
                    if ((num5 & num6) == num6)
                    {
                        builder.Append(string.Format("{0:00},", num + 1));
                    }
                }
                int num7 = Convert.ToInt32(msgArray[0x21]);
                for (num = 0; num < (12 - num7); num++)
                {
                    builder.Append(",");
                }
                string inputString = string.Format("$GPGSA,A,{0:0},{1},{2:F1},", num2, builder.ToString(), Convert.ToDouble(msgArray[0x22]) * 0.2);
                return this.NMEA_AddCheckSum(inputString);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GPS_NMEA_OutputGSV(string[] msgArray)
        {
            double num7 = 0.0;
            int num2 = 0;
            try
            {
                int num8 = Convert.ToInt32(msgArray[3]);
                int index = 4;
                int num4 = 0;
                for (index = 4; num4 < num8; index += 14)
                {
                    if (Convert.ToInt32(msgArray[index]) != 0)
                    {
                        num2++;
                    }
                    num4++;
                }
                int num = (num2 + 3) / 4;
                num4 = 0;
                index = 4;
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                for (int i = 1; i <= num; i++)
                {
                    builder2.Append(string.Format("$GPGSV,{0:D},{1:D},{2:D2}", num, i, num2));
                    int num6 = 0;
                    while ((num6 < 4) && (num4 < 12))
                    {
                        if (num4 < 12)
                        {
                            num7 = 0.0;
                            int num10 = 0;
                            uint num11 = Convert.ToUInt16(msgArray[index + 3]);
                            bool flag = false;
                            if ((num11 & 1) == 1)
                            {
                                flag = true;
                                for (int j = 0; j < 10; j++)
                                {
                                    int num12 = Convert.ToInt32(msgArray[(index + 4) + j]);
                                    if (num12 > 0)
                                    {
                                        num10++;
                                        num7 += num12;
                                    }
                                }
                                num7 /= (num10 > 0) ? ((double) num10) : 1.0;
                            }
                            int num13 = Convert.ToInt32(msgArray[index]);
                            double num14 = Convert.ToDouble(msgArray[index + 2]);
                            double num15 = Convert.ToDouble(msgArray[index + 1]);
                            if (num13 != 0)
                            {
                                builder2.Append(string.Format(",{0:00},{1:00},{2:000},{3}", new object[] { (num13 >= 120) ? (num13 - 0x57) : num13, num14, num15, flag ? num7.ToString("00") : string.Empty }));
                                num6++;
                            }
                            num4++;
                            index += 14;
                        }
                    }
                    builder.Append(this.NMEA_AddCheckSum(builder2.ToString()));
                    builder2.Remove(0, builder2.Length);
                }
                return builder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GPS_NMEA_OutputRMC(string[] msgArray)
        {
            double num = 0.0;
            short num2 = 0;
            double num3 = 0.0;
            double num4 = 0.0;
            short num5 = 0;
            double num6 = 0.0;
            string str = "";
            string str2 = "";
            try
            {
                char ch;
                char ch2;
                int num8 = Convert.ToInt32(msgArray[2]);
                if ((num8 & 7) == 0)
                {
                    ch = 'N';
                    ch2 = 'V';
                }
                else if ((num8 & 7) == 7)
                {
                    ch = 'E';
                    ch2 = 'A';
                }
                else if ((num8 & 0x80) == 0x80)
                {
                    ch = 'D';
                    ch2 = 'A';
                }
                else
                {
                    ch = 'A';
                    ch2 = 'A';
                }
                num = Convert.ToDouble(msgArray[12]) * 1E-07;
                num4 = Convert.ToDouble(msgArray[13]) * 1E-07;
                num2 = (short) num;
                num3 = Math.Abs((double) ((num - num2) * 60.0));
                num5 = (short) num4;
                num6 = Math.Abs((double) ((num4 - num5) * 60.0));
                str = this.Float2AsciiPad((float) num3, 2, 4);
                str2 = this.Float2AsciiPad((float) num6, 2, 4);
                double num7 = ((Convert.ToDouble(msgArray[0x11]) * 0.01) * 3600.0) / 1851.9648000000002;
                long result = 0L;
                Math.DivRem(Convert.ToInt64(msgArray[5]), 100L, out result);
                string inputString = string.Format("$GPRMC,{0:00}{1:00}{2:00.000},{3},{4:D2}{5},{6},{7:D3}{8},{9},{10:F1},{11:F1},{12:00}{13:00}{14:00},,,{15}", new object[] { Convert.ToByte(msgArray[8]), Convert.ToByte(msgArray[9]), Convert.ToDouble(msgArray[10]) * 0.001, ch2, Math.Abs(num2), str.ToString(), (num >= 0.0) ? 'N' : 'S', Math.Abs(num5), str2.ToString(), (num4 < 0.0) ? 'W' : 'E', num7, Convert.ToDouble(msgArray[0x12]) * 0.01, Convert.ToByte(msgArray[7]), Convert.ToByte(msgArray[6]), Convert.ToByte(result), ch });
                return this.NMEA_AddCheckSum(inputString);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GPS_NMEA_OutputVTG(string[] msgArray)
        {
            double num = 0.0;
            double num2 = 0.0;
            try
            {
                char ch;
                int num3 = Convert.ToInt32(msgArray[2]);
                if ((num3 & 7) == 0)
                {
                    ch = 'N';
                }
                else if ((num3 & 7) == 7)
                {
                    ch = 'E';
                }
                else if ((num3 & 0x80) == 0x80)
                {
                    ch = 'D';
                }
                else
                {
                    ch = 'A';
                }
                double num4 = Convert.ToDouble(msgArray[0x11]);
                num = ((num4 * 0.01) * 3600.0) / 1851.9648000000002;
                num2 = ((num4 * 0.01) * 3600.0) / 1000.0;
                string inputString = string.Format("$GPVTG,{0:F1},T,,M,{1:F1},N,{2:F1},K,{3}", new object[] { Convert.ToDouble(msgArray[0x12]) * 0.01, num, num2, ch });
                return this.NMEA_AddCheckSum(inputString);
            }
            catch
            {
                return string.Empty;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmFileConversion));
            this.frmFileConversionFilesCntLabel = new Label();
            this.autoTestRefreshBtn = new Button();
            this.autoTestClearAvailableListBtn = new Button();
            this.automationTestAbortBtn = new Button();
            this.autoTestRunBtn = new Button();
            this.autoTestRemoveAllBtn = new Button();
            this.autoTestRemoveBtn = new Button();
            this.autoTestAddBtn = new Button();
            this.autoTestAddAllBtn = new Button();
            this.autoTestExitBtn = new Button();
            this.fileConversionAvailableFilesListVal = new ListBox();
            this.fileConversionToConvertFilesListVal = new ListBox();
            this.autoTestDirBrowser = new Button();
            this.autoTestRunScriptsLabel = new Label();
            this.autoTestAvailableScriptsLabel = new Label();
            this.autoTestFilePathLabel = new Label();
            this.autoTestDirVal = new TextBox();
            this.includeDateTimeChkBox = new CheckBox();
            this.convertedFilesLabel = new Label();
            this.progressBar1 = new ProgressBar();
            this.useSSBMsg41RadioBtn = new RadioButton();
            this.useOspMsg69RadioBtn = new RadioButton();
            this.fileConversionStatusLabel = new Label();
            this.autoDetectRadioBtn = new RadioButton();
            base.SuspendLayout();
            this.frmFileConversionFilesCntLabel.AutoSize = true;
            this.frmFileConversionFilesCntLabel.Location = new Point(0x215, 0x54);
            this.frmFileConversionFilesCntLabel.Name = "frmFileConversionFilesCntLabel";
            this.frmFileConversionFilesCntLabel.Size = new Size(13, 13);
            this.frmFileConversionFilesCntLabel.TabIndex = 0x26;
            this.frmFileConversionFilesCntLabel.Text = "0";
            this.autoTestRefreshBtn.Location = new Point(0x91, 0x4f);
            this.autoTestRefreshBtn.Name = "autoTestRefreshBtn";
            this.autoTestRefreshBtn.Size = new Size(0x4b, 0x17);
            this.autoTestRefreshBtn.TabIndex = 0x1c;
            this.autoTestRefreshBtn.Text = "Re&fresh";
            this.autoTestRefreshBtn.UseVisualStyleBackColor = true;
            this.autoTestRefreshBtn.Click += new EventHandler(this.fileConversionRefreshBtn_Click);
            this.autoTestClearAvailableListBtn.Location = new Point(0xec, 0x4f);
            this.autoTestClearAvailableListBtn.Name = "autoTestClearAvailableListBtn";
            this.autoTestClearAvailableListBtn.Size = new Size(0x4b, 0x17);
            this.autoTestClearAvailableListBtn.TabIndex = 0x1d;
            this.autoTestClearAvailableListBtn.Text = "&Clear";
            this.autoTestClearAvailableListBtn.UseVisualStyleBackColor = true;
            this.autoTestClearAvailableListBtn.Click += new EventHandler(this.fileConversionClearAvailableListBtn_Click);
            this.automationTestAbortBtn.Location = new Point(0x155, 0x1db);
            this.automationTestAbortBtn.Name = "automationTestAbortBtn";
            this.automationTestAbortBtn.Size = new Size(0x4b, 0x17);
            this.automationTestAbortBtn.TabIndex = 0x2c;
            this.automationTestAbortBtn.Text = "A&bort";
            this.automationTestAbortBtn.UseVisualStyleBackColor = true;
            this.automationTestAbortBtn.Click += new EventHandler(this.fileConvserionAbortBtn_Click);
            this.autoTestRunBtn.Location = new Point(0xef, 0x1db);
            this.autoTestRunBtn.Name = "autoTestRunBtn";
            this.autoTestRunBtn.Size = new Size(0x4b, 0x17);
            this.autoTestRunBtn.TabIndex = 0x2b;
            this.autoTestRunBtn.Text = "&Start";
            this.autoTestRunBtn.UseVisualStyleBackColor = true;
            this.autoTestRunBtn.Click += new EventHandler(this.fileConversionRunBtn_Click);
            this.autoTestRemoveAllBtn.Location = new Point(0x146, 0x10b);
            this.autoTestRemoveAllBtn.Name = "autoTestRemoveAllBtn";
            this.autoTestRemoveAllBtn.Size = new Size(0x63, 0x17);
            this.autoTestRemoveAllBtn.TabIndex = 0x22;
            this.autoTestRemoveAllBtn.Text = "Remove All <&<";
            this.autoTestRemoveAllBtn.UseVisualStyleBackColor = true;
            this.autoTestRemoveAllBtn.Click += new EventHandler(this.fileConversionRemoveAllBtn_Click);
            this.autoTestRemoveBtn.Location = new Point(0x146, 0xee);
            this.autoTestRemoveBtn.Name = "autoTestRemoveBtn";
            this.autoTestRemoveBtn.Size = new Size(0x63, 0x17);
            this.autoTestRemoveBtn.TabIndex = 0x21;
            this.autoTestRemoveBtn.Text = "&Remove <";
            this.autoTestRemoveBtn.UseVisualStyleBackColor = true;
            this.autoTestRemoveBtn.Click += new EventHandler(this.fileConversionRemoveBtn_Click);
            this.autoTestAddBtn.Location = new Point(0x146, 180);
            this.autoTestAddBtn.Name = "autoTestAddBtn";
            this.autoTestAddBtn.Size = new Size(0x63, 0x17);
            this.autoTestAddBtn.TabIndex = 0x1f;
            this.autoTestAddBtn.Text = "&Add >";
            this.autoTestAddBtn.UseVisualStyleBackColor = true;
            this.autoTestAddBtn.Click += new EventHandler(this.fileConversionAddBtn_Click);
            this.autoTestAddAllBtn.Location = new Point(0x146, 0xd1);
            this.autoTestAddAllBtn.Name = "autoTestAddAllBtn";
            this.autoTestAddAllBtn.Size = new Size(0x63, 0x17);
            this.autoTestAddAllBtn.TabIndex = 0x20;
            this.autoTestAddAllBtn.Text = "Add All >&>";
            this.autoTestAddAllBtn.UseVisualStyleBackColor = true;
            this.autoTestAddAllBtn.Click += new EventHandler(this.fileConversionAddAllBtn_Click);
            this.autoTestExitBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.autoTestExitBtn.Location = new Point(0x1bb, 0x1db);
            this.autoTestExitBtn.Name = "autoTestExitBtn";
            this.autoTestExitBtn.Size = new Size(0x4b, 0x17);
            this.autoTestExitBtn.TabIndex = 0x2d;
            this.autoTestExitBtn.Text = "E&xit";
            this.autoTestExitBtn.UseVisualStyleBackColor = true;
            this.autoTestExitBtn.Click += new EventHandler(this.fileConversionExitBtn_Click);
            this.fileConversionAvailableFilesListVal.FormattingEnabled = true;
            this.fileConversionAvailableFilesListVal.HorizontalScrollbar = true;
            this.fileConversionAvailableFilesListVal.Location = new Point(0x16, 110);
            this.fileConversionAvailableFilesListVal.Name = "fileConversionAvailableFilesListVal";
            this.fileConversionAvailableFilesListVal.ScrollAlwaysVisible = true;
            this.fileConversionAvailableFilesListVal.Size = new Size(290, 0x13c);
            this.fileConversionAvailableFilesListVal.TabIndex = 30;
            this.fileConversionAvailableFilesListVal.DoubleClick += new EventHandler(this.fileConversionAvailableFilesListVal_DoubleClick);
            this.fileConversionAvailableFilesListVal.KeyPress += new KeyPressEventHandler(this.fileConversionAvailableFilesListVal_KeyPress);
            this.fileConversionToConvertFilesListVal.FormattingEnabled = true;
            this.fileConversionToConvertFilesListVal.HorizontalScrollbar = true;
            this.fileConversionToConvertFilesListVal.Location = new Point(440, 110);
            this.fileConversionToConvertFilesListVal.Name = "fileConversionToConvertFilesListVal";
            this.fileConversionToConvertFilesListVal.ScrollAlwaysVisible = true;
            this.fileConversionToConvertFilesListVal.Size = new Size(0x127, 0x13c);
            this.fileConversionToConvertFilesListVal.TabIndex = 0x27;
            this.fileConversionToConvertFilesListVal.DoubleClick += new EventHandler(this.fileConversionToConvertFilesListVal_DoubleClick);
            this.autoTestDirBrowser.Location = new Point(0x2c5, 0x1f);
            this.autoTestDirBrowser.Name = "autoTestDirBrowser";
            this.autoTestDirBrowser.Size = new Size(0x1a, 0x17);
            this.autoTestDirBrowser.TabIndex = 0x1a;
            this.autoTestDirBrowser.Text = "...";
            this.autoTestDirBrowser.UseVisualStyleBackColor = true;
            this.autoTestDirBrowser.Click += new EventHandler(this.fileConversionDirBrowser_Click);
            this.autoTestRunScriptsLabel.AutoSize = true;
            this.autoTestRunScriptsLabel.Location = new Point(0x1b5, 0x54);
            this.autoTestRunScriptsLabel.Name = "autoTestRunScriptsLabel";
            this.autoTestRunScriptsLabel.Size = new Size(0x57, 13);
            this.autoTestRunScriptsLabel.TabIndex = 0x25;
            this.autoTestRunScriptsLabel.Text = "Files to Convert:";
            this.autoTestAvailableScriptsLabel.AutoSize = true;
            this.autoTestAvailableScriptsLabel.Location = new Point(0x13, 0x54);
            this.autoTestAvailableScriptsLabel.Name = "autoTestAvailableScriptsLabel";
            this.autoTestAvailableScriptsLabel.Size = new Size(0x4a, 13);
            this.autoTestAvailableScriptsLabel.TabIndex = 0x1b;
            this.autoTestAvailableScriptsLabel.Text = "Available Files";
            this.autoTestFilePathLabel.AutoSize = true;
            this.autoTestFilePathLabel.Location = new Point(0x13, 9);
            this.autoTestFilePathLabel.Name = "autoTestFilePathLabel";
            this.autoTestFilePathLabel.Size = new Size(0x44, 13);
            this.autoTestFilePathLabel.TabIndex = 0x18;
            this.autoTestFilePathLabel.Text = "File Directory";
            this.autoTestDirVal.Location = new Point(0x16, 0x1f);
            this.autoTestDirVal.Name = "autoTestDirVal";
            this.autoTestDirVal.Size = new Size(0x298, 20);
            this.autoTestDirVal.TabIndex = 0x19;
            this.includeDateTimeChkBox.AutoSize = true;
            this.includeDateTimeChkBox.Location = new Point(0x16, 0x3a);
            this.includeDateTimeChkBox.Name = "includeDateTimeChkBox";
            this.includeDateTimeChkBox.Size = new Size(0x76, 0x11);
            this.includeDateTimeChkBox.TabIndex = 0x2e;
            this.includeDateTimeChkBox.Text = "include date string?";
            this.includeDateTimeChkBox.UseVisualStyleBackColor = true;
            this.convertedFilesLabel.AutoSize = true;
            this.convertedFilesLabel.Location = new Point(0x260, 0x54);
            this.convertedFilesLabel.Name = "convertedFilesLabel";
            this.convertedFilesLabel.Size = new Size(0x53, 13);
            this.convertedFilesLabel.TabIndex = 0x2f;
            this.convertedFilesLabel.Text = "Files Converted:";
            this.progressBar1.Location = new Point(0x18, 0x1b7);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(710, 12);
            this.progressBar1.TabIndex = 0x30;
            this.useSSBMsg41RadioBtn.AutoSize = true;
            this.useSSBMsg41RadioBtn.Location = new Point(0x93, 0x3a);
            this.useSSBMsg41RadioBtn.Name = "useSSBMsg41RadioBtn";
            this.useSSBMsg41RadioBtn.Size = new Size(0x53, 0x11);
            this.useSSBMsg41RadioBtn.TabIndex = 0x31;
            this.useSSBMsg41RadioBtn.TabStop = true;
            this.useSSBMsg41RadioBtn.Text = "Use SSB 41";
            this.useSSBMsg41RadioBtn.UseVisualStyleBackColor = true;
            this.useOspMsg69RadioBtn.AutoSize = true;
            this.useOspMsg69RadioBtn.Location = new Point(0xef, 0x39);
            this.useOspMsg69RadioBtn.Name = "useOspMsg69RadioBtn";
            this.useOspMsg69RadioBtn.Size = new Size(0x54, 0x11);
            this.useOspMsg69RadioBtn.TabIndex = 0x31;
            this.useOspMsg69RadioBtn.TabStop = true;
            this.useOspMsg69RadioBtn.Text = "Use OSP 69";
            this.useOspMsg69RadioBtn.UseVisualStyleBackColor = true;
            this.fileConversionStatusLabel.AutoSize = true;
            this.fileConversionStatusLabel.Location = new Point(0x16, 0x1d1);
            this.fileConversionStatusLabel.Name = "fileConversionStatusLabel";
            this.fileConversionStatusLabel.Size = new Size(0x3b, 13);
            this.fileConversionStatusLabel.TabIndex = 50;
            this.fileConversionStatusLabel.Text = "Status: idle";
            this.autoDetectRadioBtn.AutoSize = true;
            this.autoDetectRadioBtn.Location = new Point(0x14c, 0x3a);
            this.autoDetectRadioBtn.Name = "autoDetectRadioBtn";
            this.autoDetectRadioBtn.Size = new Size(0x52, 0x11);
            this.autoDetectRadioBtn.TabIndex = 0x31;
            this.autoDetectRadioBtn.TabStop = true;
            this.autoDetectRadioBtn.Text = "Auto Detect";
            this.autoDetectRadioBtn.UseVisualStyleBackColor = true;
            base.AcceptButton = this.autoTestRunBtn;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.autoTestExitBtn;
            base.ClientSize = new Size(0x2f3, 0x20c);
            base.Controls.Add(this.fileConversionStatusLabel);
            base.Controls.Add(this.autoDetectRadioBtn);
            base.Controls.Add(this.useOspMsg69RadioBtn);
            base.Controls.Add(this.useSSBMsg41RadioBtn);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.convertedFilesLabel);
            base.Controls.Add(this.includeDateTimeChkBox);
            base.Controls.Add(this.frmFileConversionFilesCntLabel);
            base.Controls.Add(this.autoTestRefreshBtn);
            base.Controls.Add(this.autoTestClearAvailableListBtn);
            base.Controls.Add(this.automationTestAbortBtn);
            base.Controls.Add(this.autoTestRunBtn);
            base.Controls.Add(this.autoTestRemoveAllBtn);
            base.Controls.Add(this.autoTestRemoveBtn);
            base.Controls.Add(this.autoTestAddBtn);
            base.Controls.Add(this.autoTestAddAllBtn);
            base.Controls.Add(this.autoTestExitBtn);
            base.Controls.Add(this.fileConversionAvailableFilesListVal);
            base.Controls.Add(this.fileConversionToConvertFilesListVal);
            base.Controls.Add(this.autoTestDirBrowser);
            base.Controls.Add(this.autoTestRunScriptsLabel);
            base.Controls.Add(this.autoTestAvailableScriptsLabel);
            base.Controls.Add(this.autoTestFilePathLabel);
            base.Controls.Add(this.autoTestDirVal);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmFileConversion";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "File Conversion";
            base.Load += new EventHandler(this.frmFileConversion_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private int msg69ToMsg4AndMsg41Format(string inputString, ref string[] msg41Array)
        {
            string[] strArray = inputString.Split(new char[] { ',' });
            int num = 0;
            try
            {
                Convert.ToInt32(strArray[2]);
                num = Convert.ToInt32(strArray[3]);
                Convert.ToInt32(strArray[4]);
            }
            catch
            {
            }
            if (num == 0)
            {
                return 1;
            }
            int num2 = 0;
            int num3 = 0;
            int inWeek = 0;
            double inTOW = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            int num8 = 0;
            double num9 = 0.0;
            double num10 = 0.0;
            double num11 = 0.0;
            int inUTCOffset = 0;
            int num13 = 0;
            uint num14 = 0;
            try
            {
                num2 = Convert.ToInt32(strArray[5]);
                num3 = Convert.ToInt32(strArray[6]);
                Convert.ToInt32(strArray[7]);
                inWeek = Convert.ToInt32(strArray[8]);
                inTOW = Convert.ToDouble(strArray[9]) / 1000.0;
                num6 = (Convert.ToDouble(strArray[10]) * 180.0) / 4294967296;
                num7 = (Convert.ToDouble(strArray[11]) * 360.0) / 4294967296;
                num8 = Convert.ToInt32(strArray[12]);
                double num1 = (Convert.ToDouble(strArray[13]) * 180.0) / 256.0;
                num9 = (Convert.ToDouble(strArray[0x10]) * 0.1) - 500.0;
                num10 = Convert.ToDouble(strArray[0x12]) * 0.0625;
                num11 = (Convert.ToDouble(strArray[0x13]) * 360.0) / 65536.0;
                Convert.ToDouble(strArray[20]);
                Convert.ToDouble(strArray[0x15]);
                Convert.ToDouble(strArray[0x16]);
                Convert.ToDouble(strArray[0x17]);
                Convert.ToDouble(strArray[0x18]);
                Convert.ToInt32(strArray[0x19]);
                Convert.ToUInt16(strArray[0x1a]);
                Convert.ToDouble(strArray[0x1c]);
                if ((num8 & 8) == 8)
                {
                    inUTCOffset = Convert.ToInt32(strArray[0x1d]);
                }
                else
                {
                    inUTCOffset = 15;
                }
                num13 = Convert.ToInt32(strArray[30]);
            }
            catch
            {
            }
            if (num2 <= 0)
            {
                return 1;
            }
            msg41Array[0] = "41";
            msg41Array[1] = "0";
            switch ((num3 & 3))
            {
                case 0:
                    msg41Array[2] = "5";
                    break;

                case 1:
                    msg41Array[2] = "6";
                    break;

                default:
                    msg41Array[2] = "0";
                    break;
            }
            msg41Array[3] = strArray[8];
            msg41Array[4] = strArray[9];
            GPSDateTime time = new GPSDateTime();
            time.SetTime(inWeek, inTOW);
            time.SetUTCOffset(inUTCOffset);
            DateTime time2 = time.GetTime();
            msg41Array[5] = time2.Year.ToString();
            msg41Array[6] = time2.Month.ToString();
            msg41Array[7] = time2.Day.ToString();
            msg41Array[8] = time2.Hour.ToString();
            msg41Array[9] = time2.Minute.ToString();
            msg41Array[10] = ((time2.Second * 0x3e8) + time2.Millisecond).ToString();
            msg41Array[12] = string.Format("{0:F0}", num6 * 10000000.0);
            msg41Array[13] = string.Format("{0:F0}", num7 * 10000000.0);
            if ((num8 & 2) == 2)
            {
                msg41Array[14] = string.Format("{0:F0}", num9 * 100.0);
            }
            else
            {
                msg41Array[14] = "0";
            }
            msg41Array[15] = "0";
            msg41Array[0x10] = "0";
            if ((num8 & 4) == 4)
            {
                msg41Array[0x11] = string.Format("{0:F0}", num10 * 100.0);
                msg41Array[0x12] = string.Format("{0:F0}", num11 * 100.0);
            }
            else
            {
                msg41Array[0x11] = "0";
                msg41Array[0x12] = "0";
            }
            for (int i = 0x13; i <= 0x20; i++)
            {
                msg41Array[i] = "0";
            }
            msg41Array[0x21] = num13.ToString();
            msg41Array[0x22] = "0";
            msg41Array[0x23] = "0";
            try
            {
                int[] numArray = new int[num13];
                int[] numArray2 = new int[num13];
                int[] numArray3 = new int[num13];
                if ((num8 & 0x10) == 0x10)
                {
                    int index = 0;
                    int num19 = 0x1f;
                    while (index < num13)
                    {
                        numArray[index] = Convert.ToInt32(strArray[num19++]);
                        numArray2[index] = Convert.ToInt32(strArray[num19++]);
                        numArray3[index] = Convert.ToInt32(strArray[num19++]);
                        num14 |= ((uint) 1) << (numArray[index] - 1);
                        index++;
                    }
                }
                else
                {
                    for (int j = 0; j < num13; j++)
                    {
                        numArray[j] = 0;
                        numArray2[j] = 0;
                        numArray3[j] = 0;
                        num14 = 0;
                    }
                }
            }
            catch
            {
                return 1;
            }
            msg41Array[11] = string.Format("{0}", num14);
            return 0;
        }

        private string NMEA_AddCheckSum(string inputString)
        {
            inputString.Replace(" ", "");
            char[] chArray = inputString.ToCharArray();
            byte num = 0;
            int num2 = 1;
            while (num2 < inputString.Length)
            {
                num = (byte) (num ^ Convert.ToByte(chArray[num2++]));
            }
            return (inputString + string.Format("*{0:X2}\r\n", num));
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.updateParent != null)
            {
                this.updateParent();
            }
            this.fileConversionClosing();
        }

        private int saveNExit()
        {
            int num = 0;
            IniHelper helper = new IniHelper(this.fileConversionIniPath);
            string str = string.Empty;
            foreach (string str2 in this.filesFullPathLists)
            {
                str = str + str2 + ",";
            }
            str = str.TrimEnd(new char[] { ',' });
            if (str.Length != 0)
            {
                str.Replace(" ", "");
            }
            if (File.Exists(this.fileConversionIniPath))
            {
                if ((File.GetAttributes(this.fileConversionIniPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    MessageBox.Show(string.Format("{0} File is read only!\nPlease change property and retry", this.fileConversionIniPath), "Error");
                    return 1;
                }
                helper.IniWriteValue("SETUP", "AVAILABLE_SCRIPTS", str);
                helper.IniWriteValue("SETUP", "SCRIPTS_DIR", this.autoTestDirVal.Text);
                return num;
            }
            StreamWriter writer = File.CreateText(this.fileConversionIniPath);
            writer.WriteLine("[EMAIL]");
            writer.WriteLine("Email.Sender=\"STING@sirf.com\"");
            writer.WriteLine("Email.Copy=\"\"");
            writer.WriteLine("Email.Priority=\"MailPriority.Normal\"");
            writer.WriteLine("Email.Encoding=\"Encoding.ASCII\"");
            writer.WriteLine("Email.Subject=\"SiRFLive Auto Test Report\"");
            writer.WriteLine("Email.Message=\"\nBest Regards\n\n SiRFLive Team\"");
            writer.WriteLine("Email.Smtp=\"192.168.2.254\"");
            writer.WriteLine("[SETUP]");
            writer.WriteLine("SCRIPTS_DIR={0}", this.autoTestDirVal.Text);
            writer.WriteLine("AVAILABLE_SCRIPTS={0}", str);
            writer.Close();
            return num;
        }

        private void updateAvailableFiles()
        {
            this.fileConversionAvailableFilesListVal.Items.Clear();
            this.filesFullPathLists.Clear();
            try
            {
                foreach (string str in Directory.GetFiles(this.autoTestDirVal.Text))
                {
                    if (this._fileType == ".gpx")
                    {
                        if (str.EndsWith(".gp2") || str.EndsWith(".gpx"))
                        {
                            this.addAvailableFiles(str);
                        }
                    }
                    else if (str.EndsWith(this._fileType))
                    {
                        this.addAvailableFiles(str);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error!");
            }
            this.filesArray = this.filesFullPathLists.ToArray();
        }

        private void updateFiles2ConvertCnt()
        {
            string str = string.Format("{0}", this.fileConversionToConvertFilesListVal.Items.Count);
            this.frmFileConversionFilesCntLabel.Text = str;
        }

        internal delegate void updateParentEventHandler();
    }
}

