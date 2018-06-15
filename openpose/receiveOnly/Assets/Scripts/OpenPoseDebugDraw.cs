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
	private OpenPoseMapping mapping;
	
	// private members
	private bool dataReceived = false;
	private string receivedResults = "";
	private JObject json = null;
	

	void Start () {
		mapping = new OpenPoseMapping();
		
		// setup texture for webcam
		webcamTexture = new WebCamTexture();
	}
	
	void Update () {
		// parse incoming results
		if(dataReceived)
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
			mapping.height = targetHeight;
			mapping.width = (targetHeight/modelHeight) * modelWidth;
			mapping.offset = (targetWidth - mapping.width) / 2f;
		}else 
		{ 
			// simple scaling if no webcam feed
			mapping.height = targetHeight;
			mapping.width = targetWidth;
			mapping.offset = 0;
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
        Gizmos.matrix = transform.localToWorldMatrix;
		foreach(JToken human in humans)
		{
			Gizmos.color = partColor;
			OpenPoseGizmos.DrawParts(human,mapping,sphereRadiuis);
			
			Gizmos.color = lineColor;
			OpenPoseGizmos.DrawConnections(human,mapping);
		}
	}
	
	private void DrawWebCam(){
		if(useWebCamFeed)
		{
			Gizmos.DrawGUITexture(new Rect(0,0,webcamTexture.width,-webcamTexture.height),webcamTexture);
		}
	}
}
