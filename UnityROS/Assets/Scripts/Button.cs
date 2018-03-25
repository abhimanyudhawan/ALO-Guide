using UnityEngine;
using System.Collections;
//using ROSBridgeLib;
using System.Reflection;
using System;
//using ROSBridgeLib.geometry_msgs;
//using ROSBridgeLib.turtlesim;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using ZXing.Multi;
//using System.Net.Http;

/**
 * This is a toy example of the Unity-ROS interface talking to the TurtleSim 
 * tutorial (circa Groovy). Note that due to some changes since then this will have
 * to be slightly re-written, but as its a test ....
 * 
 * THis does all the ROS work.
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.0
 **/

public class Button : MonoBehaviour  {
    static string previous = null;
    private WebCamTexture camTexture;
    private Rect screenRect;
    private int x;
    public Text txt;

 //   private ROSBridgeWebSocketConnection ros = null;	
	//private Boolean _useJoysticks;
	//private Boolean lineOn;

	// the critical thing here is to define our subscribers, publishers and service response handlers
	void Start () {
		//_useJoysticks = Input.GetJoystickNames ().Length > 0;
		//ros = new ROSBridgeWebSocketConnection ("ws://192.168.43.197", 9090);
		//ros.AddSubscriber (typeof(Turtle1ColorSensor));
		//ros.AddSubscriber (typeof(Turtle1Pose));
		//ros.AddPublisher (typeof(Turtle1Teleop));
		//ros.AddServiceResponse (typeof(Turtle1ServiceResponse));
		//ros.Connect ();
		//ros.CallService ("/turtle1/set_pen", "{\"off\": 0}");
		//lineOn = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        while (Screen.width<100)
        {

        }
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        WebCamDevice[] webCam = WebCamTexture.devices;
        camTexture = new WebCamTexture
        {
            requestedHeight = Screen.height,
            requestedWidth = Screen.width,
            deviceName = webCam[0].name
        };
        if (camTexture != null)
        {
            camTexture.Play();
        }
        not_detected(1f);
        InvokeRepeating("CrazyFocusPocus", 1f, 1f);
    }

    IEnumerator not_detected(float time)
    {
        yield return new WaitForSeconds(time);
        GUI.TextArea(screenRect, "not detected");
        // Code to execute after the delay
    }
    private void OnGUI()
    {
        //drawing the camera on screen

        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);

        //do the reading - you might want to attempt to read less often than you draw on the screen for performance sake
        try
        {
            BarcodeReader barcodeReader = new BarcodeReader();
            //decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODEE TEXT FROM QR:" + result.Text);
                GUIStyle p = new GUIStyle();
                p.fontSize = 50;
                p.fontStyle = FontStyle.Bold;
                p.normal.textColor = Color.cyan;
                GUI.TextArea(screenRect, "Detected! "+result.Text, p);
                if (result.Text != previous)
                {
                    Send_image(int.Parse(result.Text), "done");
                    previous = result.Text;
                }
                not_detected(1f);

            }
            else
                txt.text = "Not detectde";
        }
        catch (System.Exception ex)
        {
            //Debug.LogWarning(ex.Message);
        }
    }

    void CrazyFocusPocus()
    {

        // Get activity instance (standard way, solid)
        var pl_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = pl_class.GetStatic<AndroidJavaObject>("currentActivity");

        // Get instance of UnityPlayer (hacky but will live)
        var pl_inst = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");

        // Get list of camera wrappers in UnityPlayer (very hacky, will die if D becomes C tomorrow)
        var list = pl_inst.Get<AndroidJavaObject>("y");
        x = list.Call<int>("size");

        if (x == 0) return;

        // Get the first element of list (active camera wrapper)
        var cam_holder = list.Call<AndroidJavaObject>("get", 0);

        // get camera (this is totally insane, again if "a" becomes not-"a" one day )
        var cam = cam_holder.Get<AndroidJavaObject>("a");

        // Call my setup camera routine in Android plugin  (will set params and call autoFocus)
        var jc = new AndroidJavaClass("org.example.ScriptBridge.JavaClass");
        jc.CallStatic("enableAutofocus", new[] { cam });

    }
    // extremely important to disconnect from ROS. OTherwise packets continue to flow
 //   void OnApplicationQuit() {
	//	if(ros!=null)
	//		ros.Disconnect ();
	//}

	public class MyClass
		{
				public int book_id;
				public string location;
				//public string playerName;
		}

	public void Send_image (int id=9, string loc="ooo"){
                Debug.Log("senttttt!!");
				MyClass myObject = new MyClass ();
				myObject.book_id = id;
				myObject.location = loc;
				string jason = JsonUtility.ToJson (myObject);


				//string str = "{'name':'ish'}";
				StartCoroutine(postRequest("https://calm-cliffs-36731.herokuapp.com/check",jason));
				// Load an image from a local file.
				Debug.Log("done");

		}

		IEnumerator postRequest(string url, string json){
				var uwr = new UnityWebRequest(url, "POST");
				byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
				uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
				uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
				uwr.SetRequestHeader("Content-Type", "application/json");

				//Send the request then wait here until it returns
				yield return uwr.SendWebRequest();

				if (uwr.isNetworkError)
				{
						Debug.Log("Error While Sending: " + uwr.error);
				}
				else
				{
						Debug.Log("Received: " + uwr.downloadHandler.text);
				}

		}
	// Update is called once per frame in Unity. The Unity camera follows the robot (which is driven by
	// the ROS environment. We also use the joystick or cursor keys to generate teleoperational commands
	// that are sent to the ROS world, which drives the robot which ...
	//public void Send_message () {
	//	float _dx=0.1f, _dy=0.2f;
	//	float linear = _dy * 0.5f;
	//	float angular = -_dx * 0.2f;

	//	TwistMsg msg = new TwistMsg (new Vector3Msg(linear, 10, 10), new Vector3Msg(-10, 40, angular));

	//	ros.Publish (Turtle1Teleop.GetMessageTopic (), msg);

	//	if (Input.GetKeyDown (KeyCode.T)) {
	//		if (lineOn)
	//			ros.CallService ("/turtle1/set_pen", "{\"off\": 1}");
	//		else
	//			ros.CallService ("/turtle1/set_pen", "{\"off\": 0}");
	//		lineOn = !lineOn;
	//	}
	//	ros.Render ();

	//}
}
