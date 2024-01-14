using System;
using System.Collections.Generic;
using TFF.Salvatore.SaveLoadSystem.Utils;
using TFF.Core.DesignPatterns.RuntimeSets;
using UnityEngine;
using TFF.Salvatore.SaveLoadSystem.Input;

namespace TFF.Salvatore.SaveLoadSystem.Core
{
    public abstract class ASaveElement : ARuntimeSetHandler<string, ASaveElement>, ISaveElement
    {
        [Tooltip("Select where to save the game")]
        [SerializeField] public ESelectionSaveType saveType;
        [Tooltip("This save ID has to be unique and inmutable during the gameplay. Is going to be used by Salvatore to save and localize this object information.")] 
        [SerializeField] private string _saveId;
        public Guid guid; // When save this guid will be transform to a string
        protected List<DataChunk> dataList = new();

        /**
         * Here we generate the guid using the saveId
         */
        private void Awake()
        {
            guid = GuidService.GenerateGuidFromString(_saveId);
            AddItemToSet(guid.ToString(), this);
        }

        // Used to save information provided by this object
        public virtual List<DataChunk> OnSave() => dataList;

        // Used to load information provided by this object
        public virtual void OnLoad(List<DataChunk> data) { }
    }

    public struct DataChunk
    {
        public Guid guid;
        public Dictionary<string, object> data;
    }
}
