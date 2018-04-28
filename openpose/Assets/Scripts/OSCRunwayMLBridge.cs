using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class OSCRunwayMLBridge : MonoBehaviour {
	
	public int port = 57200;
	public OpenPoseDebugDraw listener;
	
	private OSCReciever reciever;

	void Start () {
		reciever = new OSCReciever();
		reciever.Open(port);
	}
	
	void Update () {
		bool found = false;
		OSCMessage newMessage = null;
		while(reciever.hasWaitingMessages())
		{
			newMessage = reciever.getNextMessage();
			found = true;
			Debug.Log("message received: "+newMessage.Address);
			Debug.Log(DataToString(newMessage.Data));
		}
		
		if(found && listener)
		{
			listener.sendResults(newMessage.Data[0].ToString());
		}
	}
	
	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}
}
