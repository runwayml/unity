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

using UnityEngine;
using UnityEngine.Events;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RecordAndPlay;

public class OpenPoseListener : DataListener
{
    public OpenPoseMapping mapping = new OpenPoseMapping();
    [Header("Styling")]
	public float sphereRadiuis = 20f; 
	public Color partColor = Color.red;
	public Color lineColor = Color.black;
    
    private JObject json = null;

    //forward frames via unity events
    public override void ProcessData(DataFrame data)
    {
        StringData stringData = data as StringData;
        json = JObject.Parse(stringData.data);
    }
    
    void OnDrawGizmos(){
			
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
}
