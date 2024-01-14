using System;
using System.Linq;

namespace TFF.Core.DesignPatterns.Events
{
	[Serializable]
	public class BasicEvent
	{
		/**********************/
		/* Private Attributes */
		/**********************/
		private Action _onEventRaised = delegate {};

		/*********************/
		/* Public Attributes */
		/*********************/
		public virtual Delegate[] Listeners => _onEventRaised.GetInvocationList();

		/******************/
		/* Public Methods */
		/******************/
		public void Invoke() => _onEventRaised?.Invoke();
		public void Subscribe(Action listener) => _onEventRaised += listener;
		public void Unsubscribe(Action listener) => _onEventRaised -= listener;
	}

	/// <summary>
	/// Abstract class which works as an extension of the basic event, allowing information to be passed through an
	/// event
	/// </summary>
	[Serializable]
	public class BasicEvent<T> : BasicEvent
	{
		/**********************/
		/* Private Attributes */
		/**********************/
		private Action<T> _onEventRaisedT = delegate {};

		/*********************/
		/* Public Attributes */
		/*********************/
		public override Delegate[] Listeners =>
			base.Listeners.ToList().Concat(_onEventRaisedT.GetInvocationList()).ToArray();

		/******************/
		/* Public Methods */
		/******************/
		public void Invoke(T data)
		{
			_onEventRaisedT.Invoke(data);
			base.Invoke();
		}
		public void Subscribe(Action<T> listener) => _onEventRaisedT += listener;
		public void Unsubscribe(Action<T> listener) => _onEventRaisedT -= listener;
	}
}