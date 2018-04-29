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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/* 
	OpenPoseDebugDraw is taking care of parsing the Json results 
	and drawing connections and parts for every human
 */
public class OpenPoseDebugDraw : MonoBehaviour {
	
	public float sphereRadiuis = 0.02f; //parts are drawn as spheres
	public bool drawTestData = false;
	
	private string [,] connections;
	private bool running = false;
	private bool dataReceived = false;
	private string receivedResults = "";
	private JObject json = null;

	void Start () {
		// setting up pairs of body connections
		SetupConnections();
		
		// subscribe to runway results
		OSCRunwayBridge.SubscribeResultsHandler(this.UpdateResults);
		
		//only draw gizmos if application is running
		running = true; 
	}
	
	void Update () {
		// parse test data or incoming life results
		if(drawTestData)
		{
			json = LoadTestJson();		
		}else if(dataReceived)
		{
			json = JObject.Parse(receivedResults);
		}
	}
	
	// handler for receiving results
	public void UpdateResults(string results){
		receivedResults = results;
		dataReceived = true;
	}
	
	void OnDrawGizmos(){
		if(!running || json == null)
			return;
			
		JArray humans = (JArray)json["results"]["humans"]; 
		foreach(JToken human in humans)
		{
			DrawParts(human);
			DrawConnections(human);
		}
	}
	
	private void DrawParts(JToken human){
		foreach(JToken bodypart in human)
		{
				float x = (float)bodypart[1];
				float y = (float)bodypart[2];
				
				Gizmos.color = Color.yellow;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawSphere(new Vector3(x,-y,0),sphereRadiuis);
		}
	}
	
	private void DrawConnections(JToken human){
		for(int c = 0;c<connections.GetLength(0);c++)
		{
			JToken start = null, end = null;
			foreach(JToken bodypart in human)
			{
				string name = (string)bodypart[0];
				
				if(name == connections[c,0])
				{
					start = bodypart;
				}
				else if(name == connections[c,1])
				{
					end = bodypart;
				}
			}
			
			if(start != null && end != null){
				float sx = (float)start[1];
				float sy = (float)start[2];
				Vector3 startPoint = new Vector3(sx,-sy,0);
				float ex = (float)end[1];
				float ey = (float)end[2];
				Vector3 endPoint = new Vector3(ex,-ey,0);
				Gizmos.color = Color.red;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawLine(startPoint,endPoint);
			}
		}
	}
	
	private void SetupConnections(){
		connections = new string[,] {
        {"Nose", "Left_Eye"},
        {"Left_Eye", "Left_Ear"},
        {"Nose", "Right_Eye"},
        {"Right_Eye", "Right_Ear"},
        {"Nose", "Neck"},
        {"Neck", "Right_Shoulder"},
        {"Neck", "Left_Shoulder"},
        {"Right_Shoulder", "Right_Elbow"},
        {"Right_Elbow", "Right_Wrist"},
        {"Left_Shoulder", "Left_Elbow"},
        {"Left_Elbow", "Left_Wrist"},
        {"Neck", "Right_Hip"},
        {"Right_Hip", "Right_Knee"},
        {"Right_Knee", "Right_Ankle"},
        {"Neck", "Left_Hip"},
        {"Left_Hip", "Left_Knee"},
        {"Left_Knee", "Left_Ankle"}
    };
	}
	
	private JObject LoadTestJson(){
		JObject json = JObject.Parse(@"{'results':{'humans':[[['Nose',0.42592592592592593,0.13043478260869565],['Neck',0.42592592592592593,0.34782608695652173],['Right_Shoulder',0.2962962962962963,0.34782608695652173],['Right_Elbow',0.16666666666666666,0.5434782608695652],['Left_Shoulder',0.5740740740740741,0.34782608695652173],['Left_Elbow',0.6666666666666666,0.6086956521739131],['Left_Ear',0.5,0.13043478260869565],['Left_Eye',0.4629629629629629,0.08695652173913043],['Right_Eye',0.38888888888888884,0.08695652173913043],['Right_Ear',0.37037037037037035,0.13043478260869565]]]}}");
		
		return json;
	}
}
