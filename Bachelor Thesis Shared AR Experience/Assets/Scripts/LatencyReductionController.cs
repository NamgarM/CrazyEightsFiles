using System;
using Fusion;
using Testing;
using UnityEngine;

public class LatencyReductionController : MonoBehaviour
{
    public Action OnPredictionSwitchedOn;
    [SerializeField] private NetworkProjectConfig _networkProjectConfig;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private bool _isPredictionOn = false;
    [SerializeField] private bool _isDeltaSnapshotsOn = false;
    [SerializeField] private bool _isPhysicsOn = true;
    
    void Start()
    {
        if(_isPredictionOn && OnPredictionSwitchedOn != null)
            OnPredictionSwitchedOn.Invoke();
        
        if (_isDeltaSnapshotsOn && _networkProjectConfig != null)
            _networkProjectConfig.Simulation.ReplicationMode = SimulationConfig.StateReplicationModes.DeltaSnapshots;
        else if(_networkProjectConfig != null)
        {
            _networkProjectConfig.Simulation.ReplicationMode = SimulationConfig.StateReplicationModes.EventualConsistency;
        }

        if (_isPhysicsOn)
            _networkProjectConfig.ServerPhysicsMode = NetworkProjectConfig.PhysicsModes.ClientPrediction;
        
        //_spawner.gameObject.SetActive(true);

    }
}
