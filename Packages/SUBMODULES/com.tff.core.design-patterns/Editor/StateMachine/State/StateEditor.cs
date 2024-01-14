using TFF.Core.DesignPatterns.StateMachine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.State
{
	[CustomEditor(typeof(SoState))]
	public class StateEditor : UnityEditor.Editor
	{
		/**********************/
		/* Private attributes */
		/**********************/
		// List of actions within the State
		private ReorderableList _actionList;

		/*******************/
		/* Private methods */
		/*******************/
		private static void SetupActionList(ReorderableList actionList)
		{
			// Sets list title
			actionList.drawHeaderCallback += rect => GUI.Label(rect, "List of actions");

			// Sets height of all the elements to allow correct display of the action names
			actionList.elementHeight *= 1.5f;

			// Sets action to be performed when the action list changes
			actionList.onChangedCallback +=
				l => l.serializedProperty.serializedObject.ApplyModifiedProperties();

			// Sets action to be performed when a new action is added to the list
			actionList.onAddCallback += list =>
			{
				// Creates the new element
				var count = list.count;
				list.serializedProperty.InsertArrayElementAtIndex(count);

				// Gets the new element and initialises with no action
				SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(count);
				element.objectReferenceValue = null;
			};

			// Sets action to be performed when the action in the list has to be drawn
			actionList.drawElementCallback += (rect, index, _, _) =>
			{
				// Generates the rect that contains the action
				Rect r = rect;
				r.height = EditorGUIUtility.singleLineHeight;
				r.y += 5;
				r.x += 5;

				SerializedProperty element = actionList.serializedProperty.GetArrayElementAtIndex(index);
				if (element.objectReferenceValue != null)
				{
					// Creates the box interactable where the action can be changed
					var labelElements= element.objectReferenceValue.name.Split("_");
					var label = $"{labelElements[^1]} {labelElements[^2]}";
					r.width = 35;
					EditorGUI.PropertyField(r, element, GUIContent.none);

					// Creates the label with the name of the actual action
					r.width = rect.width - 50;
					r.x += 42;
					GUI.Label(r, label, EditorStyles.boldLabel);
				}
				else
				{
					// Draws the action as empty
					EditorGUI.PropertyField(r, element, GUIContent.none);
				}
			};

			// Sets action to be performed when the background of each action changes
			actionList.drawElementBackgroundCallback += (rect, index, _, isFocused) =>
			{
				if (isFocused) EditorGUI.DrawRect(rect, ContentStyle.Focused);
				EditorGUI.DrawRect(rect, index % 2 != 0 ? ContentStyle.ZebraDark : ContentStyle.ZebraLight);
			};
		}

		private void DoUndo() => serializedObject.UpdateIfRequiredOrScript();

		/*************************/
		/* Unity Event Functions */
		/*************************/
		public override void OnInspectorGUI()
		{
			_actionList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			Undo.undoRedoPerformed += DoUndo;

			// Creates list of serialised actions based on the actions defined in the Scriptable Object
			_actionList = new ReorderableList(serializedObject, serializedObject.FindProperty("actions"),
				true, true, true, true);
			SetupActionList(_actionList);
		}

		private void OnDisable() => Undo.undoRedoPerformed -= DoUndo;
	}
}