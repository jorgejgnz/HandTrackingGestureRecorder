using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct Gesture
{
    public string gestureName;
    public List<Vector3> positionsPerFinger; // Relative to hand
    public UnityEvent onRecognized;

    public Gesture(string name, List<Vector3> positions, UnityEvent onRecognized)
    {
        this.gestureName = name;
        this.positionsPerFinger = positions;
        this.onRecognized = onRecognized;
    }

    public Gesture(string name, List<Vector3> positions)
    {
        this.gestureName = name;
        this.positionsPerFinger = positions;
        this.onRecognized = new UnityEvent();
    }

}

public class GestureRecognizer : MonoBehaviour
{
    [SerializeField]
    public List<Gesture> savedGestures = new List<Gesture>();

    public float theresold = 1.0f;

    public UnityEvent onNothindDetected;

    [HideInInspector]
    public Gesture gestureDetected;
    bool sthWasDetected;

    [Header("Objects")]
    public GameObject hand;
    public GameObject[] fingers;

    [Header("Debugging")]
    public string gestureNameDetected = "";

    [Header("Saving")]
    public string gestureName = "";


    // Start is called before the first frame update
    void Start()
    {
        sthWasDetected = false;
        onNothindDetected.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        gestureDetected = Recognize();

        gestureNameDetected = gestureDetected.gestureName;

        if (gestureDetected.Equals(new Gesture()) && sthWasDetected)
        {
            sthWasDetected = false;
            onNothindDetected.Invoke();
        }
        else if (!gestureDetected.Equals(new Gesture()))
        {
            sthWasDetected = true;
            gestureDetected.onRecognized.Invoke();
        }
    }

    public void SaveAsGesture()
    {
        Vector3 fingerRelativePos;

        Gesture g = new Gesture();
        g.gestureName = gestureName;

        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < fingers.Length; i++)
        {
            fingerRelativePos = hand.transform.InverseTransformPoint(fingers[i].transform.position);
            positions.Add(fingerRelativePos);
        }

        g.positionsPerFinger = positions;

        savedGestures.Add(g);

    }

    public Gesture Recognize()
    {
        Vector3 fingerRelativePos;
        bool discardGesture = false;
        float sumDistances;
        float minSumDistances = Mathf.Infinity;
        Gesture bestCandidate = new Gesture();

        // For each gesture
        for (int i = 0; i < savedGestures.Count; i++)
        {
            // If the number of fingers does not match, it returns an error
            if (fingers.Length != savedGestures[i].positionsPerFinger.Count) throw new Exception("Different number of tracked fingers");

            sumDistances = 0f;

            // For each finger
            for (int j = 0; j < fingers.Length; j++)
            {
                fingerRelativePos = hand.transform.InverseTransformPoint(fingers[j].transform.position);

                // If at least one finger does not enter the theresold we discard the gesture
                if (Vector3.Distance(fingerRelativePos, savedGestures[i].positionsPerFinger[j]) > theresold)
                {
                    discardGesture = true;
                    break;
                }

                // If all the fingers entered, then we calculate the total of their distances
                sumDistances += Vector3.Distance(fingerRelativePos, savedGestures[i].positionsPerFinger[j]);
            }

            // If we have to discard the gesture, we skip it
            if (discardGesture)
            {
                discardGesture = false;
                continue;
            }

            // If it is valid and the sum of its distances is less than the existing record, it is replaced because it is a better candidate 
            if (sumDistances < minSumDistances)
            {
                minSumDistances = sumDistances;
                bestCandidate = savedGestures[i];
            }
        }

        // If we've found something, we'll return it
        // If we haven't found anything, we return it anyway (newly created object)
        return bestCandidate;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(GestureRecognizer))]
public class CustomInspector_GestureRecognizer : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GestureRecognizer myScript = (GestureRecognizer)target;
        if (GUILayout.Button("Save current gesture"))
        {
            myScript.SaveAsGesture();
            OnInspectorGUI();
        }



    }
}
#endif
