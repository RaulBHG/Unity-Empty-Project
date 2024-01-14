using System;
using TFF.Core.DesignPatterns.StateMachine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.Transition
{
	public class AddTransitionDisplay : IDisposable
	{
		/**********************/
		/* Private attributes */
		/**********************/
		private SerialisedTransition SerialisedTransition { get; }
		private readonly SerializedObject _transition;
		private readonly ReorderableList _conditionsList;
		private readonly TransitionTableEditor _editor;
		// Flag to know if a transition is being added
		private bool _toggle;

		/********************/
		/* Internal methods */
		/********************/
		internal AddTransitionDisplay(TransitionTableEditor editor)
		{
			_editor = editor;
			_transition = new SerializedObject(ScriptableObject.CreateInstance<TransitionSo>());
			SerialisedTransition = new SerialisedTransition(_transition.FindProperty("item"));
			_conditionsList = new ReorderableList(_transition, SerialisedTransition.Conditions);
			SetupConditionsList(_conditionsList);
		}

		internal void Display(Rect position)
		{
			position.x += 8;
			position.width -= 16;
			Rect rect = position;
			var listHeight = _conditionsList.GetHeight();
			var singleLineHeight = EditorGUIUtility.singleLineHeight;

			// Display add button only if not already adding a transition
			if (!_toggle)
			{
				position.height = singleLineHeight;

				// Reserve space
				GUILayoutUtility.GetRect(position.width, position.height);

				if (!GUI.Button(position, "Add Transition")) return;

				_toggle = true;
				SerialisedTransition.ClearProperties();

				return;
			}

			// Background
			{
				position.height = listHeight + singleLineHeight * 4;
				DrawRect(position, ContentStyle.LightGray);
			}

			// Reserve space
			GUILayoutUtility.GetRect(position.width, position.height);

			// State Fields
			{
				position.y += 10;
				position.x += 20;
				StatePropField(position, "Origin", SerialisedTransition.OriginState);
				position.x = rect.width / 2 + 20;
				StatePropField(position, "Target", SerialisedTransition.TargetState);
			}

			// Conditions List
			{
				position.y += 30;
				position.x = rect.x + 5;
				position.height = listHeight;
				position.width -= 10;
				_conditionsList.DoList(position);
			}

			// Add and cancel buttons
			{
				position.y += position.height + 5;
				position.height = singleLineHeight;
				position.width = rect.width / 2 - 20;
				if (GUI.Button(position, "Add Transition"))
				{
					if (SerialisedTransition.OriginState.objectReferenceValue == null)
						Debug.LogException(new ArgumentNullException("Origin"));
					else if (SerialisedTransition.TargetState.objectReferenceValue == null)
						Debug.LogException(new ArgumentNullException("Target"));
					else if (SerialisedTransition.OriginState.objectReferenceValue ==
							 SerialisedTransition.TargetState.objectReferenceValue)
						Debug.LogException(new InvalidOperationException("Origin and Target states are equal"));
					else
					{
						_editor.AddTransition(SerialisedTransition);
						_toggle = false;
					}
				}
				position.x += rect.width / 2;
				if (GUI.Button(position, "Cancel"))
				{
					_toggle = false;
				}
			}

			void StatePropField(Rect pos, string label, SerializedProperty prop)
			{
				pos.height = singleLineHeight;
				LabelField(pos, label);
				pos.x += 40;
				pos.width /= 4;
				PropertyField(pos, prop, GUIContent.none);
			}
		}

		/******************/
		/* Public methods */
		/******************/
		public void Dispose()
		{
			UnityEngine.Object.DestroyImmediate(_transition.targetObject);
			_transition.Dispose();
			GC.SuppressFinalize(this);
		}

		/*******************/
		/* Private methods */
		/*******************/
		private static void SetupConditionsList(ReorderableList reorderableList)
		{
			reorderableList.elementHeight *= 2.3f;
			reorderableList.drawHeaderCallback += rect => GUI.Label(rect, "Conditions");

			// Defines behaviour when a new element is added to the list of conditions
			reorderableList.onAddCallback += list =>
			{
				var count = list.count;
				list.serializedProperty.InsertArrayElementAtIndex(count);
				SerializedProperty sp = list.serializedProperty.GetArrayElementAtIndex(count);
				sp.FindPropertyRelative("condition").objectReferenceValue = null;
				sp.FindPropertyRelative("expectedResult").enumValueIndex = 0;
				sp.FindPropertyRelative("operator").enumValueIndex = 0;
			};

			// Defines behaviour when the list is changed
			reorderableList.onChangedCallback += 
				_ => reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();

			// Defines behaviour when an element has to be drawn
			reorderableList.drawElementCallback += (rect, index, _, _) =>
			{
				SerializedProperty sp = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				rect = new Rect(rect.x, rect.y + 2.5f, rect.width, EditorGUIUtility.singleLineHeight);
				SerializedProperty condition = sp.FindPropertyRelative("condition");
				if (condition.objectReferenceValue != null)
				{
					var label = condition.objectReferenceValue.name;

					// Adds first label into the box
					GUI.Label(rect, "If");

					// Adds label with the condition name
					GUI.Label(
						new Rect(rect.x + 20, rect.y, rect.width, rect.height), label, EditorStyles.boldLabel);

					// Adds property field to allow condition selection
					PropertyField(
						new Rect(rect.x + rect.width - 180, rect.y, 20, rect.height),
						condition,
						GUIContent.none);
				}
				// If the condition is not defined, shows a blank property field
				else
					PropertyField(new Rect(rect.x, rect.y, 150, rect.height), condition, GUIContent.none);

				LabelField(new Rect(rect.x + rect.width - 120, rect.y, 20, rect.height), "Is");
				PropertyField(new Rect(rect.x + rect.width - 60, rect.y, 60, rect.height),
					sp.FindPropertyRelative("expectedResult"), GUIContent.none);
				PropertyField(new Rect(rect.x + 20, rect.y + EditorGUIUtility.singleLineHeight + 5,
					60, rect.height), sp.FindPropertyRelative("operator"), GUIContent.none);
			};

			// Defines behaviour regarding the background of the list of conditions.
			reorderableList.drawElementBackgroundCallback += (rect, index, _, isFocused) =>
			{
				if (isFocused) DrawRect(rect, ContentStyle.Focused);
				DrawRect(rect, index % 2 != 0 ? ContentStyle.ZebraDark : ContentStyle.ZebraLight);
			};
		}

		/// <summary>
		/// Scriptable object to serialize a Transition item
		/// </summary>
		internal class TransitionSo : ScriptableObject
		{
			public StrTransition item;
		}
	}
}