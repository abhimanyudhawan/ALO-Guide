using UnityEngine;
using System.Collections;
using ROSBridgeLib;
using System.Reflection;
using System;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.turtlesim;

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
	private ROSBridgeWebSocketConnection ros = null;	
	private Boolean _useJoysticks;
	private Boolean lineOn;

	// the critical thing here is to define our subscribers, publishers and service response handlers
	void Start () {
		_useJoysticks = Input.GetJoystickNames ().Length > 0;
		ros = new ROSBridgeWebSocketConnection ("ws://192.168.43.197", 9090);
		ros.AddSubscriber (typeof(Turtle1ColorSensor));
		ros.AddSubscriber (typeof(Turtle1Pose));
		ros.AddPublisher (typeof(Turtle1Teleop));
		ros.AddServiceResponse (typeof(Turtle1ServiceResponse));
		ros.Connect ();
		ros.CallService ("/turtle1/set_pen", "{\"off\": 0}");
		lineOn = true;
	}

	// extremely important to disconnect from ROS. OTherwise packets continue to flow
	void OnApplicationQuit() {
		if(ros!=null)
			ros.Disconnect ();
	}

	// Update is called once per frame in Unity. The Unity camera follows the robot (which is driven by
	// the ROS environment. We also use the joystick or cursor keys to generate teleoperational commands
	// that are sent to the ROS world, which drives the robot which ...
	public void Send_message () {
		float _dx=0.1f, _dy=0.2f;
		float linear = _dy * 0.5f;
		float angular = -_dx * 0.2f;

		TwistMsg msg = new TwistMsg (new Vector3Msg(linear, 10, 10), new Vector3Msg(-10, 40, angular));

		ros.Publish (Turtle1Teleop.GetMessageTopic (), msg);

		if (Input.GetKeyDown (KeyCode.T)) {
			if (lineOn)
				ros.CallService ("/turtle1/set_pen", "{\"off\": 1}");
			else
				ros.CallService ("/turtle1/set_pen", "{\"off\": 0}");
			lineOn = !lineOn;
		}
		ros.Render ();

	}
}
