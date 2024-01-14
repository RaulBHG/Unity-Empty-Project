using TFF.Core.DesignPatterns.Factory;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Pool
{
    public abstract class ASoComponentPool<T> : ASoPool<T> where T : AMbPoolItem
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [SerializeField] private ASoFactory<T> itemFactorySo;

        /************************/
        /* Protected Attributes */
        /************************/
        protected override IFactory<T> PoolFactory => itemFactorySo;

        /**********************/
        /* Private Attributes */
        /**********************/
        private Transform _parent;

        private Transform _root;
        private Transform Root
        {
            get
            {
                if (_root != null) return _root;

                _root = new GameObject(name).transform;
                _root.SetParent(_parent);
                return _root;
            }
        }

        /*******************/
        /* Virtual Methods */
        /*******************/
        protected override T Request(string scene)
        {
            T item = base.Request(scene);
            item.gameObject.SetActive(true);
            return item;
        }

        protected override void Dispose(T item)
        {
            item.transform.SetParent(Root.transform);
            item.gameObject.SetActive(false);
            base.Dispose(item);
        }

        protected override T Create()
        {
            T newItem = base.Create();
            newItem.transform.SetParent(Root.transform);
            newItem.gameObject.SetActive(false);
            return newItem;
        }

        /********************/
        /* Internal Methods */
        /********************/
        internal void SetParent(Transform transform)
        {
            _parent = transform;
            Root.SetParent(_parent);
        }
    }
}