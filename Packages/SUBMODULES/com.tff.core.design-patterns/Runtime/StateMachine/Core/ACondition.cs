namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public abstract class ACondition : IStateElement
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		public ASoCondition Condition;

		/************************/
		/* Protected Attributes */
		/************************/
		protected AMbStateMachineController Controller;

		/**********************/
		/* Private Attributes */
		/**********************/
		private bool _isCached;
		private bool _cachedStatement;

		/********************/
		/* Internal Methods */
		/********************/
		/// <summary>
		/// Method to catch the <see><cref>Statement</cref></see>
		/// </summary>
		/// <returns>Cached Statement</returns>
		internal bool GetStatement()
		{
			if (_isCached) return _cachedStatement;

			// Catches the statement
			_isCached = true;
			_cachedStatement = Statement();
			return _cachedStatement;
		}

		internal void ClearCache() => _isCached = false;

		/********************/
		/* Abstract Methods */
		/********************/
		protected abstract void CatchReferences(MbStateMachine stateMachine);
		protected abstract bool Statement();

		/*******************/
		/* Private Methods */
		/*******************/
		private void ExecuteIfMomentDefined(EStateMachineMoment moment)
		{
			if (!Condition.whenToCatchCondition.Enabled) return;
			if (!moment.Equals(Condition.whenToCatchCondition.Value)) return;
			ClearCache();
			GetStatement();
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

	/// <summary>
	/// Structure that stores not only the condition to be met, but also the expected result
	/// </summary>
	public readonly struct StrCondition
	{
		internal readonly ACondition Condition;
		private readonly bool _expectedResult;

		public StrCondition(ACondition condition, bool expectedResult)
		{
			Condition = condition;
			_expectedResult = expectedResult;
		}

		public bool IsMet() => Condition.GetStatement() == _expectedResult;
	}
}