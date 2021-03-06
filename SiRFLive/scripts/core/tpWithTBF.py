import scriptGlobals
import scriptUtilities
import scriptSim

try:  
    # Read configuration parameters
    # Data in pair key and value
    sCfg = ConfigParser.ConfigParser()
    sCfg.read(scriptGlobals.ScriptConfigFilePath )
    
    logTime = sCfg.getint('TEST_PARAM', 'LOG_TIME')    
    TPTBFArray = sCfg.get('POWER_PARAM', 'UPDATE_RATE')
    TPOnTimeArray = sCfg.get('POWER_PARAM', 'ON_TIME')
    TPMaxOffTimeArray = sCfg.get('POWER_PARAM', 'MAX_OFF_TIME')
    TPMaxSearchTimeArray = sCfg.get('POWER_PARAM', 'MAX_SEARCH_TIME')
    
    # Convert string to array
    TPTBFArray = eval('['+TPTBFArray+']')
    TPOnTimeArray = eval('['+TPOnTimeArray+']')
    TPMaxOffTimeArray = eval('['+TPMaxOffTimeArray+']')
    TPMaxSearchTimeArray = eval('['+TPMaxSearchTimeArray+']')
    
    testContinue = True;
    paramsLenArray = [len(TPTBFArray), len(TPOnTimeArray), len(TPMaxOffTimeArray), len(TPMaxSearchTimeArray)]
    if (scriptUtilities.areAllEqual(paramsLenArray) == False):
	print ("Error: Length of TP parameters NOT equal ")
	scriptGlobals.TestAborted = True
    else:	
	if (scriptGlobals.SignalSource == General.clsGlobal.SIM):
	    if (scriptSim.isSimRunning() == True):
		result = MessageBoxEx.Show("SIM is running -- Proceed?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information,20000)
		if (result == DialogResult.Yes):
		    testContinue = True
		    scriptSim.simStop()
		    scriptSim.simLoad(scriptGlobals.SimFile)		
		    scriptSim.simRun()
		else:
		    testContinue = False
	    else:
		scriptSim.simLoad(scriptGlobals.SimFile)	    
		scriptSim.simRun()
	if (testContinue == True):
	    scriptGlobals.MainFrame.Delay(5)
	    # set to high level for factory reset
	    if (scriptGlobals.SignalType.ToLower() == "dbhz"):
		defaultAtten = Utilities.HelperFunctions.GetCalibrationAtten(scriptGlobals.CableLoss,40,scriptGlobals.SignalType)
	    elif (scriptGlobals.SignalType.ToLower() == "dbm"):
		defaultAtten = Utilities.HelperFunctions.GetCalibrationAtten(scriptGlobals.CableLoss,-130,scriptGlobals.SignalType)
	    else:
		print "Signal Type is not correct"
		defaultAtten = 0
	    # set atten
	    scriptUtilities.setAtten(defaultAtten)    
	    
	    # setup each active com ports
	    comIdx = 0	
	    for usePort in scriptGlobals.DutMainPortsList:	
		if (usePort < 0):
		    comIdx = comIdx + 1
		    continue
		myPort = scriptGlobals.DutPortManagersList[comIdx]
		
		# reset logfile name
		myPort.comm.Log.SetDurationLoggingStatusLabel("")
		if (myPort.comm.IsSourceDeviceOpen() == False):
		    if (myPort.comm.OpenPort() == False):
			portList[comIdx] = -1
			continue
		    myPort.RunAsyncProcess()
		if (myPort.SignalViewLocation.IsOpen == False):
		    mainFrame.CreateSignalViewWin(myPort)
		if (myPort.DebugViewLocation.IsOpen == False):
		    mainFrame.CreateDebugViewWin(myPort)
		myPort.comm.RxCtrl.OpenChannel("SSB")
		myPort.comm.RxCtrl.PollSWVersion()
		myPort.comm.ReadAutoReplyData(scriptGlobals.ScriptConfigFilePath)
		myPort.comm.RxCtrl.ResetCtrl.Reset(General.clsGlobal.HOT)  
			
		comIdx = comIdx + 1
	    scriptUtilities.init()
	    mainFrame.UpdateGUIFromScript()
	    mainFrame.Delay(10)
	    # setup each active com ports
	    comIdx = 0	
	    for usePort in scriptGlobals.DutMainPortsList:	
		if (usePort < 0):
		    comIdx = comIdx + 1
		    continue
		myPort = scriptGlobals.DutPortManagersList[comIdx]
		myPort.comm.RxCtrl.OpenChannel("SSB")
		myPort.comm.RxCtrl.OpenChannel("STAT")
	
		comId = comIdx + 1
	    # Loop for TP configuration
	    for TPIndex in range(len(TPOnTimeArray)):
		if (scriptGlobals.TestAborted == True):
		    print "Test aborted"
		    break
		TPMaxOffTime = TPMaxOffTimeArray[TPIndex]
		TPMaxSearchTime = TPMaxSearchTimeArray[TPIndex]
		TPTBF = TPTBFArray[TPIndex]
		TPOnTime = TPOnTimeArray[TPIndex]	
		TPDutyCycle = TPOnTime/TPTBF*10
		
		# Loop for each test levels    
		for levelIndex in range(0, len(scriptGlobals.TestSignalLevelsList)):
		    level = scriptGlobals.TestSignalLevelsList[levelIndex]		    
		    
		    atten = Utilities.HelperFunctions.GetCalibrationAtten(scriptGlobals.CableLoss,level,scriptGlobals.SignalType)
		    diffAtten = atten - defaultAtten
		    if ((levelIndex == 0) and (diffAtten > 5) and (atten > 5)):
			dropAtten = divmod(atten,5)
			drop5dBLoop = dropAtten[0]
			restAtten = dropAtten[1]
	    
			for dropIndex in range(0, int(drop5dBLoop)+1):
			    atten1 = 5*dropIndex + defaultAtten
			    scriptUtilities.setAtten(atten1)
			    mainFrame.Delay(20)
	    
			atten1 = restAtten +  atten1
			scriptUtilities.setAtten(atten1)
	    
		    else:
			scriptUtilities.setAtten(atten)
	    
		    # setup each active com ports
		    comIdx = 0	
		    for usePort in scriptGlobals.DutMainPortsList:	
			if (usePort < 0):
			    comIdx = comIdx + 1
			    continue
			myPort = scriptGlobals.DutPortManagersList[comIdx]
			myPort.comm.RxCtrl.OpenChannel("SSB")
			myPort.comm.RxCtrl.OpenChannel("STAT")
			myPort.comm.RxCtrl.PollSWVersion()
			# Create directory for log files
			Now = time.localtime(time.time())
			timeNowStr = time.strftime("%m%d%Y_%H%M%S", Now)
			testName = "LA9%d%d" %(TPOnTime/100,TPTBF)
			portLogFile = "%s%s_%d-%d_%s_%s_%s%s" %(scriptGlobals.TestResultsDirectory,testName,TPOnTime,TPTBF,scriptGlobals.DutNamesList[comIdx],timeNowStr,myPort.comm.PortName,scriptGlobals.LogFileExtsList[comIdx])
			
			myPort.comm.RxCtrl.PollSWVersion()
			# update Test Info	    
			scriptGlobals.TestID = "%s-%d-%d" % (scriptGlobals.TestName,TPOnTime,TPTBF)	 
			Now = time.localtime(time.time())
			scriptGlobals.StartTime = time.strftime("%m/%d/%Y %H:%M:%S", Now)
			scriptUtilities.updateDUTInfo(comIdx)
			myPort.comm.m_TestSetup.Atten = atten
			
			myPort.comm.Log.OpenFile(portLogFile)
	    
			myPort.comm.RxCtrl.ResetCtrl.ResetInitParams.Enable_Navlib_Data = True
			myPort.comm.RxCtrl.ResetCtrl.ResetInitParams.Enable_Development_Data = True
			myPort.comm.RxCtrl.ResetCtrl.ResetInitParams.EnableFullSystemReset = True
			myPort.comm.RxCtrl.ResetCtrl.ResetInitParams.EnableEncryptedData = False
	    
			myPort.comm.RxCtrl.ResetCtrl.Reset(General.clsGlobal.HOT)
	    
			# Send full power mode
			myPort.comm.LowPowerParams.Mode = 0;
			myPort.comm.RxCtrl.SetPowerMode(False);
	    
			comIdx = comIdx + 1
	    
		    navStatus = False
		    count = 0
		    while((navStatus == False) and (count < 12)):
			navStatus = mainFrame.GetNavStatus("*")
			count = count + 1
			print "Wait for nav loop %d" %(count)
			if ((navStatus == True) or (count >= 12)):
			    break
			# if ((General.clsGlobal.Abort == True) or (General.clsGlobal.AbortSingle == True)):
			    # scriptGlobals.TestAborted = True
			    # break
			mainFrame.Delay(5)
	    
		    # Set TP mode
		    logStr = "TP(%d,%d) begins..." %(TPOnTime,TPTBF)
		    scriptUtilities.logApp("*", logStr)
		    comIdx = 0	
		    for usePort in scriptGlobals.DutMainPortsList:	
			if (usePort < 0):
			    comIdx = comIdx + 1
			    continue
			myPort = scriptGlobals.DutPortManagersList[comIdx]
			myPort.comm.LowPowerParams.Mode = 3;
			myPort.comm.LowPowerParams.TPDutyCycle = TPDutyCycle
			myPort.comm.LowPowerParams.TPOnTime = TPOnTime
			myPort.comm.LowPowerParams.TPUpdateRate = TPTBF
			myPort.comm.LowPowerParams.TPMaxOffTime = TPMaxOffTime
			myPort.comm.LowPowerParams.TPMaxSearchTime = TPMaxSearchTime
	    
			myPort.comm.RxCtrl.SetPowerMode(False);
			comIdx = comIdx + 1
	    
		    print "%s: Start logging for %d seconds ... " % (time.strftime("%Y/%m/%d %H:%M:%S", time.localtime()),logTime)
		    mainFrame.Delay(logTime)
	    
		    #cleanup
	
		    comIdx = 0	
		    for usePort in scriptGlobals.DutMainPortsList:	
			if (usePort < 0):
			    comIdx = comIdx + 1
			    continue
			myPort = scriptGlobals.DutPortManagersList[comIdx]
			# Send full power mode
			myPort.comm.LowPowerParams.Mode = 0;
			myPort.comm.RxCtrl.SetPowerMode(False);
			myPort.comm.Log.CloseFile()		    
			comIdx = comIdx + 1  
		
		# wait for cycle to end
		print "%s: Wait for cycle end (%d seconds) ... " % (time.strftime("%Y/%m/%d %H:%M:%S", time.localtime()),TPTBF)
		mainFrame.Delay(TPTBF)
		# End Loop for TP Configuration
		
	    # set default atten
	    scriptUtilities.setAtten(defaultAtten)	    
	    print "Done: %s" % (scriptGlobals.ScriptName)    
    
    mainFrame.SetScriptDone(True)
    
    #cleanup
    scriptGlobals.Exiting  = True
    comIdx = 0	
    for usePort in scriptGlobals.DutMainPortsList:	
	if (usePort < 0):
	    comIdx = comIdx + 1
	    continue
	myPort = scriptGlobals.DutPortManagersList[comIdx]
	# Send full power mode
	myPort.comm.ListenersCtrl.Stop()
	myPort.comm.ListenersCtrl.Cleanup()
	myPort.comm.ClosePort()
	# myPort.StopAsyncProcess()
	comIdx = comIdx + 1

except:
    scriptUtilities.ExceptionHandler()

