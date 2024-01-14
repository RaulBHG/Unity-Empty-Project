using System.Collections.Generic;
using UnityEngine;

namespace TFF.Core.DesignPatterns.RuntimeSets
{
    public abstract class ASoRuntimeSet<T> : ScriptableObject
    {
        /************************/
        /* Protected Attributes */
        /************************/
        protected readonly List<T> _items = new();

        /******************/
        /* Public Methods */
        /******************/
        public void Add(T element)
        {
            if (_items.Contains(element)) return;
            _items.Add(element);
        }

        public void Remove(T element)
        {
            if (!_items.Contains(element)) return;
            _items.Remove(element);
        }

        public List<T> GetAllItems() => _items;

        /*********************/
        /*  Virtual Methods  */
        /*********************/
        public virtual T GetItem() => _items[0];
    }

    public abstract class ASoRuntimeSet<TK,TV> : ScriptableObject
    {
        /************************/
        /* Protected Attributes */
        /************************/
        protected readonly Dictionary<TK,TV> _items = new();

        /******************/
        /* Public Methods */
        /******************/
        public void Add(TK key, TV value)
        {
            if (_items.ContainsKey(key)) return;
            _items[key] = value;
        }

        public void Remove(TK key)
        {
            if (!_items.ContainsKey(key)) return;
            _items.Remove(key);
        }

        public Dictionary<TK,TV> GetAllItems() => _items;

        /*********************/
        /*  Virtual Methods  */
        /*********************/
        public virtual TV GetItem(TK key) => _items[key];
    }
}