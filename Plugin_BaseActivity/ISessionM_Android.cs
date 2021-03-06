using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SessionMMiniJSON;

/*!
 * SessionM Android Native Implementation.
 */ 
#if UNITY_ANDROID
public class ISessionM_Android : ISessionM
{	
	private SessionM sessionMGameObject;
	private ISessionMCallback callback;
	private SessionMEventListener listener;
	
	private static AndroidJavaObject androidInstance;
	
	private Boolean isPresented = false;
	
	public ISessionM_Android(SessionM sessionMParent)
	{
		sessionMGameObject = sessionMParent;
		
		initAndroidInstance();
		
		CreateListenerObject();
		
		SetShouldAutoUpdateAchievementsList(SessionM.shouldAutoUpdateAchievementsList);
		SetMessagesEnabled(SessionM.shouldEnableMessages);
		SetLogLevel(sessionMParent.logLevel);
		if (SessionM.serviceRegion == ServiceRegion.Custom) {
			SetServerType(SessionM.serverURL);
		} else {
			SetServiceRegion(SessionM.serviceRegion);
		}
		if (SessionM.shouldAutoStartSession && sessionMGameObject.androidAppId != null) {
			StartSession(sessionMGameObject.androidAppId);
		}
	}
	
	private void CreateListenerObject()
	{
		listener = sessionMGameObject.gameObject.AddComponent<SessionMEventListener>();
		
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.CallStatic("setCallbackGameObjectName", sessionMGameObject.gameObject.name);
		}
		
		listener.SetNativeParent(this);
		
