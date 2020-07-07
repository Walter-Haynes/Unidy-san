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

    private readonly Vector3 _screenCenter = new Vector3(x: Screen.width / 2f, y: Screen.height / 2f);

    private void SpawnUnidys(int unidyCount)
    {
        //Debug.Log($"amount of new Unidys is {unidyCount}");

        //for(int i = 0; i < unidyCount; i++)

        if(unidyPrefab == null) return;
        Vector3 __position = TransparentWindow.Camera.ScreenToWorldPoint(position: _screenCenter); //Input.mousePosition);
            
        Object.Instantiate(original: unidyPrefab, __position, rotation: Quaternion.identity);
            
        Debug.Log("<b> - <i> SPAWN! </i> - </b>");
    }
}
