namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public enum EStateMachineMoment
	{
		None, OnEnterState, OnExitState, OnEnable, OnStart, OnFixedUpdate, OnUpdate, OnLateUpdate, OnDrawGizmos,
		OnApplicationPause, OnApplicationQuit, OnDisable, OnDestroy
	}
}