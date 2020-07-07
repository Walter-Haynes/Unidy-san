using UnityEngine;

public class UnidySpawner : MonoBehaviour
{
    //[Required]
    [SerializeField] private GameObject unidyPrefab = null;
    
    private void Start()
    {
        if(ErrorCatcher.InstanceExists)
        {
            ErrorCatcher.Instance.NewErrors_Event += SpawnUnidys;
        }
    }

    private void OnDisable()
    {
        if(ErrorCatcher.InstanceExists)
        {
            ErrorCatcher.Instance.NewErrors_Event -= SpawnUnidys;
        }
    }

    private void SpawnUnidys(int unidyCount)
    {
        //Debug.Log($"amount of new Unidys is {unidyCount}");
        
        Debug.Log("<b> - <i> SPAWN! </i> - </b>");

        //for(int i = 0; i < unidyCount; i++)

        if(unidyPrefab != null)
        {
            Object.Instantiate(original: unidyPrefab, position: Vector3.zero, rotation: Quaternion.identity);   
        }
    }
}