		if(callback != null) {
			listener.SetCallback(callback);
		}
	}
	
	public void StartSession(string appId)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			if(appId != null) {
				androidInstance.Call("startSession", activityObject, appId);
			} else if(sessionMGameObject.androidAppId != null) {
				androidInstance.Call("startSession", activityObject, sessionMGameObject.androidAppId);
			}
		}
	}
	
	public SessionState GetSessionState()
	{
		SessionState state = SessionState.Stopped;
		
		using (AndroidJavaObject stateObject = androidInstance.Call<AndroidJavaObject>("getSessionState")) {
			string stateName = stateObject.Call<string>("name");
			if(stateName.Equals("STOPPED")) {
				state = SessionState.Stopped;
			} else if(stateName.Equals("STARTED_ONLINE")) {
				state = SessionState.StartedOnline;
			} else if(stateName.Equals("STARTED_OFFLINE")) {
				state = SessionState.StartedOffline;
			}
		}
		
		return state;
	}
	
	public string GetUser()
	{
		string userJSON = null;
		
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			userJSON = activityObject.Call<string>("getUser");
		}
		
		return userJSON;
	}

	public bool LogInUserWithEmail(string email, string password) {
		bool success;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			success = activityObject.Call<bool>("logInUserWithEmail", email, password);
		}
		return success;
	}

	public void LogOutUser() {
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("logOutUser");
		}
	}

	public bool SignUpUser(string email, string password, string birthYear, string gender, string zipCode) {
		bool success;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			success = activityObject.Call<bool>("signUpUser", email, password, birthYear, gender, zipCode);
		}
		return success;
	}

	public void SetUserOptOutStatus(bool status){
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setUserOptOutStatus", status);
		}
	}
	
	public void SetShouldAutoUpdateAchievementsList(bool shouldAutoUpdate)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setShouldAutoUpdateAchievementsList", shouldAutoUpdate);                   
		}
	}

    public void SetSessionAutoStartEnabled(bool autoStart)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setSessionAutoStartEnabled", autoStart);                   
		}
	}

	public bool IsSessionAutoStartEnabled()
	{
		bool isEnabled = true;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			isEnabled = activityObject.Call<bool>("isSessionAutoStartEnabled");                   
		}
		return isEnabled;
	}
	
	public void UpdateAchievementsList()
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("updateAchievementsList");                  
		}
	}
	
	public int GetUnclaimedAchievementCount()
	{
		int count = 0;
		
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			count = activityObject.Call<int>("getUnclaimedAchievementCount");			
		}
		
		return count;
	}
	
	public string GetUnclaimedAchievementData() 
	{
		string achievementJSON = null;
		
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			achievementJSON = activityObject.Call<string>("getUnclaimedAchievementJSON");			
		}
		
		return achievementJSON;
	}
	
	
	public void LogAction(string action) 
	{
		androidInstance.Call("logAction", action);
	}
	
	public void LogAction(string action, int count) 
	{
		androidInstance.Call("logAction", action, count);
	}

	public void LogAction(string action, int count, Dictionary<string, object> payloads)
	{
		string payloadsJSON = Json.Serialize(payloads);
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("logAction", action, count, payloadsJSON);
		}
	}
	
	public bool PresentActivity(ActivityType type)
	{
		using (AndroidJavaObject activityType = GetAndroidActivityTypeObject(type),
		       activityObject = GetCurrentActivity()) {
			isPresented = activityObject.Call<bool>("presentActivity", activityType);			
		}
		return isPresented;
	}
	
	public void DismissActivity()
	{
		if (isPresented) {
			androidInstance.Call ("dismissActivity");
			isPresented = false;
		}
	}
	
	public bool IsActivityPresented()
	{
		bool presented = false;
		presented = androidInstance.Call<bool>("isActivityPresented");
		
		return presented;
	}
	
	public bool IsActivityAvailable(ActivityType type)
	{
		bool available = false;

		using (AndroidJavaObject activityType = GetAndroidActivityTypeObject(type),
		       activityObject = GetCurrentActivity()) {
			available = activityObject.Call<bool>("isActivityAvailable", activityType);			
		}
		return available;
	}
	
	public void SetLogLevel(LogLevel level)
	{
		// use logcat on Android
	}
	
	public LogLevel GetLogLevel()
	{
		return LogLevel.Off;
	}

	public string GetSDKVersion()
	{
		return androidInstance.Call<string>("getSDKVersion");			
	}
	
	public string GetRewards()
	{
		string rewardsJSON = null;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			rewardsJSON = activityObject.Call<string>("getRewardsJSON");
		}
		return rewardsJSON;
	}

	public string GetMessagesList()
	{
		string messagesJSON = null;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			messagesJSON = activityObject.Call<string>("getMessagesList");
		}
		return messagesJSON;
	}

	public void SetMessagesEnabled(bool enabled)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setMessagesEnabled", enabled);
		}
	}
	
	public void SetMetaData(string data, string key)
	{
		androidInstance.Call("setMetaData", key, data);
	}

	public bool AuthenticateWithToken(string provider, string token)
	{
		return androidInstance.Call<bool>("authenticateWithToken", provider, token);
	}

	public void SetServiceRegion(ServiceRegion serviceRegion)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
            //Always 0 for now
			activityObject.Call("setServiceRegion", 0);                  
		}
	}

	public void SetServerType(string url)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setServerType", url);                  
		}
	}

	public void SetAppKey(string appKey)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call("setAppKey", appKey);                  
		}
	}
	
	public void NotifyPresented()
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			isPresented = activityObject.Call<bool>("notifyCustomAchievementPresented");
		}
	}
	
	public void NotifyDismissed()
	{
		if (isPresented) {
			using (AndroidJavaObject activityObject = GetCurrentActivity()) {
				activityObject.Call ("notifyCustomAchievementCancelled");
				isPresented = false;
			}
		}
	}
	
	public void NotifyClaimed()
	{	
		if (isPresented) {
			using (AndroidJavaObject activityObject = GetCurrentActivity()) {
				activityObject.Call ("notifyCustomAchievementClaimed");
				isPresented = false;
			}
		}
	}

	public void PresentTierList()
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call ("presentTierList");
		}
	}

	public string GetTiers()
	{
		string tiers = null;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			tiers = activityObject.Call<string> ("getTiers");
		}
		return tiers;
	}
	
	public double GetApplicationMultiplier()
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			return activityObject.Call<double> ("getApplicationMultiplier");
		}
	}

	public void UpdateOffers()
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call ("fetchOffers");
		}
	}

	public string GetOffers()
	{
		string offers = null;
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			offers = activityObject.Call<string> ("getOffers");
		}
		return offers;
	}

	public void FetchContent(string contentID, bool isExternalID)
	{
		using (AndroidJavaObject activityObject = GetCurrentActivity()) {
			activityObject.Call ("fetchContent", contentID, isExternalID);
		}
	}

	public void SetCallback(ISessionMCallback callback) 
	{
		this.callback = callback;
		listener.SetCallback(callback);
	}
	
	public ISessionMCallback GetCallback() 
	{
		return this.callback;
	}
	
	// MonoBehavior 
	
	public AndroidJavaObject GetCurrentActivity() 
	{
		using (AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			return playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}
	
	private AndroidJavaObject GetAndroidActivityTypeObject(ActivityType type) 
	{
		if(Application.platform != RuntimePlatform.Android) {
			return null;	
		}
		
		using (AndroidJavaClass typeClass = new AndroidJavaClass("com.sessionm.api.SessionM$ActivityType")) {
			string typeString = null;
			if(type == ActivityType.Achievement) {
				typeString = "ACHIEVEMENT";	
			} else if(type == ActivityType.Portal) {
				typeString = "PORTAL";	
			}
			
			AndroidJavaObject activityType = typeClass.CallStatic<AndroidJavaObject>("valueOf", typeString);
			return activityType;		
		}
	}
	
	protected static void initAndroidInstance()
	{
		using (AndroidJavaClass sessionMClass = new AndroidJavaClass("com.sessionm.api.SessionM")) {
			androidInstance = sessionMClass.CallStatic<AndroidJavaObject>("getInstance"); 
		}
	}
}
#endif
