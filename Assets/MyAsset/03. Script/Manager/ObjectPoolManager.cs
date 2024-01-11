using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class ObjectPoolManager : MonoBehaviour
    {
        [SerializeField] private Transform m_ActiveMonsterPool;
        [SerializeField] private Transform m_ActiveObjectPool;
        private static Transform cachedTransform;

        private void Awake() => cachedTransform = transform;

        /// <summary>
        /// Pooing 객체 등록
        /// </summary>
        /// <param name="_poolableScript"> PoolableScript를 상속받는 객체 </param>
        /// <param name="_parent"> Hierarchy 오브젝트 위치 </param>
        /// <returns>PoolingObject</returns>
        public static PoolingObject Register(PoolableScript _poolableScript, Transform _parent) => new(_poolableScript, _parent);

        /// <summary>
        /// Pooling 정보 객체
        /// </summary>
        public class PoolingObject
        {
            private readonly Queue<PoolableScript> poolableQueue;
            private readonly PoolableScript script;
            private readonly Transform parent;

            public PoolingObject(PoolableScript _script, Transform _parent)
            {
                poolableQueue = new();
                script = _script;
                parent = _parent;
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
            private PoolableScript CreateNewObject() 
            {
                var newObj = Instantiate(script);
                newObj.gameObject.SetActive(false);
                newObj.transform.SetParent(cachedTransform);
                return newObj;
            }

            /// <summary>
            /// 생성된 PoolableScript 반환
            /// </summary>
            /// <param name="preActive">오브젝트를 미리 활성화 할것인가</param>
            /// <returns>PoolableScript로 반환</returns>
            public PoolableScript GetObject(bool preActive, bool preSetParent = true)
            {
                PoolableScript obj;
                if (poolableQueue.Count > 0) obj = poolableQueue.Dequeue();
                else                         obj = CreateNewObject();

                if (preSetParent) obj.transform.SetParent(parent);
                obj.gameObject.SetActive(preActive);

                return obj;
            }

            /// <summary>
            /// 오브젝트 Pool로 반납
            /// </summary>
            /// <param name="obj">PoolableScript를 상속받는 객체</param>
            public void ReturnObject(PoolableScript obj)
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(cachedTransform);
                poolableQueue.Enqueue(obj);
            }
        }
    }
}
