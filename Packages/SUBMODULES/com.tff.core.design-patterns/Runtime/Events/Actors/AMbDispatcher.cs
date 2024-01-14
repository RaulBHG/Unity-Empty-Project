using System.Collections;
using TFF.Core.DesignPatterns.Events.Channels;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Events.Actors
{
    public abstract class AMbDispatcher<T> : MonoBehaviour where T : ASoEventChannel
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [Header("Broadcasts On")]
        [Tooltip("Channel that the current dispatcher will be using to broadcast the event")]
        [SerializeField] protected T eventChannel;

        /*********************/
        /* Protected Methods */
        /*********************/
        protected virtual IEnumerator Dispatch()
        {
            eventChannel.Invoke();
            yield break;
        }
    }

    public abstract class AMbDispatcher<TE,TD> : MonoBehaviour where TE : ASoEventChannel<TD>
    {
        /*************************/
        /* Serialised Attributes */
        /*************************/
        [Header("Broadcasts On")]
        [Tooltip("Channel that the current dispatcher will be using to broadcast the event")]
        [SerializeField] protected TE eventChannel;

        /*********************/
        /* Protected Methods */
        /*********************/
        protected virtual IEnumerator Dispatch(TD dataToDispatch)
        {
            if (dataToDispatch == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Event System: Data to be dispatched from {name} is null. Skipped.");
#endif
                yield break;
            }
            eventChannel.Invoke(dataToDispatch);
        }
    }
}