using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Agora.Rtc;
using Agora_RTC_Plugin.API_Example.Examples.Basic.JoinChannelVideo;

public class AgoraUnityVideo : MonoBehaviour
{
    // Fill in your app ID
    private string _appID = "0a26d2f87aca418f95af395a3e0d29b6";
    // Fill in your channel name
    private string _channelName = "Check";
    // Fill in a temporary token
    private string _token = "007eJxTYDg5/Z/AtcdMKQsvRxzeLyqUe2z6XO3VaXIrzZdei65+sqJbgcEg0cgsxSjNwjwxOdHE0CLN0jQxzRhIGKcapBhZJpl58ZanNQQyMuy1+MPACIUgPiuDc0ZqcjYDAwDjxiDu";
    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal IRtcEngine RtcEngine;

    public GameObject canvas;


    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };

    public static AgoraUnityVideo instance;

    private void Awake()
    {
        instance = this;
    }
    public void CheckPermissions()
    {

        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
    }

    public void SetupUI()
    {
        GameObject go = GameObject.Find("LocalView");
        LocalView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        GameObject wo = GameObject.Find("RemoteView");
        RemoteView = wo.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("Leave");
        go.GetComponent<Button>().onClick.AddListener(Leave);
        go = GameObject.Find("Join");
        go.GetComponent<Button>().onClick.AddListener(Join);
    }

    public void SetupVideoSDKEngine()
    {
        // Create an IRtcEngine instance
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new RtcEngineContext(_appID, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
        // Initialize the instance
        RtcEngine.Initialize(context);
    }
    // Create a user event handler instance and set it as the engine event handler
    public void InitEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }

    // Implement your own EventHandler class by inheriting the IRtcEngineEventHandler interface class implementation
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraUnityVideo _videoSample;
        internal UserEventHandler(AgoraUnityVideo videoSample)
        {
            _videoSample = videoSample;
        }
        // error callback
        public override void OnError(int err, string msg)
        {
        }
        // Triggered when a local user successfully joins the channel
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            _videoSample.LocalView.SetForUser(0, "");
        }
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            // Set the remote video display
            _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            // Start video rendering
            _videoSample.RemoteView.SetEnable(true);
            Debug.Log("Remote user joined");
        }
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _videoSample.RemoteView.SetEnable(false);
        }
    }
    

    public void Join()
    {
        //canvas.SetActive(true);
        Debug.Log("join");
        // Enable the video module
        RtcEngine.EnableVideo();

        // Set channel media options
        ChannelMediaOptions options = new ChannelMediaOptions();
        // Start video rendering
        LocalView.SetEnable(true);
        // Automatically subscribe to all audio streams
        options.autoSubscribeAudio.SetValue(true);
        // Automatically subscribe to all video streams
        options.autoSubscribeVideo.SetValue(true);
        // Set the channel profile to live broadcast
        options.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        //Set the user role as host
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        // Join a channel
        RtcEngine.JoinChannel(_token, _channelName, 0, options);

    }
    




    public void Leave()
    {
        Debug.Log("Leaving _channelName");
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Disable the video module
        RtcEngine.DisableVideo();
        // Stop remote video rendering
        RemoteView.SetEnable(false);
        // Stop local video rendering
        LocalView.SetEnable(false);
    }
    void Update()
    {
        //CheckPermissions();
    }
    void Start()
    {
        //SetupVideoSDKEngine();
        //InitEventHandler();
       // SetupUI();
    }

    
    void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            // Destroy IRtcEngine
            RtcEngine.Dispose();
            RtcEngine = null;
        }
    }
   
    
}
