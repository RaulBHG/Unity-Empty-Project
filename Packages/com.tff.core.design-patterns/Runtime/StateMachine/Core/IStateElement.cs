namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public interface IStateElement
	{
		/******************/
		/* State Methods */
		/******************/
		public void OnEnterState();
		public void OnExitState();

		/*************************/
		/* Unity Event Functions */
		/*************************/
		public void OnEnable();
		public void OnStart();
		public void OnFixedUpdate();
		public void OnUpdate();
		public void OnLateUpdate();
		public void OnDrawGizmos();
		public void OnApplicationPause(bool pauseStatus);
		public void OnApplicationQuit();
		public void OnDisable();
		public void OnDestroy();
	}
}