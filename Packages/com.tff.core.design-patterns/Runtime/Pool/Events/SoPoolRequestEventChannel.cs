using System;
using System.Collections.Generic;
using TFF.Core.DesignPatterns.Events.Channels;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Pool.Events
{
	[CreateAssetMenu(fileName = "SO_EvCh_PoolRequest_NEW", menuName = "TFF/Pool/New Pool Request Event Channel")]
	public class SoPoolRequestEventChannel : ASoEventChannel<StrPoolRequestData> {}

	public struct StrPoolRequestData
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		// Include here all the elements to be sent through this channel
		public readonly string Scene;
		public readonly int NumberOfItems;
		public readonly Action<List<AMbPoolItem>> Callback;

		public StrPoolRequestData(string scene)
		{
			Scene = scene;
			NumberOfItems = 1;
			Callback = null;
		}

		public StrPoolRequestData(string scene, int numberOfItems)
		{
			Scene = scene;
			NumberOfItems = numberOfItems;
			Callback = null;
		}

		public StrPoolRequestData(string scene, Action<List<AMbPoolItem>> callback)
		{
			Scene = scene;
			NumberOfItems = 1;
			Callback = callback;
		}

		public StrPoolRequestData(string scene, int numberOfItems, Action<List<AMbPoolItem>> callback)
		{
			Scene = scene;
			NumberOfItems = numberOfItems;
			Callback = callback;
		}
	}
}