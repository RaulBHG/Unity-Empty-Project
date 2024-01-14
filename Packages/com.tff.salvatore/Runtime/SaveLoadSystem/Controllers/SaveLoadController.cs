using System;
using System.Collections;
using System.Collections.Generic;
using TFF.Core.DesignPatterns.RuntimeSets;
using TFF.Salvatore.SaveLoadSystem.Core;
using TFF.Core.DesignPatterns.Events.Actors;
using UnityEngine;
using TFF.Salvatore.SaveLoadSystem.Events;
using TFF.Salvatore.SaveLoadSystem.Input;
using TFF.Salvatore.SaveLoadSystem.Utils;

using System.Linq;
using TFF.Salvatore.SaveLoadSystem.SaveLoadTypes;

namespace TFF.Salvatore.SaveLoadSystem.Controllers
{
    public class SaveLoadController : AMbListener<SoSaveSystemEventChannel, StrSaveData>
    {
         /*************************/
         /* Serialised Attributes */
         /*************************/
         [SerializeField] private ASoRuntimeSet<string, ASaveElement> saveRuntimeSet;
         [SerializeField] private bool encrypted = true;

         public Dictionary<string, List<DataChunk>> snapshotData = new();
         public Dictionary<string, List<DataChunk>> snapshotOutData = new();

         public Dictionary<string, ASaveElement> saveElements = new();

         private ISaveLoader saveLoader;

         private void Start()
         {
             saveLoader = new FileSaveLoader(encrypted, ref snapshotData, ref snapshotOutData);
             saveElements = saveRuntimeSet.GetAllItems(); // Obtain all save elements
         }

         // Used by the event to execute save system
         protected override IEnumerator Answer(StrSaveData data)
         {
             ESelectionTemporalPersistent type = data.selectionTemporalPersistent;
             ESelectionSaveLoad saveLoad = data.selectionSaveLoad;
             int slot = data.slot;


             switch(saveLoad)
             {
                 case ESelectionSaveLoad.Save:
                     if(type == ESelectionTemporalPersistent.Global)
                     {
                         OnSave(); // Save GlobalData first
                         break;
                     }

                     // By default we save the snapshot
                     OnSaveSnapshot();
                     // If is persistent we add it to the file
                     if (type == ESelectionTemporalPersistent.Persistent)
                         OnSave(slot);

                     break;
                 case ESelectionSaveLoad.Load:
                     // Load the data of the file into the snapshotData or snapshotOutData
                     if (type != ESelectionTemporalPersistent.Temporal)
                     {
                         OnLoad(); // Load outData first
                         OnLoad(slot);
                     }
                     // By default we load the snapshot
                     OnLoadSnapshot();
                     break;
             }
             yield return null;
         }


         // Removes the data of the scene
         public void RemoveSceneData()
         {
             Guid guidToSearch = GuidService.GenerateGuidFromString("saveType");

             List<string> keysToRemove = snapshotData
                 .Where(pair => pair.Value.Any(chunk =>
                     chunk.guid == guidToSearch &&
                     (string)chunk.data["saveType"] == ESelectionSaveType.SceneSave.ToString()))
                 .Select(pair => pair.Key)
                 .ToList();

             foreach (string key in keysToRemove)
             {
                 snapshotData.Remove(key);
             }
         }



        // Used to save temporal game
        public void OnSaveSnapshot()
        {
            // Obtain guid and data from every SaveElement
            foreach (KeyValuePair<string, ASaveElement> element in saveElements)
            {
                string key = element.Value.guid.ToString();
                if (element.Value.saveType == ESelectionSaveType.GlobalSave)
                {        
                    // Save the out data (data outside the game you are playing)
                    snapshotOutData[key] = element.Value.OnSave();
                }
                else
                {
                    // Save the normal data
                    snapshotData[key] = element.Value.OnSave();

                    // To save the dataType
                    DataChunk saveType = new()
                    {
                        guid = GuidService.GenerateGuidFromString("saveType"),
                        data = new()
                        {
                            { "saveType", element.Value.saveType.ToString() }
                        }
                    };
                    snapshotData[key].Add(saveType);
                }
            }
            Debug.Log("Snapshot Saved.");
        }

        // Used to load temporal game. This doest load GlobalData, just scene and save data
        public void OnLoadSnapshot()
        {
            // Call OnLoad of every SaveElement
            foreach (KeyValuePair<string, List<DataChunk>> data in snapshotData)
            {
                ASaveElement saveElement = saveElements[data.Key];
                saveElement.OnLoad(data.Value);
            }
        }

        // Used to save persistent game
        public bool OnSave(int slot = 0)
        {
            return saveLoader.OnSave(slot);
        }

        // Used to load persistent game
        public void OnLoad(int slot = 0)
        {
            if (slot == 0)
            {
                snapshotOutData = saveLoader.OnLoad(slot);
            }
            else
            {
                snapshotData = saveLoader.OnLoad(slot);
                PlayerPrefs.SetInt("SlotGame", slot);
            }
        }

    }

}
