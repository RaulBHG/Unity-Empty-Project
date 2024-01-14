using System;
using System.Collections.Generic;
using TFF.Core.DesignPatterns.StateMachine.Core;
using UnityEngine;

namespace TFF.Core.DesignPatterns.StateMachine
{
	public class MbStateMachine : MonoBehaviour
	{
		/*************************/
		/* Serialised Attributes */
		/*************************/
		[SerializeField] private SoTransitionTable transitionTable;
		[SerializeField] private bool logCurrentState = true;

		/**********************/
		/* Private Attributes */
		/**********************/
		private State _currentState;
		private Transform _transform;
		private readonly Dictionary<Type, Component> _cachedComponents = new();
		private readonly Dictionary<string, Transform> _cachedTransforms = new();

		/******************/
		/* Public Methods */
		/******************/
		public new T GetComponent<T>() where T : Component
		{
			if (TryGetComponent(out T component)) return component;

			Debug.LogWarning($"{typeof(T).Name} not found in {name}.");
			return null;
		}

		public Transform GetChildTransform(string objectName)
		{
			if (TryGetTransform(objectName, out Transform wantedTransform)) return wantedTransform;

			Debug.LogWarning($"{objectName} children object not found in {name}.");
			return null;
		}

		/*******************/
		/* Private Methods */
		/*******************/
		private void PerformTransition(State target)
		{
			_currentState.OnExitState();
			_currentState = target;
			_currentState.OnEnterState();
		}

		private new bool TryGetComponent<T>(out T component) where T : Component
		{
			Type type = typeof(T);
			if (_cachedComponents.TryGetValue(type, out Component value))
			{
				component = (T) value;
				return true;
			}

			component = (T) GetComponent(typeof(T));
			if (component == null) component = (T) GetComponentInChildren(typeof(T));
			if (component != null) _cachedComponents.Add(type, component);

			return component != null;
		}

		private bool TryGetTransform(string objectName, out Transform wantedTransform)
		{
			if (_cachedTransforms.TryGetValue(objectName, out Transform value))
			{
				wantedTransform = value;
				return true;
			}

			wantedTransform = FindRecursively(_transform, objectName);
			if (wantedTransform != null) _cachedTransforms.Add(objectName, wantedTransform);

			return wantedTransform != null;
		}

		/// <summary>
		/// Method used to find an object within all the children in the parent transform
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="objectName"></param>
		/// <returns>Found child</returns>
		private static Transform FindRecursively(Transform parent, string objectName)
		{
			// Loops over each child
			foreach (Transform child in parent)
			{
				// Returns if the child is the one we are looking for
				if (child.name.Equals(objectName)) return child;

				// Looks for it in the child
				Transform found = FindRecursively(child, objectName);

				// Returns if found
				if (found != null) return found;
			}
			return null;
		}

		/*************************/
		/* Unity Event Functions */
		/*************************/
		private void Awake()
		{
			if (transitionTable == null)
			{
				Debug.LogWarning($"Missing transition table on {transform.parent.name} object");
				return;
			}

			_transform = transform;
			_currentState = transitionTable.OnAwake(this);
		}

		private void OnEnable()
		{
			if (transitionTable == null) return;

			transitionTable.Enable();
		}

		private void Start()
		{
			if (transitionTable == null) return;

			transitionTable.Start();
			_currentState.OnEnterState();
		}

		private void FixedUpdate()
		{
			if (transitionTable == null) return;

			_currentState.OnFixedUpdate();
			if (_currentState.HasToTransition(out State target)) PerformTransition(target);
		}

		private void Update()
		{
			if (transitionTable == null) return;
			if (logCurrentState) Debug.Log($"State Machine {name} --> {_currentState.Name}");

			_currentState.OnUpdate();
			if (_currentState.HasToTransition(out State target)) PerformTransition(target);
		}

		private void LateUpdate()
		{
			if (transitionTable == null) return;

			_currentState.OnLateUpdate();
			if (_currentState.HasToTransition(out State target)) PerformTransition(target);
		}

		private void OnDrawGizmos()
		{
			if (transitionTable == null) return;

			_currentState?.OnDrawGizmos();
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (transitionTable == null) return;

			_currentState?.OnApplicationPause(pauseStatus);
		}

		private void OnApplicationQuit()
		{
			if (transitionTable == null) return;

			_currentState?.OnApplicationQuit();
		}

		private void OnDisable()
		{
			if (transitionTable == null) return;
			transitionTable.Disable();
		}

		private void OnDestroy()
		{
			if (transitionTable == null) return;
			transitionTable.Destroy();
		}
	}
}