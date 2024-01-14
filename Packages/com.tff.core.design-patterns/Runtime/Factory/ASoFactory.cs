using UnityEngine;

namespace TFF.Core.DesignPatterns.Factory
{
    public abstract class ASoFactory<T> : ScriptableObject, IFactory<T>
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [SerializeField] protected T prefab;

        public abstract T Create();
    }
}