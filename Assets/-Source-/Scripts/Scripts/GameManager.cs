using UnityEngine;

public class GameManager : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Quit();
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}
