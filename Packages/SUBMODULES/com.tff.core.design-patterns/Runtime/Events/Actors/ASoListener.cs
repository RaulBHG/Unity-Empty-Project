using TFF.Core.DesignPatterns.Events.Channels;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Events.Actors
{
	public abstract class ASoListener<T> : ScriptableObject where T : ASoEventChannel
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
		protected abstract void Answer();

		/*************************/
		/* Unity Event Functions */
		/*************************/
		protected virtual void OnEnable() => eventChannel.Subscribe(Answer);
		protected virtual void OnDisable() => eventChannel.Unsubscribe(Answer);
	}

	public abstract class ASoListener<TE,TD> : ScriptableObject where TE : ASoEventChannel<TD>
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
		protected abstract void Answer(TD data);

		/*************************/
		/* Unity Event Functions */
		/*************************/
		protected virtual void OnEnable() => eventChannel.Subscribe(Answer);
		protected virtual void OnDisable() => eventChannel.Unsubscribe(Answer);
	}
}