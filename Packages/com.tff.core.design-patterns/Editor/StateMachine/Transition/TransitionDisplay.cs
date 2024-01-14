using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.Transition
{
	public class TransitionDisplay
	{
		/***********************/
		/* Internal attributes */
		/***********************/
		internal SerialisedTransition SerialisedTransition { get; }

		/**********************/
		/* Private attributes */
		/**********************/
		private readonly ReorderableList _conditionsList;
		private readonly TransitionTableEditor _editor;

		/********************/
		/* Internal methods */
		/********************/
		internal TransitionDisplay(SerialisedTransition serializedTransition, TransitionTableEditor editor)
		{
			SerialisedTransition = serializedTransition;
			_conditionsList = 
				new ReorderableList(SerialisedTransition.Transition.serializedObject, SerialisedTransition.Conditions,
					true, false, true, true);
			SetupConditionsList(_conditionsList);
			_editor = editor;
		}

		internal bool Display(ref Rect position)
		{
			Rect rect = position;
			var listHeight = _conditionsList.GetHeight();
			var singleLineHeight = EditorGUIUtility.singleLineHeight;

			// Reserve space
			{
				rect.height = singleLineHeight + 10 + listHeight;
				GUILayoutUtility.GetRect(rect.width, rect.height);
				position.y += rect.height + 5;
			}

			// Background
			{
				rect.x += 5;
				rect.width -= 10;
				rect.height -= listHeight;
				DrawRect(rect, ContentStyle.DarkGray);
			}

			// Transition Header
			{
				rect.x += 3;
				LabelField(rect, "To");

				rect.x += 20;
				var labelElements= SerialisedTransition.TargetState.objectReferenceValue.name.Split("_");
				LabelField(rect, $"{labelElements[^1]} {labelElements[^2]}", EditorStyles.boldLabel);
			}


			// Buttons
			{
				Rect buttonRect = new Rect(x: rect.width - 25, y: rect.y + 5, width: 30, height: 18);

				int i, l;
				{
					var transitions =
						_editor.GetStateTransitions(SerialisedTransition.OriginState.objectReferenceValue);
					l = transitions.Count - 1;
					i = transitions.FindIndex(t => t.Index == SerialisedTransition.Index);
				}

				// Remove transition
				if (Button(buttonRect, "Toolbar Minus"))
				{
					_editor.RemoveTransition(SerialisedTransition);
					return true;
				}
				buttonRect.x -= 35;

				// Move transition down
				if (i < l)
				{
					if (Button(buttonRect, "scrolldown"))
					{
						_editor.ReorderTransition(SerialisedTransition, false);
						return true;
					}
					buttonRect.x -= 35;
				}

				// Move transition up
				if (i > 0)
				{
					if (Button(buttonRect, "scrollup"))
					{
						_editor.ReorderTransition(SerialisedTransition, true);
						return true;
					}
					buttonRect.x -= 35;
				}

				// State editor
				if (Button(buttonRect, "SceneViewTools"))
				{
					_editor.DisplayStateEditor(SerialisedTransition.TargetState.objectReferenceValue);
					return true;
				}

				bool Button(Rect pos, string icon) => GUI.Button(pos, EditorGUIUtility.IconContent(icon));
			}

			rect.x = position.x + 5;
			rect.y += rect.height;
			rect.width = position.width - 10;
			rect.height = listHeight;

			// Display conditions
			_conditionsList.DoList(rect);

			return false;
		}

		/*******************/
		/* Private methods */
		/*******************/
		private static void SetupConditionsList(ReorderableList reorderableList)
		{
			reorderableList.elementHeight *= 2.3f;
			reorderableList.headerHeight = 1f;
			reorderableList.onAddCallback += l =>
			{
				var count = l.count;
				l.serializedProperty.InsertArrayElementAtIndex(count);
				SerializedProperty sp = l.serializedProperty.GetArrayElementAtIndex(count);
				sp.FindPropertyRelative("condition").objectReferenceValue = null;
				sp.FindPropertyRelative("expectedResult").enumValueIndex = 0;
				sp.FindPropertyRelative("operator").enumValueIndex = 0;
			};

			// Defines behaviour when the list is changed
			reorderableList.onChangedCallback += l =>
				l.serializedProperty.serializedObject.ApplyModifiedProperties();

			// Defines behaviour when an element has to be drawn
			reorderableList.drawElementCallback += (rect, index, _, _) =>
			{
				SerializedProperty sp = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				rect = new Rect(rect.x, rect.y + 2.5f, rect.width, EditorGUIUtility.singleLineHeight);

				SerializedProperty condition = sp.FindPropertyRelative("condition");

				// If the condition is defined, shows the condition data
				if (condition.objectReferenceValue != null)
				{
					var labelElements= condition.objectReferenceValue.name.Split("_");
					var label = $"{labelElements[^1]} {labelElements[^2]}";

					// Adds first label into the box
					GUI.Label(rect, "If");

					// Adds property field to allow condition selection
					Rect r = rect;
					r.x += 20;
					r.width = 35;
					PropertyField(r, condition, GUIContent.none);
					r.x += 40;
					r.width = rect.width - 120;
					GUI.Label(r, label, EditorStyles.boldLabel);
				}
				// If the condition is not defined, shows a blank property field
				else
				{
					PropertyField(new Rect(rect.x, rect.y, 150, rect.height), condition, GUIContent.none);
				}

				// Draw the boolean value expected by the condition (i.e. "Is True", "Is False")
				LabelField(new Rect(rect.x + rect.width - 80, rect.y, 20, rect.height), "Is");
				PropertyField(new Rect(rect.x + rect.width - 60, rect.y, 60, rect.height),
					sp.FindPropertyRelative("expectedResult"), GUIContent.none);

				// Only display the logic condition if there's another one after this
				if (index < reorderableList.count - 1)
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
	}
}