namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public class Transition : IStateElement
	{
		/**********************/
		/* Private Attributes */
		/**********************/
		private readonly State _target;
		private readonly StrCondition[] _conditions;
		private readonly int[] _andGroups;
		private readonly bool[] _results;

		/******************/
		/* Public Methods */
		/******************/
		public Transition(State target, StrCondition[] conditions, int[] andGroups = null)
		{
			_target = target;
			_conditions = conditions;
			_andGroups = andGroups is {Length: > 0} ? andGroups : new int[1];
			_results = new bool[_andGroups.Length];
		}

		/// <summary>
		/// Checks whether the conditions to transition to the target state are met.
		/// </summary>
		/// <param name="state">Returns the state to transition to. Null if the conditions aren't met.</param>
		/// <returns>True if the conditions are met.</returns>
		public bool TryToTransition(out State state)
		{
			state = HasToTransition() ? _target : null;
			return state != null;
		}

		/********************/
		/* Internal Methods */
		/********************/
		internal void ClearConditionsCache()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.ClearCache();
		}

		/*******************/
		/* Private Methods */
		/*******************/
		private bool HasToTransition()
		{
			// Verifies each one of the conditions in AND group batches
			var length = _andGroups.Length;
			for (int andIndex = 0, conditionIndex = 0;
				andIndex < length && conditionIndex < _conditions.Length; andIndex++)
			{
				for (var orIndex = 0; orIndex < _andGroups[andIndex]; orIndex++, conditionIndex++)
				{
					_results[andIndex] = orIndex == 0
						? _conditions[conditionIndex].IsMet()
						: _conditions[conditionIndex].IsMet() && _results[andIndex];
				}
			}

			// Verifies if at least one of the groups is fulfilled
			var result = false;
			for (var batchIndex = 0; batchIndex < length && !result; batchIndex++) result |= _results[batchIndex];

			return result;
		}

		/*************************/
		/* IStateElement Methods */
		/*************************/
		public void OnEnterState()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnEnterState();
		}

		public void OnExitState()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnExitState();
		}

		/*************************/
		/* IStateElement Methods */
		/*************************/
		public void OnEnable()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnEnable();
		}

		public void OnStart()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnStart();
		}

		public void OnFixedUpdate()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnFixedUpdate();
		}

		public void OnUpdate()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnUpdate();
		}

		public void OnLateUpdate()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnLateUpdate();
		}

		public void OnDrawGizmos()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnDrawGizmos();
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnApplicationPause(pauseStatus);
		}

		public void OnApplicationQuit()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnApplicationQuit();
		}

		public void OnDisable()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnDisable();
		}

		public void OnDestroy()
		{
			foreach (StrCondition sc in _conditions) sc.Condition.OnDestroy();
		}
	}
}