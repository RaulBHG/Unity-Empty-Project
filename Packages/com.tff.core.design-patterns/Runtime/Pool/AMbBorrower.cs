using System.Collections;
using System.Collections.Generic;
using Runtime.Pool.Events;
using TFF.Core.DesignPatterns.Events.Channels;
using TFF.Core.DesignPatterns.Pool.Events;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Pool
{
	public abstract class AMbBorrower<TP,T> : MonoBehaviour
		where TP : ASoComponentPool<T>
		where T : AMbPoolItem
	{
		/*************************/
		/* Serialised Attributes */
		/*************************/
		[Header("Item Pool Parameters")]
		[SerializeField] private TP poolSo;
		[Tooltip("Total number of items the borrower needs to work properly")]
		[SerializeField] private int itemsNeeded;

		[Header("Broadcasts On")]
		[SerializeField] private SoPoolRequestEventChannel requestEventChannel;
		[SerializeField] private SoPoolDisposeEventChannel disposeEventChannel;

		[Header("Listens to")]
		[SerializeField] private SoStringEventChannel sceneUnloadedEventChannel;

		/************************/
		/* Protected Attributes */
		/************************/
		protected List<AMbPoolItem> itemsInUse;

		/**********************/
		/* Private Attributes */
		/**********************/
		private string _borrowerScene;
		private bool _failedRequest;

		/********************/
		/* Abstract methods */
		/********************/
		protected abstract void ConfigureOnRequest();
		protected abstract void ConfigureOnDispose(List<AMbPoolItem> items);

		/*********************/
		/* Protected methods */
		/*********************/
		protected IEnumerator Request()
		{
			// Checks pool initialisation
			if (!poolSo.IsInitialised)
			{
				Debug.LogWarning($"Borrower {name} tried to request items from not initialised pool " +
								 $"{poolSo.name}");
				yield break;
			}

			// Requests the items
			requestEventChannel.Invoke(new StrPoolRequestData(_borrowerScene, itemsNeeded, OnRequest));

			// Waits for the items to return to the borrower
			yield return new WaitUntil(() => itemsInUse is {Count: > 0} || _failedRequest);

			// Shows failed request message if necessary
			if (_failedRequest)
			{
				Debug.LogWarning($"Borrower {name} failed its request to pool {poolSo.name}");
				_failedRequest = false;
				yield break;
			}

			// Applies custom configuration after request
			ConfigureOnRequest();
		}

		protected void Dispose(T item)
		{
			// Checks pool initialisation
			if (!poolSo.IsInitialised)
			{
				Debug.LogWarning($"Borrower {name} tried to dispose items from not initialised pool " +
								 $"{poolSo.name}");
				return;
			}

			// Gets list
			var items = new List<AMbPoolItem> {item};

			// Applies custom configuration before disposal
			ConfigureOnDispose(items);

			// Disposes the item
			disposeEventChannel.Invoke(new StrPoolDisposeData(items));

			// Removes the item from the list
			itemsInUse.Remove(item);
		}

		protected void Dispose(List<AMbPoolItem> items)
		{
			// Checks pool initialisation
			if (!poolSo.IsInitialised)
			{
				Debug.LogWarning($"Borrower {name} tried to dispose items from not initialised pool " +
								 $"{poolSo.name}");
				return;
			}

			// Applies custom configuration before disposal
			ConfigureOnDispose(items);

			// Disposes the items
			disposeEventChannel.Invoke(new StrPoolDisposeData(items));

			// Removes the items from the list
			items.ForEach(i => itemsInUse.Remove(i));
		}

		/*******************/
		/* Private methods */
		/*******************/
		private void OnRequest(List<AMbPoolItem> items)
		{
			if (items.TrueForAll(i => typeof(T) != i.GetType()))
			{
				Debug.LogWarning($"Some requested items are not of type {typeof(T)}");
				_failedRequest = true;
				return;
			}

			itemsInUse = items;
		}

		private void OnSceneUnloaded(string scene)
		{
			if (scene.Equals(_borrowerScene)) itemsInUse.Clear();
		}

		/*************************/
		/* Unity Event Functions */
		/*************************/
		protected virtual void Awake()
		{
			// Stores borrower scene
			_borrowerScene = gameObject.scene.path;

			// Initialises sound emitter pool
			poolSo.Initialise();

			// Sets controller as the pool parent
			poolSo.SetParent(transform);
		}

		protected virtual void OnEnable() => sceneUnloadedEventChannel.Subscribe(OnSceneUnloaded);
		protected virtual void OnDisable() => sceneUnloadedEventChannel.Unsubscribe(OnSceneUnloaded);
	}
}