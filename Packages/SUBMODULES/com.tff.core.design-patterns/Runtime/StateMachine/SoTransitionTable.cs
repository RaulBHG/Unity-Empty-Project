using System;
using System.Collections.Generic;
using System.Linq;
using TFF.Core.DesignPatterns.StateMachine.Core;
using UnityEngine;

namespace TFF.Core.DesignPatterns.StateMachine
{
	[CreateAssetMenu(fileName = "SO_SM_TransitionTable_NEW", menuName = "TFF/State Machine/New Transition Table")]
	public class SoTransitionTable : ScriptableObject
	{
		/*************************/
		/* Serialised Attributes */
		/*************************/
		[SerializeField] private StrTransition[] transitions;

		/**********************/
		/* Private Attributes */
		/**********************/
		private readonly List<State> _states = new();

		/*******************/
		/* Private Methods */
		/*******************/
		private static Transition GetTransition(MbStateMachine stateMachine, 
			Dictionary<ScriptableObject, IStateElement> instances, State targetState,
			IList<StrTransitionCondition> transitionConditions)
		{
			// Gets all the conditions
			var length = transitionConditions.Count;
			var conditions = new StrCondition[length];
			for (var i = 0; i < length; i++)
			{
				conditions[i] = transitionConditions[i].condition.GetCondition(stateMachine, instances,
					Result.True.Equals(transitionConditions[i].expectedResult));
			}

			// Creates an array that takes into account which conditions shall be fulfilled together, that is,
			// those conditions that are connected by an AND operator
			List<int> andGroupsList = new();
			for (var i = 0; i < length; i++)
			{
				var idx = andGroupsList.Count;
				andGroupsList.Add(1);
				while (i < length - 1 && Operator.And.Equals(transitionConditions[i].@operator))
				{
					i++;
					andGroupsList[idx]++;
				}
			}
			var andGroups = andGroupsList.ToArray();

			// Creates the transition
			return new Transition(targetState, conditions, andGroups);
		}

		/*************************/
		/* Unity Event Functions */
		/*************************/
		internal State OnAwake(MbStateMachine stateMachine)
		{
			// Initialises local structures
			List<Transition> currentTransitions = new();

			// Initialises dictionary that contains local instances of the loaded scriptable objects so far.
			Dictionary<ScriptableObject, IStateElement> instances = new();

			// Gets the transitions grouped by the origin state
			var transitionsBatches = transitions.GroupBy(t => t.origin);

			// Loops over each transition group
			foreach (var transitionsBatch in transitionsBatches)
			{
				// Gets origin state and adds it to the list
				State origin = transitionsBatch.Key.GetState(stateMachine, instances);
				_states.Add(origin);

				// Loops over each transition in the batch
				currentTransitions.Clear();
				foreach (StrTransition transition in transitionsBatch)
				{
					// Gets target state and adds it to the list
					State target = transition.target.GetState(stateMachine, instances);
					_states.Add(target);

					// Gets the transition from the data within the Scriptable Objects
					currentTransitions.Add(GetTransition(stateMachine, instances, target, transition.conditions));
				}

				// Once all the transitions have been generated, adds all of them to the origin state
				origin.Transitions = currentTransitions.ToArray();
			}

			if (_states.Count > 0) return _states[0];

			Debug.LogWarning($"Transitions Table {name} is empty.");
			return new State("None");
		}

		internal void Enable() => _states.ForEach(s => s.OnEnable());
		internal void Start() => _states.ForEach(s => s.OnStart());
		internal void Disable() => _states.ForEach(s => s.OnDisable());
		internal void Destroy() => _states.ForEach(s => s.OnDestroy());
	}

	[Serializable]
	public struct StrTransition
	{
		public SoState origin;
		public SoState target;
		public StrTransitionCondition[] conditions;
	}

	[Serializable]
	public struct StrTransitionCondition
	{
		public ASoCondition condition;
		public Result expectedResult;
		public Operator @operator;
	}

	public enum Result { True, False }
	public enum Operator { And, Or }
}