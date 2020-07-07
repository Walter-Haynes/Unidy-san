using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class UnidySpawner : MonoBehaviour
{
    //[Required]
    [SerializeField] private GameObject unidyPrefab = null;

    private int UnidyCountToSpawn { get; set; } = 0;
    
    private void Start()
    {
        if(ErrorCatcher.InstanceExists)
        {
            ErrorCatcher.Instance.NewErrors_Event += PrimeUnidySpawning;
        }
    }

    private void OnDisable()
    {
        if(ErrorCatcher.InstanceExists)
        {
            ErrorCatcher.Instance.NewErrors_Event -= PrimeUnidySpawning;
        }
    }

    private readonly Vector3 _screenCenter = new Vector3(x: Screen.width / 2f, y: Screen.height / 2f);

    private void PrimeUnidySpawning(int newUnidyCount)
        => UnidyCountToSpawn += newUnidyCount;

    private void Update()
    {
        SpawnUnidySans();
    }

    private void SpawnUnidySans()
    {
        if(unidyPrefab == null)
        {
            Debug.LogWarning("unidyPrefab is NULL");
            return;
        }

        for(int i = 0; i < UnidyCountToSpawn; i++)
        {
            
            Debug.Log(message: "<i> Get Position </i>");
            
            Vector3 __position = TransparentWindow.Camera.ScreenToWorldPoint(position: _screenCenter); //Input.mousePosition);
            //Vector3 __position = Vector3.zero;

            Debug.Log(message: "<i> Pre-Spawn </i>");
                
            Object.Instantiate(original: unidyPrefab, __position, rotation: Quaternion.identity);
                
            Debug.Log(message: "<b> - <i> SPAWN! </i> - </b>");
        }

        UnidyCountToSpawn = 0;
    }
    
    
}
