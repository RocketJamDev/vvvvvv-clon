using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Transition : MonoBehaviour
{
    public bool verticalTransition = false;
    public Transform leftRoom;
    public Transform rightRoom;
    public Transform topRoom;
    public Transform bottomRoom;
}

[CustomEditor(typeof(Transition))]
public class TransitionHandlerEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        Transition myScript = target as Transition;

        myScript.verticalTransition = GUILayout.Toggle(myScript.verticalTransition, "Vertical Transition");

        if (myScript.verticalTransition)
        {
            myScript.topRoom = EditorGUILayout.ObjectField("Top Room", myScript.topRoom, typeof(Transform), true) as Transform;
            myScript.bottomRoom = EditorGUILayout.ObjectField("Bottom Room", myScript.bottomRoom, typeof(Transform), true) as Transform;
        } 
        else
        {
            myScript.leftRoom = EditorGUILayout.ObjectField("Left Room", myScript.leftRoom, typeof(Transform), true) as Transform;
            myScript.rightRoom = EditorGUILayout.ObjectField("Right Room", myScript.rightRoom, typeof(Transform), true) as Transform;
        }

    }
}