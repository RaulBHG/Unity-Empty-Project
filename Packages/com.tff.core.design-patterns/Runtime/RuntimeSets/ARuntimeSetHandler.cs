using UnityEngine;

namespace TFF.Core.DesignPatterns.RuntimeSets
{
    public abstract class ARuntimeSetHandler<T> : MonoBehaviour
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [SerializeField] private ASoRuntimeSet<T> runtimeSet;

        /*********************/
        /* Protected Methods */
        /*********************/
        protected void AddItemToSet(T item) => runtimeSet.Add(item);
        protected void RemoveItemFromSet(T item) => runtimeSet.Remove(item);
        protected T GetItem() => runtimeSet.GetItem();
    }

    public abstract class ARuntimeSetHandler<TK,TV> : MonoBehaviour
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [SerializeField] private ASoRuntimeSet<TK,TV> runtimeSet;

        /*********************/
        /* Protected Methods */
        /*********************/
        protected void AddItemToSet(TK key, TV value) => runtimeSet.Add(key, value);
        protected void RemoveItemFromSet(TK key) => runtimeSet.Remove(key);
        protected TV GetItem(TK key) => runtimeSet.GetItem(key);
    }
}