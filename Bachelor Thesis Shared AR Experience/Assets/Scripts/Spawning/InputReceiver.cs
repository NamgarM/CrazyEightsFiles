using System.Collections;
using System.Collections.Generic;
using BachelorThesis;
using Fusion;
using Testing;
using UnityEngine;

public class InputReceiver : NetworkBehaviour
{
    [SerializeField] private Spawner _spawner;
    [SerializeField] private Transform _spawningTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawningTransform = new GameObject().transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.ServerSpawningPos != Vector3.zero && _spawner.SpawningPos != null)
            {
                _spawningTransform.position = data.ServerSpawningPos;
                _spawningTransform.rotation = data.ServerSpawningRotation;
                _spawner.SpawningPos = _spawningTransform;
            }
        }
    }
}
