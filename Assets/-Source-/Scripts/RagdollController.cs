using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private GameObject leftHandIk, rightHandIk, leftFootIk, rightFootIk;
    [SerializeField] private Animator animator;
    public bool RagdollActive { get; private set; }

    private HingeJoint2D[] joints;
    private Rigidbody2D[] rbs;

    private Dictionary<Rigidbody2D, Vector3> initialPos = new Dictionary<Rigidbody2D, Vector3>();
    private Dictionary<Rigidbody2D, Quaternion> initialRot = new Dictionary<Rigidbody2D, Quaternion>();

    // Start is called before the first frame update
    private void Awake()
    {
        joints = GetComponentsInChildren<HingeJoint2D>();
        rbs = GetComponentsInChildren<Rigidbody2D>();

        foreach (var rb in rbs)
        {
            initialPos.Add(rb, rb.transform.localPosition);
            initialRot.Add(rb, rb.transform.localRotation);
        }

        DisableRagdoll();
    }

    private void RecordTransform()
    {
        foreach (var rb in rbs)
        {
            initialPos[rb] = rb.transform.localPosition;
            initialRot[rb] = rb.transform.localRotation;
        }
    }

    public void ActivateRagdoll()
    {
        RagdollActive = true;
        RecordTransform(); //record last bones transform, for disabling later
        animator.enabled = false;
        foreach (var rb in rbs)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        foreach (var joint in joints)
        {
            joint.enabled = true;
        }

        leftHandIk.SetActive(false);
        leftFootIk.SetActive(false);
        rightHandIk.SetActive(false);
        rightFootIk.SetActive(false);
    }

    public void DisableRagdoll()
    {
        RagdollActive = false;
        animator.enabled = true;
        foreach (var rb in rbs)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            rb.transform.localPosition = initialPos[rb];
            rb.transform.localRotation = initialRot[rb];
        }

        foreach (var joint in joints)
        {
            joint.enabled = false;
        }

        leftHandIk.SetActive(true);
        leftFootIk.SetActive(true);
        rightHandIk.SetActive(true);
        rightFootIk.SetActive(true);
    }
}