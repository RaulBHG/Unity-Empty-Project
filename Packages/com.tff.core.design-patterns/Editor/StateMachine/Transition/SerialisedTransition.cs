using UnityEditor;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.Transition
{
	public readonly struct SerialisedTransition
	{
		/***********************/
		/* Internal attributes */
		/***********************/
		internal readonly SerializedProperty Transition;
		internal readonly SerializedProperty OriginState;
		internal readonly SerializedProperty TargetState;
		internal readonly SerializedProperty Conditions;
		internal readonly int Index;

		internal SerialisedTransition(SerializedProperty transition, int index)
		{
			Transition = transition.GetArrayElementAtIndex(index);
			OriginState = Transition.FindPropertyRelative("origin");
			TargetState = Transition.FindPropertyRelative("target");
			Conditions = Transition.FindPropertyRelative("conditions");
			Index = index;
		}

		internal SerialisedTransition(SerializedProperty transition)
		{
			Transition = transition;
			OriginState = Transition.FindPropertyRelative("origin");
			TargetState = Transition.FindPropertyRelative("target");
			Conditions = Transition.FindPropertyRelative("conditions");
			Index = -1;
		}

		internal void ClearProperties()
		{
			OriginState.objectReferenceValue = null;
			TargetState.objectReferenceValue = null;
			Conditions.ClearArray();
		}
	}
}