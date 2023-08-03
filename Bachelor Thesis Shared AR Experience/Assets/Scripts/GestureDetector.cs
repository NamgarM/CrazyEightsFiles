using System;
using System.Collections;
using System.Collections.Generic;
using Testing;
using UnityEngine;

public class GestureDetector : MonoBehaviour
{
    [SerializeField] private float _detectionThreshold = 0.1f;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private List<Gesture> _gestures;
    [SerializeField] private OVRSkeleton _ovrSkeleton;
    [SerializeField] private bool _isSavingMode = true;
    
    private List<OVRBone> _fingerBones;
    
    private bool _areBonesInitialized = false;
    private bool _isTablePlaced = false;
    
    private Gesture _previousGesture;
    private Gesture _currentGesture;
    public Action<Vector3, Quaternion> OnHandGesture;
    
    private void Awake()
    {
        _spawner.OnVrUserSpawned += SetSkeleton;
    }

    private void SetSkeleton(UserComponents userComponents)
    {
        _ovrSkeleton = userComponents.LeftOvrSkeleton;

        var setBones = SetBones();
        StartCoroutine(setBones);
    }

    IEnumerator SetBones()
    {
        while (_ovrSkeleton.Bones.Count == 0) {
            yield return null;
        }

        _fingerBones = new List<OVRBone>(_ovrSkeleton.Bones);
        _previousGesture = new Gesture();
        _areBonesInitialized = true;
    }

    private void Update()
    {
        if (_areBonesInitialized)
        {
            if (Input.GetKeyDown(KeyCode.Space) && _isSavingMode)
                SaveGesture();

            var _currentGesture = DetectGesture();
            var hasDetected = !_currentGesture.Equals(new Gesture());
            // Check, if new gesture was detected
            if (hasDetected && !_currentGesture.Equals(_previousGesture) && _currentGesture.Name != "")
            {
                _previousGesture = _currentGesture;
                if(!_isTablePlaced && _currentGesture.Name == "Point with Two Fingers")
                {
                    //OnHandGesture.Invoke(_currentGesture.HandPos, Quaternion.identity);
                    _isTablePlaced = true;
                }
            }
        }
    }

    private void SaveGesture()
    {
        Gesture gesture = new Gesture();
        gesture.Name = "Gesture";
        var data = new List<Vector3>();
        foreach (var fingerBone in _fingerBones)
        {// possible solutions: local pos bones, local rotation of bones, dist, etc
            data.Add(_ovrSkeleton.transform.InverseTransformPoint(fingerBone.Transform.position));
        }

        gesture.FingerDatas = data;
        _gestures.Add(gesture);
    }

    private Gesture DetectGesture()
    {
        Gesture currentGesture = new Gesture();
        float currentMinDist = Mathf.Infinity;
        
        foreach (var gesture in _gestures)
        {
            var sumDist = 0f;
            var isDiscarded = false;
            for (int i = 0; i < _fingerBones.Count; i++)
            {
                var currentInfo =
                    _ovrSkeleton.transform.InverseTransformPoint(_fingerBones[i].Transform.position);
                var dist = Vector3
                    .Distance(currentInfo, gesture.FingerDatas[i]);
                if (dist > _detectionThreshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDist += dist;
            }

            if (!isDiscarded && sumDist < currentMinDist)
            {
                currentMinDist = sumDist;
                currentGesture = gesture;
            }
        }

        currentGesture.HandPos = _ovrSkeleton.transform.position;

        return currentGesture;
    }
}

[Serializable]
public class Gesture
{
    public string Name;
    public List<Vector3> FingerDatas;
    public Vector3 HandPos;
}
