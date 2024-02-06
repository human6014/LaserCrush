using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Manager;

public class TestAssetLaser : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_WaveVelocity;
    [SerializeField] private float m_DistDivisionValue = 1;
    [SerializeField] private float m_WaveAmplitude = 1.5f;
    [SerializeField] private float m_WaveFrequency = 0.5f;

    private LineRenderer m_LineRenderer;

    private Vector2 m_StartPoint;
    private Vector2 m_EndPoint;
    private Vector2 m_DirectionVector;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();

        m_StartPoint = transform.position;
        m_EndPoint = Vector2.zero;
        m_DirectionVector = transform.up;
    }

    private void Update()
    {
        Fire();
    }

    private void Fire()
    {
        RaycastHit2D hit = Physics2D.Raycast(m_StartPoint, m_DirectionVector, Mathf.Infinity, RayManager.LaserHitableLayer);
        m_EndPoint = hit.point;

        int pointCount = (int)(hit.distance / m_DistDivisionValue);

        m_LineRenderer.positionCount = pointCount + 1;

        float xDirection = m_DirectionVector.x;
        float yDirection = m_DirectionVector.y;
        Vector3 initPos = transform.position;
        Vector3 pos;

        float waveOffset;
        for (int i = 0; i < pointCount + 1; i++)
        {
            waveOffset = Mathf.Sin(i * m_WaveFrequency) * m_WaveAmplitude;

            pos = new Vector3(
                initPos.x + m_DistDivisionValue * xDirection * i + waveOffset * -m_DirectionVector.y,
                initPos.y + m_DistDivisionValue * yDirection * i + waveOffset * m_DirectionVector.x,
                0);

            m_LineRenderer.SetPosition(i, pos);
        }
    }
}
