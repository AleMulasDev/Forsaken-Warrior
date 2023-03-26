using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
    [SerializeField] protected Vector3 m_from = new Vector3(0.0F, 45.0F, 0.0F);
    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, -45.0F, 0.0F);
    [SerializeField] protected float m_frequency = 1.0F;
    private float _selfRotateTimer;
    private bool _shouldRotate = true;

    private void SelfRotateObject()
    {
        Quaternion from = Quaternion.Euler(this.m_from);
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * _selfRotateTimer * this.m_frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);
    }

    void Update()
    {
        if (!_shouldRotate) return;

        _selfRotateTimer += Time.deltaTime;
        SelfRotateObject();
    }

    public void StopRotating()
    {
        _shouldRotate = false;
    }
}
