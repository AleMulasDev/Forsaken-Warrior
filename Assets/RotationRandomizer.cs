using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
    [SerializeField] Vector3 rotation;
    [SerializeField] private float speed;

    [SerializeField] private float degrees = 0;

    private bool _shouldRotate = true;

    private void Start()
    {
        rotation = new Vector3(GetRandomRotation(rotation.x), GetRandomRotation(rotation.y), GetRandomRotation(rotation.z));
    }

    private float GetRandomRotation(float axis)
    {
        return (Random.Range(0, 2) == 0 ? axis : -axis);
    }

    void Update()
    {
        if (!_shouldRotate) return;

        transform.Rotate(rotation * Time.deltaTime, degrees * speed);
    }

    public void StopRotating()
    {
        _shouldRotate = false;
    }
}
