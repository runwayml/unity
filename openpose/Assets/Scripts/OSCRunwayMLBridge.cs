using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

/* Bridge responsible for receiving OSC messages and forwarding them to subscribers */
public class OSCRunwayMLBridge : MonoBehaviour {
	
	public int port = 57200;
	
	private OSCReciever reciever;
	
	// delegate to subscribe for results
	public delegate void UpdateResultsDelegate(string results);
	private static UpdateResultsDelegate updateResults = null;
	
	public static void SubscribeResultsHandler(UpdateResultsDelegate handler){
		updateResults += handler;
	}

	void Start () {
		reciever = new OSCReciever();
		reciever.Open(port);
	}
	
	void Update () {
		// only processing/forwarding the last oscmessage in case data rate > frame rate
		bool found = false;
		OSCMessage newMessage = null;
		while(reciever.hasWaitingMessages()) 
		{
			newMessage = reciever.getNextMessage();
			found = true;
			Debug.Log("message received: "+newMessage.Address);
			Debug.Log(DataToString(newMessage.Data));
		}
		
		if(found && updateResults!=null)
		{
			// send result json string to all subscribers
			updateResults(newMessage.Data[0].ToString());
		}
	}
	
	// helper method to Log incoming messages
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
