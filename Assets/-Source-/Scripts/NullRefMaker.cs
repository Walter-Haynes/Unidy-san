//using Sirenix.OdinInspector;
using UnityEngine;

public class NullRefMaker : MonoBehaviour
{

    #pragma warning disable 649
    private GameObject _gameObject;
    #pragma warning restore 649
    
    //[Button]
    [ContextMenu("Create Error")]
    void CreateNullReferenceException()
    {
        string __bla = _gameObject.name;
    }
}
