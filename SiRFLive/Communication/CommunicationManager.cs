﻿namespace SiRFLive.Communication
{
    using AardvarkI2CClassLibrary;
    using CommMgrClassLibrary;
    using CommonClassLibrary;
    using CommonUtilsClassLibrary;
    using LogManagerClassLibrary;
    using OpenNETCF.IO.Serial;
    using SiRFLive.Analysis;
    using SiRFLive.Configuration;
    using SiRFLive.General;
    using SiRFLive.GUI.Commmunication;
    using SiRFLive.MessageHandling;
    using SiRFLive.Reporting;
    using SiRFLive.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.IO.Ports;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;

    public class CommunicationManager : IDisposable
    {
        private string _aidingProtocol;
        private string _authenCode;
        private string _autoReplyConfigFilePath;
        private byte _b1;
        private byte _b2;
        private string _bankTime;
        private string _baudRate;
        private bool _closing;
        private int _commErrorCnt;
        private byte[] _CSVBuff;
        private string _currentBaud;
        private string _currentProtocol;
        private int _cwInterferenceDetectionModeIndex;
        private string _dataBits;
        private object _dataReadLock;
        private string _debugViewMatchStr;
        private CommonUtilsClass _debugViewRTBDisplay;
        private MyPanel _displayPanelCompass;
        private MyPanel _displayPanelDRSensors;
        private MyPanel _displayPanelDRStatusStates;
        private MyPanel _displayPanelLocation;
        private MyPanel _displayPanelMEMS;
        private MyPanel _displayPanelPitch;
        private MyPanel _displayPanelRoll;
        private DataGridView _displayPanelSatelliteStats;
        private MyPanel _displayPanelSignal;
        private MyPanel _displayPanelSVs;
        private MyPanel _displayPanelSVTraj;
        private string _eeDayNum;
        private string _eeSelected;
        private bool _enableCompassView;
        private bool _enableLocationMapView;
        private bool _enableMEMSView;
        private bool _enableSatelliteStats;
        private bool _enableSignalView;
        private bool _enableSVsMap;
        private CommonUtilsClass _errorViewRTBDisplay;
        private int _flowControl;
        private Thread _freqAidfromTTBThread;
        private string _freqAidMsgQName;
        private bool _headerDetected;
        private HelperFunctions _helperFunctions;
        private string _hostAppCfgFilePath;
        private string _hostAppMEMSCfgPath;
        private string _hostPair1;
        private string _hostSWFilePath;
        private Thread _hwCfgAidfromTTBThread;
        private string _hwCfgMsgQName;
        private Thread _I2CDataProcessThread;
        private string _I2CmasterAddr;
        private string _I2Cport;
        private string _I2CslaveAddr;
        private int _idx_CSVBuff;
        private bool _IMUAvailable;
        private string _IMUFilePath;
        private List<byte> _inDataBytes;
        private CommonClass.InputDeviceModes _InputDeviceMode;
        private System.Timers.Timer _InputTimer;
        internal bool _isCheckEpoch;
        internal bool _isEpochMessage;
        private bool _isFirstCalled;
        private double _lastAidingSessionReportedClockDrift;
        private uint _lastClockDrift;
        private int _ldoMode;
        private int _lnaType;
        private double _locationMapRadius;
        private object _lockCommErrorCnt;
        private object _lockwrite;
        private LogManager _log;
        internal lowPowerParams _lowPowerParams;
        private string _messageProtocol;
        private bool _monitorNav;
        private string _myWindowTitle;
        private string _nmea_Str;
        private bool _ok2send;
        private string _parity;
        private string _portName;
        private string _portNum;
        private string _posAidMsgQName;
        private string _posReqAckMsgQName;
        private CommonClass.ProductType _productFamily;
        internal string _protocol;
        private int _readBuffer;
        private bool _requiredRunHost;
        private bool _requireEE;
        private string _resetPort;
        private CommonUtilsClass _responseViewRTBDisplay;
        private string _rxName;
        public ReceiverType _rxType;
        private string _serverName;
        private string _serverPort;
        private string _sourceDeviceName;
        private string _stopBits;
        private Thread _tcpipDataProcessThread;
        private Thread _timeAidfromTTBThread;
        private string _timeAidMsgQName;
        private string _trackerPort;
        private string _TTBPortNum;
        private TransmissionType _txTransType;
        private int _use_tcpip;
        public bool ABPModeIndicator;
        public bool ABPModePendingSet;
        public bool ABPModeToSet;
        public bool AutoDetectProtocolAndBaudDone;
        public AutoReplyMgr AutoReplyCtrl;
        public int ChannelOrderMsg1;
        public CommMgrClass CMC;
        public CommWrapper comPort;
        public bool ComProcessDataEnd;
        public string ConnectErrorString;
        public DataForGUI dataGui;
        public DataForGUI dataGui_ALL;
        public TrackerIC dataICTrack;
        public DataForPlotting dataPlot;
        public Regex DebugRegExpressionHandler;
        public int DebugSettings;
        public bool DebugViewIsMatchEnable;
        public string DefaultTCXOFreq;
        public object DisplayDataLock;
        public Queue DisplayQueue;
        public string ErrorCfgFilePath;
        public string ErrorLogFilePath;
        public List<string> ErrorStringList;
        public string ExtSResetNLineUsage;
        public bool FiveHzNavModeIndicator;
        public bool FiveHzNavModePendingSet;
        public bool FiveHzNavModeToSet;
        public bool Flag_didFacRst_atNMEA;
        public bool Flag_didFacRst_atSSB;
        public bool Flag_gotNMEAMsg_afterFacRst;
        public bool Flag_gotSSBMsg_afterFacRst;
        public bool Flag_Rcvd_NMEAMsgs_Aft_Connect;
        public bool Flag_Rcvd_SSBMsgs_Aft_Connect;
        public bool Flag_RxConnectionRequested;
        public int HostAppCmdWinId;
        public bool I2CModeSwitchDone;
        public bool IsAutoDetectBaud;
        private bool isDisposed;
        public bool IsUserSpecifiedMsgLog;
        public bool IsVersion4_1_A8AndAbove;
        public ListenerManager ListenersCtrl;
        public object LockErrorLog;
        private object lockread;
        public object LockSignalDataUpdate;
        public object LockSignalDataUpdate_ALL;
        private object lockwrite;
        public TransmissionType LogFormat;
        public NavigationAnalysisData m_NavData;
        public MsgFactory m_Protocols;
        public TestSetup m_TestSetup;
        private static int MAX_BYTES_BUFFER = 0x2800;
        private static int MAX_MSG_BUFFER = 0x2800;
        private static int MAX_MSG_LENGTH = 0x1000;
        private const int MAX_SERIAL_BUFFER_LENTH = 0x3e8000;
        public int MAX_SIG_BUFFER;
        public bool MEMSModeToSet;
        public CommWrapper MPMWakeupPort;
        private bool msgid4Update;
        public NavigationParametersClass NavigationParamrters;
        public NavSolutionClass NavSolutionParams;
        public Hashtable NavTruthDataHash;
        private string NMEA_navMode;
        public string OnOffLineUsage;
        public int OrderOfThisMsg1;
        public Receiver RxCtrl;
        public TransmissionType RxTransType;
        public bool SBASRangingPendingSet;
        public bool SBASRangingToSet;
        public int SendFlag;
        public Queue<SignalData> SignalDataQ;
        public Queue<SignalData> SignalDataQ_ALL;
        public SiRFNavParams SiRFNavStartStop;
        public string SrcDeviceName;
        public string StringDRInputCarBusData;
        public string StringDRNavState;
        public string StringDRNavStatus;
        public string StringDRNavSubsystemData;
        public string StringDRSensorData;
        public string StringRxNavParams;
        public Queue ToSendMsgQueue;
        public string ToSwitchBaud;
        public string ToSwitchProtocol;
        public int TotalOfGroupMsg1;
        public TrackerIC TrackerICCtrl;
        public string TrackerPortSelect;
        public CommWrapper TTBPort;
        public string UserSpecifiedLogCfgFilePath;
        public List<string> UserSpecifiedMsgList;
        public List<string> UserSpecifiedSubStringList;
        public bool ViewAll;
        public int WarmupDelay;

        public event UpdateParentPortEventHandler UpdatePortMainWinTitle;

        public event UpdateParentEventHandler UpdateWinTitle;

        public CommunicationManager()
        {
            this._ok2send = true;
            this.SendFlag = 1;
            this._baudRate = string.Empty;
            this._parity = string.Empty;
            this._stopBits = string.Empty;
            this._dataBits = string.Empty;
            this._portName = string.Empty;
            this._portNum = "-1";
            this._TTBPortNum = "-1";
            this._rxName = string.Empty;
            this._sourceDeviceName = string.Empty;
            this._readBuffer = 0x2000;
            this._lowPowerParams = new lowPowerParams();
            this.comPort = new CommWrapper();
            this.SrcDeviceName = string.Empty;
            this.lockread = new object();
            this.lockwrite = new object();
            this._protocol = "SSB";
            this._rxType = ReceiverType.SLC;
            this.m_Protocols = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this.m_TestSetup = new TestSetup();
            this._productFamily = CommonClass.ProductType.GSD4e;
            this._trackerPort = string.Empty;
            this._resetPort = string.Empty;
            this._hostPair1 = string.Empty;
            this._requiredRunHost = true;
            this._hostSWFilePath = string.Empty;
            this._hostAppCfgFilePath = string.Empty;
            this._hostAppMEMSCfgPath = string.Empty;
            this._I2CmasterAddr = string.Empty;
            this._I2CslaveAddr = string.Empty;
            this._I2Cport = string.Empty;
            this.DefaultTCXOFreq = string.Empty;
            this.TrackerPortSelect = "uart";
            this.OnOffLineUsage = "uart_cts";
            this.ExtSResetNLineUsage = "uart_dtr";
            this.WarmupDelay = 0x3ff;
            this.DebugSettings = 1;
            this.IsVersion4_1_A8AndAbove = true;
            this._requireEE = true;
            this._eeSelected = string.Empty;
            this._serverName = string.Empty;
            this._serverPort = string.Empty;
            this._authenCode = string.Empty;
            this._eeDayNum = string.Empty;
            this._bankTime = string.Empty;
            this.dataGui = new DataForGUI();
            this.dataGui_ALL = new DataForGUI(60);
            this.SignalDataQ_ALL = new Queue<SignalData>();
            this.TotalOfGroupMsg1 = 1;
            this.dataICTrack = new TrackerIC();
            this.RxCtrl = new Receiver();
            this.AutoReplyCtrl = new AutoReplyMgr();
            this.TTBPort = new CommWrapper();
            this.TrackerICCtrl = new TrackerIC();
            this.SiRFNavStartStop = new SiRFNavParams();
            this.NavigationParamrters = new NavigationParametersClass();
            this.NavSolutionParams = new NavSolutionClass();
            this.ConnectErrorString = string.Empty;
            this._hwCfgMsgQName = string.Empty;
            this._timeAidMsgQName = string.Empty;
            this._freqAidMsgQName = string.Empty;
            this._posAidMsgQName = string.Empty;
            this._posReqAckMsgQName = string.Empty;
            this._autoReplyConfigFilePath = clsGlobal.InstalledDirectory + @"\scripts\SiRFLiveAutomationSetupAutoReply.cfg";
            this.ErrorLogFilePath = string.Empty;
            this.DisplayQueue = new Queue(MAX_MSG_BUFFER);
            this.ToSwitchProtocol = "OSP";
            this.ToSwitchBaud = "115200";
            this._messageProtocol = "OSP";
            this._currentProtocol = "OSP";
            this._currentBaud = "115200";
            this._IMUFilePath = "";
            this._aidingProtocol = "SSB";
            this._myWindowTitle = string.Empty;
            this._CSVBuff = new byte[MAX_BYTES_BUFFER];
            this._enableLocationMapView = true;
            this._enableSignalView = true;
            this._enableSVsMap = true;
            this._enableSatelliteStats = true;
            this._monitorNav = true;
            this._enableMEMSView = true;
            this._enableCompassView = true;
            this._locationMapRadius = 5.0;
            this._responseViewRTBDisplay = new CommonUtilsClass();
            this._errorViewRTBDisplay = new CommonUtilsClass();
            this._debugViewRTBDisplay = new CommonUtilsClass();
            this.StringRxNavParams = string.Empty;
            this._helperFunctions = new HelperFunctions();
            this._dataReadLock = new object();
            this.DisplayDataLock = new object();
            this._inDataBytes = new List<byte>();
            this.ErrorStringList = new List<string>();
            this.LockErrorLog = new object();
            this.ErrorCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\errorLists.cfg";
            this.UserSpecifiedLogCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\userMessageLists.cfg";
            this.UserSpecifiedMsgList = new List<string>();
            this.UserSpecifiedSubStringList = new List<string>();
            this.StringDRNavStatus = string.Empty;
            this.StringDRNavState = string.Empty;
            this.StringDRNavSubsystemData = string.Empty;
            this.StringDRInputCarBusData = string.Empty;
            this.StringDRSensorData = string.Empty;
            this.I2CModeSwitchDone = true;
            this.NavTruthDataHash = new Hashtable();
            this._debugViewMatchStr = string.Empty;
            this.ToSendMsgQueue = new Queue();
            this._lockwrite = new object();
            this.MAX_SIG_BUFFER = 5;
            this.LockSignalDataUpdate = new object();
            this.LockSignalDataUpdate_ALL = new object();
            this.SignalDataQ = new Queue<SignalData>();
            this.NMEA_navMode = "No Fix";
            this._lockCommErrorCnt = new object();
            this.dataPlot = new DataForPlotting();
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this._baudRate = "115200";
            this._parity = "None";
            this._stopBits = "1";
            this._dataBits = "8";
            this._portName = "COM8";
            this._rxType = ReceiverType.SLC;
            this.RxTransType = TransmissionType.GPS;
            this.LogFormat = TransmissionType.GPS;
            this._idx_CSVBuff = 0;
            this._CSVBuff.Initialize();
            this._b1 = 0;
            this._b2 = 0;
            this._nmea_Str = "";
            this._lastAidingSessionReportedClockDrift = 0.0;
            this._lastClockDrift = 0;
            this._log = new LogManager(clsGlobal.SiRFLiveVersion);
            this.CMC = new CommMgrClass();
            if (File.Exists(this.ErrorCfgFilePath))
            {
                this.ErrorStringList.Clear();
                StreamReader reader = new StreamReader(this.ErrorCfgFilePath);
                string str = reader.ReadLine();
                if ((str != null) && (str != string.Empty))
                {
                    foreach (string str2 in str.Split(new char[] { '%' }))
                    {
                        string str3 = str2.TrimEnd(new char[0]);
                        this.ErrorStringList.Add(str3.TrimStart(new char[0]));
                    }
                }
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            if (File.Exists(this.UserSpecifiedLogCfgFilePath))
            {
                this.UserSpecifiedMsgList.Clear();
                this.UserSpecifiedSubStringList.Clear();
                StreamReader reader2 = new StreamReader(this.UserSpecifiedLogCfgFilePath);
                string str4 = reader2.ReadLine();
                if ((str4 != null) && (str4 != string.Empty))
                {
                    foreach (string str5 in str4.Split(new char[] { '%' }))
                    {
                        string str6 = str5.TrimEnd(new char[0]);
                        this.UserSpecifiedMsgList.Add(str6.TrimStart(new char[0]));
                    }
                    str4 = reader2.ReadLine();
                }
                if ((str4 != null) && (str4 != string.Empty))
                {
                    foreach (string str7 in str4.Split(new char[] { '%' }))
                    {
                        string str8 = str7.TrimEnd(new char[0]);
                        this.UserSpecifiedSubStringList.Add(str8.TrimStart(new char[0]));
                    }
                }
                if (this.UserSpecifiedSubStringList.Count < this.UserSpecifiedMsgList.Count)
                {
                    int num = this.UserSpecifiedMsgList.Count - this.UserSpecifiedSubStringList.Count;
                    for (int i = 0; i < num; i++)
                    {
                        this.UserSpecifiedSubStringList.Add("Y");
                    }
                }
                reader2.Close();
                reader2.Dispose();
                reader2 = null;
            }
        }

        public CommunicationManager(bool hasPlotData)
        {
            this._ok2send = true;
            this.SendFlag = 1;
            this._baudRate = string.Empty;
            this._parity = string.Empty;
            this._stopBits = string.Empty;
            this._dataBits = string.Empty;
            this._portName = string.Empty;
            this._portNum = "-1";
            this._TTBPortNum = "-1";
            this._rxName = string.Empty;
            this._sourceDeviceName = string.Empty;
            this._readBuffer = 0x2000;
            this._lowPowerParams = new lowPowerParams();
            this.comPort = new CommWrapper();
            this.SrcDeviceName = string.Empty;
            this.lockread = new object();
            this.lockwrite = new object();
            this._protocol = "SSB";
            this._rxType = ReceiverType.SLC;
            this.m_Protocols = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this.m_TestSetup = new TestSetup();
            this._productFamily = CommonClass.ProductType.GSD4e;
            this._trackerPort = string.Empty;
            this._resetPort = string.Empty;
            this._hostPair1 = string.Empty;
            this._requiredRunHost = true;
            this._hostSWFilePath = string.Empty;
            this._hostAppCfgFilePath = string.Empty;
            this._hostAppMEMSCfgPath = string.Empty;
            this._I2CmasterAddr = string.Empty;
            this._I2CslaveAddr = string.Empty;
            this._I2Cport = string.Empty;
            this.DefaultTCXOFreq = string.Empty;
            this.TrackerPortSelect = "uart";
            this.OnOffLineUsage = "uart_cts";
            this.ExtSResetNLineUsage = "uart_dtr";
            this.WarmupDelay = 0x3ff;
            this.DebugSettings = 1;
            this.IsVersion4_1_A8AndAbove = true;
            this._requireEE = true;
            this._eeSelected = string.Empty;
            this._serverName = string.Empty;
            this._serverPort = string.Empty;
            this._authenCode = string.Empty;
            this._eeDayNum = string.Empty;
            this._bankTime = string.Empty;
            this.dataGui = new DataForGUI();
            this.dataGui_ALL = new DataForGUI(60);
            this.SignalDataQ_ALL = new Queue<SignalData>();
            this.TotalOfGroupMsg1 = 1;
            this.dataICTrack = new TrackerIC();
            this.RxCtrl = new Receiver();
            this.AutoReplyCtrl = new AutoReplyMgr();
            this.TTBPort = new CommWrapper();
            this.TrackerICCtrl = new TrackerIC();
            this.SiRFNavStartStop = new SiRFNavParams();
            this.NavigationParamrters = new NavigationParametersClass();
            this.NavSolutionParams = new NavSolutionClass();
            this.ConnectErrorString = string.Empty;
            this._hwCfgMsgQName = string.Empty;
            this._timeAidMsgQName = string.Empty;
            this._freqAidMsgQName = string.Empty;
            this._posAidMsgQName = string.Empty;
            this._posReqAckMsgQName = string.Empty;
            this._autoReplyConfigFilePath = clsGlobal.InstalledDirectory + @"\scripts\SiRFLiveAutomationSetupAutoReply.cfg";
            this.ErrorLogFilePath = string.Empty;
            this.DisplayQueue = new Queue(MAX_MSG_BUFFER);
            this.ToSwitchProtocol = "OSP";
            this.ToSwitchBaud = "115200";
            this._messageProtocol = "OSP";
            this._currentProtocol = "OSP";
            this._currentBaud = "115200";
            this._IMUFilePath = "";
            this._aidingProtocol = "SSB";
            this._myWindowTitle = string.Empty;
            this._CSVBuff = new byte[MAX_BYTES_BUFFER];
            this._enableLocationMapView = true;
            this._enableSignalView = true;
            this._enableSVsMap = true;
            this._enableSatelliteStats = true;
            this._monitorNav = true;
            this._enableMEMSView = true;
            this._enableCompassView = true;
            this._locationMapRadius = 5.0;
            this._responseViewRTBDisplay = new CommonUtilsClass();
            this._errorViewRTBDisplay = new CommonUtilsClass();
            this._debugViewRTBDisplay = new CommonUtilsClass();
            this.StringRxNavParams = string.Empty;
            this._helperFunctions = new HelperFunctions();
            this._dataReadLock = new object();
            this.DisplayDataLock = new object();
            this._inDataBytes = new List<byte>();
            this.ErrorStringList = new List<string>();
            this.LockErrorLog = new object();
            this.ErrorCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\errorLists.cfg";
            this.UserSpecifiedLogCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\userMessageLists.cfg";
            this.UserSpecifiedMsgList = new List<string>();
            this.UserSpecifiedSubStringList = new List<string>();
            this.StringDRNavStatus = string.Empty;
            this.StringDRNavState = string.Empty;
            this.StringDRNavSubsystemData = string.Empty;
            this.StringDRInputCarBusData = string.Empty;
            this.StringDRSensorData = string.Empty;
            this.I2CModeSwitchDone = true;
            this.NavTruthDataHash = new Hashtable();
            this._debugViewMatchStr = string.Empty;
            this.ToSendMsgQueue = new Queue();
            this._lockwrite = new object();
            this.MAX_SIG_BUFFER = 5;
            this.LockSignalDataUpdate = new object();
            this.LockSignalDataUpdate_ALL = new object();
            this.SignalDataQ = new Queue<SignalData>();
            this.NMEA_navMode = "No Fix";
            this._lockCommErrorCnt = new object();
            if (hasPlotData)
            {
                this.dataPlot = new DataForPlotting();
            }
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this._baudRate = "115200";
            this._parity = "None";
            this._stopBits = "1";
            this._dataBits = "8";
            this._portName = "COM8";
            this._rxType = ReceiverType.SLC;
            this.RxTransType = TransmissionType.GPS;
            this.LogFormat = TransmissionType.GPS;
            this._idx_CSVBuff = 0;
            this._CSVBuff.Initialize();
            this._b1 = 0;
            this._b2 = 0;
            this._nmea_Str = "";
            this._lastAidingSessionReportedClockDrift = 0.0;
            this._lastClockDrift = 0;
            this._log = new LogManager(clsGlobal.SiRFLiveVersion);
            this.CMC = new CommMgrClass();
            if (File.Exists(this.ErrorCfgFilePath))
            {
                this.ErrorStringList.Clear();
                StreamReader reader = new StreamReader(this.ErrorCfgFilePath);
                string str = reader.ReadLine();
                if ((str != null) && (str != string.Empty))
                {
                    foreach (string str2 in str.Split(new char[] { '%' }))
                    {
                        string str3 = str2.TrimEnd(new char[0]);
                        this.ErrorStringList.Add(str3.TrimStart(new char[0]));
                    }
                }
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            if (File.Exists(this.UserSpecifiedLogCfgFilePath))
            {
                this.UserSpecifiedMsgList.Clear();
                this.UserSpecifiedSubStringList.Clear();
                StreamReader reader2 = new StreamReader(this.UserSpecifiedLogCfgFilePath);
                string str4 = reader2.ReadLine();
                if ((str4 != null) && (str4 != string.Empty))
                {
                    foreach (string str5 in str4.Split(new char[] { '%' }))
                    {
                        string str6 = str5.TrimEnd(new char[0]);
                        this.UserSpecifiedMsgList.Add(str6.TrimStart(new char[0]));
                    }
                    str4 = reader2.ReadLine();
                }
                if ((str4 != null) && (str4 != string.Empty))
                {
                    foreach (string str7 in str4.Split(new char[] { '%' }))
                    {
                        string str8 = str7.TrimEnd(new char[0]);
                        this.UserSpecifiedSubStringList.Add(str8.TrimStart(new char[0]));
                    }
                }
                if (this.UserSpecifiedSubStringList.Count < this.UserSpecifiedMsgList.Count)
                {
                    int num = this.UserSpecifiedMsgList.Count - this.UserSpecifiedSubStringList.Count;
                    for (int i = 0; i < num; i++)
                    {
                        this.UserSpecifiedSubStringList.Add("Y");
                    }
                }
                reader2.Close();
                reader2.Dispose();
                reader2 = null;
            }
        }

        public CommunicationManager(string str)
        {
            this._ok2send = true;
            this.SendFlag = 1;
            this._baudRate = string.Empty;
            this._parity = string.Empty;
            this._stopBits = string.Empty;
            this._dataBits = string.Empty;
            this._portName = string.Empty;
            this._portNum = "-1";
            this._TTBPortNum = "-1";
            this._rxName = string.Empty;
            this._sourceDeviceName = string.Empty;
            this._readBuffer = 0x2000;
            this._lowPowerParams = new lowPowerParams();
            this.comPort = new CommWrapper();
            this.SrcDeviceName = string.Empty;
            this.lockread = new object();
            this.lockwrite = new object();
            this._protocol = "SSB";
            this._rxType = ReceiverType.SLC;
            this.m_Protocols = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this.m_TestSetup = new TestSetup();
            this._productFamily = CommonClass.ProductType.GSD4e;
            this._trackerPort = string.Empty;
            this._resetPort = string.Empty;
            this._hostPair1 = string.Empty;
            this._requiredRunHost = true;
            this._hostSWFilePath = string.Empty;
            this._hostAppCfgFilePath = string.Empty;
            this._hostAppMEMSCfgPath = string.Empty;
            this._I2CmasterAddr = string.Empty;
            this._I2CslaveAddr = string.Empty;
            this._I2Cport = string.Empty;
            this.DefaultTCXOFreq = string.Empty;
            this.TrackerPortSelect = "uart";
            this.OnOffLineUsage = "uart_cts";
            this.ExtSResetNLineUsage = "uart_dtr";
            this.WarmupDelay = 0x3ff;
            this.DebugSettings = 1;
            this.IsVersion4_1_A8AndAbove = true;
            this._requireEE = true;
            this._eeSelected = string.Empty;
            this._serverName = string.Empty;
            this._serverPort = string.Empty;
            this._authenCode = string.Empty;
            this._eeDayNum = string.Empty;
            this._bankTime = string.Empty;
            this.dataGui = new DataForGUI();
            this.dataGui_ALL = new DataForGUI(60);
            this.SignalDataQ_ALL = new Queue<SignalData>();
            this.TotalOfGroupMsg1 = 1;
            this.dataICTrack = new TrackerIC();
            this.RxCtrl = new Receiver();
            this.AutoReplyCtrl = new AutoReplyMgr();
            this.TTBPort = new CommWrapper();
            this.TrackerICCtrl = new TrackerIC();
            this.SiRFNavStartStop = new SiRFNavParams();
            this.NavigationParamrters = new NavigationParametersClass();
            this.NavSolutionParams = new NavSolutionClass();
            this.ConnectErrorString = string.Empty;
            this._hwCfgMsgQName = string.Empty;
            this._timeAidMsgQName = string.Empty;
            this._freqAidMsgQName = string.Empty;
            this._posAidMsgQName = string.Empty;
            this._posReqAckMsgQName = string.Empty;
            this._autoReplyConfigFilePath = clsGlobal.InstalledDirectory + @"\scripts\SiRFLiveAutomationSetupAutoReply.cfg";
            this.ErrorLogFilePath = string.Empty;
            this.DisplayQueue = new Queue(MAX_MSG_BUFFER);
            this.ToSwitchProtocol = "OSP";
            this.ToSwitchBaud = "115200";
            this._messageProtocol = "OSP";
            this._currentProtocol = "OSP";
            this._currentBaud = "115200";
            this._IMUFilePath = "";
            this._aidingProtocol = "SSB";
            this._myWindowTitle = string.Empty;
            this._CSVBuff = new byte[MAX_BYTES_BUFFER];
            this._enableLocationMapView = true;
            this._enableSignalView = true;
            this._enableSVsMap = true;
            this._enableSatelliteStats = true;
            this._monitorNav = true;
            this._enableMEMSView = true;
            this._enableCompassView = true;
            this._locationMapRadius = 5.0;
            this._responseViewRTBDisplay = new CommonUtilsClass();
            this._errorViewRTBDisplay = new CommonUtilsClass();
            this._debugViewRTBDisplay = new CommonUtilsClass();
            this.StringRxNavParams = string.Empty;
            this._helperFunctions = new HelperFunctions();
            this._dataReadLock = new object();
            this.DisplayDataLock = new object();
            this._inDataBytes = new List<byte>();
            this.ErrorStringList = new List<string>();
            this.LockErrorLog = new object();
            this.ErrorCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\errorLists.cfg";
            this.UserSpecifiedLogCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\userMessageLists.cfg";
            this.UserSpecifiedMsgList = new List<string>();
            this.UserSpecifiedSubStringList = new List<string>();
            this.StringDRNavStatus = string.Empty;
            this.StringDRNavState = string.Empty;
            this.StringDRNavSubsystemData = string.Empty;
            this.StringDRInputCarBusData = string.Empty;
            this.StringDRSensorData = string.Empty;
            this.I2CModeSwitchDone = true;
            this.NavTruthDataHash = new Hashtable();
            this._debugViewMatchStr = string.Empty;
            this.ToSendMsgQueue = new Queue();
            this._lockwrite = new object();
            this.MAX_SIG_BUFFER = 5;
            this.LockSignalDataUpdate = new object();
            this.LockSignalDataUpdate_ALL = new object();
            this.SignalDataQ = new Queue<SignalData>();
            this.NMEA_navMode = "No Fix";
            this._lockCommErrorCnt = new object();
            this._baudRate = str;
            this._parity = str;
            this._stopBits = str;
            this._dataBits = str;
            this._portName = str;
            this.sourceDeviceName = str;
            this._rxType = ReceiverType.OSP;
            this.RxTransType = TransmissionType.GPS;
            this.LogFormat = TransmissionType.GPS;
            this.RxCtrl = new Receiver(str);
        }

        public CommunicationManager(string baud, string par, string sBits, string dBits, string name, CommonClass.MyRichTextBox rtb, MyPanel pnl, MyPanel pn_SVs, MyPanel pn_Loc, ReceiverType rx)
        {
            this._ok2send = true;
            this.SendFlag = 1;
            this._baudRate = string.Empty;
            this._parity = string.Empty;
            this._stopBits = string.Empty;
            this._dataBits = string.Empty;
            this._portName = string.Empty;
            this._portNum = "-1";
            this._TTBPortNum = "-1";
            this._rxName = string.Empty;
            this._sourceDeviceName = string.Empty;
            this._readBuffer = 0x2000;
            this._lowPowerParams = new lowPowerParams();
            this.comPort = new CommWrapper();
            this.SrcDeviceName = string.Empty;
            this.lockread = new object();
            this.lockwrite = new object();
            this._protocol = "SSB";
            this._rxType = ReceiverType.SLC;
            this.m_Protocols = new MsgFactory(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols.xml");
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this.m_TestSetup = new TestSetup();
            this._productFamily = CommonClass.ProductType.GSD4e;
            this._trackerPort = string.Empty;
            this._resetPort = string.Empty;
            this._hostPair1 = string.Empty;
            this._requiredRunHost = true;
            this._hostSWFilePath = string.Empty;
            this._hostAppCfgFilePath = string.Empty;
            this._hostAppMEMSCfgPath = string.Empty;
            this._I2CmasterAddr = string.Empty;
            this._I2CslaveAddr = string.Empty;
            this._I2Cport = string.Empty;
            this.DefaultTCXOFreq = string.Empty;
            this.TrackerPortSelect = "uart";
            this.OnOffLineUsage = "uart_cts";
            this.ExtSResetNLineUsage = "uart_dtr";
            this.WarmupDelay = 0x3ff;
            this.DebugSettings = 1;
            this.IsVersion4_1_A8AndAbove = true;
            this._requireEE = true;
            this._eeSelected = string.Empty;
            this._serverName = string.Empty;
            this._serverPort = string.Empty;
            this._authenCode = string.Empty;
            this._eeDayNum = string.Empty;
            this._bankTime = string.Empty;
            this.dataGui = new DataForGUI();
            this.dataGui_ALL = new DataForGUI(60);
            this.SignalDataQ_ALL = new Queue<SignalData>();
            this.TotalOfGroupMsg1 = 1;
            this.dataICTrack = new TrackerIC();
            this.RxCtrl = new Receiver();
            this.AutoReplyCtrl = new AutoReplyMgr();
            this.TTBPort = new CommWrapper();
            this.TrackerICCtrl = new TrackerIC();
            this.SiRFNavStartStop = new SiRFNavParams();
            this.NavigationParamrters = new NavigationParametersClass();
            this.NavSolutionParams = new NavSolutionClass();
            this.ConnectErrorString = string.Empty;
            this._hwCfgMsgQName = string.Empty;
            this._timeAidMsgQName = string.Empty;
            this._freqAidMsgQName = string.Empty;
            this._posAidMsgQName = string.Empty;
            this._posReqAckMsgQName = string.Empty;
            this._autoReplyConfigFilePath = clsGlobal.InstalledDirectory + @"\scripts\SiRFLiveAutomationSetupAutoReply.cfg";
            this.ErrorLogFilePath = string.Empty;
            this.DisplayQueue = new Queue(MAX_MSG_BUFFER);
            this.ToSwitchProtocol = "OSP";
            this.ToSwitchBaud = "115200";
            this._messageProtocol = "OSP";
            this._currentProtocol = "OSP";
            this._currentBaud = "115200";
            this._IMUFilePath = "";
            this._aidingProtocol = "SSB";
            this._myWindowTitle = string.Empty;
            this._CSVBuff = new byte[MAX_BYTES_BUFFER];
            this._enableLocationMapView = true;
            this._enableSignalView = true;
            this._enableSVsMap = true;
            this._enableSatelliteStats = true;
            this._monitorNav = true;
            this._enableMEMSView = true;
            this._enableCompassView = true;
            this._locationMapRadius = 5.0;
            this._responseViewRTBDisplay = new CommonUtilsClass();
            this._errorViewRTBDisplay = new CommonUtilsClass();
            this._debugViewRTBDisplay = new CommonUtilsClass();
            this.StringRxNavParams = string.Empty;
            this._helperFunctions = new HelperFunctions();
            this._dataReadLock = new object();
            this.DisplayDataLock = new object();
            this._inDataBytes = new List<byte>();
            this.ErrorStringList = new List<string>();
            this.LockErrorLog = new object();
            this.ErrorCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\errorLists.cfg";
            this.UserSpecifiedLogCfgFilePath = clsGlobal.InstalledDirectory + @"\Config\userMessageLists.cfg";
            this.UserSpecifiedMsgList = new List<string>();
            this.UserSpecifiedSubStringList = new List<string>();
            this.StringDRNavStatus = string.Empty;
            this.StringDRNavState = string.Empty;
            this.StringDRNavSubsystemData = string.Empty;
            this.StringDRInputCarBusData = string.Empty;
            this.StringDRSensorData = string.Empty;
            this.I2CModeSwitchDone = true;
            this.NavTruthDataHash = new Hashtable();
            this._debugViewMatchStr = string.Empty;
            this.ToSendMsgQueue = new Queue();
            this._lockwrite = new object();
            this.MAX_SIG_BUFFER = 5;
            this.LockSignalDataUpdate = new object();
            this.LockSignalDataUpdate_ALL = new object();
            this.SignalDataQ = new Queue<SignalData>();
            this.NMEA_navMode = "No Fix";
            this._lockCommErrorCnt = new object();
            this.m_NavData = new NavigationAnalysisData(ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\ReferenceLocation.xml");
            this._baudRate = baud;
            this._parity = par;
            this._stopBits = sBits;
            this._dataBits = dBits;
            this._portName = name;
            this._displayPanelSignal = pnl;
            this._displayPanelSVs = pn_SVs;
            this._displayPanelLocation = pn_Loc;
            this._rxType = rx;
            this._rxType = ReceiverType.SLC;
            this.RxTransType = TransmissionType.GPS;
            this._idx_CSVBuff = 0;
            this._CSVBuff.Initialize();
            this._b1 = 0;
            this._b2 = 0;
            this._nmea_Str = "";
            this._lastAidingSessionReportedClockDrift = 0.0;
            this._lastClockDrift = 0;
        }

        private void _errorHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            this.WriteApp(string.Format(clsGlobal.MyCulture, "Serial comm error {0}", new object[] { e.ToString() }));
        }

        private void approxPosAidingHandler()
        {
            if (this.AutoReplyCtrl.ApproxPositionCtrl.Reject)
            {
                this.RxCtrl.SendReject(2, -1, -1, 4, "ApproxPosCfg");
                if (((this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled == 0) && (this.AutoReplyCtrl.HWCfgCtrl.CoarseTimeEnabled == 0)) && (this.AutoReplyCtrl.HWCfgCtrl.PreciseTimeEnabled == 0))
                {
                    this.processPosRequest();
                }
            }
            else
            {
                this.WriteData(this.AutoReplyCtrl.ApproxPosRespMsg);
                this.WriteApp(string.Format(clsGlobal.MyCulture, "Approx Position Response:  Lat:{0:F8}  Lon:{1:F8}  Alt:{2:F2} Est Horz Err (ICD): {3:F2}  Est Vert Err:{4:F2} m", new object[] { this.AutoReplyCtrl.ApproxPositionCtrl.Lat, this.AutoReplyCtrl.ApproxPositionCtrl.Lon, this.AutoReplyCtrl.ApproxPositionCtrl.Alt, this.AutoReplyCtrl.ApproxPositionCtrl.EstHorrErr, this.AutoReplyCtrl.ApproxPositionCtrl.EstVertiErr }));
                if (((this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled == 0) && (this.AutoReplyCtrl.HWCfgCtrl.CoarseTimeEnabled == 0)) && (this.AutoReplyCtrl.HWCfgCtrl.PreciseTimeEnabled == 0))
                {
                    this.processPosRequest();
                }
            }
        }

        private string ByteArrToHexString(byte[] input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte num in input)
            {
                builder.Append(Convert.ToString(num, 0x10).PadLeft(2, '0').PadRight(3, ' '));
            }
            return builder.ToString().ToUpper();
        }

        private string ByteToCSV(byte[] comByte)
        {
            string str = "";
            List<byte> list = new List<byte>();
            list.Clear();
            byte num = 0;
            byte num2 = 0;
            foreach (byte num3 in comByte)
            {
                num2 = num3;
                if ((num == 0xb3) && (num2 == 160))
                {
                    str = str + this.m_Protocols.ConvertRawToFields(list.ToArray()) + "\r\n";
                    list.Clear();
                    list.Add(num3);
                }
                else
                {
                    list.Add(num3);
                }
                num = num2;
            }
            return (str + this.m_Protocols.ConvertRawToFields(list.ToArray()));
        }

        public string ByteToMsgQueue(byte[] comByte1)
        {
            int index = 4;
            int num2 = -1;
            int num3 = 5;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (((this._rxType == ReceiverType.SLC) || (this._rxType == ReceiverType.TTB)) && (this.RxCtrl.MessageProtocol == "SSB"))
            {
                num3 = 6;
                index = 5;
                num2 = 4;
            }
            else
            {
                num3 = 5;
                index = 4;
                num2 = 0;
            }
            if (this._inDataBytes.Count > 0)
            {
                List<byte> list = new List<byte>();
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                lock (this._dataReadLock)
                {
                    foreach (byte num4 in this._inDataBytes)
                    {
                        list.Add(num4);
                    }
                    this._inDataBytes.Clear();
                }
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                StringBuilder builder = new StringBuilder();
                int count = list.Count;
                byte[] msgB = list.ToArray();
                if (this.Log.IsBin)
                {
                    this.Log.Write(msgB);
                }
                list.Clear();
                for (int i = 0; i < count; i++)
                {
                    string str;
                    if (this._idx_CSVBuff >= MAX_BYTES_BUFFER)
                    {
                        this._idx_CSVBuff = 0;
                        this._headerDetected = false;
                        if ((this._InputDeviceMode != CommonClass.InputDeviceModes.I2C) || (this.CMC.HostAppI2CSlave.I2CTalkMode != CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE))
                        {
                            string str2 = "Error index = " + this._idx_CSVBuff.ToString();
                            this.WriteApp(str2);
                            this.WriteApp(this.ByteArrToHexString(this._CSVBuff));
                        }
                        continue;
                    }
                    byte num7 = msgB[i];
                    this._b2 = num7;
                    if (((this._b1 == 160) && (this._b2 == 0xa2)) && !this._headerDetected)
                    {
                        this._headerDetected = true;
                        this._idx_CSVBuff = 0;
                        this._CSVBuff[this._idx_CSVBuff++] = this._b1;
                        this._CSVBuff[this._idx_CSVBuff++] = this._b2;
                        this._b1 = this._b2;
                        continue;
                    }
                    if ((this._b1 != 0xb0) || (this._b2 != 0xb3))
                    {
                        goto Label_10CA;
                    }
                    this._CSVBuff[this._idx_CSVBuff++] = this._b2;
                    if ((this._CSVBuff[0] != 160) || (this._CSVBuff[1] != 0xa2))
                    {
                        goto Label_10AF;
                    }
                    string timeStampInString = HelperFunctions.GetTimeStampInString();
                    int num8 = (this._CSVBuff[2] * 0x100) + this._CSVBuff[3];
                    int num9 = this._idx_CSVBuff - 8;
                    if ((this.SendFlag == 1) && (this.ToSendMsgQueue.Count > 0))
                    {
                        bool flag4 = this.OK_TO_SEND;
                        this.OK_TO_SEND = true;
                        this.WriteData();
                        this.OK_TO_SEND = flag4;
                    }
                    byte[] destinationArray = new byte[this._idx_CSVBuff];
                    Array.Copy(this._CSVBuff, destinationArray, this._idx_CSVBuff);
                    if (num8 > MAX_MSG_LENGTH)
                    {
                        str = this.ByteArrToHexString(destinationArray);
                        string str4 = string.Format("\r\n ### INVALID MESSAGE LENGTH: Len={0} (0x{1:X}): " + str + " ### \r\n", num8, num8);
                        this.WriteApp(str4);
                        this._headerDetected = false;
                        this._idx_CSVBuff = 0;
                        continue;
                    }
                    if (num8 != num9)
                    {
                        goto Label_1048;
                    }
                    int num10 = destinationArray[num2];
                    int num11 = destinationArray[index];
                    int num12 = destinationArray[num3];
                    str = this.ByteArrToHexString(destinationArray);
                    MessageQData data = new MessageQData();
                    data.MessageSource = CommonClass.MessageSource.RX_OUTPUT;
                    data.MessageChanId = num10;
                    data.MessageId = num11;
                    data.MessageSubId = num12;
                    data.MessageType = CommonClass.MessageType.Incoming;
                    data.MessageText = str;
                    data.MessageTime = timeStampInString;
                    lock (this.DisplayDataLock)
                    {
                        if (this.DisplayQueue.Count > MAX_MSG_BUFFER)
                        {
                            this.DisplayQueue.Dequeue();
                        }
                        this.DisplayQueue.Enqueue(data);
                    }
                    switch (num11)
                    {
                        case 0x2e:
                        case 0x3f:
                            if (this.ResponseViewRTBDisplay.DisplayWindow != null)
                            {
                                CommonUtilsClass responseViewRTBDisplay = this.ResponseViewRTBDisplay;
                                responseViewRTBDisplay.LineCount++;
                                this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, this.m_Protocols.ConvertRawToFields(destinationArray));
                            }
                            break;

                        case 0x30:
                            if (clsGlobal.IsMarketingUser())
                            {
                                break;
                            }
                            if (num12 != 1)
                            {
                                goto Label_0A13;
                            }
                            clsGlobal.IsDROn = true;
                            if (clsGlobal.IsDRVerDetected)
                            {
                                goto Label_09CC;
                            }
                            if (destinationArray.Length <= 0x19)
                            {
                                goto Label_09BC;
                            }
                            clsGlobal.DRVersion = "2.0";
                            goto Label_09C6;

                        case 0x29:
                            this.getSatellitesDataMsg41(destinationArray);
                            break;

                        case 30:
                            this.getSatellitesDataMsg30(destinationArray);
                            break;

                        case 2:
                            if (num10 == 1)
                            {
                                goto Label_04F4;
                            }
                            this.getSatellitesDataMsg2(destinationArray);
                            break;

                        case 4:
                            this.getSatellitesDataMsg4(destinationArray);
                            break;

                        case 6:
                            if (((num2 == 0) || (num10 == 0xee)) || (num10 == 0xcc))
                            {
                                this.RxCtrl.ResetCtrl.ResetRxSwVersion = this.m_Protocols.ConvertRawToFields(destinationArray);
                                this.Log.FirmwareSWVersion = this.RxCtrl.ResetCtrl.ResetRxSwVersion;
                                this._myWindowTitle = this.comPort.PortName + " " + this.RxCtrl.ResetCtrl.ResetRxSwVersion;
                                this.UpdateWindowTitleBar(this._myWindowTitle);
                                if (this.ResponseViewRTBDisplay.DisplayWindow != null)
                                {
                                    CommonUtilsClass class1 = this.ResponseViewRTBDisplay;
                                    class1.LineCount++;
                                    this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, this.RxCtrl.ResetCtrl.ResetRxSwVersion);
                                }
                            }
                            else if (num10 == 0xbb)
                            {
                                this.RxCtrl.GetTTFFValues(data.MessageTime, data.MessageText, this.AutoReplyCtrl.TTBTimeAidingParams.Type);
                                this.RxCtrl.GetTTFF();
                            }
                            break;

                        case 7:
                            this.getClkInfoMsg7(destinationArray);
                            break;

                        case 0x10:
                            if ((num10 == 2) && this.AutoReplyCtrl.AutoReplyParams.AutoReplyHWCfg)
                            {
                                if (this.RxCtrl.ResetCtrl.ResetType.Contains("FACTORY"))
                                {
                                    goto Label_0719;
                                }
                                this.hwConfigResponseHandler();
                            }
                            break;

                        case 0x11:
                            if ((num10 == 2) && this.AutoReplyCtrl.AutoReplyParams.AutoReplyTimeTrans)
                            {
                                this.timeAidingHandler();
                            }
                            break;

                        case 0x12:
                            if (num10 != 2)
                            {
                                goto Label_0B6E;
                            }
                            if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyFreqTrans)
                            {
                                this.freqAidingHandler(HelperFunctions.ByteToHex(destinationArray));
                            }
                            break;

                        case 0x13:
                            if (num10 != 2)
                            {
                                goto Label_07E3;
                            }
                            if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyApproxPos)
                            {
                                this.approxPosAidingHandler();
                            }
                            break;

                        case 0x45:
                            if (num12 != 1)
                            {
                                goto Label_0BDA;
                            }
                            this.RxCtrl.GetPosition(HelperFunctions.GetTimeStampInString(), HelperFunctions.ByteToHex(destinationArray));
                            break;

                        case 0x47:
                            if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyHWCfg)
                            {
                                if (this.RxCtrl.ResetCtrl.ResetType.Contains("FACTORY"))
                                {
                                    goto Label_06B5;
                                }
                                this.hwConfigResponseHandler();
                            }
                            break;

                        case 0x48:
                            switch (destinationArray[num3])
                            {
                                case 1:
                                    goto Label_0D16;

                                case 2:
                                    goto Label_0D23;

                                case 3:
                                    goto Label_0D30;

                                case 4:
                                    goto Label_0D3D;
                            }
                            break;

                        case 0x49:
                            switch (num12)
                            {
                                case 1:
                                    goto Label_0757;

                                case 2:
                                    goto Label_0777;

                                case 3:
                                    goto Label_0797;
                            }
                            break;

                        case 0x4b:
                            if ((num12 == 1) && (destinationArray[num3 + 1] == 210))
                            {
                                this.posReqAckHandler();
                            }
                            break;

                        case 0xac:
                            if (!clsGlobal.IsMarketingUser())
                            {
                                if (num12 == 9)
                                {
                                    this.StringDRInputCarBusData = this.RxCtrl.FormatInputCarBusDataString(this.m_Protocols.ConvertRawToFields(destinationArray));
                                }
                                if (this.DisplayPanelDRSensors != null)
                                {
                                    this.DisplayPanelDRSensors.Invalidate();
                                }
                            }
                            break;

                        case 0xb2:
                            switch (destinationArray[num3])
                            {
                                case 4:
                                    if (destinationArray[index + 2] == 0)
                                    {
                                        if (this.ResponseViewRTBDisplay.DisplayWindow != null)
                                        {
                                            CommonUtilsClass class3 = this.ResponseViewRTBDisplay;
                                            class3.LineCount++;
                                            this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, HelperFunctions.ByteToHex(destinationArray));
                                        }
                                    }
                                    else if (destinationArray[index + 2] == 2)
                                    {
                                        if (this.ResponseViewRTBDisplay.DisplayWindow != null)
                                        {
                                            CommonUtilsClass class4 = this.ResponseViewRTBDisplay;
                                            class4.LineCount++;
                                            this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, HelperFunctions.ByteToHex(destinationArray));
                                        }
                                        FileStream output = File.Create(@"C:\MultiPeek.bin");
                                        BinaryWriter writer = new BinaryWriter(output);
                                        for (int j = index + 9; j < (destinationArray.Length - 4); j++)
                                        {
                                            writer.Write(destinationArray[j]);
                                        }
                                        writer.Close();
                                        output.Close();
                                    }
                                    break;

                                case 10:
                                    this.PollICTrackerSettings(destinationArray);
                                    break;
                            }
                            break;

                        case 0xe1:
                            if (num12 != 6)
                            {
                                goto Label_0DEC;
                            }
                            if (this.MessageProtocol != "OSP")
                            {
                                this.RxCtrl.GetTTFFValues(data.MessageTime, data.MessageText, this.AutoReplyCtrl.TTBTimeAidingParams.Type);
                                this.RxCtrl.GetTTFF();
                            }
                            else if (!this.AutoReplyCtrl.AutoReplyParams.AutoReply)
                            {
                                this.RxCtrl.GetTTFFValues(data.MessageTime, data.MessageText, this.AutoReplyCtrl.TTBTimeAidingParams.Type);
                                this.RxCtrl.GetTTFF();
                            }
                            break;
                    }
                    goto Label_0E62;
                Label_04F4:
                    if (this.AutoReplyCtrl.PositionRequestCtrl.LocMethod == 0)
                    {
                        this.RxCtrl.GetMeasurement(HelperFunctions.GetTimeStampInString(), HelperFunctions.ByteToHex(destinationArray));
                    }
                    else
                    {
                        this.RxCtrl.GetPosition(HelperFunctions.GetTimeStampInString(), HelperFunctions.ByteToHex(destinationArray));
                    }
                    goto Label_0E62;
                Label_06B5:
                    if (this.RxCtrl.ResetCtrl.IsAidingPerformedOnFactory)
                    {
                        this.hwConfigResponseHandler();
                    }
                    goto Label_0E62;
                Label_0719:
                    if (this.RxCtrl.ResetCtrl.IsAidingPerformedOnFactory)
                    {
                        this.hwConfigResponseHandler();
                    }
                    goto Label_0E62;
                Label_0757:
                    if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyApproxPos)
                    {
                        this.approxPosAidingHandler();
                    }
                    goto Label_0E62;
                Label_0777:
                    if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyTimeTrans)
                    {
                        this.timeAidingHandler();
                    }
                    goto Label_0E62;
                Label_0797:
                    if (this.AutoReplyCtrl.AutoReplyParams.AutoReplyFreqTrans)
                    {
                        this.freqAidingHandler(HelperFunctions.ByteToHex(destinationArray));
                    }
                    goto Label_0E62;
                Label_07E3:
                    this.getNavigationParams(destinationArray);
                    int aBMMode = this.NavigationParamrters.ABMMode;
                    int num14 = this.NavigationParamrters.FiveHzNavMode << 2;
                    int num15 = this.NavigationParamrters.SBASRangingMode << 3;
                    if (this.ABPModePendingSet)
                    {
                        if (this.ABPModeToSet)
                        {
                            this.RxCtrl.SetPosCalcMode((1 + num14) + num15);
                            this.ABPModePendingSet = false;
                        }
                        else
                        {
                            this.RxCtrl.SetPosCalcMode(num14 + num15);
                            this.ABPModePendingSet = false;
                        }
                        this.RxCtrl.PollNavigationParameters();
                    }
                    if (this.FiveHzNavModePendingSet)
                    {
                        if (this.FiveHzNavModeToSet)
                        {
                            this.RxCtrl.SetPosCalcMode((aBMMode + 4) + num15);
                            this.FiveHzNavModePendingSet = false;
                        }
                        else
                        {
                            this.RxCtrl.SetPosCalcMode(aBMMode + num15);
                            this.FiveHzNavModePendingSet = false;
                        }
                        this.RxCtrl.PollNavigationParameters();
                    }
                    if (this.SBASRangingPendingSet)
                    {
                        if (this.SBASRangingToSet)
                        {
                            this.RxCtrl.SetPosCalcMode((aBMMode + num14) + 8);
                            this.SBASRangingPendingSet = false;
                        }
                        else
                        {
                            this.RxCtrl.SetPosCalcMode(aBMMode + num14);
                            this.SBASRangingPendingSet = false;
                        }
                        this.RxCtrl.PollNavigationParameters();
                    }
                    if (this.ResponseViewRTBDisplay.DisplayWindow != null)
                    {
                        CommonUtilsClass class2 = this.ResponseViewRTBDisplay;
                        class2.LineCount++;
                        this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, this.m_Protocols.ConvertRawToFields(destinationArray));
                        this.StringRxNavParams = this.RxCtrl.FormatNavParameters(this.m_Protocols.ConvertRawToFields(destinationArray));
                        this.ResponseViewRTBDisplay.DisplayData(CommonClass.MessageType.Normal, this.StringRxNavParams);
                        if (this.RxCurrentTransmissionType == TransmissionType.GPS)
                        {
                            this._log.WriteLine(this.StringRxNavParams);
                        }
                    }
                    goto Label_0E62;
                Label_09BC:
                    clsGlobal.DRVersion = "1.0";
                Label_09C6:
                    clsGlobal.IsDRVerDetected = true;
                Label_09CC:
                    if (clsGlobal.DRVersion == "2.0")
                    {
                        destinationArray[5] = (byte) (destinationArray[5] + 200);
                    }
                    string csvString = this.m_Protocols.ConvertRawToFields(destinationArray);
                    this.StringDRNavStatus = this.RxCtrl.FormatDRStatusString(csvString);
                    goto Label_0AAE;
                Label_0A13:
                    if (num12 == 2)
                    {
                        if (!clsGlobal.IsDRVerDetected)
                        {
                            if (destinationArray.Length > 40)
                            {
                                clsGlobal.DRVersion = "2.0";
                            }
                            else
                            {
                                clsGlobal.DRVersion = "1.0";
                            }
                            clsGlobal.IsDRVerDetected = true;
                        }
                        if (clsGlobal.DRVersion == "2.0")
                        {
                            destinationArray[5] = (byte) (destinationArray[5] + 200);
                        }
                        string str6 = this.m_Protocols.ConvertRawToFields(destinationArray);
                        this.StringDRNavState = this.RxCtrl.FormatDRStateString(str6);
                    }
                    else if (num12 == 3)
                    {
                        string str7 = this.m_Protocols.ConvertRawToFields(destinationArray);
                        this.StringDRNavSubsystemData = this.RxCtrl.FormatNavSubSysString(str7);
                    }
                Label_0AAE:
                    if (this.DisplayPanelDRStatusStates != null)
                    {
                        this.DisplayPanelDRStatusStates.Invalidate();
                    }
                    goto Label_0E62;
                Label_0B6E:
                    if (destinationArray[index + 1] == 1)
                    {
                        this.OK_TO_SEND = true;
                        this.WriteData();
                    }
                    else
                    {
                        this.OK_TO_SEND = false;
                    }
                    goto Label_0E62;
                Label_0BDA:
                    if (num12 == 2)
                    {
                        this.RxCtrl.GetMeasurement(HelperFunctions.GetTimeStampInString(), HelperFunctions.ByteToHex(destinationArray));
                    }
                    goto Label_0E62;
                Label_0D16:
                    this.getSensorReadingsMsg72(destinationArray);
                    goto Label_0E62;
                Label_0D23:
                    this.getFactoryParamsMsg72(destinationArray);
                    goto Label_0E62;
                Label_0D30:
                    this.getRxStateMsg72(destinationArray);
                    goto Label_0E62;
                Label_0D3D:
                    this.getHeadingDataMsg72(destinationArray);
                    goto Label_0E62;
                Label_0DEC:
                    if (num12 == 7)
                    {
                        this.RxCtrl.GetTTFFValues(data.MessageTime, data.MessageText, this.AutoReplyCtrl.TTBTimeAidingParams.Type);
                        this.RxCtrl.GetTTFF();
                    }
                Label_0E62:
                    try
                    {
                        if (this.ListenersCtrl != null)
                        {
                            lock (this.ListenersCtrl.ListenerLock)
                            {
                                foreach (string str8 in this.ListenersCtrl.ListenerList.Keys)
                                {
                                    ListenerContent content = (ListenerContent) this.ListenersCtrl.ListenerList[str8];
                                    if ((content.Source == 0) && content.State)
                                    {
                                        if (content.Chan > 0)
                                        {
                                            if (content.Chan == this._CSVBuff[num2])
                                            {
                                                flag3 = true;
                                            }
                                        }
                                        else
                                        {
                                            flag3 = true;
                                        }
                                        if (content.MsgId > 0)
                                        {
                                            if (content.MsgId == this._CSVBuff[index])
                                            {
                                                flag = true;
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (content.SubId > 0)
                                        {
                                            if (content.SubId == this._CSVBuff[index + 1])
                                            {
                                                flag2 = true;
                                            }
                                        }
                                        else
                                        {
                                            flag2 = true;
                                        }
                                        if ((flag3 && flag) && flag2)
                                        {
                                            MessageQData data2 = new MessageQData();
                                            data2.MessageTime = timeStampInString;
                                            data2.MessageText = str;
                                            data2.MessageCommMgr = this.PortName;
                                            if (content.QueueData.Count > 0x2710)
                                            {
                                                content.QueueData.Dequeue();
                                            }
                                            else
                                            {
                                                content.QueueData.Enqueue(data2);
                                            }
                                        }
                                        flag3 = flag = flag2 = false;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        if (!this._closing)
                        {
                            string errorStr = "CommunicationManager: ByteToMsgQueue() ListenersCtrl Processing - " + exception.Message.ToString();
                            this.ErrorPrint(errorStr);
                        }
                    }
                    if (this._commErrorCnt > 0)
                    {
                        lock (this._lockCommErrorCnt)
                        {
                            this._commErrorCnt = 0;
                        }
                    }
                    builder.Append(str + "\r\n");
                    goto Label_10AF;
                Label_1048:
                    if (num9 < num8)
                    {
                        continue;
                    }
                    string msg = string.Format("\r\n ### INVALID MESSAGE LENGTH: expected={0} (0x{1:X}), actual={2} (0x{3:X}): " + this.ByteArrToHexString(destinationArray) + " ### \r\n", new object[] { num8, num8, num9, num9 });
                    this.WriteApp(msg);
                Label_10AF:
                    this._CSVBuff.Initialize();
                    this._idx_CSVBuff = 0;
                    this._headerDetected = false;
                    continue;
                Label_10CA:
                    this._CSVBuff[this._idx_CSVBuff] = num7;
                    this._idx_CSVBuff++;
                    this._b1 = this._b2;
                }
                if (builder.Length > 0)
                {
                    return builder.ToString().ToUpper();
                }
            }
            return string.Empty;
        }

        public string ByteToNMEAText()
        {
            List<byte> list = new List<byte>();
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            lock (this._dataReadLock)
            {
                foreach (byte num in this._inDataBytes)
                {
                    list.Add(num);
                }
                this._inDataBytes.Clear();
            }
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            int count = list.Count;
            byte[] msgB = list.ToArray();
            if (this.Log.IsBin)
            {
                this.Log.Write(msgB);
            }
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                if (this._idx_CSVBuff >= MAX_BYTES_BUFFER)
                {
                    this._idx_CSVBuff = 0;
                    this._headerDetected = false;
                    if ((this._InputDeviceMode != CommonClass.InputDeviceModes.I2C) || (this.CMC.HostAppI2CSlave.I2CTalkMode != CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE))
                    {
                        string msg = "Error index = " + this._idx_CSVBuff.ToString();
                        this.WriteApp(msg);
                        this.WriteApp(this.ByteArrToHexString(this._CSVBuff));
                    }
                    continue;
                }
                char ch = Convert.ToChar(msgB[i]);
                if (ch == '$')
                {
                    this._headerDetected = true;
                    this._idx_CSVBuff = 0;
                    this._CSVBuff[this._idx_CSVBuff++] = msgB[i];
                    continue;
                }
                if (ch != '\n')
                {
                    goto Label_0461;
                }
                if (this.Flag_didFacRst_atSSB)
                {
                    this.Flag_didFacRst_atSSB = false;
                    this.Flag_gotNMEAMsg_afterFacRst = true;
                }
                StringBuilder builder = new StringBuilder();
                for (int j = 0; j < this._idx_CSVBuff; j++)
                {
                    builder.Append(Convert.ToChar(this._CSVBuff[j]));
                }
                MessageQData data = new MessageQData();
                string str2 = builder.ToString();
                int num5 = -1;
                int num6 = -1;
                int num7 = -1;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                string timeStampInString = HelperFunctions.GetTimeStampInString();
                data.MessageSource = CommonClass.MessageSource.RX_OUTPUT;
                data.MessageChanId = 0;
                data.MessageId = 0;
                data.MessageSubId = 0;
                data.MessageType = CommonClass.MessageType.Incoming;
                data.MessageText = str2;
                data.MessageTime = timeStampInString;
                lock (this.DisplayDataLock)
                {
                    if (this.DisplayQueue.Count > MAX_MSG_BUFFER)
                    {
                        this.DisplayQueue.Dequeue();
                    }
                    this.DisplayQueue.Enqueue(data);
                }
                if (this._rxType == ReceiverType.NMEA)
                {
                    try
                    {
                        object obj4;
                        str2 = str2.Trim(new char[] { '\n', '\r' });
                        if (!str2.StartsWith("$"))
                        {
                            goto Label_0432;
                        }
                        string str6 = str2.Substring(0, 6);
                        if (str6 != null)
                        {
                            if (!(str6 == "$GPGGA"))
                            {
                                if (((str6 == "$GPGSV") || (str6 == "$GPRMC")) || (str6 == "$GPGSA"))
                                {
                                    goto Label_02E9;
                                }
                            }
                            else
                            {
                                num5 = 0x29;
                                flag = true;
                            }
                        }
                        goto Label_02EC;
                    Label_02E9:
                        flag = true;
                    Label_02EC:
                        Monitor.Enter(obj4 = this.ListenersCtrl.ListenerLock);
                        try
                        {
                            foreach (string str5 in this.ListenersCtrl.ListenerList.Keys)
                            {
                                ListenerContent content = (ListenerContent) this.ListenersCtrl.ListenerList[str5];
                                if ((content.Source == 0) && content.State)
                                {
                                    if (content.Chan > 0)
                                    {
                                        if (content.Chan == num6)
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    else
                                    {
                                        flag2 = true;
                                    }
                                    if (content.MsgId > 0)
                                    {
                                        if (content.MsgId == num5)
                                        {
                                            flag3 = true;
                                        }
                                    }
                                    else
                                    {
                                        flag3 = true;
                                    }
                                    if (content.SubId > 0)
                                    {
                                        if (content.SubId == num7)
                                        {
                                            flag4 = true;
                                        }
                                    }
                                    else
                                    {
                                        flag4 = true;
                                    }
                                    if ((flag2 && flag3) && flag4)
                                    {
                                        content.MessageText = str2;
                                        content.MessageTime = timeStampInString;
                                    }
                                    flag2 = flag3 = flag4 = false;
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(obj4);
                        }
                        if (flag)
                        {
                            this.getSignalForGUI_NMEA(str2);
                        }
                        data.MessageText = str2;
                        goto Label_0432;
                    }
                    catch (Exception exception)
                    {
                        this.ErrorPrint(exception.Message);
                        goto Label_0432;
                    }
                }
                this.ErrorPrint(str2);
            Label_0432:
                this._idx_CSVBuff = 0;
                if (this._commErrorCnt <= 0)
                {
                    continue;
                }
                lock (this._lockCommErrorCnt)
                {
                    this._commErrorCnt = 0;
                    continue;
                }
            Label_0461:
                this._CSVBuff[this._idx_CSVBuff++] = msgB[i];
            }
            return string.Empty;
        }

        private string ByteToSSB(byte[] comByte)
        {
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            byte num = 0;
            foreach (byte num2 in comByte)
            {
                byte num3 = num2;
                if (num == 160)
                {
                    if (num3 == 0xa2)
                    {
                        builder.Append("\r\n\r\n");
                    }
                    builder.Append(Convert.ToString(num, 0x10).PadLeft(2, '0').PadRight(3, ' '));
                }
                switch (num3)
                {
                    case 160:
                    {
                        builder.Append(Convert.ToString(num2, 0x10).PadLeft(2, '0').PadRight(3, ' '));
						break;
                    }
                }
            }
            return builder.ToString().ToUpper();
        }

        private void cleanupClass()
        {
            this.dataGui = null;
            if (this.dataPlot != null)
            {
                this.dataPlot.Dispose();
                this.dataPlot = null;
            }
            if (this._debugViewRTBDisplay != null)
            {
                this._debugViewRTBDisplay = null;
            }
            if (this._displayPanelCompass != null)
            {
                this._displayPanelCompass.Dispose();
                this._displayPanelCompass = null;
            }
            if (this._displayPanelDRSensors != null)
            {
                this._displayPanelDRSensors.Dispose();
                this._displayPanelDRSensors = null;
            }
            if (this._displayPanelDRStatusStates != null)
            {
                this._displayPanelDRStatusStates.Dispose();
                this._displayPanelDRStatusStates = null;
            }
            if (this._displayPanelLocation != null)
            {
                this._displayPanelLocation.Dispose();
                this._displayPanelLocation = null;
            }
            if (this._displayPanelMEMS != null)
            {
                this._displayPanelMEMS.Dispose();
                this._displayPanelMEMS = null;
            }
            if (this._displayPanelPitch != null)
            {
                this._displayPanelPitch.Dispose();
                this._displayPanelPitch = null;
            }
            if (this._displayPanelRoll != null)
            {
                this._displayPanelRoll.Dispose();
                this._displayPanelRoll = null;
            }
            if (this._displayPanelSatelliteStats != null)
            {
                this._displayPanelSatelliteStats.Dispose();
                this._displayPanelSatelliteStats = null;
            }
            if (this._displayPanelSignal != null)
            {
                this._displayPanelSignal.Dispose();
                this._displayPanelSignal = null;
            }
            if (this._displayPanelSVs != null)
            {
                this._displayPanelSVs.Dispose();
                this._displayPanelSVs = null;
            }
            if (this._displayPanelSVTraj != null)
            {
                this._displayPanelSVTraj.Dispose();
                this._displayPanelSVTraj = null;
            }
            if (this._errorViewRTBDisplay != null)
            {
                this._errorViewRTBDisplay = null;
            }
            if (this._responseViewRTBDisplay != null)
            {
                this._responseViewRTBDisplay = null;
            }
            if (this.RxCtrl != null)
            {
                this.RxCtrl.Dispose();
                this.RxCtrl = null;
            }
            this.ListenersCtrl = null;
            this.AutoReplyCtrl = null;
            if (this.TTBPort != null)
            {
                this.TTBPort.Dispose();
                this.TTBPort = null;
            }
            if (this.comPort != null)
            {
                this.comPort.Dispose();
                this.comPort = null;
            }
            this.NavigationParamrters = null;
            this.NavTruthDataHash = null;
            this.NavSolutionParams = null;
            this.TrackerICCtrl = null;
            this.SiRFNavStartStop = null;
            this.m_NavData = null;
            this._CSVBuff = null;
            this.m_Protocols.Dispose();
            this.m_Protocols = null;
            this.CMC = null;
            this.Log = null;
            this.ToSendMsgQueue.Clear();
            this.ToSendMsgQueue = null;
            this.ErrorStringList.Clear();
            this.ErrorStringList = null;
            this.UserSpecifiedMsgList.Clear();
            this.UserSpecifiedMsgList = null;
            this.UserSpecifiedSubStringList.Clear();
            this.UserSpecifiedSubStringList = null;
            this._inDataBytes.Clear();
            this._inDataBytes = null;
        }

        public bool ClosePort()
        {
            this._closing = true;
            try
            {
                if ((this.TTBPort != null) && this.TTBPort.IsOpen)
                {
                    this.WriteData_TTB("A0A2 0009 CCA6 0102 0100 0000 0081 76B0 B3");
                    this.WriteData_TTB("A0A2 0009 CCA6 0104 0100 0000 0081 78B0 B3");
                    this.TTBPort.Close();
                }
                if ((this.MPMWakeupPort != null) && this.MPMWakeupPort.IsOpen)
                {
                    this.Toggle4eWakeupPort();
                    this.MPMWakeupPort.Close();
                }
                switch (this._InputDeviceMode)
                {
                    case CommonClass.InputDeviceModes.RS232:
                        if (this.comPort.IsOpen)
                        {
                            try
                            {
                                this.comPort.Close();
                            }
                            catch
                            {
                                MessageBox.Show("Error closing serial port");
                            }
                            if (this.UpdateWinTitle != null)
                            {
                                this.UpdateWinTitle(this._portName + ": Idle");
                            }
                        }
                        break;

                    case CommonClass.InputDeviceModes.TCP_Client:
                        if (this._tcpipDataProcessThread != null)
                        {
                            this._tcpipDataProcessThread.Abort();
                        }
                        this.CMC.HostAppClient.Close();
                        break;

                    case CommonClass.InputDeviceModes.TCP_Server:
                        if (this._tcpipDataProcessThread != null)
                        {
                            this._tcpipDataProcessThread.Abort();
                        }
                        this.CMC.HostAppServer.Close();
                        break;

                    case CommonClass.InputDeviceModes.I2C:
                        this.AutoDetectProtocolAndBaudDone = false;
                        if (this._I2CDataProcessThread != null)
                        {
                            this._I2CDataProcessThread.Abort();
                        }
                        this.CMC.HostAppI2CSlave.Close();
                        break;
                }
                if (this._log.IsFileOpen())
                {
                    this._log.CloseFile();
                }
                this._idx_CSVBuff = 0;
                this._headerDetected = false;
                return true;
            }
            catch (Exception exception)
            {
                if (!this._closing)
                {
                    this.ErrorPrint(exception.Message);
                }
                return false;
            }
        }

        public void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (this.comPort.BytesToRead > 0)
                {
                    byte[] buffer = this.comPort.Read();
                    this.ByteToMsgQueue(buffer);
                }
            }
            catch (Exception exception)
            {
                this.ErrorPrint(exception.Message);
            }
        }

        private void comPort_OnError(string Description)
        {
            this.ErrorViewRTBDisplay.DisplayData(CommonClass.MessageType.Error, Description);
            if (this._commErrorCnt > 500)
            {
                if (!this.AutoDetectProtocolAndBaudDone || (this.comPort == null))
                {
                    return;
                }
                this.comPort.Close();
                Thread.Sleep(100);
                this.portDataInit();
                Thread.Sleep(100);
                this.comPort.Open();
                lock (this._lockCommErrorCnt)
                {
                    this._commErrorCnt = 0;
                    return;
                }
            }
            lock (this._lockCommErrorCnt)
            {
                this._commErrorCnt++;
            }
        }

        public int comPortDataReceivedHandler()
        {
            if (!this.AutoDetectProtocolAndBaudDone)
            {
                return 0;
            }
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                switch (this.RxTransType)
                {
                    case TransmissionType.Text:
                    {
                        int num = this.comPort.BytesToRead;
                        if (num > 0)
                        {
                            byte[] buffer = this.comPort.Read();
                            lock (this._dataReadLock)
                            {
                                for (int i = 0; i < num; i++)
                                {
                                    this._inDataBytes.Add(buffer[i]);
                                }
                            }
                        }
                        Thread.CurrentThread.Priority = ThreadPriority.Normal;
                        return num;
                    }
                    case TransmissionType.Hex:
                    {
                        MessageQData data = new MessageQData();
                        data.MessageId = 0;
                        data.MessageType = CommonClass.MessageType.Incoming;
                        int num1 = this.comPort.BytesToRead;
                        byte[] input = this.comPort.Read();
                        if (input.Length > 0)
                        {
                            data.MessageText = HelperFunctions.ByteToHex(input);
                        }
                        lock (this.DisplayDataLock)
                        {
                            this.DisplayQueue.Enqueue(data);
                        }
                        Thread.CurrentThread.Priority = ThreadPriority.Normal;
                        return input.Length;
                    }
                }
                int bytesToRead = this.comPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] buffer3 = this.comPort.Read();
                    bytesToRead = buffer3.Length;
                    lock (this._dataReadLock)
                    {
                        for (int j = 0; j < bytesToRead; j++)
                        {
                            this._inDataBytes.Add(buffer3[j]);
                        }
                    }
                }
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                return bytesToRead;
            }
            catch (Exception exception)
            {
                if (!this._closing)
                {
                    this.ErrorPrint(exception.Message);
                }
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                if (exception.Message.Contains("close"))
                {
                    return -1;
                }
                return 0;
            }
        }

        private string ConvertHexStringToDecimal(string hexString, string dataType, double scale)
        {
            string[] strArray = new string[0];
            string str2 = string.Empty;
            if (hexString == "")
            {
                return "";
            }
            switch (dataType)
            {
                case "UINT16":
                {
                    double num = ((double) ushort.Parse(hexString, NumberStyles.HexNumber)) / scale;
                    strArray = num.ToString("#0.000000").Split(new char[] { '.' });
                    str2 = strArray[1].TrimEnd(new char[] { '0' });
                    if (str2.Length > 0)
                    {
                        return (strArray[0] + "." + str2);
                    }
                    return strArray[0];
                }
                case "UINT32":
                {
                    double num2 = ((double) uint.Parse(hexString, NumberStyles.HexNumber)) / scale;
                    strArray = num2.ToString("#0.000000").Split(new char[] { '.' });
                    str2 = strArray[1].TrimEnd(new char[] { '0' });
                    if (str2.Length > 0)
                    {
                        return (strArray[0] + "." + str2);
                    }
                    return strArray[0];
                }
            }
            double num3 = ((double) uint.Parse(hexString, NumberStyles.HexNumber)) / scale;
            strArray = num3.ToString("#0.000000").Split(new char[] { '.' });
            str2 = strArray[1].TrimEnd(new char[] { '0' });
            if (str2.Length > 0)
            {
                return (strArray[0] + "." + str2);
            }
            return strArray[0];
        }

        public static HostAppConfigParams CreateHostAppParams()
        {
            return new HostAppConfigParams();
        }

        public void DebugPrint(CommonClass.MessageType type, string inputData)
        {
            if (this.DebugViewRTBDisplay != null)
            {
                CommonUtilsClass debugViewRTBDisplay = this.DebugViewRTBDisplay;
                debugViewRTBDisplay.LineCount++;
                this.DebugViewRTBDisplay.DisplayData(type, inputData);
            }
        }

        public void DisableDataPlot()
        {
            this.dataPlot = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed && disposing)
            {
                this.cleanupClass();
            }
            this.isDisposed = true;
        }

        public void ErrorPrint(string errorStr)
        {
            if ((errorStr != null) && (errorStr != string.Empty))
            {
                StringBuilder builder = new StringBuilder();
                DateTime now = DateTime.Now;
                builder.AppendFormat("{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}:{5:D2}.{6:D3}\t({7})\t{8}", new object[] { now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond, 0, errorStr });
                if (this.ErrorViewRTBDisplay != null)
                {
                    CommonUtilsClass errorViewRTBDisplay = this.ErrorViewRTBDisplay;
                    errorViewRTBDisplay.LineCount++;
                    this.ErrorViewRTBDisplay.DisplayData(CommonClass.MessageType.Error, builder.ToString());
                }
                this.Log.WriteLine(builder.ToString());
            }
        }

        public bool File_DataReceived(CommonClass.MessageType type, string inputString, bool match, string matchString, string timeStamp)
        {
            bool flag = false;
            try
            {
                if (matchString == string.Empty)
                {
                    match = false;
                    matchString = @"(\w+)";
                }
                Regex regex = new Regex(matchString, RegexOptions.Compiled);
                if (!this._isFirstCalled)
                {
                    this.SetupRxCtrl();
                    this._isFirstCalled = true;
                }
                if (type == CommonClass.MessageType.Incoming)
                {
                    if (this.RxTransType == TransmissionType.Text)
                    {
                        MessageQData data = new MessageQData();
                        data.MessageId = 0;
                        data.MessageType = CommonClass.MessageType.Incoming;
                        string str = string.Empty;
                        int num = -1;
                        int num2 = -1;
                        int num3 = -1;
                        bool flag2 = false;
                        bool flag3 = false;
                        bool flag4 = false;
                        bool flag5 = false;
                        string timeStampInString = HelperFunctions.GetTimeStampInString();
                        if (this._rxType == ReceiverType.NMEA)
                        {
                            try
                            {
                                object obj2;
                                str = inputString.Trim(new char[] { '\n', '\r' });
                                if (!str.StartsWith("$"))
                                {
                                    goto Label_024E;
                                }
                                string str8 = str.Substring(0, 6);
                                if (str8 != null)
                                {
                                    if (!(str8 == "$GPGGA"))
                                    {
                                        if (((str8 == "$GPGSV") || (str8 == "$GPRMC")) || (str8 == "$GPGSA"))
                                        {
                                            goto Label_010D;
                                        }
                                    }
                                    else
                                    {
                                        num = 0x29;
                                        flag2 = true;
                                    }
                                }
                                goto Label_0110;
                            Label_010D:
                                flag2 = true;
                            Label_0110:
                                Monitor.Enter(obj2 = this.ListenersCtrl.ListenerLock);
                                try
                                {
                                    foreach (string str4 in this.ListenersCtrl.ListenerList.Keys)
                                    {
                                        ListenerContent content = (ListenerContent) this.ListenersCtrl.ListenerList[str4];
                                        if ((content.Source == 0) && content.State)
                                        {
                                            if (content.Chan > 0)
                                            {
                                                if (content.Chan == num2)
                                                {
                                                    flag3 = true;
                                                }
                                            }
                                            else
                                            {
                                                flag3 = true;
                                            }
                                            if (content.MsgId > 0)
                                            {
                                                if (content.MsgId == num)
                                                {
                                                    flag4 = true;
                                                }
                                            }
                                            else
                                            {
                                                flag4 = true;
                                            }
                                            if (content.SubId > 0)
                                            {
                                                if (content.SubId == num3)
                                                {
                                                    flag5 = true;
                                                }
                                            }
                                            else
                                            {
                                                flag5 = true;
                                            }
                                            if ((flag3 && flag4) && flag5)
                                            {
                                                content.MessageText = str;
                                                content.MessageTime = timeStampInString;
                                            }
                                            flag3 = flag4 = flag5 = false;
                                        }
                                    }
                                }
                                finally
                                {
                                    Monitor.Exit(obj2);
                                }
                                if (flag2)
                                {
                                    this.getSignalForGUI_NMEA(str);
                                }
                                data.MessageText = str;
                                this.DebugPrint(CommonClass.MessageType.Incoming, str);
                                this._log.WriteLine(str);
                                return flag;
                            Label_024E:
                                this.DebugPrint(CommonClass.MessageType.Incoming, str);
                                this._log.WriteLine(str);
                            }
                            catch (Exception exception)
                            {
                                this.ErrorPrint(exception.Message);
                            }
                            return flag;
                        }
                        str = inputString;
                        this.DebugPrint(CommonClass.MessageType.Incoming, str);
                        this._log.WriteLine(str);
                        return flag;
                    }
                    if (this.RxTransType == TransmissionType.Hex)
                    {
                        if (match)
                        {
                            flag = regex.IsMatch(inputString);
                            if (flag)
                            {
                                type = CommonClass.MessageType.Error;
                            }
                        }
                        this.DebugPrint(type, inputString);
                        this._log.WriteLine(inputString);
                        return flag;
                    }
                    byte[] buffer = HelperFunctions.HexToByte(inputString);
                    string str5 = string.Empty;
                    string[] separator = new string[] { "\r\n" };
                    string input = string.Empty;
                    foreach (byte num4 in buffer)
                    {
                        this._inDataBytes.Add(num4);
                    }
                    str5 = this.ByteToMsgQueue(new byte[0]);
                    if (str5.Length != 0)
                    {
                        foreach (string str7 in str5.Split(separator, StringSplitOptions.None))
                        {
                            if (str7.Length == 0)
                            {
                                continue;
                            }
                            switch (this.RxTransType)
                            {
                                case TransmissionType.SSB:
                                    input = str7;
                                    break;

                                case TransmissionType.GP2:
                                    input = CommonUtilsClass.LogToGP2(str7, timeStamp);
                                    break;

                                case TransmissionType.GPS:
                                    input = this.LogToCSV(str7);
                                    break;

                                default:
                                    input = str7;
                                    break;
                            }
                            if (match)
                            {
                                flag = regex.IsMatch(input);
                                if (flag)
                                {
                                    type = CommonClass.MessageType.Matching;
                                }
                            }
                            if (this._isCheckEpoch && this._isEpochMessage)
                            {
                                flag = true;
                                this._isCheckEpoch = false;
                            }
                            this.DebugPrint(type, input);
                        }
                    }
                    return flag;
                }
                if (match)
                {
                    flag = regex.IsMatch(inputString);
                    if (flag)
                    {
                        type = CommonClass.MessageType.Matching;
                    }
                }
                if (this._isCheckEpoch && this._isEpochMessage)
                {
                    flag = true;
                    this._isCheckEpoch = false;
                }
                this.DebugPrint(type, inputString);
                this._log.WriteLine(inputString);
            }
            catch (Exception exception2)
            {
                this.ErrorPrint(exception2.ToString());
            }
            return flag;
        }

        public void File_ResetProtocol()
        {
            this.portDataInit();
            this._isFirstCalled = false;
        }

        public void FilePlaybackCommSetup()
        {
            string str = this._messageProtocol;
            if (str != null)
            {
                if (!(str == "SSB"))
                {
                    if (!(str == "OSP"))
                    {
                        if (str == "NMEA")
                        {
                            this._txTransType = TransmissionType.Text;
                            this.RxTransType = TransmissionType.Text;
                            this.LogFormat = TransmissionType.Text;
                            this._rxType = ReceiverType.NMEA;
                            this.RxCtrl = new NMEAReceiver();
                            this.RxCtrl.RxCommWindow = this;
                            this.RxCtrl.DutStationSetup = this.m_TestSetup;
                            this.RxCtrl.RxNavData = this.m_NavData;
                            this.RxCtrl.MessageProtocol = this._messageProtocol;
                            this.RxCtrl.AidingProtocol = this._aidingProtocol;
                            this.RxCtrl.ResetCtrl = new NMEAReset();
                            this.RxCtrl.ResetCtrl.ResetComm = this;
                            this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                            this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                            this.ListenersCtrl = new NMEAListnerManager();
                            this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                            this.ListenersCtrl.RxType = this._rxType;
                        }
                        return;
                    }
                }
                else
                {
                    this._txTransType = TransmissionType.GP2;
                    this.LogFormat = TransmissionType.GPS;
                    this.RxCtrl = new SS3AndGSD3TWReceiver();
                    this.RxCtrl.RxCommWindow = this;
                    this.RxCtrl.DutStationSetup = this.m_TestSetup;
                    this.RxCtrl.RxNavData = this.m_NavData;
                    this.RxCtrl.MessageProtocol = this._messageProtocol;
                    if (this._rxType == ReceiverType.GSW)
                    {
                        this._aidingProtocol = "SSB";
                    }
                    else
                    {
                        this._aidingProtocol = "AI3";
                    }
                    this.RxCtrl.AidingProtocol = this._aidingProtocol;
                    this.RxCtrl.ControlChannelProtocolFile = ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols_F.xml";
                    this.RxCtrl.ControlChannelVersion = "2.1";
                    this.RxCtrl.ResetCtrl = new FAndSSBReset();
                    this.RxCtrl.ResetCtrl.ResetComm = this;
                    this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                    this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                    this.AutoReplyCtrl = new AutoReplyMgr_F(this.RxCtrl.ControlChannelProtocolFile);
                    this.AutoReplyCtrl.ControlChannelVersion = this.RxCtrl.ControlChannelVersion;
                    this.ListenersCtrl = new ListenerManager();
                    this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                    this.ListenersCtrl.RxType = this._rxType;
                    this.AutoReplyCtrl = new AutoReplyMgr_F();
                    return;
                }
                this._txTransType = TransmissionType.GP2;
                this.LogFormat = TransmissionType.GPS;
                this.RxCtrl = new OSPReceiver();
                this.RxCtrl.RxCommWindow = this;
                this.RxCtrl.DutStationSetup = this.m_TestSetup;
                this.RxCtrl.RxNavData = this.m_NavData;
                this.RxCtrl.MessageProtocol = this._messageProtocol;
                this.RxCtrl.AidingProtocol = this._aidingProtocol;
                this.RxCtrl.ControlChannelProtocolFile = ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols_F.xml";
                this.RxCtrl.ControlChannelVersion = "1.0";
                this.RxCtrl.ResetCtrl = new OSPReset();
                this.RxCtrl.ResetCtrl.ResetComm = this;
                this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                this.AutoReplyCtrl = new AutoReplyMgr_OSP(this.RxCtrl.ControlChannelProtocolFile);
                this.AutoReplyCtrl.ControlChannelVersion = this.RxCtrl.ControlChannelVersion;
                this.ListenersCtrl = new OSPListnerManager();
                this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                this.ListenersCtrl.RxType = this._rxType;
            }
        }

        ~CommunicationManager()
        {
            this.cleanupClass();
        }

        private void freqAidHandler_thread()
        {
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            this.freqAidingResponseWithTTB();
        }

        private void freqAidingHandler(string myMsg)
        {
            if (this.AutoReplyCtrl.FreqTransferCtrl.Reject)
            {
                this.sendFreqAidingReject();
            }
            else
            {
                this.AutoReplyCtrl.FreqTransferCtrl.ScaledFreqOffset = utils_AutoReply.getScaleFreqOffset(this.AutoReplyCtrl.FreqTransferCtrl.DefaultFreqIndex, this.AutoReplyCtrl.FreqTransferCtrl.Accuracy, this.AutoReplyCtrl.FreqTransferCtrl.FreqOffset, this._lastClockDrift, this._lastAidingSessionReportedClockDrift, this.AutoReplyCtrl.FreqTransferCtrl.SpecifiedRefFreq, this.AutoReplyCtrl.FreqTransferCtrl.FreqAidingMethod);
                this.AutoReplyCtrl.AutoReplyFreqTransferResp();
                if (!this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForFreqAid)
                {
                    this.WriteData(this.AutoReplyCtrl.FreqTransferRespMsg);
                    string str = this.AutoReplyCtrl.FreqTransferRespMsg.Replace(" ", "");
                    if (str.Length > 20)
                    {
                        this.WriteApp(this.RxCtrl.FormatFreqTransferResponse(str.Substring(12, str.Length - 20)));
                    }
                    this.processPosRequest();
                }
                else if ((this.TTBPort != null) && this.TTBPort.IsOpen)
                {
                    this.AutoReplyCtrl.FreqTransferRequestMsg = myMsg;
                    this._freqAidfromTTBThread = new Thread(new ThreadStart(this.freqAidHandler_thread));
                    this._freqAidfromTTBThread.IsBackground = true;
                    this._freqAidfromTTBThread.Start();
                }
                else
                {
                    this.WriteApp("TTB port open -- send reject");
                    this.sendFreqAidingReject();
                }
            }
        }

        private void freqAidingResponseWithTTB()
        {
            string msg = this.AutoReplyCtrl.TranslateOSPFreqTransferRequestMsgToTTB(this.AutoReplyCtrl.FreqTransferRequestMsg);
            this.WriteData_TTB(msg);
            string freqTransferRespMsg = this.AutoReplyCtrl.FreqTransferRespMsg;
            string str3 = this.AutoReplyCtrl.TranslateSLCFreqTransRespMsgToTTB(freqTransferRespMsg);
            this.WriteData_TTB(str3);
            string hexString = this.waitforMsgFromTTB(2, 0x12);
            if (hexString != string.Empty)
            {
                hexString = hexString.Replace(" ", "");
                string str6 = this.AutoReplyCtrl.TranslateTTBFreqTransferRespMsgToSLC(hexString);
                this.WriteData(str6);
                string str7 = str6.Replace(" ", "");
                if (str7.Length > 20)
                {
                    this.WriteApp(this.RxCtrl.FormatFreqTransferResponse(str7.Substring(12, str7.Length - 20)));
                }
                this.processPosRequest();
            }
            else
            {
                this.WriteApp("No freq aiding response received from TTB, send reject");
                this.sendFreqAidingReject();
            }
        }

        public string GetAcqAssistDataFromTTB()
        {
            string msg = "A0 A2 00 02 CC E5" + utils_AutoReply.GetChecksum("CC E5", true) + "B0 B3";
            this.WriteData_TTB(msg);
            string ttbAcqAssistData = this.waitforMsgFromTTB(0xcc, 0x56);
            return this.AutoReplyCtrl.AcqAssistFromTTBRespMsg(ttbAcqAssistData);
        }

        public string GetAlmanacDataFromTTB()
        {
            string msg = "A0A20002CCE4" + utils_AutoReply.GetChecksum("CCE4", true) + "B0B3";
            this.WriteData_TTB(msg);
            return this.waitforMsgFromTTB(0xcc, 0x54);
        }

        public string GetAuxNavDataFromTTB()
        {
            string msg = "A0A20002CCEB" + utils_AutoReply.GetChecksum("CCEB", true) + "B0B3";
            this.WriteData_TTB(msg);
            return this.waitforMsgFromTTB(0xcc, 0x59);
        }

        private void getClkInfoMsg7(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            string s = string.Empty;
            try
            {
                if (hashtable.ContainsKey("Clock Drift"))
                {
                    s = (string) hashtable["Clock Drift"];
                    this.LastClockDrift = uint.Parse(s);
                    this.NavSolutionParams.ClockDrift = this.LastClockDrift;
                }
            }
            catch
            {
            }
            try
            {
                if (hashtable.ContainsKey("Extended GPS Week"))
                {
                    s = (string) hashtable["Extended GPS Week"];
                    this.NavSolutionParams.WeekNumber = ushort.Parse(s);
                }
                if (hashtable.ContainsKey("GPS TOW"))
                {
                    s = (string) hashtable["GPS TOW"];
                    this.NavSolutionParams.TOW = uint.Parse(s);
                }
                if (hashtable.ContainsKey("SVs"))
                {
                    s = (string) hashtable["SVs"];
                    this.NavSolutionParams.SVsUsed = byte.Parse(s);
                }
                if (hashtable.ContainsKey("Clock Bias"))
                {
                    s = (string) hashtable["Clock Bias"];
                    this.NavSolutionParams.ClockBias = uint.Parse(s);
                }
                if (hashtable.ContainsKey("Estimated GPS Time"))
                {
                    s = (string) hashtable["Estimated GPS Time"];
                    this.NavSolutionParams.EstGPSTime = uint.Parse(s);
                }
            }
            catch
            {
            }
        }

        public string GetEphDataFromTTB()
        {
            string msg = "A0A20002CCE3" + utils_AutoReply.GetChecksum("CC E3", true) + "B0B3";
            this.WriteData_TTB(msg);
            string ttbEphData = this.waitforMsgFromTTB(0xcc, 0x53);
            return this.AutoReplyCtrl.EphFromTTBRespMsg(ttbEphData);
        }

        private void getFactoryParamsMsg72(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            if (hashtable.ContainsKey("SENSOR_ID"))
            {
                this.dataGui.SensorID = Convert.ToUInt16((string) hashtable["SENSOR_ID"]);
            }
            if (hashtable.ContainsKey("NUM_INIT_READ_REG_SEN"))
            {
                int num = Convert.ToInt16((string) hashtable["NUM_INIT_READ_REG_SEN"]);
                for (int i = 1; i < num; i++)
                {
                    string str = "NUM_BYTES_REG" + Convert.ToString(i);
                    string str2 = "DATA_REG" + Convert.ToString(i);
                    this.dataGui.numBytesReg = Convert.ToByte((string) hashtable[str]);
                    this.dataGui.dataReg = Convert.ToByte((string) hashtable[str2]);
                }
            }
        }

        private void getHeadingDataMsg72(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            if (hashtable.ContainsKey("Heading"))
            {
                this.dataGui.HeadingDegrees = Convert.ToInt16(Convert.ToDouble((string) hashtable["Heading"]));
                if (this.dataGui.HeadingDegrees == 360.0)
                {
                    this.dataGui.HeadingDegrees = 0.0;
                }
            }
            if (hashtable.ContainsKey("Heading"))
            {
                this.dataGui.HeadingDegrees = Convert.ToInt16(Convert.ToDouble((string) hashtable["Heading"]));
            }
            if (hashtable.ContainsKey("Pitch"))
            {
                this.dataGui.PitchDegrees = Convert.ToInt16(Convert.ToDouble((string) hashtable["Pitch"]));
            }
            if (hashtable.ContainsKey("Roll"))
            {
                this.dataGui.RollDegrees = Convert.ToInt16(Convert.ToDouble((string) hashtable["Roll"]));
            }
            if (hashtable.ContainsKey("HeadingUnc"))
            {
                this.dataGui.HeadingUnc = Convert.ToUInt16((string) hashtable["HeadingUnc"]);
            }
            if (hashtable.ContainsKey("PitchUnc"))
            {
                this.dataGui.PitchUnc = Convert.ToUInt16((string) hashtable["PitchUnc"]);
            }
            if (hashtable.ContainsKey("RollUnc"))
            {
                this.dataGui.RollUnc = Convert.ToUInt16((string) hashtable["RollUnc"]);
            }
            if (hashtable.ContainsKey("Status"))
            {
                this.dataGui.CalStatus = Convert.ToByte((string) hashtable["Status"]);
            }
            if (this._displayPanelCompass != null)
            {
                this._displayPanelCompass.Invalidate();
            }
            if (this._displayPanelPitch != null)
            {
                this._displayPanelPitch.Invalidate();
            }
            if (this._displayPanelRoll != null)
            {
                this._displayPanelRoll.Invalidate();
            }
        }

        public void GetIMUDataForGUI()
        {
            try
            {
                this.dataGui.TruePositions.PositionList = this._helperFunctions.ReadIMUFile(this._IMUFilePath, 0.0, 0.0);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        internal Hashtable getMPMStatsDataForGUIFromCSV(int mid, int sid, string protocol, string csvString)
        {
            return this.m_Protocols.ConvertCSVToHash(mid, sid, protocol, csvString);
        }

        private void getMsg1SatelliteDataFromHash(Hashtable msgH)
        {
            string key = "Message Information";
            if (msgH.ContainsKey(key))
            {
                byte num = Convert.ToByte((string) msgH[key]);
                this.TotalOfGroupMsg1 = (num & 240) >> 4;
                this.OrderOfThisMsg1 = num & 15;
            }
            if (this.OrderOfThisMsg1 == 0)
            {
                this.ChannelOrderMsg1 = 0;
                for (int j = 0; j < DataForGUI.MAX_PRN; j++)
                {
                    this.dataGui_ALL.PRN_Arr_CNO[j] = 0f;
                    this.dataGui_ALL.PRN_Arr_Azimuth[j] = 0f;
                    this.dataGui_ALL.PRN_Arr_Elev[j] = 0f;
                    this.dataGui_ALL.PRN_Arr_State[j] = 0;
                    this.dataGui_ALL.PRN_Arr_ID[j] = 0;
                    this.dataGui_ALL.PRN_Arr_Info[j] = 0;
                    this.dataGui_ALL.PRN_Arr_Status[j] = 0;
                }
                for (int k = 0; k < 60; k++)
                {
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_CNO_All[k] = 0f;
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_Azimuth_All[k] = 0f;
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_Elev_All[k] = 0f;
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_State_All[k] = 0;
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_ID_All[k] = 0;
                    this.dataGui_ALL.SignalDataForGUI_All.CHAN_SV_Info[k] = 0;
                }
            }
            StringBuilder builder = new StringBuilder();
            float num4 = 0f;
            int num5 = 1;
            for (int i = this.ChannelOrderMsg1; i < 60; i++)
            {
                ushort num7 = 0;
                byte index = 0;
                string str2 = "SV" + ((i + 1)).ToString();
                string str5 = "Status " + str2;
                string str6 = "Elev " + str2;
                string str7 = "Azimuth " + str2;
                string str3 = "SV Info " + str2;
                string str4 = "Average C/No " + str2;
                if (msgH.ContainsKey(str3))
                {
                    num7 = Convert.ToUInt16((string) msgH[str3]);
                    index = (byte) (num7 & 0xff);
                    if ((num7 & 0xe000) != 0)
                    {
                        if ((num7 & 0xe000) == 0x40)
                        {
                            index = (byte) (index + 210);
                        }
                        else
                        {
                            int num1 = num7 & 0xe000;
                        }
                    }
                    if ((index > 0) && (index < DataForGUI.MAX_PRN))
                    {
                        this.dataGui_ALL.PRN_Arr_ID[index] = index;
                        this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_ID_All[i] = index;
                        if (msgH.ContainsKey(str5))
                        {
                            this.dataGui_ALL.PRN_Arr_Status[index] = Convert.ToUInt32((string) msgH[str5]);
                            this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_Status_All[i] = this.dataGui_ALL.PRN_Arr_Status[index];
                        }
                        if (msgH.ContainsKey(str4))
                        {
                            this.dataGui_ALL.PRN_Arr_CNO[index] = Convert.ToUInt32((string) msgH[str4]);
                            this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_CNO_All[i] = this.dataGui_ALL.PRN_Arr_CNO[index];
                        }
                        if (msgH.ContainsKey(str6))
                        {
                            this.dataGui_ALL.PRN_Arr_Elev[index] = Convert.ToSingle((string) msgH[str6]);
                            this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_Elev_All[i] = this.dataGui_ALL.PRN_Arr_Elev[index];
                        }
                        if (msgH.ContainsKey(str7))
                        {
                            this.dataGui_ALL.PRN_Arr_Azimuth[index] = Convert.ToSingle((string) msgH[str7]);
                            this.dataGui_ALL.SignalDataForGUI_All.CHAN_Arr_Azimuth_All[i] = this.dataGui_ALL.PRN_Arr_Azimuth[index];
                        }
                        if (this.dataGui.PRN_Arr_CNO[index] != 0f)
                        {
                            num4 += this.dataGui_ALL.PRN_Arr_CNO[index];
                            num5++;
                        }
                        builder.Append(string.Format(clsGlobal.MyCulture, "{0:F2}", new object[] { this.dataGui_ALL.PRN_Arr_CNO[index] }));
                        builder.Append(",");
                        this.ChannelOrderMsg1++;
                    }
                    else
                    {
                        builder.Append("0,");
                    }
                }
                else
                {
                    builder.Append("0,");
                }
            }
            builder.Append(string.Format(clsGlobal.MyCulture, "{0:F2}", new object[] { num4 / ((float) num5) }));
            this.m_NavData.AvgCNo = builder.ToString();
            lock (this.LockSignalDataUpdate_ALL)
            {
                if (this.SignalDataQ_ALL.Count >= this.MAX_SIG_BUFFER)
                {
                    this.SignalDataQ_ALL.Dequeue();
                }
                this.SignalDataQ_ALL.Enqueue(this.dataGui_ALL.SignalDataForGUI_All);
            }
            if (this.DisplayPanelSignal != null)
            {
                this.DisplayPanelSignal.Invalidate();
            }
            if (this.DisplayPanelSVs != null)
            {
                this.DisplayPanelSVs.Invalidate();
            }
            if (this.DisplayPanelSVTraj != null)
            {
                this.DisplayPanelSVTraj.Invalidate();
            }
            if (this.DisplayPanelSatelliteStats != null)
            {
                this.msgid4Update = true;
                this.DisplayPanelSatelliteStats.Invalidate();
            }
            if (this.DisplayPanelMEMS != null)
            {
                this.DisplayPanelMEMS.Invalidate();
            }
            if (this.DisplayPanelCompass != null)
            {
                this.DisplayPanelCompass.Invalidate();
                this.DisplayPanelPitch.Invalidate();
                this.DisplayPanelRoll.Invalidate();
            }
        }

        private void getMsg2DataFromHash(Hashtable msgH)
        {
            this._isEpochMessage = true;
            for (int i = 0; i < DataForGUI.MAX_PRN; i++)
            {
                this.dataGui.PRN_Arr_PRNforSolution[i] = 0;
            }
            for (int j = 1; j < 13; j++)
            {
                string key = "PRN CH" + j.ToString();
                if (msgH.ContainsKey(key))
                {
                    byte index = Convert.ToByte((string) msgH[key]);
                    if ((index > 0) && (index < DataForGUI.MAX_PRN))
                    {
                        this.dataGui.PRN_Arr_PRNforSolution[index] = 1;
                    }
                }
            }
            if (msgH.ContainsKey("Mode 1"))
            {
                this.dataGui._PMODE = (byte) (Convert.ToByte((string) msgH["Mode 1"]) & 7);
                string gpsTow = string.Empty;
                string gpsWeek = string.Empty;
                if (this.dataGui._PMODE >= 4)
                {
                    this.m_NavData.IsNav = true;
                    string s = string.Empty;
                    if (this._monitorNav)
                    {
                        if (msgH.ContainsKey("GPS TOW"))
                        {
                            gpsTow = (string) msgH["GPS TOW"];
                        }
                        if (msgH.ContainsKey("GPS Week"))
                        {
                            gpsWeek = (string) msgH["GPS Week"];
                        }
                        this.RxCtrl.SetGPSTime(gpsWeek, gpsTow);
                        this._monitorNav = false;
                    }
                    if (msgH.ContainsKey("X Pos"))
                    {
                        s = (string) msgH["X Pos"];
                        this.NavSolutionParams.ECEFX = int.Parse(s);
                    }
                    if (msgH.ContainsKey("Y Pos"))
                    {
                        s = (string) msgH["Y Pos"];
                        this.NavSolutionParams.ECEFY = int.Parse(s);
                    }
                    if (msgH.ContainsKey("Z Pos"))
                    {
                        s = (string) msgH["Z Pos"];
                        this.NavSolutionParams.ECEFZ = int.Parse(s);
                    }
                }
                else
                {
                    this.m_NavData.IsNav = false;
                    this._monitorNav = true;
                }
            }
        }

        private void getMsg30DataFromHash(Hashtable msgH)
        {
            byte index = 0;
            if (msgH.ContainsKey("Satellite ID"))
            {
                index = Convert.ToByte((string) msgH["Satellite ID"]);
            }
            byte num2 = 0;
            if (msgH.ContainsKey("Ephemeris Flag"))
            {
                num2 = Convert.ToByte((string) msgH["Ephemeris Flag"]);
            }
            if ((index >= 0) && (index < DataForGUI.MAX_PRN))
            {
                this.dataGui.PRN_Arr_UseCGEE[index] = (num2 & 0x20) != 0;
                this.dataGui.PRN_Arr_UseSGEE[index] = (num2 & 0x10) != 0;
            }
            this.dataGui.TimeLastMsg30Rcvd = DateTime.Now;
        }

        private void getMsg41DataFromHash(Hashtable msgH)
        {
            ushort navValid = 0xffff;
            PositionInfo.PositionStruct struct2 = new PositionInfo.PositionStruct();
            if (msgH.ContainsKey("Nav Valid"))
            {
                struct2.NavValid = Convert.ToUInt16((string) msgH["Nav Valid"]);
                navValid = struct2.NavValid;
            }
            if (msgH.ContainsKey("TOW"))
            {
                struct2.TOW = Convert.ToDouble((string) msgH["TOW"]);
                if ((this.dataPlot != null) && (navValid == 0))
                {
                    this.dataPlot.InsertTow_nvplot(struct2.TOW / 1000.0);
                }
            }
            if (msgH.ContainsKey("Latitude"))
            {
                struct2.Latitude = Convert.ToDouble((string) msgH["Latitude"]) / 10000000.0;
                if ((this.dataPlot != null) && (navValid == 0))
                {
                    this.dataPlot.InsertLat(struct2.Latitude);
                }
            }
            if (msgH.ContainsKey("Longitude"))
            {
                struct2.Longitude = Convert.ToDouble((string) msgH["Longitude"]) / 10000000.0;
                if ((this.dataPlot != null) && (navValid == 0))
                {
                    this.dataPlot.InsertLon(struct2.Longitude);
                }
            }
            if (msgH.ContainsKey("Altitude from Ellipsoid"))
            {
                struct2.Altitude = Convert.ToDouble((string) msgH["Altitude from Ellipsoid"]) / 100.0;
                if ((this.dataPlot != null) && (navValid == 0))
                {
                    this.dataPlot.InsertAlt(struct2.Altitude);
                    this.dataPlot.UpdateIdx_nvplot();
                }
            }
            if (msgH.ContainsKey("HDOP"))
            {
                struct2.HDOP = Convert.ToDouble((string) msgH["HDOP"]) / 5.0;
            }
            if (msgH.ContainsKey("Satellite ID List"))
            {
                struct2.SatellitesUsed = Convert.ToUInt32((string) msgH["Satellite ID List"]);
            }
            if (msgH.ContainsKey("UTC Hour"))
            {
                struct2.RxTime_Hour = Convert.ToInt32((string) msgH["UTC Hour"]);
            }
            if (msgH.ContainsKey("UTC Minute"))
            {
                struct2.RxTime_Minute = Convert.ToInt32((string) msgH["UTC Minute"]);
            }
            if (msgH.ContainsKey("UTC Second"))
            {
                struct2.RxTime_second = Convert.ToUInt16((string) msgH["UTC Second"]);
            }
            if (msgH.ContainsKey("Speed Over Ground (SOG)"))
            {
                struct2.Speed = Convert.ToDouble((string) msgH["Speed Over Ground (SOG)"]) / 100.0;
            }
            if (msgH.ContainsKey("Course Over Ground (COG True)"))
            {
                struct2.Heading = Convert.ToDouble((string) msgH["Course Over Ground (COG True)"]) / 100.0;
            }
            if (msgH.ContainsKey("Extended Week Number"))
            {
                struct2.ExtWeek = Convert.ToUInt16((string) msgH["Extended Week Number"]);
            }
            if (msgH.ContainsKey("Number of SVs in Fix"))
            {
                this.m_NavData.NumSVsInFix = Convert.ToInt32((string) msgH["Number of SVs in Fix"]);
            }
            string str = "Mode: ";
            byte num2 = 0;
            if (msgH.ContainsKey("NAV Type"))
            {
                struct2.NavType = Convert.ToUInt16((string) msgH["NAV Type"]);
                num2 = (byte) (struct2.NavType & 7);
                switch (num2)
                {
                    case 1:
                        str = str + "1-SV KF";
                        break;

                    case 2:
                        str = str + "2-SVs KF";
                        break;

                    case 3:
                        str = str + "3-SVs KF";
                        break;

                    case 4:
                        str = str + "> 4-SVs KF";
                        break;

                    case 5:
                        str = str + "2-D LSQ";
                        break;

                    case 6:
                        str = str + "3-D LSQ";
                        break;

                    case 7:
                        str = str + "DR";
                        break;

                    default:
                        str = str + "No Fix";
                        break;
                }
                byte num3 = (byte) (struct2.NavType & 8);
                if (num3 == 8)
                {
                    str = str + " + TP";
                }
                byte num4 = (byte) (struct2.NavType & 0x80);
                if (num4 == 0x80)
                {
                    str = str + " + Dgps";
                }
            }
            struct2.NavModeString = str;
            if (!this.AutoReplyCtrl.AutoReplyParams.AutoReply)
            {
                PositionErrorCalc calc = new PositionErrorCalc();
                if (num2 == 0)
                {
                    this.RxCtrl.ResetCtrl.ResetPositionAvailable = false;
                }
                else if (this.RxCtrl.ResetCtrl == null)
                {
                    this.m_NavData.TTFFSiRFLive = 0.0;
                }
                else
                {
                    if (!this.RxCtrl.ResetCtrl.ResetPositionAvailable)
                    {
                        this.m_NavData.TTFFSiRFLive = ((double) (DateTime.Now.Ticks - this.RxCtrl.ResetCtrl.StartResetTime)) / 10000000.0;
                        if (this.m_NavData.TTFFSiRFLive > 1.0)
                        {
                            this.m_NavData.TTFFSiRFLive -= clsGlobal.SiRFLive_TTFF_OFFSET;
                        }
                        this.m_NavData.TTFFReport = this.m_NavData.TTFFSiRFLive;
                    }
                    if (this.m_NavData.ValidatePosition)
                    {
                        double num5 = Convert.ToDouble(this.m_NavData.RefLat);
                        double num6 = Convert.ToDouble(this.m_NavData.RefLon);
                        double num7 = Convert.ToDouble(this.m_NavData.RefAlt);
                        calc.GetPositionErrorsInMeter(struct2.Latitude, struct2.Longitude, struct2.Altitude, num5, num6, num7);
                        this.m_NavData.Nav2DPositionError = calc.HorizontalError;
                        this.m_NavData.Nav3DPositionError = calc.Position3DError;
                        this.m_NavData.NavVerticalPositionError = calc.VerticalErrorInMeter;
                    }
                    else
                    {
                        this.m_NavData.Nav2DPositionError = -9999.0;
                        this.m_NavData.Nav3DPositionError = -9999.0;
                        this.m_NavData.NavVerticalPositionError = -9999.0;
                        this.m_NavData.TTFFSiRFLive = -9999.0;
                    }
                    this.m_NavData.MeasLat = struct2.Latitude;
                    this.m_NavData.MeasLon = struct2.Longitude;
                    this.m_NavData.MeasAlt = struct2.Altitude;
                    this.m_NavData.TOW = struct2.TOW;
                    if (!this.RxCtrl.ResetCtrl.ResetPositionAvailable)
                    {
                        this.m_NavData.FirstFixMeasLat = this.m_NavData.MeasLat;
                        this.m_NavData.FirstFixMeasLon = this.m_NavData.MeasLon;
                        this.m_NavData.FirstFixMeasAlt = this.m_NavData.MeasAlt;
                        this.m_NavData.FirstFix2DPositionError = this.m_NavData.Nav2DPositionError;
                        this.m_NavData.FirstFix3DPositionError = this.m_NavData.Nav3DPositionError;
                        this.m_NavData.FirstFixVerticalPositionError = this.m_NavData.NavVerticalPositionError;
                        this.m_NavData.FirstFixTOW = this.m_NavData.TOW;
                        this.RxCtrl.ResetCtrl.ResetPositionAvailable = true;
                    }
                }
            }
            if (msgH.ContainsKey("Nav Valid"))
            {
                try
                {
                    if ((Convert.ToUInt16((string) msgH["Nav Valid"]) & 0x100) == 0x100)
                    {
                        this.ABPModeIndicator = true;
                        struct2.NavModeString = struct2.NavModeString + " ABP";
                    }
                    else
                    {
                        this.ABPModeIndicator = false;
                    }
                }
                catch
                {
                }
            }
            if (this.dataGui.Positions.PositionList.Count >= 0x2710)
            {
                this.dataGui.Positions.PositionList.RemoveRange(0, this.dataGui.Positions.PositionList.Count / 2);
            }
            this.dataGui.Positions.PositionList.Add(struct2);
            if (this.DisplayPanelLocation != null)
            {
                this.DisplayPanelLocation.Invalidate();
            }
        }

        private void getMsg4DataFromHash(Hashtable msgH)
        {
            bool flag = false;
            for (int i = 0; i < DataForGUI.MAX_PRN; i++)
            {
                this.dataGui.PRN_Arr_CNO[i] = 0f;
                this.dataGui.PRN_Arr_Azimuth[i] = 0f;
                this.dataGui.PRN_Arr_Elev[i] = 0f;
                this.dataGui.PRN_Arr_State[i] = 0;
                this.dataGui.PRN_Arr_ID[i] = 0;
            }
            for (int j = 0; j < 12; j++)
            {
                this.dataGui.SignalDataForGUI.CHAN_Arr_CNO[j] = 0f;
                this.dataGui.SignalDataForGUI.CHAN_Arr_Azimuth[j] = 0f;
                this.dataGui.SignalDataForGUI.CHAN_Arr_Elev[j] = 0f;
                this.dataGui.SignalDataForGUI.CHAN_Arr_State[j] = 0;
                this.dataGui.SignalDataForGUI.CHAN_Arr_ID[j] = 0;
                for (int m = 0; m < 10; m++)
                {
                    this.dataGui.SignalDataForGUI.CHAN_MEAS_CNO[j][m] = 0;
                }
            }
            if (msgH.ContainsKey("GPS TOW"))
            {
                double num4 = Convert.ToDouble((string) msgH["GPS TOW"]);
                if (this.dataPlot != null)
                {
                    this.dataPlot.InsertTOW(num4 / 100.0);
                }
            }
            StringBuilder builder = new StringBuilder();
            float num5 = 0f;
            int num6 = 1;
            for (int k = 0; k < 12; k++)
            {
                string key = "SVID" + ((k + 1)).ToString();
                string str3 = "State " + key;
                string str4 = "Elev " + key;
                string str5 = "Azimuth " + key;
                if (msgH.ContainsKey(key))
                {
                    byte index = Convert.ToByte((string) msgH[key]);
                    if ((index > 0) && (index < DataForGUI.MAX_PRN))
                    {
                        this.dataGui.PRN_Arr_ID[index] = index;
                        this.dataGui.SignalDataForGUI.CHAN_Arr_ID[k] = index;
                        if (msgH.ContainsKey(str3))
                        {
                            this.dataGui.PRN_Arr_State[index] = Convert.ToUInt16((string) msgH[str3]);
                            this.dataGui.SignalDataForGUI.CHAN_Arr_State[k] = this.dataGui.PRN_Arr_State[index];
                        }
                        double num9 = 0.0;
                        double num10 = 0.0;
                        for (int n = 1; n < 11; n++)
                        {
                            string str2 = "C/NO" + n.ToString() + " " + key;
                            if (msgH.ContainsKey(str2))
                            {
                                double num12 = Convert.ToDouble((string) msgH[str2]);
                                num9 += num12;
                                if (num12 != 0.0)
                                {
                                    num10++;
                                }
                                this.dataGui.SignalDataForGUI.CHAN_MEAS_CNO[k][n - 1] = (int) num12;
                            }
                        }
                        if (num10 == 0.0)
                        {
                            num10 = 1.0;
                        }
                        this.dataGui.PRN_Arr_CNO[index] = (float) (num9 / num10);
                        this.dataGui.SignalDataForGUI.CHAN_Arr_CNO[k] = this.dataGui.PRN_Arr_CNO[index];
                        int num13 = -1;
                        if ((this.dataPlot != null) && (num9 > 10.0))
                        {
                            num13 = this.dataPlot.SVTrkr.getIdx(index);
                            this.dataPlot.InsertCNo(index, (double) this.dataGui.PRN_Arr_CNO[index]);
                        }
                        if (msgH.ContainsKey(str4))
                        {
                            this.dataGui.PRN_Arr_Elev[index] = Convert.ToSingle((string) msgH[str4]);
                            this.dataGui.SignalDataForGUI.CHAN_Arr_Elev[k] = this.dataGui.PRN_Arr_Elev[index];
                            if ((((this.dataPlot != null) && (num9 > 10.0)) && ((num13 >= 0) && (num13 < TrackSVRec.MAX_SVT))) && (this.dataPlot.idx_P[num13] < DataForPlotting.MAX_P))
                            {
                                this.dataPlot.elevation[num13, this.dataPlot.idx_P[num13]] = this.dataGui.PRN_Arr_Elev[index];
                            }
                        }
                        if (msgH.ContainsKey(str5))
                        {
                            this.dataGui.PRN_Arr_Azimuth[index] = Convert.ToSingle((string) msgH[str5]);
                            this.dataGui.SignalDataForGUI.CHAN_Arr_Azimuth[k] = this.dataGui.PRN_Arr_Azimuth[index];
                            if ((((this.dataPlot != null) && (num9 > 10.0)) && ((num13 >= 0) && (num13 < TrackSVRec.MAX_SVT))) && (this.dataPlot.idx_P[num13] < DataForPlotting.MAX_P))
                            {
                                this.dataPlot.azimuth[num13, this.dataPlot.idx_P[num13]] = this.dataGui.PRN_Arr_Azimuth[index];
                                flag = true;
                                this.dataPlot.idx_P[num13]++;
                                if (this.dataPlot.idx_P[num13] >= DataForPlotting.MAX_P)
                                {
                                    this.dataPlot.idx_P[num13] = 0;
                                }
                            }
                        }
                        if (this.dataPlot != null)
                        {
                            if (((num9 > 10.0) && flag) && ((num13 >= 0) && (num13 < TrackSVRec.MAX_SVT)))
                            {
                                int num14 = this.dataPlot.idx_P[num13];
                                if (((num14 >= 2) && (Math.Abs((float) (this.dataPlot.elevation[num13, num14 - 1] - this.dataPlot.elevation[num13, num14 - 2])) < 0.001)) && (Math.Abs((float) (this.dataPlot.azimuth[num13, num14 - 1] - this.dataPlot.azimuth[num13, num14 - 2])) < 0.001))
                                {
                                    this.dataPlot.idx_P[num13]--;
                                }
                                flag = false;
                            }
                            this.dataPlot.UpdateAvgCNoTable(this.dataGui.PRN_Arr_Elev[index], this.dataGui.PRN_Arr_Azimuth[index], this.dataGui.PRN_Arr_CNO[index]);
                        }
                        if (this.dataGui.PRN_Arr_CNO[index] != 0f)
                        {
                            num5 += this.dataGui.PRN_Arr_CNO[index];
                            num6++;
                        }
                        builder.Append(string.Format(clsGlobal.MyCulture, "{0:F2}", new object[] { this.dataGui.PRN_Arr_CNO[index] }));
                        builder.Append(",");
                    }
                    else
                    {
                        builder.Append("0,");
                    }
                }
                else
                {
                    builder.Append("0,");
                }
            }
            builder.Append(string.Format(clsGlobal.MyCulture, "{0:F2}", new object[] { num5 / ((float) num6) }));
            this.m_NavData.AvgCNo = builder.ToString();
            lock (this.LockSignalDataUpdate)
            {
                if (this.SignalDataQ.Count >= this.MAX_SIG_BUFFER)
                {
                    this.SignalDataQ.Dequeue();
                }
                this.SignalDataQ.Enqueue(this.dataGui.SignalDataForGUI);
            }
            if (this.DisplayPanelSignal != null)
            {
                this.DisplayPanelSignal.Invalidate();
            }
            if (this.DisplayPanelSVs != null)
            {
                this.DisplayPanelSVs.Invalidate();
            }
            if (this.DisplayPanelSVTraj != null)
            {
                this.DisplayPanelSVTraj.Invalidate();
            }
            if (this.DisplayPanelSatelliteStats != null)
            {
                this.msgid4Update = true;
                this.DisplayPanelSatelliteStats.Invalidate();
            }
            if (this.DisplayPanelMEMS != null)
            {
                this.DisplayPanelMEMS.Invalidate();
            }
            if (this.DisplayPanelCompass != null)
            {
                this.DisplayPanelCompass.Invalidate();
                this.DisplayPanelPitch.Invalidate();
                this.DisplayPanelRoll.Invalidate();
            }
            this.dataGui.ResetMsg30EphFlagsIfTimeout();
        }

        public string GetNavBitSF123FromTTB()
        {
            string msg = "A0A20002CCEC" + utils_AutoReply.GetChecksum("CCEC", true) + "B0B3";
            this.WriteData_TTB(msg);
            return this.waitforMsgFromTTB(0xcc, 90);
        }

        public string GetNavBitSF45DataSet0FromTTB()
        {
            string msg = "A0A20003CCED00" + utils_AutoReply.GetChecksum("CCED00", true) + "B0B3";
            this.WriteData_TTB(msg);
            return this.waitforMsgFromTTB(0xcc, 0x5b);
        }

        public string GetNavBitSF45DataSet1FromTTB()
        {
            string msg = "A0A20003CCED01" + utils_AutoReply.GetChecksum("CCED01", true) + "B0B3";
            this.WriteData_TTB(msg);
            return this.waitforMsgFromTTB(0xcc, 0x5b);
        }

        private void getNavigationParams(byte[] fullMsgArr)
        {
            try
            {
                Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
                if (hashtable.ContainsKey("Position Calc Mode"))
                {
                    byte num = Convert.ToByte((string) hashtable["Position Calc Mode"]);
                    this.NavigationParamrters.ABMMode = (byte) (num & 1);
                    this.NavigationParamrters.FiveHzNavMode = (byte) ((num & 4) >> 2);
                    this.NavigationParamrters.SBASRangingMode = (byte) ((num & 8) >> 3);
                }
                if (hashtable.ContainsKey("Altitude Hold Mode"))
                {
                    this.NavigationParamrters.AltitudeHoldMode = Convert.ToByte((string) hashtable["Altitude Hold Mode"]);
                }
                if (hashtable.ContainsKey("Altitude Hold Source"))
                {
                    this.NavigationParamrters.AltitudeHoldSource = Convert.ToByte((string) hashtable["Altitude Hold Source"]);
                }
                if (hashtable.ContainsKey("Altitude Source Input"))
                {
                    this.NavigationParamrters.AltitudeSourceInput = Convert.ToInt16((string) hashtable["Altitude Source Input"]);
                }
                if (hashtable.ContainsKey("Degraded Mode"))
                {
                    this.NavigationParamrters.DegradedMode = Convert.ToByte((string) hashtable["Degraded Mode"]);
                }
                if (hashtable.ContainsKey("Degraded Timeout"))
                {
                    this.NavigationParamrters.DegradedTimout = Convert.ToByte((string) hashtable["Degraded Timeout"]);
                }
                if (hashtable.ContainsKey("DR Timeout"))
                {
                    this.NavigationParamrters.DRTimeout = Convert.ToByte((string) hashtable["DR Timeout"]);
                }
                if (hashtable.ContainsKey("Track Smooth Mode"))
                {
                    this.NavigationParamrters.TrackSmoothMode = Convert.ToByte((string) hashtable["Track Smooth Mode"]);
                }
            }
            catch
            {
            }
        }

        private void getRxStateMsg72(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            if (hashtable.ContainsKey("RCVR_PHYSICAL_STATE"))
            {
                this.dataGui.MEMS_State = Convert.ToInt16((string) hashtable["RCVR_PHYSICAL_STATE"]);
            }
        }

        internal Hashtable getSatellitesDataForGUIFromCSV(int mid, int sid, string protocol, string csvString)
        {
            Hashtable msgH = this.m_Protocols.ConvertCSVToHash(mid, sid, protocol, csvString);
            this.getSatellitesDataFromHashtable(msgH);
            this.dataGui.ResetMsg30EphFlagsIfTimeout();
            return msgH;
        }

        private void getSatellitesDataForGUIFromRawBytes(byte[] fullMsgArr)
        {
            int index = 4;
            if ((fullMsgArr[4] == 0xee) || (fullMsgArr[4] == 0xcc))
            {
                index = 5;
            }
            if (((fullMsgArr[index] == 4) || (fullMsgArr[index] == 2)) || ((fullMsgArr[index] == 0x29) || (fullMsgArr[index] == 30)))
            {
                Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
                this.getSatellitesDataFromHashtable(msgH);
            }
            this.dataGui.ResetMsg30EphFlagsIfTimeout();
        }

        private void getSatellitesDataFromHashtable(Hashtable msgH)
        {
            if (msgH.ContainsKey("Message ID"))
            {
                switch (Convert.ToByte((string) msgH["Message ID"]))
                {
                    case 2:
                        this._isEpochMessage = true;
                        this.getMsg2DataFromHash(msgH);
                        return;

                    case 4:
                        this.getMsg4DataFromHash(msgH);
                        return;

                    case 0x29:
                        this.getMsg41DataFromHash(msgH);
                        return;

                    case 30:
                        this.getMsg30DataFromHash(msgH);
                        break;
                }
            }
        }

        private void getSatellitesDataMsg1(byte[] fullMsgArr)
        {
            Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            string key = "Message Sub ID";
            if (msgH.ContainsKey(key))
            {
                try
                {
                    if (Convert.ToByte((string) msgH[key]) == 2)
                    {
                        this.getMsg1SatelliteDataFromHash(msgH);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Exception in getSatellitesDataMsg1: {0}", exception.ToString());
                }
            }
        }

        private void getSatellitesDataMsg2(byte[] fullMsgArr)
        {
            Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            this.getMsg2DataFromHash(msgH);
        }

        private void getSatellitesDataMsg30(byte[] fullMsgArr)
        {
            Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            this.getMsg30DataFromHash(msgH);
        }

        private void getSatellitesDataMsg4(byte[] fullMsgArr)
        {
            Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            this.getMsg4DataFromHash(msgH);
        }

        private void getSatellitesDataMsg41(byte[] fullMsgArr)
        {
            Hashtable msgH = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            this.getMsg41DataFromHash(msgH);
        }

        private void getSensorReadingsMsg72(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            if (hashtable.ContainsKey("SENSOR_ID"))
            {
                this.dataGui.sensID = Convert.ToUInt16((string) hashtable["SENSOR_ID"]);
            }
            switch (this.dataGui.sensID)
            {
                case 14:
                    for (int i = 10; i <= (fullMsgArr[9] * 10); i += 10)
                    {
                        this.dataGui.XValue_Mag = frmMEMSView.convertHiLow2value(fullMsgArr[i + 5], fullMsgArr[i + 6]);
                        this.dataGui.YValue_Mag = frmMEMSView.convertHiLow2value(fullMsgArr[i + 7], fullMsgArr[i + 8]);
                        this.dataGui.ZValue_Mag = frmMEMSView.convertHiLow2value(fullMsgArr[i + 9], fullMsgArr[i + 10]);
                    }
                    break;

                case 15:
                    for (int j = 10; j <= (fullMsgArr[9] * 10); j += 10)
                    {
                        this.dataGui.XValue_Acc = frmMEMSView.convertHiLow2value(fullMsgArr[j + 5], fullMsgArr[j + 6]);
                        this.dataGui.YValue_Acc = frmMEMSView.convertHiLow2value(fullMsgArr[j + 7], fullMsgArr[j + 8]);
                        this.dataGui.ZValue_Acc = frmMEMSView.convertHiLow2value(fullMsgArr[j + 9], fullMsgArr[j + 10]);
                    }
                    break;

                case 0x18:
                    return;
            }
            if (this._displayPanelMEMS != null)
            {
                this._displayPanelMEMS.Invalidate();
            }
        }

        private void getSignalForGUI_NMEA(string str)
        {
            PositionInfo.PositionStruct struct2;
            string str5;
            int num8;
            string nmeaMsg = str;
            if (str.Contains("*"))
            {
                nmeaMsg = str.Substring(0, str.Length - 3);
            }
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash_NMEAMsgs(nmeaMsg);
            if (hashtable.ContainsKey("Message ID"))
            {
                string str3 = (string) hashtable["Message ID"];
                struct2 = new PositionInfo.PositionStruct();
                if (!(str3 == "$GPGGA"))
                {
                    if (!(str3 == "$GPGSA"))
                    {
                        switch (str3)
                        {
                            case "$GPRMC":
                                if (this.dataGui.Positions.PositionList.Count > 0)
                                {
                                    struct2 = (PositionInfo.PositionStruct) this.dataGui.Positions.PositionList[this.dataGui.Positions.PositionList.Count - 1];
                                }
                                if (hashtable.ContainsKey("Speed Over Ground"))
                                {
                                    try
                                    {
                                        struct2.Speed = Convert.ToDouble((string) hashtable["Speed Over Ground"]);
                                    }
                                    catch
                                    {
                                        struct2.Speed = 0.0;
                                    }
                                }
                                if (hashtable.ContainsKey("Mode"))
                                {
                                    try
                                    {
                                        byte num10 = 0;
                                        string s = (string) hashtable["Mode"];
                                        if (!byte.TryParse(s, out num10))
                                        {
                                            num10 = byte.Parse(s, NumberStyles.HexNumber);
                                        }
                                        if (num10 == 5)
                                        {
                                            this.NavigationParamrters.FiveHzNavMode = 1;
                                            this.NMEA_navMode = this.NMEA_navMode + " 5Hz";
                                        }
                                        else
                                        {
                                            this.NavigationParamrters.FiveHzNavMode = 0;
                                            this.NMEA_navMode = this.NMEA_navMode + " 1Hz";
                                        }
                                        struct2.NavModeString = this.NMEA_navMode;
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;

                            case "$GPGSV":
                            {
                                int num11 = 0;
                                int num12 = 1;
                                if (hashtable.ContainsKey("Number of Messages"))
                                {
                                    try
                                    {
                                        num11 = Convert.ToInt32((string) hashtable["Number of Messages"]);
                                    }
                                    catch
                                    {
                                        num11 = 0;
                                    }
                                }
                                if (hashtable.ContainsKey("Message Number"))
                                {
                                    try
                                    {
                                        num12 = Convert.ToInt32((string) hashtable["Message Number"]);
                                    }
                                    catch
                                    {
                                        num12 = 0;
                                    }
                                }
                                if ((num11 >= 1) && (num12 == 1))
                                {
                                    for (int j = 0; j < DataForGUI.MAX_PRN; j++)
                                    {
                                        this.dataGui.PRN_Arr_CNO[j] = 0f;
                                        this.dataGui.PRN_Arr_Azimuth[j] = 0f;
                                        this.dataGui.PRN_Arr_Elev[j] = 0f;
                                        this.dataGui.PRN_Arr_State[j] = 0;
                                        this.dataGui.PRN_Arr_ID[j] = 0;
                                    }
                                    for (int k = 0; k < 12; k++)
                                    {
                                        this.dataGui.SignalDataForGUI.CHAN_Arr_CNO[k] = 0f;
                                        this.dataGui.SignalDataForGUI.CHAN_Arr_Azimuth[k] = 0f;
                                        this.dataGui.SignalDataForGUI.CHAN_Arr_Elev[k] = 0f;
                                        this.dataGui.SignalDataForGUI.CHAN_Arr_State[k] = 0;
                                        this.dataGui.SignalDataForGUI.CHAN_Arr_ID[k] = 0;
                                        for (int m = 0; m < 10; m++)
                                        {
                                            this.dataGui.SignalDataForGUI.CHAN_MEAS_CNO[k][m] = 0;
                                        }
                                    }
                                }
                                for (int i = 0; i < 4; i++)
                                {
                                    int index = 0;
                                    string key = "Satellite ID " + ((i + 1)).ToString();
                                    string str9 = "Elevation " + ((i + 1)).ToString();
                                    string str10 = "Azimuth " + ((i + 1)).ToString();
                                    string str8 = "SNR (C/No) " + ((i + 1)).ToString();
                                    if (hashtable.ContainsKey(key) && (((string) hashtable[key]) != ""))
                                    {
                                        byte num18 = Convert.ToByte((string) hashtable[key]);
                                        index = (4 * (num12 - 1)) + i;
                                        if (index >= 12)
                                        {
                                            index = 11;
                                        }
                                        if ((num18 > 0) && (num18 < DataForGUI.MAX_PRN))
                                        {
                                            this.dataGui.PRN_Arr_ID[num18] = num18;
                                            this.dataGui.SignalDataForGUI.CHAN_Arr_ID[index] = this.dataGui.PRN_Arr_ID[num18];
                                            if (hashtable.ContainsKey(str9))
                                            {
                                                if (((string) hashtable[str9]) != "")
                                                {
                                                    this.dataGui.PRN_Arr_Elev[num18] = (float) Convert.ToDouble((string) hashtable[str9]);
                                                }
                                                else
                                                {
                                                    this.dataGui.PRN_Arr_Elev[num18] = 0f;
                                                }
                                                this.dataGui.SignalDataForGUI.CHAN_Arr_Elev[index] = this.dataGui.PRN_Arr_Elev[num18];
                                            }
                                            if (hashtable.ContainsKey(str10))
                                            {
                                                if (((string) hashtable[str10]) != "")
                                                {
                                                    this.dataGui.PRN_Arr_Azimuth[num18] = (float) Convert.ToDouble((string) hashtable[str10]);
                                                }
                                                else
                                                {
                                                    this.dataGui.PRN_Arr_Azimuth[num18] = 0f;
                                                }
                                                this.dataGui.SignalDataForGUI.CHAN_Arr_Azimuth[index] = this.dataGui.PRN_Arr_Azimuth[num18];
                                            }
                                            if (hashtable.ContainsKey(str8))
                                            {
                                                if (((string) hashtable[str8]) != "")
                                                {
                                                    this.dataGui.PRN_Arr_CNO[num18] = (float) Convert.ToDouble((string) hashtable[str8]);
                                                }
                                                else
                                                {
                                                    this.dataGui.PRN_Arr_CNO[num18] = 0f;
                                                }
                                                this.dataGui.SignalDataForGUI.CHAN_Arr_CNO[index] = this.dataGui.PRN_Arr_CNO[num18];
                                                for (int n = 0; n < 10; n++)
                                                {
                                                    this.dataGui.SignalDataForGUI.CHAN_MEAS_CNO[index][n] = (int) this.dataGui.PRN_Arr_CNO[num18];
                                                }
                                            }
                                        }
                                    }
                                }
                                lock (this.LockSignalDataUpdate)
                                {
                                    if (this.SignalDataQ.Count >= this.MAX_SIG_BUFFER)
                                    {
                                        this.SignalDataQ.Dequeue();
                                    }
                                    this.SignalDataQ.Enqueue(this.dataGui.SignalDataForGUI);
                                }
                                if (num11 == num12)
                                {
                                    if (this.DisplayPanelSignal != null)
                                    {
                                        this.DisplayPanelSignal.Invalidate();
                                    }
                                    if (this.DisplayPanelSVs != null)
                                    {
                                        this.DisplayPanelSVs.Invalidate();
                                    }
                                    if (this.DisplayPanelSVTraj != null)
                                    {
                                        this.DisplayPanelSVTraj.Invalidate();
                                    }
                                    if (this.DisplayPanelSatelliteStats != null)
                                    {
                                        this.msgid4Update = true;
                                        this.DisplayPanelSatelliteStats.Invalidate();
                                    }
                                }
                                break;
                            }
                        }
                        return;
                    }
                    if (this.dataGui.Positions.PositionList.Count > 0)
                    {
                        struct2 = (PositionInfo.PositionStruct) this.dataGui.Positions.PositionList[this.dataGui.Positions.PositionList.Count - 1];
                    }
                    str5 = "";
                    int num7 = 0;
                    if (!hashtable.ContainsKey("Mode 2"))
                    {
                        goto Label_0561;
                    }
                    try
                    {
                        num7 = Convert.ToInt32((string) hashtable["Mode 2"]);
                    }
                    catch
                    {
                        num7 = 0;
                    }
                    switch (num7)
                    {
                        case 2:
                            this.NMEA_navMode = "2-D";
                            struct2.NavValid = 0;
                            goto Label_0554;

                        case 3:
                            this.NMEA_navMode = "3-D";
                            struct2.NavValid = 0;
                            goto Label_0554;
                    }
                    this.NMEA_navMode = "No Fix";
                    struct2.NavValid = 1;
                    goto Label_0554;
                }
                long num2 = 0L;
                double num3 = 0.0;
                long result = 0L;
                if (hashtable.ContainsKey("Latitude"))
                {
                    try
                    {
                        num2 = Math.DivRem((long) (Convert.ToDouble((string) hashtable["Latitude"]) * 10000.0), 0xf4240L, out result);
                        num3 = ((double) result) / 600000.0;
                        struct2.Latitude = num2 + num3;
                    }
                    catch
                    {
                        struct2.Latitude = 0.0;
                    }
                }
                if (hashtable.ContainsKey("Latitude N/S") && (((string) hashtable["Latitude N/S"]) == "S"))
                {
                    struct2.Latitude = -struct2.Latitude;
                }
                if (hashtable.ContainsKey("Longitude"))
                {
                    try
                    {
                        num2 = Math.DivRem((long) (Convert.ToDouble((string) hashtable["Longitude"]) * 10000.0), 0xf4240L, out result);
                        num3 = ((double) result) / 600000.0;
                        struct2.Longitude = num2 + num3;
                    }
                    catch
                    {
                        struct2.Longitude = 0.0;
                    }
                }
                if (hashtable.ContainsKey("Longitude E/W") && (((string) hashtable["Longitude E/W"]) == "W"))
                {
                    struct2.Longitude = -struct2.Longitude;
                }
                if (hashtable.ContainsKey("Altitude mean sea level(geoid)"))
                {
                    try
                    {
                        struct2.Altitude = Convert.ToDouble((string) hashtable["Altitude mean sea level(geoid)"]);
                    }
                    catch
                    {
                        struct2.Altitude = 0.0;
                    }
                    if (hashtable.ContainsKey("Geoidal separation 1"))
                    {
                        try
                        {
                            double num5 = Convert.ToDouble((string) hashtable["Geoidal separation 1"]);
                            struct2.Altitude += num5;
                        }
                        catch
                        {
                            struct2.Altitude = 0.0;
                        }
                    }
                }
                if (hashtable.ContainsKey("HDOP"))
                {
                    try
                    {
                        struct2.HDOP = Convert.ToDouble((string) hashtable["HDOP"]);
                    }
                    catch
                    {
                        struct2.HDOP = 0.0;
                    }
                }
                if (hashtable.ContainsKey("Num Svs in use"))
                {
                    try
                    {
                        struct2.SatellitesUsed = Convert.ToUInt32((string) hashtable["Num Svs in use"]);
                    }
                    catch
                    {
                        struct2.SatellitesUsed = 0;
                    }
                }
                if (hashtable.ContainsKey("UTC"))
                {
                    string str4 = (string) hashtable["UTC"];
                    try
                    {
                        struct2.RxTime_Hour = Convert.ToInt32(str4.Substring(0, 2));
                    }
                    catch
                    {
                        struct2.RxTime_Hour = 0;
                    }
                    try
                    {
                        struct2.RxTime_Minute = Convert.ToInt32(str4.Substring(2, 2));
                    }
                    catch
                    {
                        struct2.RxTime_Minute = 0;
                    }
                    try
                    {
                        struct2.RxTime_second = (ushort) (Convert.ToUInt16(str4.Substring(4, 2)) * 0x3e8);
                    }
                    catch
                    {
                        struct2.RxTime_second = 0;
                    }
                }
                byte num6 = 0;
                if (hashtable.ContainsKey("GPS Quality Indicator"))
                {
                    try
                    {
                        num6 = Convert.ToByte((string) hashtable["GPS Quality Indicator"]);
                    }
                    catch
                    {
                        num6 = 0;
                    }
                }
                switch (num6)
                {
                    case 1:
                        this.dataGui._PMODE = num6;
                        struct2.NavValid = 0;
                        break;

                    case 2:
                        this.dataGui._PMODE = num6;
                        struct2.NavValid = 0;
                        break;

                    default:
                        this.dataGui._PMODE = 0;
                        struct2.NavValid = 1;
                        break;
                }
                struct2.NavModeString = this.NMEA_navMode;
                if (this.dataGui.Positions.PositionList.Count >= 0x2710)
                {
                    this.dataGui.Positions.PositionList.RemoveRange(0, this.dataGui.Positions.PositionList.Count / 2);
                }
                this.dataGui.Positions.PositionList.Add(struct2);
                if (this.DisplayPanelLocation != null)
                {
                    this.DisplayPanelLocation.Invalidate();
                }
            }
            return;
        Label_0554:
            struct2.NavModeString = this.NMEA_navMode;
        Label_0561:
            num8 = 1;
            while (num8 < 13)
            {
                str5 = "Satellite Used " + num8.ToString();
                if (hashtable.ContainsKey(str5) && (((string) hashtable[str5]) != ""))
                {
                    byte num9 = Convert.ToByte((string) hashtable[str5]);
                    this.dataGui.PRN_Arr_PRNforSolution[num9] = 1;
                }
                num8++;
            }
        }

        private void guiDisplayProccess()
        {
            MessageQData data = new MessageQData();
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            while (this.DisplayQueue.Count > 0)
            {
                data = (MessageQData) this.DisplayQueue.Dequeue();
                if ((data.MessageText != null) && (data.MessageText != string.Empty))
                {
                    string messageText = string.Empty;
                    switch (this.RxTransType)
                    {
                        case TransmissionType.SSB:
                            messageText = data.MessageText;
                            break;

                        case TransmissionType.GP2:
                            messageText = CommonUtilsClass.LogToGP2(data.MessageText, data.MessageTime);
                            break;

                        case TransmissionType.GPS:
                            messageText = this.LogToCSV(data.MessageText);
                            break;

                        default:
                            messageText = data.MessageText;
                            break;
                    }
                    if ((messageText != null) || (messageText != string.Empty))
                    {
                        builder.Append(messageText);
                        builder.Append("\r\n");
                        if (this.ViewAll || (((data.MessageId != 0xe1) && (data.MessageId != 0xff)) && ((data.MessageId != 0x40) && (data.MessageId != 0x44))))
                        {
                            continue;
                        }
                        builder2.Append(messageText);
                        builder2.Append("\r\n");
                    }
                }
            }
            string str2 = builder.ToString();
            if (str2 != string.Empty)
            {
                if (!this.ViewAll)
                {
                    string str = builder2.ToString();
                    if (str != string.Empty)
                    {
                        this.msgQDisplay(str);
                    }
                }
                else
                {
                    this.msgQDisplay(str2);
                }
                this._log.Write(str2);
            }
        }

        private void hwConfigRespHandler_thread()
        {
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            this.WriteData_TTB(this.AutoReplyCtrl.HwCfgRespMsgToTTB);
            this.waitforMsgFromTTB(2, 0x10);
            this.WriteApp("HW Config Response.");
            this.sendHWConfigResponse();
        }

        private void hwConfigResponseHandler()
        {
            if (!this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForHwCfg)
            {
                this.WriteApp("HW Config Response");
                this.sendHWConfigResponse();
            }
            else if ((this.TTBPort != null) && this.TTBPort.IsOpen)
            {
                this._hwCfgAidfromTTBThread = new Thread(new ThreadStart(this.hwConfigRespHandler_thread));
                this._hwCfgAidfromTTBThread.IsBackground = true;
                this._hwCfgAidfromTTBThread.Start();
            }
            else
            {
                this.WriteApp("HW Config Response -- TTB Port not open");
                this.sendHWConfigResponse();
            }
        }

        public bool Init4eMPMWakeupPort(int comNumber)
        {
            if (this.MPMWakeupPort == null)
            {
                this.MPMWakeupPort = new CommWrapper();
            }
            try
            {
                if (this.MPMWakeupPort.IsOpen)
                {
                    this.MPMWakeupPort.Close();
                }
                this.MPMWakeupPort.PortName = "COM" + comNumber.ToString();
                this.MPMWakeupPort.BaudRate = 0xe100;
                this.MPMWakeupPort.DataBits = 8;
                this.MPMWakeupPort.Parity = (OpenNETCF.IO.Serial.Parity) Enum.Parse(typeof(OpenNETCF.IO.Serial.Parity), "None");
                this.MPMWakeupPort.StopBits = (OpenNETCF.IO.Serial.StopBits) Enum.Parse(typeof(OpenNETCF.IO.Serial.StopBits), "One");
                this.MPMWakeupPort.Open();
                return true;
            }
            catch (Exception exception)
            {
                string errorStr = "Open MPM wakeup port: " + exception.Message;
                this.ErrorPrint(errorStr);
                return false;
            }
        }

        public byte[] InputMessageToByteArray(string msg)
        {
            if (msg == string.Empty)
            {
                return null;
            }
            switch (this._txTransType)
            {
                case TransmissionType.Text:
                    return CommonUtilsClass.StrToByteArray(msg + "\r\n");

                case TransmissionType.Hex:
                    try
                    {
                        return CommonUtilsClass.HexToByte(msg.Replace(" ", ""));
                    }
                    catch (FormatException exception)
                    {
                        this.ErrorPrint(exception.Message);
                        return null;
                    }
                    break;

                case TransmissionType.GP2:
                    break;

                default:
                    goto Label_009F;
            }
            try
            {
                return CommonUtilsClass.HexToByte(msg.Replace(" ", ""));
            }
            catch (FormatException exception2)
            {
                this.ErrorPrint(exception2.Message);
                return null;
            }
        Label_009F:
            return CommonUtilsClass.StrToByteArray(msg);
        }

        public void InvalidateAllWindows()
        {
            if (this.DisplayPanelSignal != null)
            {
                this.DisplayPanelSignal.Invalidate();
            }
            if (this.DisplayPanelSVs != null)
            {
                this.DisplayPanelSVs.Invalidate();
            }
            if (this.DisplayPanelSVTraj != null)
            {
                this.DisplayPanelSVTraj.Invalidate();
            }
            if (this.DisplayPanelSatelliteStats != null)
            {
                this.msgid4Update = true;
                this.DisplayPanelSatelliteStats.Invalidate();
            }
            if (this.DisplayPanelLocation != null)
            {
                this.DisplayPanelLocation.Invalidate();
            }
            if (this.DisplayPanelMEMS != null)
            {
                this.DisplayPanelMEMS.Invalidate();
            }
            if (this.DisplayPanelDRStatusStates != null)
            {
                this.DisplayPanelDRStatusStates.Invalidate();
            }
            if (this.DisplayPanelDRSensors != null)
            {
                this.DisplayPanelDRSensors.Invalidate();
            }
        }

        public bool IsSourceDeviceOpen()
        {
            bool isOpen = false;
            switch (this._InputDeviceMode)
            {
                case CommonClass.InputDeviceModes.RS232:
                    if (this.comPort != null)
                    {
                        isOpen = this.comPort.IsOpen;
                    }
                    return isOpen;

                case CommonClass.InputDeviceModes.TCP_Client:
                    if ((this.CMC != null) && (this.CMC.HostAppClient != null))
                    {
                        isOpen = this.CMC.HostAppClient.IsOpen();
                    }
                    return isOpen;

                case CommonClass.InputDeviceModes.TCP_Server:
                    if ((this.CMC != null) && (this.CMC.HostAppServer != null))
                    {
                        isOpen = this.CMC.HostAppServer.IsOpen();
                    }
                    return isOpen;

                case CommonClass.InputDeviceModes.FilePlayBack:
                    return isOpen;

                case CommonClass.InputDeviceModes.I2C:
                    if ((this.CMC != null) && (this.CMC.HostAppI2CSlave != null))
                    {
                        isOpen = this.CMC.HostAppI2CSlave.IsOpen();
                    }
                    return isOpen;
            }
            return isOpen;
        }

        public bool IsTTBNav()
        {
            bool flag = false;
            byte[] comByte = HelperFunctions.HexToByte(this.waitforMsgFromTTB(0xcc, 2));
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(comByte, "SSB");
            if ((hashtable != null) && hashtable.ContainsKey("Mode 1"))
            {
                byte num = (byte) (Convert.ToByte((string) hashtable["Mode 1"]) & 7);
                if (num >= 4)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private void ListenForClients()
        {
            this.CMC.HostAppServer.IsTCPServerConnected = false;
            this.CMC.HostAppServer.TCPServerListner.Start();
            while (true)
            {
                Console.WriteLine("Waiting for connection...");
                this.CMC.HostAppClient.TCPClient = this.CMC.HostAppServer.TCPServerListner.AcceptTcpClient();
                Console.WriteLine("Connection accepted.");
                this.CMC.HostAppServer.IsTCPServerConnected = true;
                this.CMC.HostAppServer.TCPServerStream = this.CMC.HostAppClient.TcpServerClient.GetStream();
                this._tcpipDataProcessThread = new Thread(new ThreadStart(this.OnInputTimerEvent));
                this._tcpipDataProcessThread.Start();
                Thread.CurrentThread.Join();
            }
        }

        public void ListenForClientToConnect()
        {
            this.CMC.HostAppServer.TCPServerListenerThread = new Thread(new ThreadStart(this.ListenForClients));
            this.CMC.HostAppServer.TCPServerListenerThread.Start();
        }

        public string LogToCSV(string hexStr)
        {
            if (hexStr.Contains("#"))
            {
                return hexStr;
            }
            return this.m_Protocols.ConvertRawToFields(HelperFunctions.HexToByte(hexStr));
        }

        private void msgQDisplay(string str)
        {
        }

        private void OnInputTimerEvent()
        {
            while (!this._closing)
            {
                try
                {
                    if (((this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Client) && this.CMC.HostAppClient.TCPClientStream.DataAvailable) && this.CMC.HostAppClient.TCPClientStream.CanRead)
                    {
                        byte[] buffer = new byte[0x100];
                        this.CMC.HostAppClient.TCPClientStream.Read(buffer, 0, 0x100);
                        lock (this._dataReadLock)
                        {
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                this._inDataBytes.Add(buffer[i]);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (!this._closing)
                    {
                        string errorStr = "CommunicationManager: OnInputTimerEvent() TCP Client - " + exception.ToString();
                        this.ErrorPrint(errorStr);
                    }
                }
                try
                {
                    if (((this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Server) && this.CMC.HostAppServer.TCPServerStream.DataAvailable) && this.CMC.HostAppServer.TCPServerStream.CanRead)
                    {
                        byte[] buffer2 = new byte[0x100];
                        this.CMC.HostAppClient.TCPClientStream.Read(buffer2, 0, 0x100);
                        lock (this._dataReadLock)
                        {
                            for (int j = 0; j < buffer2.Length; j++)
                            {
                                this._inDataBytes.Add(buffer2[j]);
                            }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    if (!this._closing)
                    {
                        string str2 = "CommunicationManager: OnInputTimerEvent() TCP Server - " + exception2;
                        this.ErrorPrint(str2);
                    }
                }
                Thread.Sleep(10);
            }
        }

        private void OnInputTimerEvent_I2C()
        {
        Label_0000:
            if (this._closing)
            {
                return;
            }
            try
            {
                if (this.CMC.HostAppI2CSlave.I2CTalkMode == CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE)
                {
                    if (!this.AutoDetectProtocolAndBaudDone || !this.I2CModeSwitchDone)
                    {
                        Thread.Sleep(10);
                        goto Label_0000;
                    }
                    if (this.CMC.HostAppI2CSlave.IsOpen())
                    {
                        byte[] buffer = new byte[0xffff];
                        short num = 0;
                        AardvarkApi.aa_i2c_read_ext(this.CMC.HostAppI2CSlave.I2CHandleMaster, this.CMC.HostAppI2CSlave.I2CMasterAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, 0x2000, buffer, ref num);
                        if (num > 0)
                        {
                            lock (this._dataReadLock)
                            {
                                for (int i = 0; i < num; i++)
                                {
                                    this._inDataBytes.Add(buffer[i]);
                                }
                            }
                        }
                        if (num < 0x2000)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                else
                {
                    if (!this.AutoDetectProtocolAndBaudDone || !this.I2CModeSwitchDone)
                    {
                        Thread.Sleep(10);
                        goto Label_0000;
                    }
                    if (this.CMC.HostAppI2CSlave.IsOpen())
                    {
                        byte addr = 0;
                        byte[] buffer2 = new byte[0xffff];
                        string errorStr = string.Empty;
                        for (int j = AardvarkApi.aa_async_poll(this.CMC.HostAppI2CSlave.I2CHandle, 500); j != 0; j = AardvarkApi.aa_async_poll(this.CMC.HostAppI2CSlave.I2CHandle, 0))
                        {
                            if (j == 1)
                            {
                                int status = AardvarkApi.aa_i2c_slave_read(this.CMC.HostAppI2CSlave.I2CHandle, ref addr, -1, buffer2);
                                if (status < 0)
                                {
                                    errorStr = string.Format("I2C: mode:{0}, error(Rx read): {1}", j, AardvarkApi.aa_status_string(status));
                                    this.ErrorPrint(errorStr);
                                }
                                lock (this._dataReadLock)
                                {
                                    for (int k = 0; k < status; k++)
                                    {
                                        this._inDataBytes.Add(buffer2[k]);
                                    }
                                    continue;
                                }
                            }
                            if (j == 4)
                            {
                                AardvarkApi.aa_spi_slave_read(this.CMC.HostAppI2CSlave.I2CHandle, -1, buffer2);
                                errorStr = string.Format("I2C: mode:{0}, error(Rx read)", j);
                                this.ErrorPrint(errorStr);
                            }
                            else if (j == 2)
                            {
                                if (AardvarkApi.aa_i2c_slave_write_stats(this.CMC.HostAppI2CSlave.I2CHandle) < 0)
                                {
                                    Thread.Sleep(10);
                                }
                            }
                            else
                            {
                                errorStr = string.Format("I2C: mode:{0}, error: No asynchronous data", j);
                                this.ErrorPrint(errorStr);
                                Thread.Sleep(10);
                                goto Label_028A;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (!this._closing)
                {
                    string str2 = "CommunicationManager: OnInputTimerEvent() I2C - " + exception.ToString();
                    this.ErrorPrint(str2);
                }
            }
        Label_028A:
            Thread.Sleep(10);
            goto Label_0000;
        }

        public bool OpenI2C()
        {
            try
            {
                this.comPort.BaudRate = int.Parse(this._baudRate);
                this.comPort.DataBits = int.Parse(this._dataBits);
                this.comPort.StopBits = (OpenNETCF.IO.Serial.StopBits) Enum.Parse(typeof(OpenNETCF.IO.Serial.StopBits), this._stopBits);
                this.comPort.Parity = (OpenNETCF.IO.Serial.Parity) Enum.Parse(typeof(OpenNETCF.IO.Serial.Parity), this._parity);
                if (this._InputDeviceMode == CommonClass.InputDeviceModes.I2C)
                {
                    if (this.CMC.HostAppI2CSlave.I2CTalkMode == CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE)
                    {
                        this._portName = "I2C" + this.CMC.HostAppI2CSlave.I2CDevicePortNumMaster.ToString();
                        this._portNum = this.CMC.HostAppI2CSlave.I2CDevicePortNumMaster.ToString();
                    }
                    else
                    {
                        this._portName = "I2C" + this.CMC.HostAppI2CSlave.I2CDevicePortNum.ToString();
                        this._portNum = this.CMC.HostAppI2CSlave.I2CDevicePortNum.ToString();
                    }
                }
                this.comPort.PortName = this._portName;
                this._sourceDeviceName = this._portName;
                if (!this.SetupRxCtrl())
                {
                    return false;
                }
                if (this._rxType != ReceiverType.SLC)
                {
                    ReceiverType type1 = this._rxType;
                }
                string inputData = "I2C opened at " + DateTime.Now;
                this.DebugPrint(CommonClass.MessageType.Normal, inputData);
                this._myWindowTitle = "Main " + this._portName + ": Connected";
                if (this.UpdateWinTitle != null)
                {
                    this.UpdateWinTitle(this._myWindowTitle);
                }
                return true;
            }
            catch (Exception exception)
            {
                this.ErrorPrint(exception.Message);
                return false;
            }
        }

        public string OpenI2CConnection()
        {
            this.OpenI2C();
            string str = string.Empty;
            if (this.CMC.HostAppI2CSlave.I2CTalkMode == CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE)
            {
                str = string.Concat(new object[] { "I2C: ", this.CMC.HostAppI2CSlave.I2CDevicePortNumMaster, ":", this.CMC.HostAppI2CSlave.I2CMasterAddress.ToString() });
            }
            else
            {
                str = string.Concat(new object[] { "I2C: ", this.CMC.HostAppI2CSlave.I2CDevicePortNum, ":", this.CMC.HostAppI2CSlave.I2CSlaveAddress.ToString() });
            }
            string inputData = string.Format(clsGlobal.MyCulture, "{0}, Rx Type: {1}", new object[] { str, this._rxType });
            this.DebugPrint(CommonClass.MessageType.Normal, inputData);
            if (!this.CMC.HostAppI2CSlave.Open())
            {
                str = "ERROR: " + str;
                this.ConnectErrorString = this.CMC.HostAppI2CSlave.ConnectErrorString;
                return str;
            }
            this.CMC.HostAppI2CSlave.init();
            this._I2CDataProcessThread = new Thread(new ThreadStart(this.OnInputTimerEvent_I2C));
            this._I2CDataProcessThread.Start();
            return str;
        }

        public bool OpenPort()
        {
            bool flag = false;
            this._closing = false;
            this.portDataInit();
            this.Log.DUTName = this._rxName;
            this.ConnectErrorString = string.Empty;
            if (this._messageProtocol == "NMEA")
            {
                clsGlobal.g_objfrmMDIMain.ChangeActionAutoTestState(false);
            }
            switch (this._InputDeviceMode)
            {
                case CommonClass.InputDeviceModes.RS232:
                    this.Log.PhysicalConnection = "UART";
                    flag = this.OpenSerialPort();
                    break;

                case CommonClass.InputDeviceModes.TCP_Client:
                    this.Log.PhysicalConnection = "TCP/IP";
                    this.SrcDeviceName = this.OpenTCPIPClient();
                    if (!this.SrcDeviceName.Contains("ERROR"))
                    {
                        flag = true;
                        break;
                    }
                    flag = false;
                    this._closing = true;
                    break;

                case CommonClass.InputDeviceModes.TCP_Server:
                    this.Log.PhysicalConnection = "TCP/IP";
                    this.SrcDeviceName = this.OpenTCPIPServer();
                    if (!this.SrcDeviceName.Contains("ERROR"))
                    {
                        flag = true;
                        break;
                    }
                    flag = false;
                    this._closing = true;
                    break;

                case CommonClass.InputDeviceModes.FilePlayBack:
                    this.Log.PhysicalConnection = "FilePlayBack";
                    flag = true;
                    break;

                case CommonClass.InputDeviceModes.I2C:
                    this.Log.PhysicalConnection = "I2C";
                    this.SrcDeviceName = this.OpenI2CConnection();
                    if (!this.SrcDeviceName.Contains("ERROR"))
                    {
                        flag = true;
                        break;
                    }
                    flag = false;
                    this._closing = true;
                    break;
            }
            if (File.Exists(this._autoReplyConfigFilePath))
            {
                try
                {
                    this.ReadAutoReplyData(this._autoReplyConfigFilePath);
                }
                catch
                {
                }
            }
            return flag;
        }

        private bool OpenSerialPort()
        {
            try
            {
                switch (this._flowControl)
                {
                    case 0:
                        this.comPort.Handshake = new FlowControlNone();
                        break;

                    case 1:
                        this.comPort.Handshake = new HandshakeXonXoff();
                        break;

                    case 2:
                        this.comPort.Handshake = new FlowControlHardware();
                        break;

                    case 3:
                        this.comPort.Handshake = new HandshakeDsrDtr();
                        break;
                }
                this.comPort.BaudRate = int.Parse(this._baudRate);
                this.comPort.DataBits = int.Parse(this._dataBits);
                this.comPort.StopBits = (OpenNETCF.IO.Serial.StopBits) Enum.Parse(typeof(OpenNETCF.IO.Serial.StopBits), this._stopBits);
                this.comPort.Parity = (OpenNETCF.IO.Serial.Parity) Enum.Parse(typeof(OpenNETCF.IO.Serial.Parity), this._parity);
                this.comPort.PortName = this._portName;
                this._sourceDeviceName = this._portName;
                this._portNum = this._portName.Trim(new char[] { 'C', 'c', 'O', 'o', 'M', 'm' });
                if (!this.SetupRxCtrl())
                {
                    return false;
                }
                this.comPort.Open();
                this.comPort.port.OnError += new Port.CommErrorEvent(this.comPort_OnError);
                if (this._rxType == ReceiverType.TTB)
                {
                    this.WriteData("A0A2 0009 CCA6 0002 0100 0000 0081 75B0 B3");
                    this.WriteData("A0A2 0009 CCA6 0004 0100 0000 0081 77B0 B3");
                }
                string str = string.Empty;
                switch (this._rxType)
                {
                    case ReceiverType.SLC:
                        str = "SLC";
                        break;

                    case ReceiverType.NMEA:
                        str = "NMEA";
                        break;

                    case ReceiverType.TTB:
                        str = "TTB";
                        break;

                    default:
                        str = "GSW";
                        break;
                }
                string inputData = string.Format("{0} Baud Rate: {1} RX Type: {2}", this.comPort.PortName, this.comPort.BaudRate, str);
                string str3 = "Port opened at " + DateTime.Now;
                this.DebugPrint(CommonClass.MessageType.Normal, str3);
                this.DebugPrint(CommonClass.MessageType.Normal, inputData);
                this._log.Write(str3 + "\r\n" + inputData + "\r\n");
                this._myWindowTitle = "Main " + this._portName + ": Connected";
                if (this.UpdateWinTitle != null)
                {
                    this.UpdateWinTitle(this._myWindowTitle);
                }
                return true;
            }
            catch (Exception exception)
            {
                this.ConnectErrorString = exception.Message;
                this.ErrorPrint(exception.Message);
                return false;
            }
        }

        public bool OpenTCPIP()
        {
            try
            {
                this.comPort.BaudRate = int.Parse(this._baudRate);
                this.comPort.DataBits = int.Parse(this._dataBits);
                this.comPort.StopBits = (OpenNETCF.IO.Serial.StopBits) Enum.Parse(typeof(OpenNETCF.IO.Serial.StopBits), this._stopBits);
                this.comPort.Parity = (OpenNETCF.IO.Serial.Parity) Enum.Parse(typeof(OpenNETCF.IO.Serial.Parity), this._parity);
                if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Client)
                {
                    this._portName = "TCP" + this.CMC.HostAppClient.TCPClientPortNum.ToString();
                    this._portNum = this.CMC.HostAppClient.TCPClientPortNum.ToString();
                }
                else if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Server)
                {
                    this._portName = "TCP" + this.CMC.HostAppServer.TCPServerPortNum.ToString();
                    this._portNum = this.CMC.HostAppServer.TCPServerPortNum.ToString();
                }
                this.comPort.PortName = this._portName;
                this._sourceDeviceName = this._portName;
                if (!this.SetupRxCtrl())
                {
                    return false;
                }
                if (this._rxType != ReceiverType.SLC)
                {
                    ReceiverType type1 = this._rxType;
                }
                string inputData = "TCP/IP opened at " + DateTime.Now;
                this.DebugPrint(CommonClass.MessageType.Normal, inputData);
                this._log.WriteLine(inputData);
                this._myWindowTitle = "Main " + this._portName + ": Connected";
                if (this.UpdateWinTitle != null)
                {
                    this.UpdateWinTitle(this._myWindowTitle);
                }
                return true;
            }
            catch (Exception exception)
            {
                this.ErrorPrint(exception.Message);
                return false;
            }
        }

        public string OpenTCPIPClient()
        {
            this.OpenTCPIP();
            string str = "TCP/IP Client: " + this.CMC.HostAppClient.TCPClientHostName + ":" + this.CMC.HostAppClient.TCPClientPortNum.ToString();
            string inputData = string.Format(clsGlobal.MyCulture, "{0}, Rx Type: {1}", new object[] { str, this._rxType });
            this.DebugPrint(CommonClass.MessageType.Normal, inputData);
            this.Log.Write(inputData + "\r\n");
            if (!this.CMC.HostAppClient.Open())
            {
                str = "ERROR: " + str;
                this.ConnectErrorString = this.CMC.HostAppClient.ConnectErrorString;
                return str;
            }
            this._tcpipDataProcessThread = new Thread(new ThreadStart(this.OnInputTimerEvent));
            this._tcpipDataProcessThread.IsBackground = true;
            this._tcpipDataProcessThread.Start();
            return str;
        }

        public string OpenTCPIPServer()
        {
            this.OpenTCPIP();
            string str = "TCP/IP Server: " + this.CMC.HostAppServer.TCPServerHostName + ":" + this.CMC.HostAppServer.TCPServerPortNum.ToString();
            string inputData = string.Format(clsGlobal.MyCulture, "{0}, Rx Type: {1}", new object[] { str, this._rxType });
            this.DebugPrint(CommonClass.MessageType.Normal, inputData);
            this.Log.Write(inputData + "\r\n");
            this._tcpipDataProcessThread = new Thread(new ThreadStart(this.OnInputTimerEvent));
            if (!this.CMC.HostAppServer.Open())
            {
                return ("ERROR: " + str);
            }
            this.ListenForClientToConnect();
            return str;
        }

        public bool OpenTTBPort(int comNumber)
        {
            if (this.TTBPort != null)
            {
                try
                {
                    if (this.TTBPort.IsOpen)
                    {
                        this.TTBPort.Close();
                        this.TTBPort = new CommWrapper();
                    }
                    this.TTBPort.PortName = "COM" + comNumber.ToString();
                    this.TTBPort.BaudRate = 0xe100;
                    this.TTBPort.DataBits = 8;
                    this.TTBPort.Parity = (OpenNETCF.IO.Serial.Parity) Enum.Parse(typeof(OpenNETCF.IO.Serial.Parity), "None");
                    this.TTBPort.StopBits = (OpenNETCF.IO.Serial.StopBits) Enum.Parse(typeof(OpenNETCF.IO.Serial.StopBits), "One");
                    this.TTBPort.Open();
                    return true;
                }
                catch (Exception exception)
                {
                    string str = "Open TTB port: " + exception.Message;
                    this.ErrorPrint(str);
                    return false;
                }
            }
            string errorStr = "Open TTB port not specified";
            this.ErrorPrint(errorStr);
            return false;
        }

        public void PollICTrackerSettings(byte[] fullMsgArr)
        {
            Hashtable hashtable = this.m_Protocols.ConvertRawToHash(fullMsgArr, "SSB");
            if (hashtable.ContainsKey("Reference Clock Frequency"))
            {
                this.dataICTrack.RefFreq = Convert.ToUInt32((string) hashtable["Reference Clock Frequency"]);
            }
            if (hashtable.ContainsKey("Reference Clock Warmup Delay"))
            {
                this.dataICTrack.StartupDelay = Convert.ToUInt16((string) hashtable["Reference Clock Warmup Delay"]);
            }
            if (hashtable.ContainsKey("Reference Clock Uncertainty"))
            {
                this.dataICTrack.RefClkUncertainty = Convert.ToUInt32((string) hashtable["Reference Clock Uncertainty"]);
            }
            if (hashtable.ContainsKey("Reference Clock Offset"))
            {
                this.dataICTrack.RefClkOffset = Convert.ToSingle((string) hashtable["Reference Clock Offset"]);
                double num = Convert.ToSingle(this.dataICTrack.RefFreq);
                double num2 = Convert.ToSingle((double) (((num * Math.Pow(10.0, -6.0)) * 1540.0) / 16.0));
                double num3 = Convert.ToSingle((double) (((num * 1540.0) / 16.0) - clsGlobal.GPSL1Frequency));
                this.dataICTrack.RefClkOffset = Convert.ToSingle((double) ((this.dataICTrack.RefClkOffset - num3) / num2));
            }
            if (hashtable.ContainsKey("External LNA Enable"))
            {
                this.dataICTrack.LNASelect = Convert.ToByte((string) hashtable["External LNA Enable"]);
            }
            if (hashtable.ContainsKey("IO Pin Config Enable"))
            {
                this.dataICTrack.IOPinConfigEnable = Convert.ToByte((string) hashtable["IO Pin Config Enable"]);
            }
            for (int i = 0; i < 11; i++)
            {
                string key = "IO Pin Config GPIO" + i.ToString();
                if (hashtable.ContainsKey(key))
                {
                    ushort num5 = Convert.ToUInt16((string) hashtable[key]);
                    this.dataICTrack.IOPinConfigs[i] = num5;
                }
            }
            if (hashtable.ContainsKey("UART Max Premable"))
            {
                this.dataICTrack.UARTPreambleMax = Convert.ToByte((string) hashtable["UART Max Premable"]);
            }
            if (hashtable.ContainsKey("UART Idle Byte Wakeup Delay"))
            {
                this.dataICTrack.UARTWakeupDelay = Convert.ToByte((string) hashtable["UART Idle Byte Wakeup Delay"]);
            }
            if (hashtable.ContainsKey("UART Baud"))
            {
                this.dataICTrack.UARTBaud = Convert.ToUInt32((string) hashtable["UART Baud"]);
            }
            if (hashtable.ContainsKey("UART HW Flow Control"))
            {
                this.dataICTrack.UARTFlowControlEnable = Convert.ToByte((string) hashtable["UART HW Flow Control"]);
            }
            if (hashtable.ContainsKey("I2C Master Address"))
            {
                this.dataICTrack.I2CMasterAddress = Convert.ToUInt16((string) hashtable["I2C Master Address"]);
            }
            if (hashtable.ContainsKey("I2C Slave Address"))
            {
                this.dataICTrack.I2CSlaveAddress = Convert.ToUInt16((string) hashtable["I2C Slave Address"]);
            }
            if (hashtable.ContainsKey("I2C Rate"))
            {
                this.dataICTrack.I2CRate = Convert.ToByte((string) hashtable["I2C Rate"]);
            }
            if (hashtable.ContainsKey("I2C Mode"))
            {
                this.dataICTrack.I2CMode = Convert.ToByte((string) hashtable["I2C Mode"]);
            }
            if (hashtable.ContainsKey("I2C Max Message length"))
            {
                this.dataICTrack.I2CMaxMsgLength = Convert.ToUInt16((string) hashtable["I2C Max Message length"]);
            }
            if (hashtable.ContainsKey("PowerControl On Off"))
            {
                this.dataICTrack.PwrCtrlOnOff = Convert.ToByte((string) hashtable["PowerControl On Off"]);
            }
            if (hashtable.ContainsKey("Backup LDO mode enabled"))
            {
                this.dataICTrack.LDOModeEnabled = Convert.ToByte((string) hashtable["Backup LDO mode enabled"]);
            }
        }

        public int PopulateData(byte[] bData, int len)
        {
            if (len > 0)
            {
                lock (this._dataReadLock)
                {
                    for (int i = 0; i < len; i++)
                    {
                        this._inDataBytes.Add(bData[i]);
                    }
                }
            }
            return len;
        }

        public void portDataInit()
        {
            this.dataGui.Positions.PositionList.Clear();
            this._CSVBuff.Initialize();
            this._idx_CSVBuff = 0;
            this._headerDetected = false;
            lock (this._dataReadLock)
            {
                this._inDataBytes.Clear();
            }
            lock (this.DisplayDataLock)
            {
                this.DisplayQueue.Clear();
            }
        }

        private void posReqAckHandler()
        {
            this.AutoReplyCtrl.PosReqAck = true;
            if ((((this.AutoReplyCtrl.HWCfgCtrl.NetworkEnhanceType & 4) == 4) && ((this.AutoReplyCtrl.HWCfgCtrl.NetworkEnhanceType & 8) != 8)) && ((this.AutoReplyCtrl.HWCfgCtrl.NetworkEnhanceType & 0x10) == 0x10))
            {
                if (this.AutoReplyCtrl.SF45DataSet0MsgFromTTB.Contains("Invalid"))
                {
                    this.WriteApp(this.AutoReplyCtrl.SF45DataSet0MsgFromTTB);
                }
                else
                {
                    this.WriteApp("Sending subframe 4,5");
                    this.WriteData(this.AutoReplyCtrl.SF45DataSet0MsgFromTTB);
                }
                if (this.AutoReplyCtrl.SF45DataSet1MsgFromTTB.Contains("Invalid"))
                {
                    this.WriteApp(this.AutoReplyCtrl.SF45DataSet1MsgFromTTB);
                }
                else
                {
                    this.WriteApp("Sending subframe 4,5");
                    this.WriteData(this.AutoReplyCtrl.SF45DataSet1MsgFromTTB);
                }
            }
            if (this.AutoReplyCtrl.PushAidingAvailability)
            {
                if (this.AutoReplyCtrl.PushAidingDelay != 0)
                {
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Elapsed += new ElapsedEventHandler(this.SendPushAiding);
                    timer.Interval = this.AutoReplyCtrl.PushAidingDelay * 0x3e8;
                    timer.AutoReset = false;
                    timer.Start();
                }
                else
                {
                    this.RxCtrl.SendPushAiding(this.AutoReplyCtrl.PushAidingMask, this.AutoReplyCtrl.ForceAidingRequestMask);
                }
            }
        }

        public void PrepareAutoReplyData()
        {
            this.AutoReplyCtrl.AidingFlag = 0;
            if (this.AutoReplyCtrl.AutoReplyParams.AutoReply)
            {
                this.AutoReplyCtrl.AutoReplyHWCfgResp();
                this.AutoReplyCtrl.AutoReplyFreqTransferResp();
                this.AutoReplyCtrl.AutoReplyApproxPositionResp();
                this.AutoReplyCtrl.AutoSendPositionRequestMsg();
            }
            if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB && this.TTBPort.IsOpen)
            {
                string ephDataFromTTB = this.GetEphDataFromTTB();
                if (ephDataFromTTB != string.Empty)
                {
                    this.AutoReplyCtrl.EphDataMsg = ephDataFromTTB;
                    this.AutoReplyCtrl.EphDataMsgBackup = ephDataFromTTB;
                }
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 1);
            }
            else if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile)
            {
                string str2 = this.AutoReplyCtrl.Get2HoursEphAidingMsgFromFile(this.AutoReplyCtrl.ControlChannelVersion, this.AutoReplyCtrl.EphFilePath);
                if (str2 != string.Empty)
                {
                    if (str2.Contains("Error"))
                    {
                        this.WriteApp("### " + str2 + " ###");
                    }
                    else
                    {
                        this.AutoReplyCtrl.EphDataMsg = str2;
                        this.AutoReplyCtrl.EphDataMsgBackup = str2;
                    }
                }
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 1);
            }
            else if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile)
            {
                GPSDateTime time = this.RxCtrl.ResetCtrl.ResetGPSTimer.GetTime();
                int gPSWeek = time.GetGPSWeek();
                double gPSTOW = time.GetGPSTOW();
                double num3 = (gPSWeek * 0x93a80) + gPSTOW;
                string[] strArray = num3.ToString().Split(new char[] { '.' });
                string str3 = this.AutoReplyCtrl.GetEphAidingMsgFromFile(this.AutoReplyCtrl.ControlChannelVersion, this.AutoReplyCtrl.EphFilePath, strArray[0]);
                if (str3 != string.Empty)
                {
                    if (str3.Contains("Error"))
                    {
                        this.WriteApp("### " + str3 + " ###");
                    }
                    else
                    {
                        this.AutoReplyCtrl.EphDataMsg = str3;
                        this.AutoReplyCtrl.EphDataMsgBackup = str3;
                    }
                }
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 1);
            }
            if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromTTB && this.TTBPort.IsOpen)
            {
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 2);
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag & 0xfe);
            }
            else if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromFile)
            {
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 2);
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag & 0xfe);
            }
            if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_Alm && this.TTBPort.IsOpen)
            {
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 4);
            }
            if (this.AutoReplyCtrl.AutoReplyParams.AutoAid_NavBit && this.TTBPort.IsOpen)
            {
                string str4 = string.Empty;
                this.AutoReplyCtrl.SetupAuxNavMsgFromTTB(this.GetAuxNavDataFromTTB());
                str4 = this.AutoReplyCtrl.SetupNavSF45FromTTB(this.GetNavBitSF45DataSet0FromTTB());
                if (str4 != string.Empty)
                {
                    this.AutoReplyCtrl.SF45DataSet0MsgFromTTB = str4;
                }
                str4 = this.AutoReplyCtrl.SetupNavSF45FromTTB(this.GetNavBitSF45DataSet1FromTTB());
                if (str4 != string.Empty)
                {
                    this.AutoReplyCtrl.SF45DataSet1MsgFromTTB = str4;
                }
                this.AutoReplyCtrl.AidingFlag = (byte) (this.AutoReplyCtrl.AidingFlag | 8);
            }
        }

        private void processPosRequest()
        {
            if (this.AutoReplyCtrl.AutoReplyParams.AutoPosReq && this.RxCtrl.AllowSessOpen)
            {
                this.RxCtrl.OpenSession(0x71);
                this.RxCtrl.SendPositionRequest(this.AutoReplyCtrl.AidingFlag);
            }
        }

        public void ReadAutoReplyData(string filepath)
        {
            IniHelper helper = new IniHelper(filepath);
            string section = string.Empty;
            string key = string.Empty;
            string msg = string.Empty;
            section = "TTB_TIME_AIDING";
            key = "ENABLE";
            try
            {
                this.AutoReplyCtrl.TTBTimeAidingParams.Enable = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
                if (this.AutoReplyCtrl.TTBTimeAidingParams.Enable)
                {
                    this.AutoReplyCtrl.TimeTransferCtrl.Reply = true;
                    this.AutoReplyCtrl.AutoReplyParams.AutoReplyTimeTrans = true;
                }
            }
            catch (Exception exception)
            {
                msg = "TTB_TIME_AIDING ENABLE: " + exception.Message;
                this.WriteApp(msg);
            }
            key = "TYPE";
            try
            {
                this.AutoReplyCtrl.TTBTimeAidingParams.Type = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception2)
            {
                msg = "TTB_TIME_AIDING TYPE: " + exception2.Message;
                this.WriteApp(msg);
            }
            key = "TIME_ACC";
            try
            {
                this.AutoReplyCtrl.TTBTimeAidingParams.Accuracy = Convert.ToUInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception3)
            {
                msg = "TTB_TIME_AIDING TIME_ACC: " + exception3.Message;
                this.WriteApp(msg);
            }
            key = "TIME_SKEW";
            try
            {
                this.AutoReplyCtrl.TTBTimeAidingParams.Skew = Convert.ToUInt32(helper.IniReadValue(section, key));
            }
            catch (Exception exception4)
            {
                msg = "TTB_TIME_AIDING TIME_ACC: " + exception4.Message;
                this.WriteApp(msg);
            }
            section = "FREQ_AIDING";
            key = "REPLY";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.Reply = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
                this.AutoReplyCtrl.AutoReplyParams.AutoReplyFreqTrans = this.AutoReplyCtrl.FreqTransferCtrl.Reply;
            }
            catch (Exception exception5)
            {
                msg = "FREQ_AIDING REPLY: " + exception5.Message;
                this.WriteApp(msg);
            }
            key = "USE_FREQ_AIDING";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.UseFreqAiding = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception6)
            {
                msg = "USE_FREQ_AIDING: " + exception6.Message;
                this.WriteApp(msg);
            }
            key = "TIME_TAG";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.TimeTag = Convert.ToUInt32(helper.IniReadValue(section, key));
            }
            catch (Exception exception7)
            {
                msg = "FREQ_AIDING TIME_TAG: " + exception7.Message;
                this.WriteApp(msg);
            }
            key = "REF_CLOCK_INFO";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.RefClkInfo = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception8)
            {
                msg = "FREQ_AIDING REF_CLOCK_INFO: " + exception8.Message;
                this.WriteApp(msg);
            }
            key = "REL_FREQ_ACC";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.Accuracy = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception9)
            {
                msg = "FREQ_AIDING REL_FREQ_ACC: " + exception9.Message;
                this.WriteApp(msg);
            }
            key = "SCALED_FREQ_OFFSET";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.ScaledFreqOffset = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception10)
            {
                msg = "FREQ_AIDING SCALED_FREQ_OFFSET: " + exception10.Message;
                this.WriteApp(msg);
            }
            key = "EXT_CLOCK_SKEW";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.ExtClkSkewppm = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception11)
            {
                msg = "FREQ_AIDING EXT_CLOCK_SKEW: " + exception11.Message;
                this.WriteApp(msg);
            }
            key = "NORMIMAL_FREQ";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.NomFreq = Convert.ToInt64(helper.IniReadValue(section, key));
            }
            catch (Exception exception12)
            {
                msg = "FREQ_AIDING NORMIMAL_FREQ: " + exception12.Message;
                this.WriteApp(msg);
            }
            key = "INCLUDE_NORM_FREQ";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.IncludeNormFreq = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception13)
            {
                msg = "FREQ_AIDING NORMIMAL_FREQ: " + exception13.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_METHOD";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqAidingMethod = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception14)
            {
                msg = "FREQ_AIDING FREQ_METHOD: " + exception14.Message;
                this.WriteApp(msg);
            }
            key = "DEFAULT_FREQ_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.DefaultFreqIndex = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception15)
            {
                msg = "FREQ_AIDING DEFAULT_FREQ_GUI_INDEX: " + exception15.Message;
                this.WriteApp(msg);
            }
            this._lastClockDrift = clsGlobal.DEFAULT_RF_FREQ[this.AutoReplyCtrl.FreqTransferCtrl.DefaultFreqIndex];
            key = "SPECIFIED_FREQ_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.SpecifiedRefFreq = Convert.ToInt16(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception16)
            {
                msg = "FREQ_AIDING SPECIFIED_FREQ_GUI_INDEX: " + exception16.Message;
                this.WriteApp(msg);
            }
            key = "SLC_REPORT_FREQ_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.SLCReportFreqGuiIndex = Convert.ToInt16(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception17)
            {
                msg = "FREQ_AIDING SLC_REPORT_FREQ_GUI_INDEX: " + exception17.Message;
                this.WriteApp(msg);
            }
            key = "REF_CLOCK_REQUEST_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.RefClockRequestGuiIndex = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception18)
            {
                msg = "FREQ_AIDING REF_CLOCK_REQUEST_GUI_INDEX: " + exception18.Message;
                this.WriteApp(msg);
            }
            key = "REF_CLOCK_ONOFF_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.RefClockOnOffGuiIndex = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception19)
            {
                msg = "FREQ_AIDING REF_CLOCK_REQUEST_GUI_INDEX: " + exception19.Message;
                this.WriteApp(msg);
            }
            key = "EXT_REF_CLOCK_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.ExtRefClockGuiIndex = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception20)
            {
                msg = "FREQ_AIDING EXT_REF_CLOCK_GUI_INDEX: " + exception20.Message;
                this.WriteApp(msg);
            }
            key = "SCALED_FREQ_OFFSET_GUI_INDEX";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.ScaledFreqOffsetGuiIndex = Convert.ToInt16(helper.IniReadValue(section, key));
            }
            catch (Exception exception21)
            {
                msg = "FREQ_AIDING SCALED_FREQ_OFFSET_GUI_INDEX: " + exception21.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_ACC_USER_GUI";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqAccUserSpecifiedGui = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception22)
            {
                msg = "FREQ_ACC_USER_GUI: " + exception22.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_OFFSET_USER_GUI";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqOffsetUserSpecifiedGui = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception23)
            {
                msg = "FREQ_OFFSET_USER_GUI: " + exception23.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_ACC_RX_GUI";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqAccFromRxGui = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception24)
            {
                msg = "FREQ_ACC_RX_GUI: " + exception24.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_OFFSET_RX_GUI";
            try
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqOffsetFromRxGui = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception25)
            {
                msg = "FREQ_OFFSET_RX_GUI: " + exception25.Message;
                this.WriteApp(msg);
            }
            if (this.AutoReplyCtrl.FreqTransferCtrl.SpecifiedRefFreq)
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqOffset = this.AutoReplyCtrl.FreqTransferCtrl.FreqOffsetUserSpecifiedGui;
                this.AutoReplyCtrl.FreqTransferCtrl.Accuracy = this.AutoReplyCtrl.FreqTransferCtrl.FreqAccUserSpecifiedGui;
            }
            else
            {
                this.AutoReplyCtrl.FreqTransferCtrl.FreqOffset = this.AutoReplyCtrl.FreqTransferCtrl.FreqOffsetFromRxGui;
                this.AutoReplyCtrl.FreqTransferCtrl.Accuracy = this.AutoReplyCtrl.FreqTransferCtrl.FreqAccFromRxGui;
            }
            key = "USE_TTB_FREQ";
            try
            {
                this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForFreqAid = Convert.ToInt32(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception26)
            {
                msg = "USE_TTB_FREQ: " + exception26.Message;
                this.WriteApp(msg);
            }
            key = "IGNORE_XO";
            try
            {
                this.AutoReplyCtrl.AutoReplyParams.FreqAidingIgnoreXO = Convert.ToInt32(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception27)
            {
                msg = "IGNORE_XO: " + exception27.Message;
                this.WriteApp(msg);
            }
            this.AutoReplyCtrl.AutoReplyFreqTransferResp();
            section = "HW_CONFIG";
            key = "REPLY";
            if (helper.IniReadValue(section, key) == "1")
            {
                this.AutoReplyCtrl.HWCfgCtrl.Reply = true;
                if (this.AutoReplyCtrl.TTBTimeAidingParams.Enable || this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForFreqAid)
                {
                    this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForHwCfg = true;
                }
                else
                {
                    this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForHwCfg = false;
                }
            }
            else
            {
                this.AutoReplyCtrl.HWCfgCtrl.Reply = false;
            }
            this.AutoReplyCtrl.AutoReplyParams.AutoReplyHWCfg = this.AutoReplyCtrl.HWCfgCtrl.Reply;
            key = "PRECISE_TIME_ENABLED";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.PreciseTimeEnabled = Convert.ToByte(helper.IniReadValue(section, key));
                if (this.AutoReplyCtrl.HWCfgCtrl.PreciseTimeEnabled == 1)
                {
                    if (this.AutoReplyCtrl.TTBTimeAidingParams.Enable)
                    {
                        this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = true;
                    }
                    else
                    {
                        this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = false;
                    }
                }
            }
            catch (Exception exception28)
            {
                msg = "PRECISE_TIME_ENABLED: " + exception28.Message;
                this.WriteApp(msg);
            }
            key = "PRECISE_TIME_DIRECTION";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.PreciseTimeDirection = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception29)
            {
                msg = "PRECISE_TIME_DIRECTION: " + exception29.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_AIDED_ENABLED";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception30)
            {
                msg = "FREQ_AIDED_ENABLED: " + exception30.Message;
                this.WriteApp(msg);
            }
            key = "FREQ_AIDED_METHOD";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.FreqAidMethod = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception31)
            {
                msg = "FREQ_AIDED_METHOD: " + exception31.Message;
                this.WriteApp(msg);
            }
            key = "RTC_AVAILABLE";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.RTCAvailabe = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception32)
            {
                msg = "RTC_AVAILABLE: " + exception32.Message;
                this.WriteApp(msg);
            }
            key = "RTC_SOURCE";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.RTCSource = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception33)
            {
                msg = "RTC_SOURCE: " + exception33.Message;
                this.WriteApp(msg);
            }
            key = "COARSE_TIME_ENABLE";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.CoarseTimeEnabled = Convert.ToByte(helper.IniReadValue(section, key));
                if (this.AutoReplyCtrl.HWCfgCtrl.CoarseTimeEnabled == 1)
                {
                    if (this.AutoReplyCtrl.TTBTimeAidingParams.Enable)
                    {
                        this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = true;
                    }
                    else
                    {
                        this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = false;
                    }
                }
            }
            catch (Exception exception34)
            {
                msg = "COARSE_TIME_ENABLE: " + exception34.Message;
                this.WriteApp(msg);
            }
            key = "REF_CLOCK_ENABLED";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.RefClkEnabled = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception35)
            {
                msg = "REF_CLOCK_ENABLED: " + exception35.Message;
                this.WriteApp(msg);
            }
            key = "NORMINAL_FREQ_HZ";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.NorminalFreqHz = Convert.ToInt64(helper.IniReadValue(section, key));
            }
            catch (Exception exception36)
            {
                msg = "NORMINAL_FREQ_HZ: " + exception36.Message;
                this.WriteApp(msg);
            }
            key = "ENHANCED_NETWORK";
            try
            {
                this.AutoReplyCtrl.HWCfgCtrl.NetworkEnhanceType = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception37)
            {
                msg = "ENHANCED_NETWORK: " + exception37.Message;
                this.WriteApp(msg);
            }
            this.AutoReplyCtrl.AutoReplyHWCfgResp();
            section = "APPROXIMATE_POSITION";
            key = "REPLY";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.Reply = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception38)
            {
                msg = "APPROXIMATE_POSITION REPLY: " + exception38.Message;
                this.WriteApp(msg);
            }
            this.AutoReplyCtrl.AutoReplyParams.AutoReplyApproxPos = this.AutoReplyCtrl.ApproxPositionCtrl.Reply;
            key = "REJECT";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.Reject = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception39)
            {
                msg = "APPROXIMATE_POSITION REJECT: " + exception39.Message;
                this.WriteApp(msg);
            }
            key = "LAT";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.Lat = Convert.ToDouble(helper.IniReadValue(section, key));
                this.RxCtrl.RxNavData.RefLat = this.AutoReplyCtrl.ApproxPositionCtrl.Lat;
            }
            catch (Exception exception40)
            {
                msg = "APPROXIMATE_POSITION LAT: " + exception40.Message;
                this.WriteApp(msg);
            }
            key = "LON";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.Lon = Convert.ToDouble(helper.IniReadValue(section, key));
                this.RxCtrl.RxNavData.RefLon = this.AutoReplyCtrl.ApproxPositionCtrl.Lon;
            }
            catch (Exception exception41)
            {
                msg = "APPROXIMATE_POSITION LON: " + exception41.Message;
                this.WriteApp(msg);
            }
            key = "ALT";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.Alt = Convert.ToDouble(helper.IniReadValue(section, key));
                this.RxCtrl.RxNavData.RefAlt = this.AutoReplyCtrl.ApproxPositionCtrl.Alt;
            }
            catch (Exception exception42)
            {
                msg = "APPROXIMATE_POSITION ALT: " + exception42.Message;
                this.WriteApp(msg);
            }
            key = "EST_HOR_ERR";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.EstHorrErr = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception43)
            {
                msg = "APPROXIMATE_POSITION EST_HOR_ERR: " + exception43.Message;
                this.WriteApp(msg);
            }
            key = "EST_VER_ERR";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.EstVertiErr = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception44)
            {
                msg = "APPROXIMATE_POSITION EST_VER_ERR: " + exception44.Message;
                this.WriteApp(msg);
            }
            key = "LAT_SKEW";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.DistanceSkew = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception45)
            {
                msg = "APPROXIMATE_POSITION LAT_SKEW: " + exception45.Message;
                this.WriteApp(msg);
            }
            key = "LON_SKEW";
            try
            {
                this.AutoReplyCtrl.ApproxPositionCtrl.HeadingSkew = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception46)
            {
                msg = "APPROXIMATE_POSITION LON_SKEW: " + exception46.Message;
                this.WriteApp(msg);
            }
            key = "LOC_NAME";
            this.m_NavData.RefLocationName = helper.IniReadValue(section, key);
            this.AutoReplyCtrl.AutoReplyApproxPositionResp();
            section = "TIME_AIDING";
            key = "REPLY";
            try
            {
                this.AutoReplyCtrl.TimeTransferCtrl.Reply = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
                this.AutoReplyCtrl.AutoReplyParams.AutoReplyTimeTrans = this.AutoReplyCtrl.TimeTransferCtrl.Reply;
            }
            catch (Exception exception47)
            {
                msg = "TIME_AIDING REPLY: " + exception47.Message;
                this.WriteApp(msg);
            }
            key = "REJECT";
            try
            {
                this.AutoReplyCtrl.TimeTransferCtrl.Reject = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception48)
            {
                msg = "TIME_AIDING REJECT: " + exception48.Message;
                this.WriteApp(msg);
            }
            key = "TIME_AIDING_TYPE";
            try
            {
                this.AutoReplyCtrl.TimeTransferCtrl.TTType = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception49)
            {
                msg = "TIME_AIDING_TYPE: " + exception49.Message;
                this.WriteApp(msg);
            }
            key = "TIME_ACC";
            try
            {
                this.AutoReplyCtrl.TimeTransferCtrl.Accuracy = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception50)
            {
                msg = "TIME_ACC: " + exception50.Message;
                this.WriteApp(msg);
            }
            key = "SKEW";
            try
            {
                this.AutoReplyCtrl.TimeTransferCtrl.Skew = Convert.ToDouble(helper.IniReadValue(section, key));
            }
            catch (Exception exception51)
            {
                msg = "SKEW: " + exception51.Message;
                this.WriteApp(msg);
            }
            key = "UTC_OFFSET";
            try
            {
                this.RxCtrl.UTCOffset = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception52)
            {
                msg = "UTC Offset: " + exception52.Message;
                this.WriteApp(msg);
            }
            key = "TIME_AIDING_SOURCE";
            int num = 0;
            try
            {
                num = Convert.ToInt32(helper.IniReadValue(section, key));
            }
            catch (Exception exception53)
            {
                msg = "TIME_AIDING_SOURCE: " + exception53.Message;
                this.WriteApp(msg);
            }
            switch (num)
            {
                case 1:
                    this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = false;
                    this.AutoReplyCtrl.AutoReplyParams.UseDOS_ForTimeAid = true;
                    this.AutoReplyCtrl.TimeTransferCtrl.Reject = false;
                    break;

                case 2:
                    this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = true;
                    this.AutoReplyCtrl.AutoReplyParams.UseDOS_ForTimeAid = false;
                    this.AutoReplyCtrl.TimeTransferCtrl.Reject = false;
                    break;

                default:
                    this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid = false;
                    this.AutoReplyCtrl.AutoReplyParams.UseDOS_ForTimeAid = false;
                    this.AutoReplyCtrl.TimeTransferCtrl.Reject = true;
                    break;
            }
            this.AutoReplyCtrl.AutoReplyTimeTransferResp();
            section = "POSITION_AIDING";
            key = "REPLY";
            try
            {
                this.AutoReplyCtrl.AutoReplyParams.AutoPosReq = Convert.ToByte(helper.IniReadValue(section, key)) == 1;
            }
            catch (Exception exception54)
            {
                msg = "POSITION_AIDING REPLY: " + exception54.Message;
                this.WriteApp(msg);
            }
            key = "NUM_FIXED";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.NumFixes = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception55)
            {
                msg = "NUM_FIXED: " + exception55.Message;
                this.WriteApp(msg);
            }
            key = "TIME_BETWEEN_FIXES";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.TimeBtwFixes = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception56)
            {
                msg = "TIME_BETWEEN_FIXES: " + exception56.Message;
                this.WriteApp(msg);
            }
            key = "HOR_ERR_MAX";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.HorrErrMax = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception57)
            {
                msg = "HOR_ERR_MAX: " + exception57.Message;
                this.WriteApp(msg);
            }
            key = "VERT_ERR_MAX";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.VertErrMax = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception58)
            {
                msg = "VERT_ERR_MAX: " + exception58.Message;
                this.WriteApp(msg);
            }
            key = "RESP_TIME_MAX";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.RespTimeMax = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception59)
            {
                msg = "RESP_TIME_MAX: " + exception59.Message;
                this.WriteApp(msg);
            }
            key = "TIME_ACC_PRIORITY";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.TimeAccPriority = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception60)
            {
                msg = "TIME_ACC_PRIORITY: " + exception60.Message;
                this.WriteApp(msg);
            }
            key = "LOC_METHOD";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.LocMethod = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception61)
            {
                msg = "LOC_METHOD: " + exception61.Message;
                this.WriteApp(msg);
            }
            key = "EPH_SOURCE";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.EphSource = Convert.ToByte(helper.IniReadValue(section, key));
                switch (this.AutoReplyCtrl.PositionRequestCtrl.EphSource)
                {
                    case 0:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile = false;
                        goto Label_1551;

                    case 1:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile = true;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile = false;
                        goto Label_1551;

                    case 2:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB = true;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile = false;
                        goto Label_1551;

                    case 3:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile = true;
                        goto Label_1551;
                }
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromFile = false;
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_Eph_fromTTB = false;
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_ExtEph_fromFile = false;
            }
            catch (Exception exception62)
            {
                msg = "EPH_SOURCE: " + exception62.Message;
                this.WriteApp(msg);
            }
        Label_1551:
            key = "EPH_FILEPATH";
            this.AutoReplyCtrl.EphFilePath = helper.IniReadValue(section, key);
            key = "EPH_REPLY";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.EphReply = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception63)
            {
                msg = "EPH_REPLY: " + exception63.Message;
                this.WriteApp(msg);
            }
            key = "ACQ_ASSIST_SOURCE";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.AcqAssistSource = Convert.ToByte(helper.IniReadValue(section, key));
                switch (this.AutoReplyCtrl.PositionRequestCtrl.AcqAssistSource)
                {
                    case 0:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromFile = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromTTB = false;
                        goto Label_16A3;

                    case 1:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromFile = false;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromTTB = true;
                        goto Label_16A3;

                    case 2:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromFile = true;
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromTTB = false;
                        goto Label_16A3;
                }
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromFile = false;
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_AcqData_fromTTB = false;
            }
            catch (Exception exception64)
            {
                msg = "ACQ_ASSIST_SOURCE: " + exception64.Message;
                this.WriteApp(msg);
            }
        Label_16A3:
            key = "ACQ_ASSIST_REPLY";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.AcqAssistReply = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception65)
            {
                msg = "ACQ_ASSIST_REPLY: " + exception65.Message;
                this.WriteApp(msg);
            }
            key = "ACQ_ASSIST_FILEPATH";
            this.AutoReplyCtrl.AcqDataFilePath = helper.IniReadValue(section, key);
            key = "ALM_SOURCE";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.AlmSource = Convert.ToByte(helper.IniReadValue(section, key));
                switch (this.AutoReplyCtrl.PositionRequestCtrl.AlmSource)
                {
                    case 0:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Alm = false;
                        goto Label_179A;

                    case 1:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_Alm = true;
                        goto Label_179A;
                }
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_Alm = false;
            }
            catch (Exception exception66)
            {
                msg = "ALM_SOURCE: " + exception66.Message;
                this.WriteApp(msg);
            }
        Label_179A:
            key = "ALM_REPLY";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.AlmReply = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception67)
            {
                msg = "ALM_REPLY: " + exception67.Message;
                this.WriteApp(msg);
            }
            key = "NAVBIT_SOURCE";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.NavBitSource = Convert.ToByte(helper.IniReadValue(section, key));
                switch (this.AutoReplyCtrl.PositionRequestCtrl.NavBitSource)
                {
                    case 0:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_NavBit = false;
                        goto Label_1878;

                    case 1:
                        this.AutoReplyCtrl.AutoReplyParams.AutoAid_NavBit = true;
                        goto Label_1878;
                }
                this.AutoReplyCtrl.AutoReplyParams.AutoAid_NavBit = false;
            }
            catch (Exception exception68)
            {
                msg = "NAVBIT_SOURCE: " + exception68.Message;
                this.WriteApp(msg);
            }
        Label_1878:
            key = "NAVBIT_REPLY";
            try
            {
                this.AutoReplyCtrl.PositionRequestCtrl.NavBitReply = Convert.ToByte(helper.IniReadValue(section, key));
            }
            catch (Exception exception69)
            {
                msg = "NAVBIT_REPLY: " + exception69.Message;
                this.WriteApp(msg);
            }
            this.AutoReplyCtrl.AutoSendPositionRequestMsg();
            this.AutoReplyCtrl.UpdateAutoReplyStatus();
        }

        public bool ReadTruthData(string filePath, int startAtLine)
        {
            return HelperFunctions.ReadCSVTruthData(filePath, startAtLine, ref this.NavTruthDataHash);
        }

        private bool RS232WriteData(byte[] msg)
        {
            bool flag = false;
            if (!this.comPort.IsOpen || (msg == null))
            {
                return flag;
            }
            try
            {
                this.comPort.Write(msg, 0, msg.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void sendFreqAidingReject()
        {
            this.RxCtrl.SendReject(2, -1, -1, 4, "FreqCfg");
            this.processPosRequest();
        }

        private void sendHWConfigResponse()
        {
            this.WriteData(this.AutoReplyCtrl.HWCfgRespMsg);
            try
            {
                this.RxCtrl.CloseSession(0);
                this.RxCtrl.AllowSessOpen = true;
            }
            catch
            {
                this.WriteApp("Error HW Config Response");
            }
        }

        private void SendPushAiding(object source, ElapsedEventArgs e)
        {
            try
            {
                this.RxCtrl.SendPushAiding(this.AutoReplyCtrl.PushAidingMask, this.AutoReplyCtrl.ForceAidingRequestMask);
            }
            catch
            {
            }
        }

        private void sendTimeAidingReject()
        {
            this.RxCtrl.SendReject(2, -1, -1, 4, "TimeCfg");
            if (this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled == 0)
            {
                this.processPosRequest();
            }
        }

        public void SendTTBAiding()
        {
            string tTBTimeAidingCfgMsg = this.AutoReplyCtrl.GetTTBTimeAidingCfgMsg();
            if (!this.TTBPort.IsOpen)
            {
                string errorStr = "TTB port is not connected";
                this.ErrorPrint(errorStr);
            }
            else
            {
                this.WriteData_TTB(tTBTimeAidingCfgMsg);
                this.waitforMsgFromTTB(0xcc, 80);
            }
        }

        public void SendTTBReset(string resetType)
        {
            string errorStr = "Write TTB error";
            if (this.TTBPort == null)
            {
                this.ErrorPrint(errorStr);
            }
            else if (!this.TTBPort.IsOpen)
            {
                errorStr = "TTB port is not connected";
                this.ErrorPrint(errorStr);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string csvMessage = string.Empty;
                string protocol = "SSB";
                string messageName = "Receiver Init";
                int mid = 0x80;
                SSB_Format format = new SSB_Format();
                string str5 = format.GetResetBitMap(resetType, true, true, false).ToString();
                builder.Append("204,");
                ArrayList list = new ArrayList();
                list = this.m_Protocols.GetInputMessageStructure(mid, -1, messageName, protocol);
                for (int i = 0; i < list.Count; i++)
                {
                    if ((((InputMsg) list[i]).fieldName == "Reset Config Bitmap") || (((InputMsg) list[i]).fieldName == "RESET TYPE"))
                    {
                        builder.Append(str5);
                        builder.Append(",");
                    }
                    else
                    {
                        builder.Append(((InputMsg) list[i]).defaultValue);
                        builder.Append(",");
                    }
                }
                csvMessage = builder.ToString().TrimEnd(new char[] { ',' });
                string msg = this.m_Protocols.ConvertFieldsToRaw(csvMessage, messageName, protocol);
                this.WriteData_TTB(msg);
            }
        }

        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(OpenNETCF.IO.Serial.Parity)))
            {
                ((ComboBox) obj).Items.Add(str);
            }
        }

        public void SetPortNameValues(object obj)
        {
            string[] portNames = CommWrapper.GetPortNames();
            Array.Sort<string>(portNames);
            foreach (string str in portNames)
            {
                ((ComboBox) obj).Items.Add(str);
            }
        }

        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(OpenNETCF.IO.Serial.StopBits)))
            {
                ((ComboBox) obj).Items.Add(str);
            }
        }

        public bool SetupRxCtrl()
        {
            bool flag = false;
            try
            {
                this.m_Protocols.m_messageProtocol = this._messageProtocol;
                string str = this._messageProtocol;
                if (str != null)
                {
                    if (!(str == "SSB"))
                    {
                        if (str == "OSP")
                        {
                            goto Label_0205;
                        }
                        if (str == "NMEA")
                        {
                            goto Label_0398;
                        }
                    }
                    else
                    {
                        this._txTransType = TransmissionType.GP2;
                        this.LogFormat = TransmissionType.GPS;
                        this.RxCtrl = new SS3AndGSD3TWReceiver();
                        this.RxCtrl.RxCommWindow = this;
                        this.RxCtrl.DutStationSetup = this.m_TestSetup;
                        this.RxCtrl.RxNavData = this.m_NavData;
                        this.RxCtrl.MessageProtocol = this._messageProtocol;
                        if (this._rxType == ReceiverType.GSW)
                        {
                            this._aidingProtocol = "SSB";
                        }
                        else
                        {
                            this._aidingProtocol = "AI3";
                        }
                        this.RxCtrl.AidingProtocol = this._aidingProtocol;
                        this.RxCtrl.ControlChannelProtocolFile = ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols_F.xml";
                        this.RxCtrl.ControlChannelVersion = "2.1";
                        this.RxCtrl.AidingProtocolVersion = "2.2";
                        this.RxCtrl.ResetCtrl = new FAndSSBReset();
                        this.RxCtrl.ResetCtrl.ResetComm = this;
                        this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                        this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                        this.RxCtrl.ResetCtrl.ResetGPSTimer = this.RxCtrl.GPSTimerEngine;
                        this.AutoReplyCtrl = new AutoReplyMgr_F(this.RxCtrl.ControlChannelProtocolFile);
                        this.AutoReplyCtrl.ControlChannelVersion = this.RxCtrl.ControlChannelVersion;
                        this.AutoReplyCtrl.AidingProtocolVersion = this.RxCtrl.AidingProtocolVersion;
                        this.ListenersCtrl = new ListenerManager();
                        this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                        this.ListenersCtrl.RxType = this._rxType;
                    }
                }
                goto Label_0493;
            Label_0205:
                this._txTransType = TransmissionType.GP2;
                this.LogFormat = TransmissionType.GPS;
                this.RxCtrl = new OSPReceiver();
                this.RxCtrl.RxCommWindow = this;
                this.RxCtrl.DutStationSetup = this.m_TestSetup;
                this.RxCtrl.RxNavData = this.m_NavData;
                this.RxCtrl.MessageProtocol = this._messageProtocol;
                this.RxCtrl.AidingProtocol = this._aidingProtocol;
                this.RxCtrl.ControlChannelProtocolFile = ConfigurationManager.AppSettings["InstalledDirectory"] + @"\Protocols\Protocols_F.xml";
                this.RxCtrl.ControlChannelVersion = "1.0";
                this.RxCtrl.AidingProtocolVersion = "1.0";
                this.RxCtrl.ResetCtrl = new OSPReset();
                this.RxCtrl.ResetCtrl.ResetComm = this;
                this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                this.RxCtrl.ResetCtrl.ResetGPSTimer = this.RxCtrl.GPSTimerEngine;
                this.AutoReplyCtrl = new AutoReplyMgr_OSP(this.RxCtrl.ControlChannelProtocolFile);
                this.AutoReplyCtrl.ControlChannelVersion = this.RxCtrl.ControlChannelVersion;
                this.AutoReplyCtrl.AidingProtocolVersion = this.RxCtrl.AidingProtocolVersion;
                this.ListenersCtrl = new OSPListnerManager();
                this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                this.ListenersCtrl.RxType = this._rxType;
                goto Label_0493;
            Label_0398:
                this._txTransType = TransmissionType.Text;
                this.RxTransType = TransmissionType.Text;
                this.LogFormat = TransmissionType.Text;
                this._rxType = ReceiverType.NMEA;
                this.RxCtrl = new NMEAReceiver();
                this.RxCtrl.RxCommWindow = this;
                this.RxCtrl.DutStationSetup = this.m_TestSetup;
                this.RxCtrl.RxNavData = this.m_NavData;
                this.RxCtrl.MessageProtocol = this._messageProtocol;
                this.RxCtrl.AidingProtocol = this._aidingProtocol;
                this.RxCtrl.ResetCtrl = new NMEAReset();
                this.RxCtrl.ResetCtrl.ResetComm = this;
                this.RxCtrl.ResetCtrl.DutStationSetup = this.RxCtrl.DutStationSetup;
                this.RxCtrl.ResetCtrl.ResetNavData = this.RxCtrl.RxNavData;
                this.ListenersCtrl = new NMEAListnerManager();
                this.ListenersCtrl.AidingProtocol = this._aidingProtocol;
                this.ListenersCtrl.RxType = this._rxType;
            Label_0493:
                flag = true;
            }
            catch (Exception exception)
            {
                this.ErrorPrint(exception.Message);
                flag = false;
            }
            return flag;
        }

        public bool SetWakeupPort(bool status)
        {
            if (this.MPMWakeupPort == null)
            {
                return false;
            }
            return this.MPMWakeupPort.SetRTS(status);
        }

        private string StreamReadLine(NetworkStream ns)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            while (ns.DataAvailable)
            {
                int num = ns.ReadByte();
                char ch = (char) num;
                builder.Append(ch);
                if (num == 0xb0)
                {
                    flag = true;
                }
                else
                {
                    if ((num == 0xb3) && flag)
                    {
                        break;
                    }
                    flag = false;
                }
            }
            return builder.ToString();
        }

        public void SwitchProtocol()
        {
            if (this.ToSwitchProtocol != this._messageProtocol)
            {
                this.AutoDetectProtocolAndBaudDone = false;
                Thread.Sleep(50);
                this._messageProtocol = this.ToSwitchProtocol;
                if (this._messageProtocol == "OSP")
                {
                    this._rxType = ReceiverType.SLC;
                    this.RxTransType = TransmissionType.GPS;
                    this._txTransType = TransmissionType.GP2;
                }
                else if (this._messageProtocol == "NMEA")
                {
                    this.dataGui.AGC_Gain = 0;
                    this._rxType = ReceiverType.NMEA;
                    this.RxTransType = TransmissionType.Text;
                    this._txTransType = TransmissionType.Text;
                }
                this.BaudRate = this.ToSwitchBaud;
                this.SetupRxCtrl();
                this.portDataInit();
                Thread.Sleep(5);
                int num = 0;
                bool flag = false;
                uint baud = uint.Parse(this.BaudRate);
                while (num++ < 5)
                {
                    flag = this.comPort.UpdateBaudSettings(baud);
                    if (flag)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }
                this.portDataInit();
                if (flag)
                {
                    if (this._messageProtocol == "OSP")
                    {
                        this.RxCtrl.SetMessageRateForFactory();
                    }
                }
                else
                {
                    this.ErrorPrint("Error updating port");
                }
                this.AutoDetectProtocolAndBaudDone = true;
            }
        }

        private void timeAidHandler_thread()
        {
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            this.timeAidingResponseWithTTB();
        }

        private void timeAidingHandler()
        {
            if (this.AutoReplyCtrl.TimeTransferCtrl.Reject)
            {
                this.sendTimeAidingReject();
            }
            else if (!this.AutoReplyCtrl.AutoReplyParams.UseTTB_ForTimeAid)
            {
                GPSDateTime time = this.RxCtrl.GPSTimerEngine.GetTime();
                this.AutoReplyCtrl.TimeTransferCtrl.WeekNum = (ushort) time.GetGPSWeek();
                this.AutoReplyCtrl.TimeTransferCtrl.TimeOfWeek = ((ulong) time.GetGPSTOW()) + ((ulong) (this.AutoReplyCtrl.TimeTransferCtrl.Skew * 100.0));
                this.AutoReplyCtrl.AutoReplyTimeTransferResp();
                this.WriteData(this.AutoReplyCtrl.TimeTransferRespMsg);
                string str = this.AutoReplyCtrl.TimeTransferRespMsg.Replace(" ", "");
                this.WriteApp(this.RxCtrl.FormatTimeTransferResponse(str.Substring(12, str.Length - 20)));
                if (this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled == 0)
                {
                    this.processPosRequest();
                }
            }
            else if ((this.TTBPort != null) && this.TTBPort.IsOpen)
            {
                this._timeAidfromTTBThread = new Thread(new ThreadStart(this.timeAidHandler_thread));
                this._timeAidfromTTBThread.IsBackground = true;
                this._timeAidfromTTBThread.Start();
            }
            else
            {
                this.WriteApp("TTB port not open -- send reject");
                this.sendTimeAidingReject();
            }
        }

        private void timeAidingResponseWithTTB()
        {
            string message = "0211";
            string msg = "A0A20002" + message + utils_AutoReply.GetChecksum(message, true) + "B0B3";
            this.WriteData_TTB(msg);
            string hexString = this.waitforMsgFromTTB(2, 0x11);
            if (hexString != string.Empty)
            {
                hexString = hexString.Replace(" ", "");
                string str5 = this.AutoReplyCtrl.TranslateTTBTimeTransferRespSLC(hexString);
                this.WriteData(str5);
                string str6 = str5.Replace(" ", "");
                if (str6.Length > 20)
                {
                    this.WriteApp(this.RxCtrl.FormatTimeTransferResponse(str6.Substring(12, str6.Length - 20)));
                }
                if (this.AutoReplyCtrl.HWCfgCtrl.FreqAidEnabled == 0)
                {
                    this.processPosRequest();
                }
            }
            else
            {
                this.WriteApp("No time aiding response from TTB -- Send reject");
                this.sendTimeAidingReject();
            }
        }

        public bool Toggle4eWakeupPort()
        {
            if (this.MPMWakeupPort == null)
            {
                return false;
            }
            return this.MPMWakeupPort.ToggleRTS();
        }

        public void TTBPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.TTBPort.IsOpen && (this.TTBPort.BytesToRead >= 10))
            {
                try
                {
                    int bytesToRead = this.TTBPort.BytesToRead;
                    this.TTBPort.Read();
                }
                catch (Exception exception)
                {
                    this.ErrorPrint(exception.Message);
                }
            }
        }

        public void UpdateTestSetup()
        {
            this.m_TestSetup.RXCommIntf = string.Format("{0}_{1}", this._portName, this._baudRate);
            this.m_TestSetup.RxSN = this._rxName;
            this.m_TestSetup.RXProductType = this.ProductFamily.ToString();
        }

        public void UpdateWindowTitleBar(string title)
        {
            if (this.UpdateWinTitle != null)
            {
                this.UpdateWinTitle(title);
            }
            if (this.UpdatePortMainWinTitle != null)
            {
                this.UpdatePortMainWinTitle(this._sourceDeviceName, title);
            }
        }

        public static bool ValidateCommManager(CommunicationManager target)
        {
            return (((target != null) && (target.RxCtrl != null)) && (target.RxCtrl.ResetCtrl != null));
        }

        public string waitforMsgFromTTB(byte chanType, byte msgID)
        {
            byte num = 0x53;
            byte num2 = 0x56;
            string inputData = string.Empty;
            int index = 5;
            int num4 = 4;
            byte[] sourceArray = new byte[MAX_BYTES_BUFFER];
            int length = 0;
            byte num6 = 0;
            byte num7 = 0;
            bool flag = false;
            int tickCount = Environment.TickCount;
            int num9 = 1;
            int num10 = 0;
            bool flag2 = false;
            StringBuilder builder = new StringBuilder(0x2710);
            while (!flag && ((Environment.TickCount - tickCount) < 0xbb8))
            {
                if ((this.TTBPort == null) || !this.TTBPort.IsOpen)
                {
                    return string.Empty;
                }
                int bytesToRead = this.TTBPort.BytesToRead;
                if (bytesToRead < 50)
                {
                    Thread.Sleep(100);
                    try
                    {
                        bytesToRead = this.TTBPort.BytesToRead;
                    }
                    catch
                    {
                    }
                }
                if (bytesToRead > 0)
                {
                    byte[] buffer2 = this.TTBPort.Read();
                    bool flag3 = false;
                    for (int i = 0; i < buffer2.Length; i++)
                    {
                        if (length >= (MAX_BYTES_BUFFER - 10))
                        {
                            length = 0;
                        }
                        byte num13 = buffer2[i];
                        num7 = num13;
                        if (((num6 == 160) && (num7 == 0xa2)) && !flag3)
                        {
                            flag3 = true;
                            length = 0;
                            sourceArray[length++] = num6;
                            sourceArray[length++] = num7;
                            num6 = num7;
                            continue;
                        }
                        if ((num6 != 0xb0) || (num7 != 0xb3))
                        {
                            goto Label_024C;
                        }
                        sourceArray[length++] = num7;
                        if ((sourceArray[0] == 160) && (sourceArray[1] == 0xa2))
                        {
                            HelperFunctions.GetTimeStampInString();
                            int num14 = (sourceArray[2] * 0x100) + sourceArray[3];
                            int num15 = length - 8;
                            if (num14 == num15)
                            {
                                flag3 = false;
                                if ((chanType != sourceArray[num4]) || (msgID != sourceArray[index]))
                                {
                                    goto Label_0240;
                                }
                                if (!flag2 && ((msgID == num) || (msgID == num2)))
                                {
                                    num9 = sourceArray[index + 1];
                                    flag2 = true;
                                }
                                byte[] buffer3 = new byte[length];
                                Array.Copy(sourceArray, buffer3, length);
                                inputData = this.ByteArrToHexString(buffer3);
                                this.DebugPrint(CommonClass.MessageType.Warning, inputData);
                                this._log.WriteLine(CommonUtilsClass.LogToGP2(inputData));
                                if (num9 > 1)
                                {
                                    builder.Append(inputData);
                                    builder.Append("\r\n");
                                }
                                num10++;
                                if (num9 != num10)
                                {
                                    goto Label_0240;
                                }
                                flag = true;
                                continue;
                            }
                            if (num15 < num14)
                            {
                                continue;
                            }
                            byte[] destinationArray = new byte[length];
                            Array.Copy(sourceArray, destinationArray, length);
                            string str2 = CommonUtilsClass.LogToGP2(this.ByteArrToHexString(destinationArray));
                            this.DebugPrint(CommonClass.MessageType.Warning, str2);
                            this._log.WriteLine(str2);
                        }
                    Label_0240:
                        sourceArray.Initialize();
                        length = 0;
                        goto Label_0259;
                    Label_024C:
                        sourceArray[length] = num13;
                        length++;
                    Label_0259:
                        num6 = num7;
                    }
                }
            }
            if (!flag)
            {
                string errorStr = "Failed to receive msg " + msgID.ToString() + " from TTB";
                this.ErrorPrint(errorStr);
                return string.Empty;
            }
            if (num9 == 1)
            {
                return inputData;
            }
            string str4 = builder.ToString().ToUpper();
            return str4.Substring(0, str4.Length - 2);
        }

        public bool waitforNMEAMsg()
        {
            byte[] buffer = new byte[MAX_BYTES_BUFFER];
            int index = 0;
            byte num2 = 0;
            byte num3 = 0;
            bool flag = false;
            int tickCount = Environment.TickCount;
            new StringBuilder(0x2710);
            while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
            {
                if ((this.comPort == null) || !this.comPort.IsOpen)
                {
                    return false;
                }
                int bytesToRead = this.comPort.BytesToRead;
                if (bytesToRead < 50)
                {
                    Thread.Sleep(5);
                    try
                    {
                        bytesToRead = this.comPort.BytesToRead;
                    }
                    catch
                    {
                    }
                }
                if (bytesToRead > 0)
                {
                    byte[] buffer2 = this.comPort.Read();
                    bool flag2 = false;
                    for (int i = 0; i < buffer2.Length; i++)
                    {
                        if (index >= (MAX_BYTES_BUFFER - 10))
                        {
                            index = 0;
                        }
                        byte num7 = buffer2[i];
                        num3 = num7;
                        if (((num2 == 0x24) && (num3 == 0x47)) && !flag2)
                        {
                            flag2 = true;
                            index = 0;
                            buffer[index++] = num2;
                            buffer[index++] = num3;
                            num2 = num3;
                        }
                        else
                        {
                            if (num3 == 10)
                            {
                                buffer[index++] = num3;
                                if ((buffer[0] == 0x24) && (buffer[1] == 0x47))
                                {
                                    flag = true;
                                    continue;
                                }
                                buffer.Initialize();
                                index = 0;
                            }
                            else
                            {
                                buffer[index] = num7;
                                index++;
                            }
                            num2 = num3;
                        }
                    }
                }
            }
            return flag;
        }

        public string waitforNMEAMsg_I2C()
        {
            string str = string.Empty;
            byte[] sourceArray = new byte[MAX_BYTES_BUFFER];
            int length = 0;
            byte num2 = 0;
            byte num3 = 0;
            bool flag = false;
            int tickCount = Environment.TickCount;
            int num5 = 1;
            int num6 = 0;
            StringBuilder builder = new StringBuilder(0x2710);
            if (this.CMC.HostAppI2CSlave.I2CTalkMode != CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_MULTI_MASTER)
            {
                if (this.CMC.HostAppI2CSlave.I2CTalkMode == CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE)
                {
                    while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
                    {
                        Thread.Sleep(500);
                        if (!this.CMC.HostAppI2CSlave.IsOpen())
                        {
                            return string.Empty;
                        }
                        byte[] buffer5 = new byte[0x2000];
                        string errorStr = string.Empty;
                        short num12 = 0;
                        AardvarkApi.aa_i2c_read_ext(this.CMC.HostAppI2CSlave.I2CHandleMaster, this.CMC.HostAppI2CSlave.I2CMasterAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, 0x2000, buffer5, ref num12);
                        int status = num12;
                        if (status < 0)
                        {
                            errorStr = string.Format("code: {0}", AardvarkApi.aa_status_string(status));
                            this.ErrorPrint(errorStr);
                        }
                        if (status > 0)
                        {
                            byte[] buffer6 = new byte[status];
                            int index = 0;
                            while (index < status)
                            {
                                buffer6[index] = buffer5[index];
                                index++;
                            }
                            bool flag3 = false;
                            for (index = 0; index < buffer6.Length; index++)
                            {
                                if (length >= (MAX_BYTES_BUFFER - 10))
                                {
                                    length = 0;
                                }
                                byte num15 = buffer6[index];
                                num3 = num15;
                                if (((num2 == 0x24) && (num3 == 0x47)) && !flag3)
                                {
                                    flag3 = true;
                                    length = 0;
                                    sourceArray[length++] = num2;
                                    sourceArray[length++] = num3;
                                    num2 = num3;
                                }
                                else
                                {
                                    if (num3 == 10)
                                    {
                                        sourceArray[length++] = num3;
                                        if ((sourceArray[0] == 0x24) && (sourceArray[1] == 0x47))
                                        {
                                            HelperFunctions.GetTimeStampInString();
                                            flag3 = false;
                                            byte[] destinationArray = new byte[length];
                                            Array.Copy(sourceArray, destinationArray, length);
                                            str = this.ByteArrToHexString(destinationArray);
                                            num6++;
                                            if (num5 == num6)
                                            {
                                                flag = true;
                                                continue;
                                            }
                                        }
                                        sourceArray.Initialize();
                                        length = 0;
                                    }
                                    else
                                    {
                                        sourceArray[length] = num15;
                                        length++;
                                    }
                                    num2 = num3;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
                {
                    Thread.Sleep(500);
                    if (!this.CMC.HostAppI2CSlave.IsOpen())
                    {
                        return string.Empty;
                    }
                    byte addr = 0;
                    byte[] buffer2 = new byte[0x3e8];
                    string str2 = string.Empty;
                    int num7 = AardvarkApi.aa_async_poll(this.CMC.HostAppI2CSlave.I2CHandle, 500);
                    if (num7 == 1)
                    {
                        int num9 = AardvarkApi.aa_i2c_slave_read(this.CMC.HostAppI2CSlave.I2CHandle, ref addr, 0x3e8, buffer2);
                        if (num9 < 0)
                        {
                            str2 = string.Format("code: {0}, error(read data): {1}", num7, AardvarkApi.aa_status_string(num9));
                            this.ErrorPrint(str2);
                        }
                        if (num9 > 0)
                        {
                            byte[] buffer3 = new byte[num9];
                            int num10 = 0;
                            while (num10 < num9)
                            {
                                buffer3[num10] = buffer2[num10];
                                num10++;
                            }
                            bool flag2 = false;
                            for (num10 = 0; num10 < buffer3.Length; num10++)
                            {
                                if (length >= (MAX_BYTES_BUFFER - 10))
                                {
                                    length = 0;
                                }
                                byte num11 = buffer3[num10];
                                num3 = num11;
                                if (((num2 == 0x24) && (num3 == 0x47)) && !flag2)
                                {
                                    flag2 = true;
                                    length = 0;
                                    sourceArray[length++] = num2;
                                    sourceArray[length++] = num3;
                                    num2 = num3;
                                }
                                else
                                {
                                    if (num3 == 10)
                                    {
                                        sourceArray[length++] = num3;
                                        if ((sourceArray[0] == 0x24) && (sourceArray[1] == 0x47))
                                        {
                                            HelperFunctions.GetTimeStampInString();
                                            flag2 = false;
                                            byte[] buffer4 = new byte[length];
                                            Array.Copy(sourceArray, buffer4, length);
                                            str = this.ByteArrToHexString(buffer4);
                                            num6++;
                                            if (num5 == num6)
                                            {
                                                flag = true;
                                                continue;
                                            }
                                        }
                                        sourceArray.Initialize();
                                        length = 0;
                                    }
                                    else
                                    {
                                        sourceArray[length] = num11;
                                        length++;
                                    }
                                    num2 = num3;
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return string.Empty;
            }
            if (num5 == 1)
            {
                return str;
            }
            string str4 = builder.ToString().ToUpper();
            return str4.Substring(0, str4.Length - 2);
        }

        public bool waitforSSBMsg()
        {
            byte[] buffer = new byte[MAX_BYTES_BUFFER];
            int index = 0;
            byte num2 = 0;
            byte num3 = 0;
            bool flag = false;
            int tickCount = Environment.TickCount;
            new StringBuilder(0x2710);
            if ((this.comPort == null) || !this.comPort.IsOpen)
            {
                return false;
            }
            bool flag2 = false;
            while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
            {
                if ((this.comPort == null) || !this.comPort.IsOpen)
                {
                    return false;
                }
                int bytesToRead = this.comPort.BytesToRead;
                if (bytesToRead < 50)
                {
                    Thread.Sleep(5);
                    try
                    {
                        bytesToRead = this.comPort.BytesToRead;
                    }
                    catch
                    {
                    }
                }
                if (bytesToRead > 0)
                {
                    byte[] buffer2 = this.comPort.Read();
                    for (int i = 0; i < buffer2.Length; i++)
                    {
                        if (index >= (MAX_BYTES_BUFFER - 10))
                        {
                            index = 0;
                        }
                        byte num7 = buffer2[i];
                        num3 = num7;
                        if (((num2 == 160) && (num3 == 0xa2)) && !flag2)
                        {
                            flag2 = true;
                            index = 0;
                            buffer[index++] = num2;
                            buffer[index++] = num3;
                            num2 = num3;
                        }
                        else
                        {
                            if ((num2 == 0xb0) && (num3 == 0xb3))
                            {
                                buffer[index++] = num3;
                                if ((buffer[0] == 160) && (buffer[1] == 0xa2))
                                {
                                    int num8 = (buffer[2] * 0x100) + buffer[3];
                                    int num9 = index - 8;
                                    if (num8 == num9)
                                    {
                                        flag2 = false;
                                        flag = true;
                                        continue;
                                    }
                                    if (num9 < num8)
                                    {
                                        goto Label_0151;
                                    }
                                }
                                index = 0;
                            }
                            else
                            {
                                buffer[index] = num7;
                                index++;
                            }
                            num2 = num3;
                        Label_0151:;
                        }
                    }
                }
            }
            return flag;
        }

        public string waitforSSBMsg_I2C()
        {
            string str = string.Empty;
            byte[] sourceArray = new byte[MAX_BYTES_BUFFER];
            int length = 0;
            byte num2 = 0;
            byte num3 = 0;
            bool flag = false;
            int tickCount = Environment.TickCount;
            int num5 = 1;
            int num6 = 0;
            StringBuilder builder = new StringBuilder(0x2710);
            if (this.CMC.HostAppI2CSlave.I2CTalkMode != CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_MULTI_MASTER)
            {
                if (this.CMC.HostAppI2CSlave.I2CTalkMode == CommMgrClass.I2CSlave.I2CCommMode.COMM_MODE_I2C_SLAVE)
                {
                    while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
                    {
                        Thread.Sleep(0x3e8);
                        if (!this.CMC.HostAppI2CSlave.IsOpen())
                        {
                            return string.Empty;
                        }
                        byte[] buffer6 = new byte[0xffff];
                        string errorStr = string.Empty;
                        short num14 = 0;
                        AardvarkApi.aa_i2c_read_ext(this.CMC.HostAppI2CSlave.I2CHandleMaster, this.CMC.HostAppI2CSlave.I2CMasterAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, 0x2000, buffer6, ref num14);
                        int status = num14;
                        if (status < 0)
                        {
                            errorStr = string.Format("Code: {0}", AardvarkApi.aa_status_string(status));
                            this.ErrorPrint(errorStr);
                        }
                        if (status > 0)
                        {
                            byte[] buffer7 = new byte[status];
                            int index = 0;
                            while (index < status)
                            {
                                buffer7[index] = buffer6[index];
                                index++;
                            }
                            bool flag3 = false;
                            for (index = 0; index < buffer7.Length; index++)
                            {
                                if (length >= (MAX_BYTES_BUFFER - 10))
                                {
                                    length = 0;
                                }
                                byte num17 = buffer7[index];
                                num3 = num17;
                                if (((num2 == 160) && (num3 == 0xa2)) && !flag3)
                                {
                                    flag3 = true;
                                    length = 0;
                                    sourceArray[length++] = num2;
                                    sourceArray[length++] = num3;
                                    num2 = num3;
                                    continue;
                                }
                                if ((num2 != 0xb0) || (num3 != 0xb3))
                                {
                                    goto Label_0477;
                                }
                                sourceArray[length++] = num3;
                                if ((sourceArray[0] == 160) && (sourceArray[1] == 0xa2))
                                {
                                    HelperFunctions.GetTimeStampInString();
                                    int num18 = (sourceArray[2] * 0x100) + sourceArray[3];
                                    int num19 = length - 8;
                                    if (num18 == num19)
                                    {
                                        flag3 = false;
                                        byte[] buffer8 = new byte[length];
                                        Array.Copy(sourceArray, buffer8, length);
                                        str = this.ByteArrToHexString(buffer8);
                                        if (num5 > 1)
                                        {
                                            builder.Append(str);
                                            builder.Append("\r\n");
                                        }
                                        num6++;
                                        if (num5 != num6)
                                        {
                                            goto Label_046D;
                                        }
                                        flag = true;
                                        continue;
                                    }
                                    if (num19 < num18)
                                    {
                                        continue;
                                    }
                                    byte[] destinationArray = new byte[length];
                                    Array.Copy(sourceArray, destinationArray, length);
                                    CommonUtilsClass.LogToGP2(this.ByteArrToHexString(destinationArray));
                                }
                            Label_046D:
                                sourceArray.Initialize();
                                length = 0;
                                goto Label_0480;
                            Label_0477:
                                sourceArray[length] = num17;
                                length++;
                            Label_0480:
                                num2 = num3;
                            }
                        }
                    }
                }
            }
            else
            {
                while (!flag && ((Environment.TickCount - tickCount) < 0x7d0))
                {
                    Thread.Sleep(0x3e8);
                    if (!this.CMC.HostAppI2CSlave.IsOpen())
                    {
                        return string.Empty;
                    }
                    byte addr = 0;
                    byte[] buffer2 = new byte[0xffff];
                    string str2 = string.Empty;
                    int num7 = AardvarkApi.aa_async_poll(this.CMC.HostAppI2CSlave.I2CHandle, 500);
                    if (num7 == 1)
                    {
                        int num9 = AardvarkApi.aa_i2c_slave_read(this.CMC.HostAppI2CSlave.I2CHandle, ref addr, -1, buffer2);
                        if (num9 < 0)
                        {
                            str2 = string.Format("Code: {0}, error(read data): {1}", num7, AardvarkApi.aa_status_string(num9));
                            this.ErrorPrint(str2);
                        }
                        if (num9 > 0)
                        {
                            byte[] buffer3 = new byte[num9];
                            int num10 = 0;
                            while (num10 < num9)
                            {
                                buffer3[num10] = buffer2[num10];
                                num10++;
                            }
                            bool flag2 = false;
                            for (num10 = 0; num10 < buffer3.Length; num10++)
                            {
                                if (length >= (MAX_BYTES_BUFFER - 10))
                                {
                                    length = 0;
                                }
                                byte num11 = buffer3[num10];
                                num3 = num11;
                                if (((num2 == 160) && (num3 == 0xa2)) && !flag2)
                                {
                                    flag2 = true;
                                    length = 0;
                                    sourceArray[length++] = num2;
                                    sourceArray[length++] = num3;
                                    num2 = num3;
                                    continue;
                                }
                                if ((num2 != 0xb0) || (num3 != 0xb3))
                                {
                                    goto Label_0241;
                                }
                                sourceArray[length++] = num3;
                                if ((sourceArray[0] == 160) && (sourceArray[1] == 0xa2))
                                {
                                    HelperFunctions.GetTimeStampInString();
                                    int num12 = (sourceArray[2] * 0x100) + sourceArray[3];
                                    int num13 = length - 8;
                                    if (num12 == num13)
                                    {
                                        flag2 = false;
                                        byte[] buffer4 = new byte[length];
                                        Array.Copy(sourceArray, buffer4, length);
                                        str = this.ByteArrToHexString(buffer4);
                                        if (num5 > 1)
                                        {
                                            builder.Append(str);
                                            builder.Append("\r\n");
                                        }
                                        num6++;
                                        if (num5 != num6)
                                        {
                                            goto Label_0237;
                                        }
                                        flag = true;
                                        continue;
                                    }
                                    if (num13 < num12)
                                    {
                                        continue;
                                    }
                                    byte[] buffer5 = new byte[length];
                                    Array.Copy(sourceArray, buffer5, length);
                                    CommonUtilsClass.LogToGP2(this.ByteArrToHexString(buffer5));
                                }
                            Label_0237:
                                sourceArray.Initialize();
                                length = 0;
                                goto Label_024A;
                            Label_0241:
                                sourceArray[length] = num11;
                                length++;
                            Label_024A:
                                num2 = num3;
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return string.Empty;
            }
            if (num5 == 1)
            {
                return str;
            }
            string str4 = builder.ToString().ToUpper();
            return str4.Substring(0, str4.Length - 2);
        }

        public void WriteApp(string msg)
        {
            DateTime now = DateTime.Now;
            string str = string.Format(clsGlobal.MyCulture, "{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}:{5:D2}.{6:D3}", new object[] { now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second, now.Millisecond });
            string str2 = string.Format(clsGlobal.MyCulture, "{0}\t({1})\t{2}", new object[] { str, 0xff, msg });
            MessageQData data = new MessageQData();
            data.MessageSource = CommonClass.MessageSource.USER_TEXT;
            data.MessageChanId = -1;
            data.MessageId = -1;
            data.MessageSubId = -1;
            data.MessageType = CommonClass.MessageType.Outgoing;
            data.MessageText = str2;
            data.MessageTime = str;
            lock (this.DisplayDataLock)
            {
                if (this.DisplayQueue.Count > MAX_MSG_BUFFER)
                {
                    this.DisplayQueue.Dequeue();
                }
                this.DisplayQueue.Enqueue(data);
            }
        }

        public void WriteData()
        {
            if (this.OK_TO_SEND && (this.ToSendMsgQueue.Count > 0))
            {
                while (this.ToSendMsgQueue.Count > 0)
                {
                    string msg = null;
                    lock (this._lockwrite)
                    {
                        msg = (string) this.ToSendMsgQueue.Dequeue();
                    }
                    switch (msg)
                    {
                        case null:
                        case "":
                        {
                            continue;
                        }
                    }
                    byte[] buffer = this.InputMessageToByteArray(msg);
                    if (buffer != null)
                    {
                        if (this._InputDeviceMode == CommonClass.InputDeviceModes.RS232)
                        {
                            this.RS232WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Client)
                        {
                            this.CMC.HostAppClient.WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Server)
                        {
                            this.CMC.HostAppServer.WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.I2C)
                        {
                            this.CMC.HostAppI2CSlave.WriteData(buffer);
                        }
                        if (this.Log.IsBin)
                        {
                            this.Log.Write(buffer);
                        }
                    }
                }
            }
        }

        public void WriteData(string msg)
        {
            if (msg != null)
            {
                lock (this._lockwrite)
                {
                    this.ToSendMsgQueue.Enqueue(msg);
                }
                if ((this.OK_TO_SEND || (this.SendFlag == 2)) && (this.ToSendMsgQueue.Count > 0))
                {
                    while (this.ToSendMsgQueue.Count > 0)
                    {
                        string str = null;
                        lock (this._lockwrite)
                        {
                            str = (string) this.ToSendMsgQueue.Dequeue();
                        }
                        switch (str)
                        {
                            case null:
                            case "":
                            {
                                continue;
                            }
                        }
                        byte[] buffer = this.InputMessageToByteArray(str);
                        bool flag = false;
                        if (this._InputDeviceMode == CommonClass.InputDeviceModes.RS232)
                        {
                            flag = this.RS232WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Client)
                        {
                            flag = this.CMC.HostAppClient.WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.TCP_Server)
                        {
                            flag = this.CMC.HostAppServer.WriteData(buffer);
                        }
                        else if (this._InputDeviceMode == CommonClass.InputDeviceModes.I2C)
                        {
                            flag = this.CMC.HostAppI2CSlave.WriteData(buffer);
                        }
                        else
                        {
                            flag = true;
                        }
                        if (this.Log.IsBin)
                        {
                            this.Log.Write(buffer);
                        }
                        if (!flag)
                        {
                            this.ErrorPrint(string.Format("Error sending:\n{0}", str));
                        }
                        string str2 = string.Format(clsGlobal.MyCulture, "{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}:{5:D2}.{6:D3}", new object[] { DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond });
                        string str3 = str;
                        if (this._txTransType != TransmissionType.Text)
                        {
                            str3 = CommonUtilsClass.ByteToHex(buffer);
                        }
                        MessageQData data = new MessageQData();
                        data.MessageSource = CommonClass.MessageSource.RX_INPUT;
                        data.MessageChanId = -1;
                        data.MessageId = -1;
                        data.MessageSubId = -1;
                        data.MessageType = CommonClass.MessageType.Outgoing;
                        data.MessageText = string.Format(clsGlobal.MyCulture, "{0}\t({1})\t{2}", new object[] { str2, 1, str3 });
                        data.MessageTime = str2;
                        lock (this.DisplayDataLock)
                        {
                            if (this.DisplayQueue.Count > MAX_MSG_BUFFER)
                            {
                                this.DisplayQueue.Dequeue();
                            }
                            this.DisplayQueue.Enqueue(data);
                            continue;
                        }
                    }
                }
            }
        }

        public void WriteData_TTB(string msg)
        {
            if ((msg != null) && this.TTBPort.IsOpen)
            {
                byte[] inBuf = HelperFunctions.HexToByte(msg.Replace(" ", ""));
                try
                {
                    this.TTBPort.Write(inBuf, 0, inBuf.Length);
                    string str = string.Format(clsGlobal.MyCulture, "{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}:{5:D2}.{6:D3} ", new object[] { DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond });
                    string str2 = string.Format(clsGlobal.MyCulture, "{0} ({1})\t{2}", new object[] { str, 1, msg });
                    MessageQData data = new MessageQData();
                    data.MessageSource = CommonClass.MessageSource.RX_INPUT;
                    data.MessageChanId = -1;
                    data.MessageId = -1;
                    data.MessageSubId = -1;
                    data.MessageType = CommonClass.MessageType.Outgoing;
                    data.MessageText = str2;
                    data.MessageTime = str;
                    lock (this.DisplayDataLock)
                    {
                        if (this.DisplayQueue.Count > MAX_MSG_BUFFER)
                        {
                            this.DisplayQueue.Dequeue();
                        }
                        this.DisplayQueue.Enqueue(data);
                    }
                }
                catch (FormatException exception)
                {
                    this.ErrorPrint(exception.Message);
                }
            }
        }

        public string AidingProtocol
        {
            get
            {
                return this._aidingProtocol;
            }
            set
            {
                this._aidingProtocol = value;
            }
        }

        public string ApproxPositionAidingMsgQName
        {
            get
            {
                return this._posAidMsgQName;
            }
            set
            {
                this._posAidMsgQName = value;
            }
        }

        public string AuthenticationCode
        {
            get
            {
                return this._authenCode;
            }
            set
            {
                this._authenCode = value;
            }
        }

        public string BankTime
        {
            get
            {
                return this._bankTime;
            }
            set
            {
                this._bankTime = value;
            }
        }

        public string BaudRate
        {
            get
            {
                return this._baudRate;
            }
            set
            {
                this._baudRate = value;
            }
        }

        public string CurrentBaud
        {
            get
            {
                return this._currentBaud;
            }
            set
            {
                this._currentBaud = value;
            }
        }

        public string CurrentProtocol
        {
            get
            {
                return this._currentProtocol;
            }
            set
            {
                this._currentProtocol = value;
            }
        }

        public int CWInterferenceDetectModeIndex
        {
            get
            {
                return this._cwInterferenceDetectionModeIndex;
            }
            set
            {
                this._cwInterferenceDetectionModeIndex = value;
            }
        }

        public string DataBits
        {
            get
            {
                return this._dataBits;
            }
            set
            {
                this._dataBits = value;
            }
        }

        public string DebugViewMatchString
        {
            get
            {
                return this._debugViewMatchStr;
            }
            set
            {
                this._debugViewMatchStr = value;
                if (this._debugViewMatchStr != string.Empty)
                {
                    this.DebugRegExpressionHandler = new Regex(this._debugViewMatchStr, RegexOptions.Compiled);
                }
            }
        }

        public CommonUtilsClass DebugViewRTBDisplay
        {
            get
            {
                return this._debugViewRTBDisplay;
            }
            set
            {
                this._debugViewRTBDisplay = value;
            }
        }

        public MyPanel DisplayPanelCompass
        {
            get
            {
                return this._displayPanelCompass;
            }
            set
            {
                this._displayPanelCompass = value;
            }
        }

        public MyPanel DisplayPanelDRSensors
        {
            get
            {
                return this._displayPanelDRSensors;
            }
            set
            {
                this._displayPanelDRSensors = value;
            }
        }

        public MyPanel DisplayPanelDRStatusStates
        {
            get
            {
                return this._displayPanelDRStatusStates;
            }
            set
            {
                this._displayPanelDRStatusStates = value;
            }
        }

        public MyPanel DisplayPanelLocation
        {
            get
            {
                return this._displayPanelLocation;
            }
            set
            {
                this._displayPanelLocation = value;
            }
        }

        public MyPanel DisplayPanelMEMS
        {
            get
            {
                return this._displayPanelMEMS;
            }
            set
            {
                this._displayPanelMEMS = value;
            }
        }

        public MyPanel DisplayPanelPitch
        {
            get
            {
                return this._displayPanelPitch;
            }
            set
            {
                this._displayPanelPitch = value;
            }
        }

        public MyPanel DisplayPanelRoll
        {
            get
            {
                return this._displayPanelRoll;
            }
            set
            {
                this._displayPanelRoll = value;
            }
        }

        public DataGridView DisplayPanelSatelliteStats
        {
            get
            {
                return this._displayPanelSatelliteStats;
            }
            set
            {
                this._displayPanelSatelliteStats = value;
            }
        }

        public MyPanel DisplayPanelSignal
        {
            get
            {
                return this._displayPanelSignal;
            }
            set
            {
                this._displayPanelSignal = value;
            }
        }

        public MyPanel DisplayPanelSVs
        {
            get
            {
                return this._displayPanelSVs;
            }
            set
            {
                this._displayPanelSVs = value;
            }
        }

        public MyPanel DisplayPanelSVTraj
        {
            get
            {
                return this._displayPanelSVTraj;
            }
            set
            {
                this._displayPanelSVTraj = value;
            }
        }

        public string EEDayNum
        {
            get
            {
                return this._eeDayNum;
            }
            set
            {
                this._eeDayNum = value;
            }
        }

        public string EESelect
        {
            get
            {
                return this._eeSelected;
            }
            set
            {
                this._eeSelected = value;
            }
        }

        public bool EnableCompassView
        {
            get
            {
                return this._enableCompassView;
            }
            set
            {
                this._enableCompassView = value;
            }
        }

        public bool EnableLocationMapView
        {
            get
            {
                return this._enableLocationMapView;
            }
            set
            {
                this._enableLocationMapView = value;
            }
        }

        public bool EnableMEMSView
        {
            get
            {
                return this._enableMEMSView;
            }
            set
            {
                this._enableMEMSView = value;
            }
        }

        public bool EnableSatelliteStats
        {
            get
            {
                return this._enableSatelliteStats;
            }
            set
            {
                this._enableSatelliteStats = value;
            }
        }

        public bool EnableSignalView
        {
            get
            {
                return this._enableSignalView;
            }
            set
            {
                this._enableSignalView = value;
            }
        }

        public bool EnableSVsMap
        {
            get
            {
                return this._enableSVsMap;
            }
            set
            {
                this._enableSVsMap = value;
            }
        }

        public CommonUtilsClass ErrorViewRTBDisplay
        {
            get
            {
                return this._errorViewRTBDisplay;
            }
            set
            {
                this._errorViewRTBDisplay = value;
            }
        }

        public int FlowControl
        {
            get
            {
                return this._flowControl;
            }
            set
            {
                this._flowControl = value;
            }
        }

        public string FreqAidingMsgQName
        {
            get
            {
                return this._freqAidMsgQName;
            }
            set
            {
                this._freqAidMsgQName = value;
            }
        }

        public string HostAppCfgFilePath
        {
            get
            {
                return this._hostAppCfgFilePath;
            }
            set
            {
                this._hostAppCfgFilePath = value;
            }
        }

        public string HostAppMEMSCfgPath
        {
            get
            {
                return this._hostAppMEMSCfgPath;
            }
            set
            {
                this._hostAppMEMSCfgPath = value;
            }
        }

        public string HostPair1
        {
            get
            {
                return this._hostPair1;
            }
            set
            {
                this._hostPair1 = value;
            }
        }

        public string HostSWFilePath
        {
            get
            {
                return this._hostSWFilePath;
            }
            set
            {
                this._hostSWFilePath = value;
            }
        }

        public string HwCfgMsgQName
        {
            get
            {
                return this._hwCfgMsgQName;
            }
            set
            {
                this._hwCfgMsgQName = value;
            }
        }

        public string I2CMasterAddr
        {
            get
            {
                return this._I2CmasterAddr;
            }
            set
            {
                this._I2CmasterAddr = value;
            }
        }

        public string I2Cport
        {
            get
            {
                return this._I2Cport;
            }
            set
            {
                this._I2Cport = value;
            }
        }

        public string I2CSlaveAddr
        {
            get
            {
                return this._I2CslaveAddr;
            }
            set
            {
                this._I2CslaveAddr = value;
            }
        }

        public string IMUFilePath
        {
            get
            {
                return this._IMUFilePath;
            }
            set
            {
                this._IMUFilePath = value;
            }
        }

        public bool IMUPositionAvailable
        {
            get
            {
                return this._IMUAvailable;
            }
            set
            {
                this._IMUAvailable = value;
            }
        }

        public CommonClass.InputDeviceModes InputDeviceMode
        {
            get
            {
                return this._InputDeviceMode;
            }
            set
            {
                this._InputDeviceMode = value;
            }
        }

        public System.Timers.Timer InputTimer
        {
            get
            {
                return this._InputTimer;
            }
            set
            {
                this._InputTimer = value;
            }
        }

        public double LastAidingSessionReportedClockDrift
        {
            get
            {
                return this._lastAidingSessionReportedClockDrift;
            }
            set
            {
                this._lastAidingSessionReportedClockDrift = value;
            }
        }

        public uint LastClockDrift
        {
            get
            {
                return this._lastClockDrift;
            }
            set
            {
                this._lastClockDrift = value;
            }
        }

        public int LDOMode
        {
            get
            {
                return this._ldoMode;
            }
            set
            {
                this._ldoMode = value;
            }
        }

        public int LNAType
        {
            get
            {
                return this._lnaType;
            }
            set
            {
                this._lnaType = value;
            }
        }

        public double LocationMapRadius
        {
            get
            {
                return this._locationMapRadius;
            }
            set
            {
                this._locationMapRadius = value;
            }
        }

        public LogManager Log
        {
            get
            {
                return this._log;
            }
            set
            {
                this._log = value;
            }
        }

        public lowPowerParams LowPowerParams
        {
            get
            {
                return this._lowPowerParams;
            }
            set
            {
                this._lowPowerParams = value;
            }
        }

        public string MessageProtocol
        {
            get
            {
                return this._messageProtocol;
            }
            set
            {
                this._messageProtocol = value;
            }
        }

        public bool MonitorNav
        {
            get
            {
                return this._monitorNav;
            }
            set
            {
                this._monitorNav = value;
            }
        }

        public bool MsgID4Update
        {
            get
            {
                return this.msgid4Update;
            }
            set
            {
                this.msgid4Update = value;
            }
        }

        public bool OK_TO_SEND
        {
            get
            {
                return this._ok2send;
            }
            set
            {
                this._ok2send = value;
            }
        }

        public string Parity
        {
            get
            {
                return this._parity;
            }
            set
            {
                this._parity = value;
            }
        }

        public string PortName
        {
            get
            {
                return this._portName;
            }
            set
            {
                this._portName = value;
            }
        }

        public string PortNum
        {
            get
            {
                return this._portNum;
            }
            set
            {
                this._portNum = value;
            }
        }

        public string PosReqAckMsgQName
        {
            get
            {
                return this._posReqAckMsgQName;
            }
            set
            {
                this._posReqAckMsgQName = value;
            }
        }

        public CommonClass.ProductType ProductFamily
        {
            get
            {
                return this._productFamily;
            }
            set
            {
                this._productFamily = value;
            }
        }

        public int ReadBuffer
        {
            get
            {
                return this._readBuffer;
            }
            set
            {
                this._readBuffer = value;
            }
        }

        public bool RequireEE
        {
            get
            {
                return this._requireEE;
            }
            set
            {
                this._requireEE = value;
            }
        }

        public bool RequireHostRun
        {
            get
            {
                return this._requiredRunHost;
            }
            set
            {
                this._requiredRunHost = value;
            }
        }

        public string ResetPort
        {
            get
            {
                return this._resetPort;
            }
            set
            {
                this._resetPort = value;
            }
        }

        public CommonUtilsClass ResponseViewRTBDisplay
        {
            get
            {
                return this._responseViewRTBDisplay;
            }
            set
            {
                this._responseViewRTBDisplay = value;
            }
        }

        public TransmissionType RxCurrentTransmissionType
        {
            get
            {
                return this.RxTransType;
            }
            set
            {
                this.RxTransType = value;
            }
        }

        public string RxName
        {
            get
            {
                return this._rxName;
            }
            set
            {
                this._rxName = value;
            }
        }

        public ReceiverType RxType
        {
            get
            {
                return this._rxType;
            }
            set
            {
                this._rxType = value;
            }
        }

        public string ServerName
        {
            get
            {
                return this._serverName;
            }
            set
            {
                this._serverName = value;
            }
        }

        public string ServerPort
        {
            get
            {
                return this._serverPort;
            }
            set
            {
                this._serverPort = value;
            }
        }

        public string sourceDeviceName
        {
            get
            {
                return this._sourceDeviceName;
            }
            set
            {
                this._sourceDeviceName = value;
            }
        }

        public string StopBits
        {
            get
            {
                return this._stopBits;
            }
            set
            {
                this._stopBits = value;
            }
        }

        public string TimeAidingMsgQName
        {
            get
            {
                return this._timeAidMsgQName;
            }
            set
            {
                this._timeAidMsgQName = value;
            }
        }

        public string TrackerPort
        {
            get
            {
                return this._trackerPort;
            }
            set
            {
                this._trackerPort = value;
            }
        }

        public string TTBPortNum
        {
            get
            {
                return this._TTBPortNum;
            }
            set
            {
                this._TTBPortNum = value;
            }
        }

        public TransmissionType TxCurrentTransmissionType
        {
            get
            {
                return this._txTransType;
            }
            set
            {
                this._txTransType = value;
            }
        }

        public int Use_TCPIP
        {
            get
            {
                return this._use_tcpip;
            }
            set
            {
                this._use_tcpip = value;
            }
        }

        public string WindowTitle
        {
            get
            {
                return this._myWindowTitle;
            }
            set
            {
                this._myWindowTitle = value;
            }
        }

        public enum MessageType
        {
            Incoming,
            Outgoing,
            Normal,
            Warning,
            Error
        }

        public enum ReceiverType
        {
            GSW,
            SLC,
            NMEA,
            OSP,
            TTB
        }

        public enum TransmissionType
        {
            Text,
            Hex,
            SSB,
            GP2,
            GPS,
            Bin
        }

        public delegate void UpdateParentEventHandler(string titleString);

        public delegate void UpdateParentPortEventHandler(string sourceDevName, string titleString);
    }
}

