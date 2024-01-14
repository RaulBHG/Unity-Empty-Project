using UnityEngine;

namespace TFF.Core.DesignPatterns.Pool
{
	public abstract class AMbPoolItem : MonoBehaviour
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		public string CurrentScene { get; private set; }

		/********************/
		/* Abstract Methods */
		/********************/
		protected abstract void ConfigureOnRequest();
		protected abstract void ConfigureOnDispose();

		/********************/
		/* Internal Methods */
		/********************/
		internal void OnRequest(string scene)
		{
			CurrentScene = scene;
			ConfigureOnRequest();
		}

		internal void OnDispose()
		{
			CurrentScene = null;
			ConfigureOnDispose();
		}
	}
}