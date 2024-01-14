using System;
using System.Linq;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Events.Channels
{
	/// <summary>
	/// Abstract class that defines the core functioning of an event channel
	/// </summary>
	public abstract class ASoEventChannel : ScriptableObject
	{
		/**********************/
		/* Private Attributes */
		/**********************/
		private readonly BasicEvent _event = new();

		/*********************/
		/* Public Attributes */
		/*********************/
		public virtual Delegate[] Listeners => _event.Listeners;

		/******************/
		/* Public Methods */
		/******************/
		public void Invoke() => _event.Invoke();
		public virtual void Subscribe(Action listener) => _event.Subscribe(listener);
		public virtual void Unsubscribe(Action listener) => _event.Unsubscribe(listener);
	}

	/// <summary>
	/// Abstract class which works as an extension of the basic event channel, allowing information to be passed
	/// through an event.
	/// </summary>
	public abstract class ASoEventChannel<T> : ASoEventChannel
	{
		/**********************/
		/* Private Attributes */
		/**********************/
		private readonly BasicEvent<T> _eventT = new();

		/*********************/
		/* Public Attributes */
		/*********************/
		public override Delegate[] Listeners => base.Listeners.ToList().Concat(_eventT.Listeners).ToArray();

		/******************/
		/* Public Methods */
		/******************/
		public void Invoke(T value) => _eventT.Invoke(value);
		public override void Subscribe(Action listener) => _eventT.Subscribe(listener);
		public override void Unsubscribe(Action listener) => _eventT.Unsubscribe(listener);
		public void Subscribe(Action<T> listener) => _eventT.Subscribe(listener);
		public void Unsubscribe(Action<T> listener) => _eventT.Unsubscribe(listener);
	}
}