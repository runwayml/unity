// Copyright (C) 2018 Cristobal Valenzuela
//
// This file is part of RunwayML.
//
// RunwayML is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// RunwayML is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with RunwayML.  If not, see <http://www.gnu.org/licenses/>.
//
// ===========================================================================

// RUNWAY
// www.runwayml.com

// OpenPose Demo:
// Receive OSC messages from Runway
// running the OpenPose model

// Crist—bal Valenzuela, Felix Lange (porting Cris' Processing example to Unity)
// April 2018
// cv965@nyu.edu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

/* 
	Bridge responsible for receiving OSC messages from Runway 
	and forwarding them to subscribers 
*/
public class OSCRunwayBridge : MonoBehaviour {
	
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
