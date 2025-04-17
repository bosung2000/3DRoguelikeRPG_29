using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class TestWeapon : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    void Reset()
    {
        FitCollider();
    }

    public void FitCollider()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Renderer rend = GetComponent<Renderer>();

        Bounds bounds = rend.bounds;
        Vector3 size = transform.InverseTransformVector(bounds.size);

        col.direction = 1;

        col.radius = Mathf.Min(size.x, size.z) / 2f;

        col.height = size.y;

        Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
        col.center = new Vector3(0f, localCenter.y, 0f);
    }
    public void Drop()
    {
        _rb.isKinematic = false;
        _rb.useGravity = true;
    }
}
