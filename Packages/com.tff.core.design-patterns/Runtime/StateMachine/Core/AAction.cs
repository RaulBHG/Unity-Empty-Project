namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public abstract class AAction : IStateElement
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		public ASoAction Action;

		/************************/
		/* Protected Attributes */
		/************************/
		protected AMbStateMachineController Controller;

		/********************/
		/* Abstract Methods */
		/********************/
		protected abstract void CatchReferences(MbStateMachine stateMachine);
		protected abstract void ExecuteAction();

		/*******************/
		/* Private Methods */
		/*******************/
		private void ExecuteIfMomentDefined(EStateMachineMoment moment)
		{
			if (!Action.whenToExecuteAction.Enabled) return;
			if (!moment.Equals(Action.whenToExecuteAction.Value)) return;
			ExecuteAction();
		}

		/*************************/
		/* IStateElement Methods */
		/*************************/
		public virtual void OnEnterState() => ExecuteIfMomentDefined(EStateMachineMoment.OnEnterState);
		public virtual void OnExitState() => ExecuteIfMomentDefined(EStateMachineMoment.OnExitState);

		/*************************/
		/* Unity Event Functions */
		/*************************/
		public void OnAwake(MbStateMachine stateMachine)
		{
			Controller = stateMachine.GetComponent<AMbStateMachineController>();
			CatchReferences(stateMachine);
		}
		public virtual void OnEnable() => ExecuteIfMomentDefined(EStateMachineMoment.OnEnable);
		public virtual void OnStart() => ExecuteIfMomentDefined(EStateMachineMoment.OnStart);
		public virtual void OnFixedUpdate() => ExecuteIfMomentDefined(EStateMachineMoment.OnFixedUpdate);
		public virtual void OnUpdate() => ExecuteIfMomentDefined(EStateMachineMoment.OnUpdate);
		public virtual void OnLateUpdate() => ExecuteIfMomentDefined(EStateMachineMoment.OnLateUpdate);
		public virtual void OnDrawGizmos() => ExecuteIfMomentDefined(EStateMachineMoment.OnDrawGizmos);
		public virtual void OnApplicationPause(bool pauseStatus) =>
			ExecuteIfMomentDefined(EStateMachineMoment.OnApplicationPause);

		public virtual void OnApplicationQuit() => ExecuteIfMomentDefined(EStateMachineMoment.OnApplicationQuit);
		public virtual void OnDisable() => ExecuteIfMomentDefined(EStateMachineMoment.OnDisable);
		public virtual void OnDestroy() => ExecuteIfMomentDefined(EStateMachineMoment.OnDestroy);
	}
}