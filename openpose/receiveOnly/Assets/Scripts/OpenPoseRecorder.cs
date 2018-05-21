using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RecordAndPlay;

//record mouse position as world coordinates
public class OpenPoseRecorder : StringRecorder
{
    // handler for receiving results
    public void UpdateResults(string results)
    {
        if (isRecording)
        {
            RecordData(results);
        }
    }
}
