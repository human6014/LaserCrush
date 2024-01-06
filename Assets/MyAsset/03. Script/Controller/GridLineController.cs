using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Controller
{
    public class GridLineController : MonoBehaviour
    {
        [SerializeField] private GameObject m_GridLinePrefab;
        [SerializeField] private GameObject m_ParentGameObject;

        private LineRenderer[] m_RowlineRenderers;  //Y
        private LineRenderer[] m_CollineRenderers;  //X

        public void SetGridLineObjects(Vector2 initPos, Vector2 offset, int rowCount, int colCount)
        {
            m_RowlineRenderers = new LineRenderer[rowCount + 1];
            m_CollineRenderers = new LineRenderer[colCount + 1];

            GameObject obj;
            initPos = new Vector2(initPos.x - offset.x * 0.5f, initPos.y + offset.y * 0.5f);

            for(int i = 0; i < m_RowlineRenderers.Length; i++)
            {
                obj = Instantiate(m_GridLinePrefab, m_ParentGameObject.transform);
                m_RowlineRenderers[i] = obj.GetComponent<LineRenderer>();
                m_RowlineRenderers[i].SetPosition(0, new Vector3(initPos.x, initPos.y - offset.y * i, 0));
                m_RowlineRenderers[i].SetPosition(1, new Vector3(initPos.x + offset.x * colCount, initPos.y - offset.y * i, 0));
            }

            for(int i = 0; i < m_CollineRenderers.Length; i++)
            {
                obj = Instantiate(m_GridLinePrefab, m_ParentGameObject.transform);
                m_CollineRenderers[i] = obj.GetComponent<LineRenderer>();
                m_CollineRenderers[i].SetPosition(0, new Vector3(initPos.x + offset.x * i, initPos.y, 0));
                m_CollineRenderers[i].SetPosition(1, new Vector3(initPos.x + offset.x * i, initPos.y - offset.y * rowCount, 0));
            }
        }

        public void OnOffGridLine(bool isActive)
        {
            m_ParentGameObject.SetActive(isActive);
        }
    }
}
