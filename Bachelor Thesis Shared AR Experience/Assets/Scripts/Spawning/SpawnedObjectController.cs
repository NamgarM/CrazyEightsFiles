using BachelorThesis;
using Fusion;
using UnityEngine;

public class SpawnedObjectController : NetworkBehaviour
{
    [Networked] public Vector3 CurrentPos { get; set; }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Runner.IsClient)
        {
            transform.position = CurrentPos;
            Debug.Log("Pos: "+transform.position);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (CurrentPos != transform.position)
                CurrentPos = transform.position;
        }
    }
}
