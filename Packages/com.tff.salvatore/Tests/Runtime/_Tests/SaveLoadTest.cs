using System.Collections;
using NUnit.Framework;
using TFF.Salvatore.SaveLoadSystem.Controllers;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using TFF.Salvatore.SaveLoadSystem.Core;

namespace TFF.Salvatore.SaveLoadSystem.Tests
{
    public class SaveLoadTest
    {
        private SaveLoadController _saveLoadController;
        protected readonly string _scene = "SaveLoadTestScene1";

        // LoadScene
        [SetUp]
        public void SetUp()
        {
            EditorSceneManager.LoadScene(_scene);                        
        }

        /**
         * We cant separate this in two test cause both depends on saveElements. 
         * If we separate the test and try to run both at the same time the objects loaded by the scene 
         * are deleted on the TearDown of one test and this breaks the other one.
         */
        [UnityTest]
        public IEnumerator SaveLoadTestMain()
        {
            yield return null;
            Assert.IsTrue(true);
            _saveLoadController = Object.FindObjectOfType<SaveLoadController>();
            // Obtain guid and data from every SaveElement
            foreach (KeyValuePair<string, ASaveElement> element in _saveLoadController.saveElements)
            {
                Assert.IsTrue(element.Value.OnSave().GetType() == typeof(List<DataChunk>));
            }
            // If OnLoad throws error, the test is failed
            foreach (KeyValuePair<string, List<DataChunk>> data in _saveLoadController.snapshotData)
            {
                ASaveElement saveElement = _saveLoadController.saveElements[data.Key];
                saveElement.OnLoad(data.Value);
            }

            _saveLoadController.OnLoadSnapshot();
            _saveLoadController.OnSaveSnapshot();
            Assert.IsTrue(_saveLoadController.OnSave());
            Assert.IsTrue(_saveLoadController.OnSave(1));
            Assert.IsTrue(_saveLoadController.OnSave(2));
            Assert.IsTrue(_saveLoadController.OnSave(3));
            _saveLoadController.OnLoad();
            _saveLoadController.OnLoad(1);
            _saveLoadController.OnLoad(2);
            _saveLoadController.OnLoad(3);
            _saveLoadController.OnLoadSnapshot();
        }


        // Unload Scene
        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.UnloadSceneAsync(_scene);
        }
    }
}
