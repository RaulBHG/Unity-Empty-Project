using System.Collections;
using TFF.Core.DesignPatterns.Events.Channels;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Events.Actors
{
    public abstract class AMbListener<T> : MonoBehaviour where T : ASoEventChannel
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [Header("Listens To")]
        [Tooltip("Channel that the current listener will be watching")]
        [SerializeField] protected T eventChannel;

        /********************/
        /* Abstract Methods */
        /********************/
        protected abstract IEnumerator Answer();

        /*******************/
        /* Private Methods */
        /*******************/
        private void React() => StartCoroutine(Answer());

        /*************************/
        /* Unity Event Functions */
        /*************************/
        protected virtual void OnEnable() => eventChannel.Subscribe(React);
        protected virtual void OnDisable() => eventChannel.Unsubscribe(React);
    }

    public abstract class AMbListener<TE,TD> : MonoBehaviour where TE : ASoEventChannel<TD>
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [Header("Listens To")]
        [Tooltip("Channel that the current listener will be watching")]
        [SerializeField] protected TE eventChannel;

        /********************/
        /* Abstract Methods */
        /********************/
        protected abstract IEnumerator Answer(TD data);

        /*******************/
        /* Private Methods */
        /*******************/
        private void React(TD data) => StartCoroutine(Answer(data));

        /*************************/
        /* Unity Event Functions */
        /*************************/
        protected virtual void OnEnable() => eventChannel.Subscribe(React);
        protected virtual void OnDisable() => eventChannel.Unsubscribe(React);
    }
}