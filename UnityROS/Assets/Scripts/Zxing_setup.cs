using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class Zxing_setup : MonoBehaviour {
    private WebCamTexture camTexture;
    private Rect screenRect;
    private int x;
    public Text txt;
    Button caller;

    // Use this for initialization
    void Start () {
        caller.Send_image();
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        WebCamDevice[] webCam = WebCamTexture.devices;
        camTexture = new WebCamTexture
        {
            requestedHeight = Screen.height,
            requestedWidth = Screen.width,
        };
        if (camTexture != null)
        {
            camTexture.Play();
        }
        not_detected(0.2f);
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
            IBarcodeReader barcodeReader = new BarcodeReader();
            //decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODEE TEXT FROM QR:" + result.Text);
                GUI.TextArea(screenRect, "Detected!");
                caller.Send_image(int.Parse(result.Text), "done");
                not_detected(0.5f);
                
            }
            else
                txt.text = "Not detectde";
        }catch(System.Exception ex)
        {
            Debug.LogWarning(ex.Message);
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
    // Update is called once per frame
    void Update () {
		
	}
}
