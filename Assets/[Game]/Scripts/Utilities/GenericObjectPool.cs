using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Game_.Scripts.Utilities
{
    public class GenericObjectPool : MonoSingleton<GenericObjectPool>
    {
        [SerializeField] private List<Pool<MonoBehaviour>> scriptPools;
        private Dictionary<string, ObjectPool<MonoBehaviour>> _scriptPoolDictionary;

        #region PoolClasses
        [Serializable]
        public class Pool<T> where T : MonoBehaviour
        {
            [Tooltip("Give a tag to the pool for calling it")]
            public string tag;

            [Tooltip("Prefab of the GameObject to be pooled")]
            public T prefab;

            [Tooltip("The size (count) of the pool")]
            public int softCap, hardCap;

            [Tooltip("Whether the GameObject create at Start")]
            public bool createAtStart;
        }

        #endregion

        private void Awake()
        {
            Initialize();
            CreateAtStart();
        }

        private void Initialize()
        {
            _scriptPoolDictionary = new Dictionary<string, ObjectPool<MonoBehaviour>>();
            var scriptPoolCount = scriptPools.Count;
            for (int i = 0; i < scriptPoolCount; i++)
            {
                ObjectPool<MonoBehaviour> pool = new ObjectPool<MonoBehaviour>(CreateFunctionSc(i), OnGameObjectGet,
                    OnGameObjectRelease, OnGameObjectDestroy, true, scriptPools[i].softCap, scriptPools[i].hardCap);
                string prefabName = scriptPools[i].tag;
                _scriptPoolDictionary.Add(prefabName, pool);
            }
        }

        public void AddToPool(Pool<MonoBehaviour> poolData)
        {
            ObjectPool<MonoBehaviour> pool = new ObjectPool<MonoBehaviour>(() =>
                {
                    var pooledObject = Instantiate(poolData.prefab);
                    return pooledObject;
                }, OnGameObjectGet,
                OnGameObjectRelease, OnGameObjectDestroy, true, poolData.softCap, poolData.hardCap);
            string prefabName = tag;
            _scriptPoolDictionary.Add(prefabName, pool);
        }

        private void CreateAtStart()
        {
            for (int i = 0; i < scriptPools.Count; i++)
            {
                var p = scriptPools[i];
                if (p.createAtStart)
                {
                    for (int j = 0; j < p.softCap; j++)
                    {
                        _scriptPoolDictionary[p.tag].Get();
                    }
                }
            }
        }

        #region ScriptSpawningMethods
        
        public T Spawn<T>(string poolTag, Vector3 position) where T : MonoBehaviour
        {
            var pooledObject = _scriptPoolDictionary[poolTag].Get();
            var t = pooledObject.transform;
            t.position = position;
            pooledObject.gameObject.SetActive(true);
            return (T) pooledObject;
        }

        public T Spawn<T>(string poolTag, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            var pooledObject = _scriptPoolDictionary[poolTag].Get();
            var t = pooledObject.transform;
            t.position = position;
            t.rotation = rotation;
            pooledObject.gameObject.SetActive(true);
            return (T) pooledObject;
        }

        public T Spawn<T>(string poolTag, Transform parent) where T : MonoBehaviour
        {
            var pooledObject = _scriptPoolDictionary[poolTag].Get();
            var t = pooledObject.transform;
            t.transform.parent = parent;
            t.localPosition = Vector3.zero;
            pooledObject.gameObject.SetActive(true);
            return (T) pooledObject;
        }

        public T Spawn<T>(string poolTag, Vector3 position, Transform parent) where T : MonoBehaviour
        {
            var pooledObject = _scriptPoolDictionary[poolTag].Get();
            var t = pooledObject.transform;
            t.position = position;
            t.SetParent(parent);
            pooledObject.gameObject.SetActive(true);
            return (T) pooledObject;
        }
        
        public T Spawn<T>(string poolTag, Vector3 position, Quaternion rotation, Transform parent)
            where T : MonoBehaviour
        {
            var pooledObject = _scriptPoolDictionary[poolTag].Get();
            var t = pooledObject.transform;
            t.position = position;
            t.rotation = rotation;
            t.SetParent(parent);
            pooledObject.gameObject.SetActive(true);
            return (T) pooledObject;
        }

        #endregion

        private void OnGameObjectGet(GameObject pooledObject)
        {
            pooledObject.transform.SetParent(transform);
            pooledObject.gameObject.SetActive(false);
        }

        private void OnGameObjectGet(MonoBehaviour pooledObject)
        {
            pooledObject.transform.SetParent(transform);
            pooledObject.gameObject.SetActive(false);
        }
        

        private Func<MonoBehaviour> CreateFunctionSc(int i)
        {
            return new Func<MonoBehaviour>(() =>
            {
                var pooledObject = Instantiate(this.scriptPools[i].prefab);
                return pooledObject;
            });
        }

        private void OnGameObjectRelease(GameObject pooledObject)
        {
            pooledObject.SetActive(false);
        }

        private void OnGameObjectRelease(MonoBehaviour pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }
        

        public void ReleasePooledObject(string poolTag, MonoBehaviour pooledObject)
        {
            _scriptPoolDictionary[poolTag].Release(pooledObject);
        }

      

        private void OnGameObjectDestroy(GameObject pooledObject)
        {
            Destroy(pooledObject);
        }

        private void OnGameObjectDestroy(MonoBehaviour pooledObject)
        {
            Destroy(pooledObject);
        }
    }
}