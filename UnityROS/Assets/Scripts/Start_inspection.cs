using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ROSBridgeLib.geometry_msgs;
using UnityEngine.Networking;
using ROSBridgeLib.turtlesim;
using ROSBridgeLib;
using System;

public class Start_inspection : MonoBehaviour {
    private ROSBridgeWebSocketConnection ros = null;
    private Boolean _useJoysticks;
    private Boolean lineOn;
    public void Start()
    {
        _useJoysticks = Input.GetJoystickNames().Length > 0;
        ros = new ROSBridgeWebSocketConnection("ws://192.168.43.197", 9090);
        ros.AddSubscriber(typeof(Turtle1ColorSensor));
        ros.AddSubscriber(typeof(Turtle1Pose));
        ros.AddPublisher(typeof(Turtle1Teleop));
        ros.AddServiceResponse(typeof(Turtle1ServiceResponse));
        ros.Connect();
        ros.CallService("/turtle1/set_pen", "{\"off\": 0}");
        lineOn = true;

    }

    public void NextScreen()
    {
        SceneManager.LoadScene("button");
    }
    void OnApplicationQuit()
    {
        if (ros != null)
            ros.Disconnect();
    }
    public void Send_message()
    {
        float _dx = 0.1f, _dy = 0.2f;
        float linear = _dy * 0.5f;
        float angular = -_dx * 0.2f;

        TwistMsg msg = new TwistMsg(new Vector3Msg(linear, 10, 10), new Vector3Msg(-10, 40, angular));

        ros.Publish(Turtle1Teleop.GetMessageTopic(), msg);
        Debug.Log("Message sent to ROS");
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (lineOn)
                ros.CallService("/turtle1/set_pen", "{\"off\": 1}");
            else
                ros.CallService("/turtle1/set_pen", "{\"off\": 0}");
            lineOn = !lineOn;
        }
        ros.Render();

    }

}
