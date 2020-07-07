using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WindowBoundsCollider2D : MonoBehaviour
{
	private Camera _camera;
	private EdgeCollider2D _borderCollider;

	[Tooltip("Camera-relative size of the bounds (1 = full window, 0.5 = half). Useful for safe-areas")] [SerializeField]
	private float scale = 1f;

	[Tooltip("A larger radius helps prevent fast-moving objects from clipping through")] [SerializeField]
	private float edgeRadius = 10f;

	private void Start()
	{
		CreateCollider();
	}

	private void CreateCollider()
	{
		_camera = GetComponent<Camera>();
		_borderCollider = gameObject.AddComponent<EdgeCollider2D>();

		float __cameraPlane = _camera.orthographic ? 0 : -_camera.transform.position.z;
		_borderCollider.edgeRadius = edgeRadius;

		float __maxScale = scale;
		float __minScale = 1f - scale;
		_borderCollider.points = new[]
		{
			(Vector2) _camera.ViewportToWorldPoint(new Vector3(__minScale, __minScale, __cameraPlane)) + new Vector2(-edgeRadius, -edgeRadius),
			(Vector2) _camera.ViewportToWorldPoint(new Vector3(__minScale, __maxScale, __cameraPlane)) + new Vector2(-edgeRadius, edgeRadius),
			(Vector2) _camera.ViewportToWorldPoint(new Vector3(__maxScale, __maxScale, __cameraPlane)) + new Vector2(edgeRadius, edgeRadius),
			(Vector2) _camera.ViewportToWorldPoint(new Vector3(__maxScale, __minScale, __cameraPlane)) + new Vector2(edgeRadius, -edgeRadius),
			(Vector2) _camera.ViewportToWorldPoint(new Vector3(__minScale, __minScale, __cameraPlane)) + new Vector2(-edgeRadius, -edgeRadius),
		};
	}

	private void OnDrawGizmosSelected()
	{
		float __maxScale = scale;
		float __minScale = 1f - scale;

		if (!_camera)
		{
			_camera = GetComponent<Camera>();
		}

		float __cameraPlane = _camera.orthographic ? 0 : -_camera.transform.position.z;
		Vector3 __pointA = _camera.ViewportToWorldPoint(new Vector3(__minScale, __minScale, __cameraPlane));
		Vector3 __pointB = _camera.ViewportToWorldPoint(new Vector3(__minScale, __maxScale, __cameraPlane));
		Vector3 __pointC = _camera.ViewportToWorldPoint(new Vector3(__maxScale, __maxScale, __cameraPlane));
		Vector3 __pointD = _camera.ViewportToWorldPoint(new Vector3(__maxScale, __minScale, __cameraPlane));
		Gizmos.DrawLine(__pointA, __pointB);
		Gizmos.DrawLine(__pointB, __pointC);
		Gizmos.DrawLine(__pointC, __pointD);
		Gizmos.DrawLine(__pointD, __pointA);
	}
}