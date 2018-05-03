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
	and drawing connections and parts for every human as gizmos
 */
public class OpenPoseDebugDraw : MonoBehaviour {
	
	// Settings available in the inspector
	[Header("WebCam Settings")]
	public bool useWebCamFeed = true;
	private WebCamTexture webcamTexture = null;
	[Header("Styling")]
	public float sphereRadiuis = 20f; 
	public Color partColor = Color.red;
	public Color lineColor = Color.black;
	[Header("Mapping")]
	public float targetWidth = 1280;
	public float targetHeight = 720;
	private float modelWidth = 432;
	private float modelHeight = 368;
	private float mappingWidth;
	private float mappingHeight;
	private float mappingOffset;
	[Header("Test Data Set")]
	public bool enableTestDataSet = false;
	
	// private members
	private string [,] connections;
	private bool dataReceived = false;
	private string receivedResults = "";
	private JObject json = null;
	

	void Start () {
		// setting up pairs of body connections
		SetupConnections();
		
		// subscribe to runway results
		OSCRunwayBridge.SubscribeResultsHandler(this.UpdateResults);
		
		// setup texture for webcam
		// webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name,432,368,30); 
		// not working on internal mac cam
		webcamTexture = new WebCamTexture();
	}
	
	void Update () {
		// parse test data or incoming life results
		if(enableTestDataSet)
		{
			json = LoadTestJson();		
		}else if(dataReceived)
		{
			json = JObject.Parse(receivedResults);
		}
		
		// control webcam via inspector
		if(useWebCamFeed)
		{
			targetWidth = webcamTexture.width;
			targetHeight = webcamTexture.height;
			
			if(!webcamTexture.isPlaying)
			{
				webcamTexture.Play();
			}
		}else
		{
			if(webcamTexture.isPlaying)
			{
				webcamTexture.Stop();
			}
		}
		
		// update variables used for source to target mapping
		UpdateMappingData();
	}
	
	private void UpdateMappingData(){
		if(useWebCamFeed)
		{
			// runway seems to cut off the camera image to fit the model resolution
			mappingHeight = targetHeight;
			mappingWidth = (targetHeight/modelHeight) * modelWidth;
			mappingOffset = (targetWidth - mappingWidth) / 2f;
		}else 
		{ 
			// simple scaling if no webcam feed
			mappingHeight = targetHeight;
			mappingWidth = targetWidth;
			mappingOffset = 0;
		}
	}
	
	// handler for receiving results
	public void UpdateResults(string results){
		receivedResults = results;
		dataReceived = true;
	}
	
	void OnDrawGizmos(){
		if(!Application.isPlaying)
			return;
			
		DrawWebCam();
			
		if(json == null)
			return;	
			
		JArray humans = (JArray)json["results"]["humans"]; 
		foreach(JToken human in humans)
		{
			DrawParts(human);
			DrawConnections(human);
		}
	}
	
	private void DrawParts(JToken human){
		Gizmos.color = partColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		
		foreach(JToken bodypart in human)
		{
				Vector3 pos = GetMappedPosition((float)bodypart[1], (float)bodypart[2]);
				Gizmos.DrawSphere(pos,sphereRadiuis);
		}
	}
	
	private void DrawConnections(JToken human){
		Gizmos.color = lineColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		
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
			
			if(start != null && end != null)
			{
				Vector3 startPoint = GetMappedPosition((float)start[1], (float)start[2]);
				Vector3 endPoint = GetMappedPosition((float)end[1], (float)end[2]);
				Gizmos.DrawLine(startPoint,endPoint);
			}
		}
	}
	
	private Vector3 GetMappedPosition(float x, float y){
		float mappedX = x * mappingWidth + mappingOffset;
		float mappedY = y * mappingHeight;
		return new Vector3(mappedX,-mappedY,0);
	}
	
	private void DrawWebCam(){
		if(useWebCamFeed)
		{
			Gizmos.DrawGUITexture(new Rect(0,0,webcamTexture.width,-webcamTexture.height),webcamTexture);
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
