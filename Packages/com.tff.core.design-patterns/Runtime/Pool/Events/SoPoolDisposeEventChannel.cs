using System.Collections.Generic;
using TFF.Core.DesignPatterns.Events.Channels;
using TFF.Core.DesignPatterns.Pool;
using UnityEngine;

namespace Runtime.Pool.Events
{
	[CreateAssetMenu(fileName = "SO_EvCh_PoolDispose_NEW", menuName = "TFF/Pool/New Pool Dispose Event Channel")]
	public class SoPoolDisposeEventChannel : ASoEventChannel<StrPoolDisposeData> {}

	public struct StrPoolDisposeData
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		// Include here all the elements to be sent through this channel
		public readonly List<AMbPoolItem> Data;

		public StrPoolDisposeData(List<AMbPoolItem> data)
		{
			Data = data;
		}
	}
}