using UnityEngine;

public class DragObject : MonoBehaviour
{
	[Tooltip("What GameObject layers should the click query against?")] 
	[SerializeField] private LayerMask clickLayerMask = ~0;

	private TargetJoint2D _joint;

	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			if(!_joint) return;
			
			Destroy(_joint);
			_joint = null;

			return;
		}

		Vector2 __pos = TransparentWindow.Camera.ScreenToWorldPoint(Input.mousePosition);

		if (_joint)
		{
			_joint.target = __pos;
			return;
		}

		if (!Input.GetMouseButtonDown(0)) return;

		Collider2D __overlapCollider = Physics2D.OverlapPoint(__pos, clickLayerMask);
		if (!__overlapCollider) return;

		Rigidbody2D __attachedRigidbody = __overlapCollider.attachedRigidbody;
		if (!__attachedRigidbody) return;

		_joint = __attachedRigidbody.gameObject.AddComponent<TargetJoint2D>();
		_joint.autoConfigureTarget = false;
		_joint.anchor = __attachedRigidbody.transform.InverseTransformPoint(__pos);
	}
}