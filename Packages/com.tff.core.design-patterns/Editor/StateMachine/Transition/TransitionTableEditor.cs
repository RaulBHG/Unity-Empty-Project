using System.Collections.Generic;
using System.Linq;
using TFF.Core.DesignPatterns.Editor.StateMachine.State;
using TFF.Core.DesignPatterns.StateMachine;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.Transition
{
	[CustomEditor(typeof(SoTransitionTable))]
	public class TransitionTableEditor : UnityEditor.Editor
	{
		/**********************/
		/* Private attributes */
		/**********************/
		// Property with all the transitions within the table
		private SerializedProperty _transitions;
		// List of origin states
		private List<Object> _states;
		// Grouped transitions by origin state used to show transitions
		private List<List<TransitionDisplay>> _groupedTransitions;
		// Element used to add new transitions
		private AddTransitionDisplay _addTransition;
		// Index of the state currently toggled on, -1 if none is.
		private int _toggledIndex = -1;
		// Editor to display the StateSO inspector.
		private UnityEditor.Editor _cachedStateEditor;
		private bool _displayStateEditor;

		/********************/
		/* Internal methods */
		/********************/
		/// <summary>
		/// Move a transition up or down
		/// </summary>
		/// <param name="serialisedTransition">The transition to move</param>
		/// <param name="up">Move up(true) or down(false)</param>
		internal void ReorderTransition(SerialisedTransition serialisedTransition, bool up)
		{
			var stateIndex = _states.IndexOf(serialisedTransition.OriginState.objectReferenceValue);
			var stateTransitions = _groupedTransitions[stateIndex];
			var index = stateTransitions.FindIndex(t =>
				t.SerialisedTransition.Index == serialisedTransition.Index);

			(int currentIndex, int targetIndex) = up ?
				(serialisedTransition.Index, stateTransitions[index - 1].SerialisedTransition.Index) :
				(stateTransitions[index + 1].SerialisedTransition.Index, serialisedTransition.Index);

			_transitions.MoveArrayElement(currentIndex, targetIndex);

			ApplyModifications($"Moved transition to {serialisedTransition.TargetState.objectReferenceValue.name} " +
								$"{(up ? "up" : "down")}");

			_toggledIndex = stateIndex;
		}

		/// <summary>
		/// Add a new transition. If a transition with the same origin and target states is found,
		/// the conditions in the new transition are added to it.
		/// </summary>
		/// <param name="source">Source Transition</param>
		internal void AddTransition(SerialisedTransition source)
		{
			SerialisedTransition transition;
			if (TryGetTransition(source.OriginState, source.TargetState,
				out var originIndex, out var targetIndex))
			{
				transition = _groupedTransitions[originIndex][targetIndex].SerialisedTransition;
			}
			else
			{
				var count = _transitions.arraySize;
				_transitions.InsertArrayElementAtIndex(count);
				transition = new SerialisedTransition(_transitions.GetArrayElementAtIndex(count));
				transition.ClearProperties();
				transition.OriginState.objectReferenceValue = source.OriginState.objectReferenceValue;
				transition.TargetState.objectReferenceValue = source.TargetState.objectReferenceValue;
			}

			CopyConditions(transition.Conditions, source.Conditions);
			ApplyModifications($"Added transition from {transition.OriginState} to {transition.TargetState}");

			_toggledIndex = originIndex >= 0 ? originIndex : _states.Count - 1;
		}

		/// <summary>
		/// Remove a transition.
		/// </summary>
		/// <param name="serialisedTransition">Transition to delete.</param>
		internal void RemoveTransition(SerialisedTransition serialisedTransition)
		{
			var originName = serialisedTransition.OriginState.objectReferenceValue.name;
			var targetName = serialisedTransition.TargetState.objectReferenceValue.name;

			var stateIndex = _states.IndexOf(serialisedTransition.OriginState.objectReferenceValue);
			var transitions = _groupedTransitions[stateIndex];
			var count = transitions.Count;
			var index = transitions.FindIndex(t =>
				t.SerialisedTransition.Index == serialisedTransition.Index);
			var deleteIndex = serialisedTransition.Index;

			if (index == 0 && count > 1)
				_transitions.MoveArrayElement(transitions[1].SerialisedTransition.Index, deleteIndex++);

			_transitions.DeleteArrayElementAtIndex(deleteIndex);

			ApplyModifications($"Deleted transition from {originName} to {targetName}");

			if (count > 1) _toggledIndex = stateIndex;
		}

		internal List<SerialisedTransition> GetStateTransitions(Object state) =>
			_groupedTransitions[_states.IndexOf(state)].Select(t =>
				t.SerialisedTransition).ToList();

		internal void DisplayStateEditor(Object state)
		{
			if (_cachedStateEditor == null)
			{
				_cachedStateEditor = CreateEditor(state, typeof(StateEditor));
			}
			else
			{
				CreateCachedEditor(state, typeof(StateEditor), ref _cachedStateEditor);
			}

			_displayStateEditor = true;
		}

		/*******************/
		/* Private methods */
		/*******************/
		private void ShowTransitionTableGUI()
		{
			Separator();
			HelpBox("Click on any State's name to see the Transitions it contains, or click the Pencil/Wrench " +
					"icon to see its Actions.", MessageType.Info);
			Separator();

			// Loops over each origin state
			for (var i = 0; i < _states.Count; i++)
			{
				var transitions = _groupedTransitions[i];

				// Draws small separator between states
				Rect stateRect = BeginVertical(ContentStyle.WithPaddingAndMargins);
				EditorGUI.DrawRect(stateRect, ContentStyle.LightGray);

				// State Header
				Rect headerRect = BeginHorizontal();
				{
					BeginVertical();
					var labelElements =
						transitions[0].SerialisedTransition.OriginState.objectReferenceValue.name.Split("_");
					var label = $"{labelElements[^1]} {labelElements[^2]}";
					if (i == 0) label += " (Initial State)";

					headerRect.height = EditorGUIUtility.singleLineHeight;
					GUILayoutUtility.GetRect(headerRect.width, headerRect.height);
					headerRect.x += 5;

					// Toggle
					{
						Rect toggleRect = headerRect;
						toggleRect.width -= 140;
						_toggledIndex =
							EditorGUI.BeginFoldoutHeaderGroup(toggleRect, _toggledIndex == i, label,
								ContentStyle.StateListStyle) ? i : _toggledIndex == i ? -1 : _toggledIndex;
					}

					Separator();
					EndVertical();

					// State Header Buttons
					{
						Rect buttonRect = new(headerRect.width - 25, headerRect.y, 35, 20);

						// Move state down
						if (i < _states.Count - 1)
						{
							if (Button(buttonRect, "scrolldown"))
							{
								ReorderState(i, false);
								EarlyOut();
								return;
							}
							buttonRect.x -= 40;
						}

						// Move state up
						if (i > 0)
						{
							if (Button(buttonRect, "scrollup"))
							{
								ReorderState(i, true);
								EarlyOut();
								return;
							}
							buttonRect.x -= 40;
						}

						// Switch to state editor
						if (Button(buttonRect, "SceneViewTools"))
						{
							DisplayStateEditor(transitions[0].SerialisedTransition.OriginState.objectReferenceValue);
							EarlyOut();
							return;
						}

						// Button methods
						bool Button(Rect position, string icon) =>
							GUI.Button(position, EditorGUIUtility.IconContent(icon));
						void EarlyOut()
						{
							EndHorizontal();
							EndFoldoutHeaderGroup();
							EndVertical();
							EndHorizontal();
						}
					}
				}
				EndHorizontal();

				// Shows transitions of the one selected
				if (_toggledIndex == i)
				{
					EditorGUI.BeginChangeCheck();
					stateRect.y += EditorGUIUtility.singleLineHeight * 2;

					// Display all the transitions in the state
					foreach (TransitionDisplay transition in transitions)
					{
						// Return if there were changes
						if (transition.Display(ref stateRect))
						{
							EditorGUI.EndChangeCheck();
							EndFoldoutHeaderGroup();
							EndVertical();
							EndHorizontal();
							return;
						}
						Separator();
					}
					if (EditorGUI.EndChangeCheck())
						serializedObject.ApplyModifiedProperties();
				}

				EndFoldoutHeaderGroup();
				EndVertical();
				Separator();
			}

			Rect rect = BeginHorizontal();
			Space(rect.width - 55);

			// Display add transition button
			_addTransition.Display(rect);

			EndHorizontal();
		}

		private void GroupByOriginState()
		{
			var groupedTransitions = new Dictionary<Object, List<TransitionDisplay>>();
			for (var i = 0; i < _transitions.arraySize; i++)
			{
				SerialisedTransition serialisedTransition = new(_transitions, i);

				// If the transition is corrupted, removes it
				if (serialisedTransition.OriginState.objectReferenceValue == null)
				{
					Debug.LogError("Transition with invalid \"Origin State\" found in table " + 
									serializedObject.targetObject.name + ", deleting...");
					_transitions.DeleteArrayElementAtIndex(i);
					ApplyModifications("Invalid transition deleted");
					return;
				}
				if (serialisedTransition.TargetState.objectReferenceValue == null)
				{
					Debug.LogError("Transition with invalid \"Target State\" found in table " +
									serializedObject.targetObject.name + ", deleting...");
					_transitions.DeleteArrayElementAtIndex(i);
					ApplyModifications("Invalid transition deleted");
					return;
				}

				// If the transition has not been added, creates a new list of transition display objects
				if (!groupedTransitions.TryGetValue(serialisedTransition.OriginState.objectReferenceValue,
					out var transitionDisplays))
				{
					transitionDisplays = new List<TransitionDisplay>();
					groupedTransitions.Add(serialisedTransition.OriginState.objectReferenceValue, transitionDisplays);
				}

				// Adds a new transition display with this serialised transition
				transitionDisplays.Add(new TransitionDisplay(serialisedTransition, this));
			}

			// Generates the list of transitions grouped by origin state
			_states = groupedTransitions.Keys.ToList();
			_groupedTransitions = new List<List<TransitionDisplay>>();
			foreach (Object state in _states) _groupedTransitions.Add(groupedTransitions[state]);
		}

		/// <summary>
		/// Move a state up or down
		/// </summary>
		/// <param name="index">Index of the state</param>
		/// <param name="up">Moving up(true) or down(false)</param>
		private void ReorderState(int index, bool up)
		{
			Object toggledState = _toggledIndex > -1 ? _states[_toggledIndex] : null;

			if (!up) index++;

			var transitions = _groupedTransitions[index];
			var transitionIndex = transitions[0].SerialisedTransition.Index;
			var targetIndex = _groupedTransitions[index - 1][0].SerialisedTransition.Index;
			_transitions.MoveArrayElement(transitionIndex, targetIndex);

			ApplyModifications($"Moved {_states[index].name} State {(up ? "up" : "down")}");

			if (toggledState) _toggledIndex = _states.IndexOf(toggledState);
		}

		private bool TryGetTransition(SerializedProperty originPr, SerializedProperty targetPr,
			out int originIndex, out int targetIndex)
		{
			originIndex = _states.IndexOf(originPr.objectReferenceValue);

			targetIndex = -1;
			if (originIndex < 0) return false;

			targetIndex = _groupedTransitions[originIndex].FindIndex(t =>
				t.SerialisedTransition.TargetState.objectReferenceValue == targetPr.objectReferenceValue);

			return targetIndex >= 0;
		}

		private static void CopyConditions(SerializedProperty targetPr, SerializedProperty originPr)
		{
			for (int i = 0, j = targetPr.arraySize; i < originPr.arraySize; i++, j++)
			{
				targetPr.InsertArrayElementAtIndex(j);
				SerializedProperty cF = originPr.GetArrayElementAtIndex(i);
				SerializedProperty cT = targetPr.GetArrayElementAtIndex(j);
				cT.FindPropertyRelative("expectedResult").enumValueIndex =
					cF.FindPropertyRelative("expectedResult").enumValueIndex;
				cT.FindPropertyRelative("operator").enumValueIndex =
					cF.FindPropertyRelative("operator").enumValueIndex;
				cT.FindPropertyRelative("condition").objectReferenceValue =
					cF.FindPropertyRelative("condition").objectReferenceValue;
			}
		}

		private void ApplyModifications(string msg)
		{
			Undo.RecordObject(serializedObject.targetObject, msg);
			serializedObject.ApplyModifiedProperties();
			Reset();
		}

		private void ShowStateEditorGUI()
		{
			Separator();

			// Back button
			if (GUILayout.Button(EditorGUIUtility.IconContent("scrollleft"),
				GUILayout.Width(35), GUILayout.Height(20)))
			{
				_displayStateEditor = false;
				return;
			}

			Separator();
			HelpBox("The order of the actions represent the order of execution.", MessageType.Info);
			Separator();

			// State name
			var labelElements= _cachedStateEditor.target.name.Split("_");
			LabelField($"{labelElements[^1]} {labelElements[^2]}", EditorStyles.boldLabel);
			Separator();
			_cachedStateEditor.OnInspectorGUI();
		}

		/*************************/
		/* Unity Event Functions */
		/*************************/
		public override void OnInspectorGUI()
		{
			if (!_displayStateEditor)
				ShowTransitionTableGUI();
			else
				ShowStateEditorGUI();
		}

		/// <summary>
		/// Method to fully reset the editor. Used whenever adding, removing and reordering transitions.
		/// </summary>
		internal void Reset()
		{
			serializedObject.Update();

			// Resets transition toggle index
			Object toggledState = _toggledIndex > -1 ? _states[_toggledIndex] : null;

			// Serialises the list of transitions of the ScriptableObject
			_transitions = serializedObject.FindProperty("transitions");

			// Groups all transitions by origin state
			GroupByOriginState();
			_toggledIndex = toggledState ? _states.IndexOf(toggledState) : -1;
		}

		private void OnEnable()
		{
			_addTransition = new AddTransitionDisplay(this);
			Undo.undoRedoPerformed += Reset;
			Reset();
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= Reset;
			_addTransition.Dispose();
		}
	}
}