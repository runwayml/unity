using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OpenPoseDebugDraw : MonoBehaviour {
	
	bool running = false;
	string [,] connections;
	JObject json = null;

	// Use this for initialization
	void Start () {
		SetupConnections();
		json = LoadTestJson();
		running = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnDrawGizmos(){
		if(!running)
			return;
			
		// Debug.Log(json["results"]["humans"]);
		JArray humans = (JArray)json["results"]["humans"]; // TODO check for existence 
		foreach(JToken human in humans)
		{
			DrawParts(human);
			DrawConnections(human);
		}
	}
	
	private void DrawParts(JToken human){
		foreach(JToken bodypart in human)
		{
				float x = (float)bodypart[1] * 100;
				float y = (float)bodypart[2] * 100;
				
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(new Vector3(x,-y,0),1);
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
				float sx = (float)start[1] * 100;
				float sy = (float)start[2] * 100;
				Vector3 startPoint = new Vector3(sx,-sy,0);
				float ex = (float)end[1] * 100;
				float ey = (float)end[2] * 100;
				Vector3 endPoint = new Vector3(ex,-ey,0);
				Gizmos.color = Color.red;
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
		
		for(int c = 0;c<connections.GetLength(0);c++){
			Debug.Log(connections[c,0]);
		}
	}
	
	private JObject LoadTestJson(){
		JObject json = JObject.Parse(@"{'results':{'humans':[[['Nose',0.42592592592592593,0.13043478260869565],['Neck',0.42592592592592593,0.34782608695652173],['Right_Shoulder',0.2962962962962963,0.34782608695652173],['Right_Elbow',0.16666666666666666,0.5434782608695652],['Left_Shoulder',0.5740740740740741,0.34782608695652173],['Left_Elbow',0.6666666666666666,0.6086956521739131],['Left_Ear',0.5,0.13043478260869565],['Left_Eye',0.4629629629629629,0.08695652173913043],['Right_Eye',0.38888888888888884,0.08695652173913043],['Right_Ear',0.37037037037037035,0.13043478260869565]]]}}");
		
		// Debug.Log(json.ToString());
		return json;
	}
}
