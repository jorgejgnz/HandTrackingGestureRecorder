using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct Gesture
{
    public string gestureName;
    public List<Vector3> positionsPerFinger; // Relative to hand
    public UnityEvent onRecognized;

    public Gesture(string name, List<Vector3> positions, UnityEvent onRecognized)
    {
        gestureName = name;
        positionsPerFinger = positions;
        this.onRecognized = onRecognized;
    }

    public Gesture(string name, List<Vector3> positions)
    {
        gestureName = name;
        positionsPerFinger = positions;
        onRecognized = new UnityEvent();
    }
}

[DisallowMultipleComponent]
public class GestureRecognizer : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] private List<Gesture> savedGestures = new List<Gesture>();
    [SerializeField] private float threshold = 1.0f;
    [SerializeField] private UnityEvent onNothingDetected = default;

    [Header("Objects")] 
    [SerializeField] private GameObject hand = default;
    [SerializeField] private GameObject[] fingers = default;

    [Header("Debugging")] 
    [SerializeField] private string gestureNameDetected = default;
    [SerializeField] private Gesture gestureDetected = default;
    [SerializeField] private string gestureName = default;

    private bool _sthWasDetected;

    private void Start()
    {
        _sthWasDetected = false;
        onNothingDetected.Invoke();
    }

    private void Update()
    {
        gestureDetected = Recognize();
        gestureNameDetected = gestureDetected.gestureName;

        if (gestureDetected.Equals(new Gesture()) && _sthWasDetected)
        {
            _sthWasDetected = false;
            onNothingDetected.Invoke();
        }
        else if (!gestureDetected.Equals(new Gesture()))
        {
            _sthWasDetected = true;
            gestureDetected.onRecognized.Invoke();
        }
    }

    public void SaveAsGesture()
    {
        var g = new Gesture {gestureName = gestureName};
        var positions = fingers.Select(t => hand.transform.InverseTransformPoint(t.transform.position)).ToList();
        g.positionsPerFinger = positions;
        savedGestures.Add(g);
    }

    private Gesture Recognize()
    {
        var discardGesture = false;
        var minSumDistances = Mathf.Infinity;
        var bestCandidate = new Gesture();

        // For each gesture
        for (var i = 0; i < savedGestures.Count; i++)
        {
            // If the number of fingers does not match, it returns an error
            if (fingers.Length != savedGestures[i].positionsPerFinger.Count)
                throw new Exception("Different number of tracked fingers");

            var sumDistances = 0f;

            // For each finger
            for (var j = 0; j < fingers.Length; j++)
            {
                var fingerRelativePos = hand.transform.InverseTransformPoint(fingers[j].transform.position);

                // If at least one finger does not enter the threshold we discard the gesture
                if (Vector3.Distance(fingerRelativePos, savedGestures[i].positionsPerFinger[j]) > threshold)
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
public class CustomInspectorGestureRecognizer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var gestureRecognizer = (GestureRecognizer) target;
        if (!GUILayout.Button("Save current gesture")) return;
        gestureRecognizer.SaveAsGesture();
    }
}
#endif
