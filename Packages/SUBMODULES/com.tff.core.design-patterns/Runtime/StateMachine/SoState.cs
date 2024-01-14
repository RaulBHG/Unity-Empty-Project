using System.Collections.Generic;
using TFF.Core.DesignPatterns.StateMachine.Core;
using UnityEngine;

namespace TFF.Core.DesignPatterns.StateMachine
{
	[CreateAssetMenu(fileName = "SO_SM_State_NEW", menuName = "TFF/State Machine/New State")]
	public class SoState : ScriptableObject
	{
		/*************************/
		/* Serialised Attributes */
		/*************************/
		[SerializeField] private ASoAction[] actions;

		/********************/
		/* Internal Methods */
		/********************/
		internal State GetState(MbStateMachine stateMachine, Dictionary<ScriptableObject, IStateElement> instances)
		{
			// Checks if the state was already obtained
			if (instances.TryGetValue(this, out IStateElement state)) return (State) state;

			// Creates new state
			State newState = new(name,GetActions(stateMachine, instances));
			instances.Add(this, newState);

			return newState;
		}

		/*******************/
		/* Private Methods */
		/*******************/
		private AAction[] GetActions(MbStateMachine stateMachine, Dictionary<ScriptableObject, IStateElement> instances)
		{
			// Initialises output
			var length = actions.Length;
			var outActions = new AAction[length];

			// Loops over all action Scriptable Objects
			for (var i = 0; i < length; i++) outActions[i] = actions[i].GetAction(stateMachine, instances);
			return outActions;
		}
	}
}