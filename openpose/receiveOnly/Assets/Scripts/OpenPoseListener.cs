using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using RecordAndPlay;

public class OpenPoseListener : DataListener
{
	[System.Serializable]
	public class StringEvent : UnityEvent<string>{};
	[SerializeField]
	public StringEvent updateEvent; 
	
    public override void ProcessData(DataFrame data)
    {
		StringData stringData = data as StringData;
		updateEvent.Invoke(stringData.data);
    }
}
