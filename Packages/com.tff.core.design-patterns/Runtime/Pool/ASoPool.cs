using System.Collections.Concurrent;
using System.Collections.Generic;
using Runtime.Pool.Events;
using TFF.Core.DesignPatterns.Events.Channels;
using TFF.Core.DesignPatterns.Factory;
using TFF.Core.DesignPatterns.Pool.Events;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Pool
{
    public abstract class ASoPool<T> : ScriptableObject where T : AMbPoolItem
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [Header("Item Pool Parameters")]
        [SerializeField] private int poolSize = 20;

        [Header("Listens to")]
        [SerializeField] private SoPoolRequestEventChannel requestEventChannel;
        [SerializeField] private SoPoolDisposeEventChannel disposeEventChannel;
        [SerializeField] private SoStringEventChannel sceneUnloadedEventChannel;

        /*********************/
        /* Public Attributes */
        /*********************/
        public bool IsInitialised { get; private set; }
        public int NumberOfItems { get; private set; }

        /************************/
        /* Protected Attributes */
        /************************/
        protected readonly ConcurrentBag<T> AvailableItems = new();
        protected abstract IFactory<T> PoolFactory {get;}

        /**********************/
        /* Private Attributes */
        /**********************/
        private readonly Dictionary<string, List<AMbPoolItem>> _itemsInUse = new();

        /******************/
        /* Public Methods */
        /******************/
        public void Initialise()
        {
            if (IsInitialised) return;

            NumberOfItems = 0;
            for (var i = 0; i < poolSize; i++) AvailableItems.Add(Create());
            IsInitialised = true;
        }

        /*******************/
        /* Virtual Methods */
        /*******************/
        protected virtual T Request(string scene)
        {
            T item = AvailableItems.TryTake(out T outItem) ? outItem : Create();
            RegisterItem(scene, item);
            return item;
        }

        protected virtual void Dispose(T item)
        {
            UnregisterItem(item);
            AvailableItems.Add(item);
        }

        protected List<AMbPoolItem> Request(string scene, int numberOfItems)
        {
            List<AMbPoolItem> outItems = new();
            for (var i = 0; i < numberOfItems; i++) outItems.Add(Request(scene));
            return outItems;
        }

        protected void Dispose(List<AMbPoolItem> inItems)
        {
            foreach (AMbPoolItem item in inItems) Dispose((T) item);
        }

        protected virtual T Create()
        {
            NumberOfItems++;
            return PoolFactory.Create();
        }

        /*******************/
        /* Private Methods */
        /*******************/
        private void OnRequest(StrPoolRequestData data)
        {
            // Gets group of items
            var outItems = data.NumberOfItems > 1 ?
                Request(data.Scene, data.NumberOfItems) :
                new List<AMbPoolItem> {Request(data.Scene)};

            // Invokes a callback to return the items to the borrower
            data.Callback.Invoke(outItems);
        }

        private void OnDispose(StrPoolDisposeData data) => Dispose(data.Data);

        /// <summary>
        /// Method to dispose all the items being used in a scene that will be unloaded 
        /// </summary>
        /// <param name="scene"></param>
        private void OnSceneUnloaded(string scene)
        {
            if (!_itemsInUse.ContainsKey(scene)) return;
            Dispose(_itemsInUse[scene]);
        }

        private void RegisterItem(string scene, T item)
        {
            if (!_itemsInUse.ContainsKey(scene)) _itemsInUse[scene] = new List<AMbPoolItem>();
            if (_itemsInUse[scene].Contains(item))
            {
                Debug.LogWarning($"Pool {name} has an item duplication for scene {scene}");
                if (_itemsInUse[scene].Count == 0) _itemsInUse.Remove(scene);
                return;
            }

            item.OnRequest(scene);
            _itemsInUse[scene].Add(item);
        }

        private void UnregisterItem(T item)
        {
            var scene = item.CurrentScene;
            if (!_itemsInUse.ContainsKey(scene))
            {
                Debug.LogWarning($"Scene {scene} has no items in pool {name} to unregister");
                return;
            }

            _itemsInUse[scene].Remove(item);
            if (_itemsInUse[scene].Count == 0) _itemsInUse.Remove(scene);

            item.OnDispose();
        }

        /*************************/
        /* Unity Event Functions */
        /*************************/
        private void OnEnable()
        {
            requestEventChannel.Subscribe(OnRequest);
            disposeEventChannel.Subscribe(OnDispose);
            sceneUnloadedEventChannel.Subscribe(OnSceneUnloaded);
        }

        private void OnDisable()
        {
            requestEventChannel.Unsubscribe(OnRequest);
            disposeEventChannel.Unsubscribe(OnDispose);
            sceneUnloadedEventChannel.Unsubscribe(OnSceneUnloaded);

            AvailableItems.Clear();
            IsInitialised = false;
        }
    }
}