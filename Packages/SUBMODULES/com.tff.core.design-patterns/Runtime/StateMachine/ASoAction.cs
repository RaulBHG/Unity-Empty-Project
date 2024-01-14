using System.Collections.Generic;
using TFF.Core.DesignPatterns.StateMachine.Core;
using TFF.Core.Variables.Optional;
using UnityEngine;

namespace TFF.Core.DesignPatterns.StateMachine
{
	public abstract class ASoAction : ScriptableObject
	{
		/*********************/
		/* Public Attributes */
		/*********************/
		[SerializeField] public SOptional<EStateMachineMoment> whenToExecuteAction;

		/********************/
		/* Internal Methods */
		/********************/
		internal AAction GetAction(MbStateMachine stateMachine, Dictionary<ScriptableObject, IStateElement> instances)
		{
			// Checks if the state was already obtained
			if (instances.TryGetValue(this, out IStateElement action)) return (AAction) action;

			// Creates new state
			AAction newAction = CreateAction();
			newAction.Action = this;
			newAction.OnAwake(stateMachine);
			instances.Add(this, newAction);

			return newAction;
		}

		/********************/
		/* Abstract Methods */
		/********************/
		protected abstract AAction CreateAction();
	}

	public abstract class ASoAction<T> : ASoAction where T : AAction, new()
	{
		/************************/
		/* Protected Attributes */
		/************************/
		protected AAction ActionElement;

		/*********************/
		/* Protected Methods */
		/*********************/
		protected override AAction CreateAction()
		{
			ActionElement = new T();
			return ActionElement;
		}
	}
}