using System;
using System.Collections.Generic;

namespace TFF.Core.DesignPatterns.StateMachine.Core
{
	public class State : IStateElement
	{
		/***********************/
		/* Internal Attributes */
		/***********************/
		internal readonly string Name;
		internal Transition[] Transitions;

		/**********************/
		/* Private Attributes */
		/**********************/
		private readonly AAction[] _actions;

		/******************/
		/* Public Methods */
		/******************/
		public bool HasToTransition(out State state)
		{
			// Initialises output state
			state = null;

			// Looks for a doable transition
			foreach (Transition t in Transitions)
				if (t.TryToTransition(out state)) break;

			// Clears cached conditions
			foreach (Transition t in Transitions) t.ClearConditionsCache();

			return state != null;
		}

		/********************/
		/* Internal Methods */
		/********************/
		internal State(string name)
		{
			Name = name;
			_actions = Array.Empty<AAction>();
			Transitions = Array.Empty<Transition>();
		}

		internal State(string name, AAction[] actions)
		{
			Name = name;
			_actions = actions;
			Transitions = Array.Empty<Transition>();
		}

		/*************************/
		/* IStateElement Methods */
		/*************************/
		public void OnEnterState()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnEnterState();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnExitState()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnExitState();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		/*************************/
		/* Unity Event Functions */
		/*************************/
		public void OnEnable()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnEnable();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnStart()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnStart();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnFixedUpdate()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnFixedUpdate();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnUpdate()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnUpdate();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnLateUpdate()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnLateUpdate();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnDrawGizmos()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnDrawGizmos();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnApplicationPause(pauseStatus);
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnApplicationQuit()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnApplicationQuit();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnDisable()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnDisable();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}

		public void OnDestroy()
		{
			void LoopFunction(IEnumerable<IStateElement> els)
			{
				foreach (IStateElement e in els) e.OnDestroy();
			}
			LoopFunction(_actions);
			LoopFunction(Transitions);
		}
	}
}