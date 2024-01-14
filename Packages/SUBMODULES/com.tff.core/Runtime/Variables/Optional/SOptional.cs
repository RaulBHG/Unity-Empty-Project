using System;
using UnityEngine;

namespace TFF.Core.Variables.Optional
{
    [Serializable]
    public struct SOptional<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool Enabled => enabled;
        public T Value => value;

        public SOptional(T value)
        {
            enabled = true;
            this.value = value;
        }
    }
}