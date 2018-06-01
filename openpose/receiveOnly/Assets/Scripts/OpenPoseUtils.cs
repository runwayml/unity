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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class OpenPoseMapping
{
    public float width = 1280;
    public float height = 720;
    public float offset = 0;
}

public static class OpenPoseGizmos
{

    private static string[,] connections = new string[,] {
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
        {"Left_Knee", "Left_Ankle"}};

    public static void DrawParts(JToken human, OpenPoseMapping mapping, float radius = 1)
    {
        foreach (JToken bodypart in human)
        {
            Vector3 pos = GetMappedPosition((float)bodypart[1], (float)bodypart[2], mapping);
            Gizmos.DrawSphere(pos, radius);
        }
    }

    public static void DrawConnections(JToken human, OpenPoseMapping mapping)
    {
        for (int c = 0; c < connections.GetLength(0); c++)
        {
            JToken start = null, end = null;
            foreach (JToken bodypart in human)
            {
                string name = (string)bodypart[0];

                if (name == connections[c, 0])
                {
                    start = bodypart;
                }
                else if (name == connections[c, 1])
                {
                    end = bodypart;
                }
            }

            if (start != null && end != null)
            {
                Vector3 startPoint = GetMappedPosition((float)start[1], (float)start[2], mapping);
                Vector3 endPoint = GetMappedPosition((float)end[1], (float)end[2], mapping);
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
    }

    private static Vector3 GetMappedPosition(float x, float y, OpenPoseMapping mapping)
    {
        float mappedX = x * mapping.width + mapping.offset;
        float mappedY = y * mapping.height;
        return new Vector3(mappedX, -mappedY, 0);
    }
}
