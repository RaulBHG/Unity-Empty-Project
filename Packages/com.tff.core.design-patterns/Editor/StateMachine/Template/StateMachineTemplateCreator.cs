using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TFF.Core.DesignPatterns.Editor.StateMachine.Template
{
	internal class StateMachineTemplateCreator
	{
		private const string Resources = "Packages/com.tff.core.design-patterns/Editor/StateMachine/Template/Resources";

		[MenuItem("Assets/Create/TFF/State Machine/Scripts/New Action Script", false, 0)]
		public static void CreateActionScript() =>
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
				ScriptableObject.CreateInstance<SolveScriptName>(),
				"New Action.cs",
				(Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
				$"{Path.GetFullPath(Resources)}/ActionTemplate.txt");

		[MenuItem("Assets/Create/TFF/State Machine/Scripts/New Condition Script", false, 0)]
		public static void CreateConditionScript() =>
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
				ScriptableObject.CreateInstance<SolveScriptName>(),
				"New Condition.cs",
				(Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
				$"{Path.GetFullPath(Resources)}/ConditionTemplate.txt");

		private class SolveScriptName : EndNameEditAction
		{
			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				// Gets input data
				var text = File.ReadAllText(resourceFile);
				var fileName = Path.GetFileName(pathName);

				// Removes spaces and adds Scriptable Object hint in the name of the script
				var newName = fileName.Replace(" ", "")
					.Replace("So", "")
					.Replace("Action","")
					.Replace("Condition","")
					.Replace(".cs","");
				newName = "So" + newName +
						  (fileName.Contains("Action") ? "Action" : fileName.Contains("Condition") ? "Condition" : "") +
						  ".cs";
				pathName = pathName.Replace(fileName, newName);
				fileName = newName;

				// Replaces the script name
				var scriptName = Path.GetFileNameWithoutExtension(fileName);
				scriptName = scriptName.Replace("So", "")
					.Replace("Action", "")
					.Replace("Condition", "");
				text = text.Replace("#SCRIPT_NAME#", scriptName);

				// Builds and replaces the actual name with spaces
				for (var i = scriptName.Length - 1; i > 0; i--)
					if (char.IsUpper(scriptName[i]) && char.IsLower(scriptName[i - 1]))
						scriptName = scriptName.Insert(i, " ");
				text = text.Replace("#ACTUAL_NAME_SPACES#", scriptName);

				// Replaces name space
				var nsArray = pathName.Split("/");
				var nameSpace = string.Join(".",new ArraySegment<string>(nsArray, 2, nsArray.Length-3));
				text = text.Replace("#NAMESPACE#", nameSpace);

				// Writes the final name of the script
				File.WriteAllText(Path.GetFullPath(pathName), text, new UTF8Encoding(true));
				AssetDatabase.ImportAsset(pathName);
				ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath(pathName, typeof(Object)));
			}
		}
	}
}