using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using TFF.Salvatore.SaveLoadSystem.Controllers;
using System.Collections.Generic;
using TFF.Salvatore.SaveLoadSystem.Core;

namespace TFF.Salvatore.SaveLoadSystem.Editor
{
    public class SaveLoadInspector : EditorWindow
    {
        [MenuItem("Window/Custom Editors/SaveLoadSystem/SaveLoadInspector")]
        public static void ShowInspector()
        {
            SaveLoadInspector wnd = GetWindow<SaveLoadInspector>();
            wnd.titleContent = new GUIContent("SaveLoadInspector");
        }

        private bool _canLoad = false;

        private SaveLoadController _saveLoadController;

        private VisualElement _successLayout;
        private VisualElement _errorLayout;
        private Label _errorLabel;

        private ScrollView _snapshotGlobalData;
        private ScrollView _snapshotGameSceneData;
        private Button _saveSnapshot;
        private Button _saveGame;
        private Button _loadSnapshot;
        private Button _loadGame;
        private Button _removeSceneData;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.tff.salvatore/Editor/SaveLoadSystem/SaveLoadInspector.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            _successLayout = root.Q<VisualElement>("SuccessLayout");
            _errorLayout = root.Q<VisualElement>("ErrorLayout");
            _errorLabel = root.Q<Label>("ErrorLabel");

            if (EditorApplication.isPlaying)
            {
                _canLoad = true;
                // Find elements
                _snapshotGlobalData = root.Q<ScrollView>("SnapshotGlobalData");
                _snapshotGameSceneData = root.Q<ScrollView>("SnapshotGameSceneData");
                _saveSnapshot = root.Q<Button>("SaveSnapshot");
                _saveGame = root.Q<Button>("SaveGame");
                _loadSnapshot = root.Q<Button>("LoadSnapshot");
                _loadGame = root.Q<Button>("LoadGame");
                _removeSceneData = root.Q<Button>("RemoveSceneData");

                // Search SaveLoadController Running On the scene
                GameObject saveLoadControllerObject = GameObject.Find("SaveLoadManager");
                if (saveLoadControllerObject != null)
                {
                    _successLayout.style.display = DisplayStyle.Flex;
                    _saveLoadController = saveLoadControllerObject.GetComponent<SaveLoadController>();

                    _saveSnapshot.clicked += SaveSnapshot;
                    _saveGame.clicked += SaveGame;
                    _loadSnapshot.clicked += LoadSnapshot;
                    _loadGame.clicked += LoadGame;
                    _removeSceneData.clicked += RemoveSceneData;
                }
                else
                {
                    _errorLabel.text = "SaveLoadManaget no encontrado en la escena.";
                    _errorLayout.style.display = DisplayStyle.Flex;
                }
                
                                
            }
            else
            {
                _errorLayout.style.display = DisplayStyle.Flex;
            }
        }
        

        private void SaveSnapshot()
        {
            _saveLoadController.OnSaveSnapshot();
        }
        private void LoadSnapshot()
        {
            _saveLoadController.OnLoadSnapshot();
        }
        private void SaveGame()
        {
            _saveLoadController.OnSave();
            _saveLoadController.OnSave(1);
        }
        private void LoadGame()
        {
            _saveLoadController.OnLoad();
            _saveLoadController.OnLoad(1);
        }
        private void RemoveSceneData()
        {
            _saveLoadController.RemoveSceneData();
        }

        // Get Snapshot Data
        private void Update()
        {
            if (_canLoad)
            {
                _snapshotGlobalData.Clear();
                _snapshotGameSceneData.Clear();

                void ProcessData(Dictionary<string, List<DataChunk>> data, ScrollView targetList)
                {
                    foreach (KeyValuePair<string, List<DataChunk>> pair in data)
                    {
                        Label parentLabel = new Label($"{pair.Key}:");
                        parentLabel.AddToClassList("parentItem");
                        targetList.Add(parentLabel);
                        foreach (DataChunk dataChunk in pair.Value)
                        {
                            foreach (var dataEntry in dataChunk.data)
                            {
                                targetList.Add(new Label($"{dataEntry.Key}: {dataEntry.Value}"));
                            }
                        }
                    }
                }

                ProcessData(_saveLoadController.snapshotOutData, _snapshotGlobalData);
                ProcessData(_saveLoadController.snapshotData, _snapshotGameSceneData);
            }

        }

    }
}