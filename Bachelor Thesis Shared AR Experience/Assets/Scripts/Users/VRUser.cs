using BachelorThesis;
using Fusion;
using Testing;
using UnityEngine;

public class VRUser : NetworkBehaviour
{
    private Spawner _spawner;

    private void Start()
    {
        _spawner = FindObjectOfType<Spawner>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.ServerSpawningPos != Vector3.zero && _spawner != null)
            {
                _spawner.SpawningPos.position = data.ServerSpawningPos;
                Debug.Log("Table placed");
            }
        }
    }
}
