using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	[Tooltip("Prefab to spawn on click")] [SerializeField]
	private GameObject prefab = null;

	[Tooltip("If enabled, SystemInput will allow clicks to be detected even without window focus")] [SerializeField]
	private bool useSystemInputIfAvailable = false;

	private void Update()
	{
		if (useSystemInputIfAvailable)
		{
			if (SystemInput.GetMouseButtonDown(1))
			{
				InstantiatePrefab();
			}

			return;
		}

		if (Input.GetMouseButtonDown(1))
		{
			InstantiatePrefab();
		}
	}

	private void InstantiatePrefab()
	{
		var pos = TransparentWindow.Camera.ScreenToWorldPoint(Input.mousePosition);
		Instantiate(prefab, pos, Quaternion.identity);
	}
}