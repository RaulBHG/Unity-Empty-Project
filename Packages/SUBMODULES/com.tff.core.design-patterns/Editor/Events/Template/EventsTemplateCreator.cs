﻿using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TFF.Core.DesignPatterns.Editor.Events.Template
{
	internal class EventsTemplateCreator
	{
		private const string Resources = "Packages/com.tff.core.design-patterns/Editor/Events/Template/Resources";

		[MenuItem("Assets/Create/TFF/Event System/Scripts/New Event Channel Script", false, 0)]
		public static void CreateActionScript() =>
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
				ScriptableObject.CreateInstance<SolveScriptName>(),
				"New Event Channel.cs",
				(Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
				$"{Path.GetFullPath(Resources)}/EventTemplate.txt");

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
					.Replace("EventChannel","")
					.Replace("Event Channel","")
					.Replace(".cs","");
				newName = "So" + newName + "EventChannel.cs";
				pathName = pathName.Replace(fileName, newName);
				fileName = newName;

				// Replaces the script name
				var scriptName = Path.GetFileNameWithoutExtension(fileName);
				scriptName = scriptName.Replace("So", "")
					.Replace("EventChannel","")
					.Replace("Event Channel","");
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