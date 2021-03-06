MainFrame= object()
Console= object()
# Contant
TestBeginLabel = "### Begin Test ###"
InitializedLabel = "### Initializing... ###"

TestAborted= False
Exiting= False

# RF playback params
DeviceID=0
RFPlaybackConfigParams=[]
PlayTimeList = []
FileToPlayList = []
SiteNamesList=[]

# Sim params
SimAddr = "127.0.0.1"
SimPort = 15650
SimFile = ""
SimInitialAtten=0

# Test setups
BaseLogDirectory=""
TestOperator="Guest"
TestRun="N/A"
StartTime="N/A"
EndTime="N/A"
TestID="N/A"
TestName="N/A"
TestDescription="N/A"
DataClass="N/A"
TestGroup="N/A"
SignalSource=""
SignalMargin="N/A"
TTFFLimit="N/A"
HrzErrorLimit="N/A"
TTFFTimeout="N/A"
PowerSource=""
AttenSource=""
IsAidingTest=0
TestResultsDirectory=""
SitesList=[]
LogFilesList=[]
TTFFLogFilesList=[]
LogFileExtsList=[]
SignalType="N/A"
CableLoss=0.0
TestSignalLevelsList=[]
ScriptConfigFilePath=""
UseTTBForAiding=0
ScriptName=""

# host app
HostAppDirectory=[]
HostAppFilesList=[]
HostAppArgumentsList=[]
HostAppExtraArgumentsList=[]
HostAppRunRequiredList=[]
PatchRunRequiredList=[]
PatchFilesList=[]

# DUT setups
DutMainPortsList=[]
DutTTBPortsList=[]
DutTrackerPortsList=[]
DutHostPortsList=[]
DutResetPortsList=[]
DutBaudRateList=[]
DutNamesList=[]
DutTypesList=[]
DutRxLogTypesList=[]
DutTxLogTypesList=[]
DutDefaultClksList=[]
DutMessageProtocolsList=[]
DutAidingProtocolsList=[]
DutAi3ICDsList=[]
DutFICDsList=[]
DutExtClkFreqsList=[]
DutDefaultClkIndicesList=[]
DutSourceDeviceList=[]
DutFamilyList=[]
DutLNATypesList=[]
DutLDOModesList=[]
DutLNATypesStringList=[]
DutLDOModesStringList=[]
DutIPAddrsList=[]
DutSiRFNavInterfaceStringsList=[]
DUTPlatform=[]
DUTRev=[]
DUTMfg=[]
DUTPackage=[]
DUTRefFreqSrc=[]
DUTTemperature=[]
DUTTemperatureUnit=[]
DUTVoltage=[]

DutPortManagersList = []

# read config SiRFNav params
SiRFNavStartStop_Version = [1]
SiRFNavStartStop_StartMode = 0
SiRFNavStartStop_UARTMaxPreamble = 37
SiRFNavStartStop_UARTIdleByteWakeupDelay = 1023
#SiRFNavStartStop_ReferenceClockOffset = int(rxDefaultClk[comIdx])#0x177FA
SiRFNavStartStop_FlowControl = 0
#SiRFNavStartStop_LNAType = int(rxLNAType[comIdx])
SiRFNavStartStop_DebugSettings = 1
SiRFNavStartStop_ReferenceClockWarmupDelay = 1023
# myPort.comm.SiRFNavStartStop.ReferenceClockFrequency =  0xf9c568 # 0x18CBA80 
#SiRFNavStartStop_ReferenceClockFrequency =  int(rxExtClkFreq[comIdx]) 
SiRFNavStartStop_ReferenceClockUncertainty = 2952
SiRFNavStartStop_BaudRate = 460800
SiRFNavStartStop_IOPinConfigurationMode = 1
SiRFNavStartStop_IOPinConfiguration0 = 0
SiRFNavStartStop_IOPinConfiguration1 = 0
SiRFNavStartStop_IOPinConfiguration2 = 4
SiRFNavStartStop_IOPinConfiguration3 = 60
SiRFNavStartStop_IOPinConfiguration4 = 0
SiRFNavStartStop_IOPinConfiguration5 = 60
SiRFNavStartStop_IOPinConfiguration6 = 0
SiRFNavStartStop_IOPinConfiguration7 = 0
SiRFNavStartStop_IOPinConfiguration8 = 0
SiRFNavStartStop_IOPinConfiguration9 = 0
SiRFNavStartStop_IOPinConfiguration10 =0 
SiRFNavStartStop_I2CHostAddress = 98
SiRFNavStartStop_I2CTrackerAddress = 96
SiRFNavStartStop_I2CMode = 0
SiRFNavStartStop_I2CRate = 0
SiRFNavStartStop_SPIRate = 5
SiRFNavStartStop_ONOffControl = 42
SiRFNavStartStop_FlashMode = 0
SiRFNavStartStop_StorageMode = 1
SiRFNavStartStop_TrackerPort = -1 # for Phytec put "C"
SiRFNavStartStop_TrackerPortSeleted = 1
SiRFNavStartStop_WeakSignalEnable = 1
# SiRFNavStartStop_LDOEnable = int(rxLDOMode[comIdx])

# Test params
getNavs = []
getTTBNavs = []
getTTFFs = []
RefX = 0
RefY = 0
RefZ = 0
RefLat = 0
RefLon = 0
RefAlt = 0
