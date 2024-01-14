using System.Collections.Generic;
using TFF.Core.DesignPatterns.StateMachine.Core;
using TFF.Core.Variables.Optional;
using UnityEngine;

namespace TFF.Core.DesignPatterns.StateMachine
{
	public abstract class ASoCondition : ScriptableObject
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		[SerializeField] public SOptional<EStateMachineMoment> whenToCatchCondition;

		/********************/
		/* Internal Methods */
		/********************/
		internal StrCondition GetCondition(MbStateMachine stateMachine,
			Dictionary<ScriptableObject, IStateElement> instances, bool expectedResult)
		{
			// Checks if the state was already obtained
			if (instances.TryGetValue(this, out IStateElement condition))
				return new StrCondition((ACondition) condition, expectedResult);

			// Creates new state
			ACondition newCondition = CreateCondition();
			newCondition.Condition = this;
			newCondition.OnAwake(stateMachine);
			instances.Add(this, newCondition);

			return new StrCondition(newCondition, expectedResult);
		}

		/********************/
		/* Abstract Methods */
		/********************/
		protected abstract ACondition CreateCondition();
	}

	public abstract class ASoCondition<T> : ASoCondition where T : ACondition, new()
	{
		/************************/
		/* Protected Attributes */
		/************************/
		protected ACondition ConditionElement;

		/*********************/
		/* Protected Methods */
		/*********************/
		protected override ACondition CreateCondition()
		{
			ConditionElement = new T();
			return ConditionElement;
		}
	}
}