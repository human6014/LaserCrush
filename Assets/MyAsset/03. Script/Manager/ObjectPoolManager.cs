using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;

namespace LaserCrush.Manager
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private static Transform m_DeActivePool;

        private void Awake() => m_DeActivePool = transform;

        /// <summary>
        /// Pooing 객체 등록과 동시에 count만큼 미리 생성
        /// </summary>
        /// <param name="poolableScript"> PoolableScript를 상속받는 객체 </param>
        /// <param name="parent"> Hierarchy 오브젝트 위치 </param>
        /// <param name="count"> 미리 생성할 개수</param>>
        /// <returns>PoolingObject</returns>
        public static PoolingObject RegisterAndGenerate(PoolableMonoBehaviour poolableScript, Transform parent, int count)
        {
            PoolingObject poolingObject = new PoolingObject(poolableScript, parent);
            poolingObject.GenerateObj(count);
            return poolingObject;
        }

        /// <summary>
        /// Pooling 정보 객체
        /// </summary>
        public class PoolingObject
        {
            private readonly Queue<PoolableMonoBehaviour> poolableQueue;
            private readonly PoolableMonoBehaviour script;
            private readonly Transform parent;

            public PoolingObject(PoolableMonoBehaviour script, Transform parent)
            {
                poolableQueue = new();
                this.script = script;
                this.parent = parent;
            }

            /// <summary>
            /// 등록된 오브젝트 생성
            /// </summary>
            /// <param name="_count"> 생성할 개수 </param>
            public void GenerateObj(int _count)
            {
                for (int i = 0; i < _count; ++i) poolableQueue.Enqueue(CreateNewObject());
            }

            /// <summary>
            /// 오브젝트 생성
            /// </summary>
            /// <returns></returns>
            private PoolableMonoBehaviour CreateNewObject()
            {
                var newObj = Instantiate(script);
                newObj.gameObject.SetActive(false);
                newObj.transform.SetParent(m_DeActivePool);
                return newObj;
            }

            /// <summary>
            /// 생성된 PoolableScript 반환
            /// </summary>
            /// <param name="preActive">오브젝트를 미리 활성화 할것인가</param>
            /// <returns>PoolableScript로 반환</returns>
            public PoolableMonoBehaviour GetObject(bool preActive)
            {
                PoolableMonoBehaviour obj;
                if (poolableQueue.Count > 0) obj = poolableQueue.Dequeue();
                else obj = CreateNewObject();

                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(preActive);

                return obj;
            }

            /// <summary>
            /// 오브젝트 Pool로 반납
            /// </summary>
            /// <param name="obj">PoolableScript를 상속받는 객체</param>
            public void ReturnObject(PoolableMonoBehaviour obj)
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(m_DeActivePool);
                poolableQueue.Enqueue(obj);
            }
        }
    }
}
