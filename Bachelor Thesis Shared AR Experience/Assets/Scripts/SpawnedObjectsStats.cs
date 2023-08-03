using BachelorThesis;
using Fusion;
using UnityEngine;
using TMPro;
public class SpawnedObjectsStats : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _spawnedObjectsAmount;
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && data.SpawnedObjectsAmount != 0)
        {
            _spawnedObjectsAmount.gameObject.SetActive(true);
            _spawnedObjectsAmount.text = "Cuboids amount: "+ data.SpawnedObjectsAmount;
        }
    }
}
