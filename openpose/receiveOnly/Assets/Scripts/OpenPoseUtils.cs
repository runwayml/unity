using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public struct OpenPoseMapping
{
    public float width;
    public float height;
    public float offset;
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
