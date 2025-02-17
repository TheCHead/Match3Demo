using UnityEngine;
using UnityEngine.Pool;

namespace Scripts.Utility
{
    public class MonoPool<T> where T : MonoBehaviour, IPoolObject<T>
    {
        public enum PoolType
        {
            Stack,
            LinkedList
        }

        private T _prefab;
        private int _maxPoolSize = 64;
        private PoolType _poolType = PoolType.Stack;
        // Collection checks will throw errors if we try to release an item that is already in the pool.
        private bool _collectionChecks = true;
        private Transform _parent;


        public MonoPool(T prefab, Transform parent, int maxPoolSize, PoolType type = PoolType.Stack, bool collectionChecks = true)
        {
            _prefab = prefab;
            _parent = parent;
            _poolType = type;
            _collectionChecks = collectionChecks;
            _maxPoolSize = maxPoolSize;
        }

        IObjectPool<T> _pool;

        public IObjectPool<T> Pool
        {
            get
            {
                if (_pool == null)
                {
                    if (_poolType == PoolType.Stack)
                        _pool = new ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, 10, _maxPoolSize);
                    else
                        _pool = new LinkedPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, _collectionChecks, _maxPoolSize);
                }
                return _pool;
            }
        }

        T CreatePooledItem()
        {
            T go = GameObject.Instantiate(_prefab, _parent);
            go.InitializePoolObject(_pool);

            return go;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(T system)
        {
            system.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(T system)
        {
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(T system)
        {
            GameObject.Destroy(system.gameObject);
        }
    }


    public interface IPoolObject<T> where T : MonoBehaviour
    {
        public void InitializePoolObject(IObjectPool<T> pool);
    }

}
