 using System;
using System.Collections.Generic;
using Fusion;
using Testing;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARUser : NetworkBehaviour
{
    public Action<Vector3, Quaternion> OnScreenTouch; 
    private GameObject _spawnedObj;
    private ARRaycastManager _arRaycastManager;
    private Vector2 _touchPosition;
    private Spawner _spawner;
    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>(); 
    
    void Awake()
    {
        _arRaycastManager = GetComponentInChildren<ARRaycastManager>();
        _spawner = FindObjectOfType<Spawner>();
        OnScreenTouch += _spawner.PlaceTable;
    }

    private bool GetTouchPos(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            return true;
        }

        touchPos = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetTouchPos(out Vector2 touchPos))
            return;
        
        if (_arRaycastManager.Raycast(touchPos, _hits, TrackableType.PlaneWithinPolygon))
        {
                var hitPose = _hits[0].pose;
                if (_spawnedObj == null)
                {
                    OnScreenTouch.Invoke(hitPose.position, hitPose.rotation);
                }
        }
    }
}
