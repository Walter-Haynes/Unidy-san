using UnityEngine;

using Sirenix.OdinInspector;

public class UnidySpawner : MonoBehaviour
{
    [Required]
    [SerializeField] private GameObject unidyPrefab = null;
    
    private void Start()
    {
        if(ErrorCatcher.InstanceExists)
        {
            ErrorCatcher.Instance.NewErrors_Event += SpawnUnidys;
        }
    }

    private void SpawnUnidys(int unidyCount)
    {
        Debug.Log($"amount of new Unidys is {unidyCount}");
        
        for(int __index = 0; __index < unidyCount; __index++)
        {
            Debug.Log("SPAWN!@");
            Instantiate(unidyPrefab);
        }
    }
}
