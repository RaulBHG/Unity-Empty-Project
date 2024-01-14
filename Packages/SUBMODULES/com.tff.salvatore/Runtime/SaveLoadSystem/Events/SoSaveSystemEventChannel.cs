using TFF.Core.DesignPatterns.Events.Channels;
using TFF.Salvatore.SaveLoadSystem.Controllers;
using TFF.Salvatore.SaveLoadSystem.Input;
using UnityEngine;


namespace TFF.Salvatore.SaveLoadSystem.Events
{
    [CreateAssetMenu(fileName = "SO_SaveSystem_EventChannel", menuName = "TFF/Save System/EventChannel")]
    public class SoSaveSystemEventChannel : ASoEventChannel<StrSaveData>
    {}

    public struct StrSaveData
    {
        public readonly ESelectionTemporalPersistent selectionTemporalPersistent;
        public readonly ESelectionSaveLoad selectionSaveLoad;
        public int slot;

        public StrSaveData(ESelectionTemporalPersistent selectionTemporalPersistent, ESelectionSaveLoad selectionSaveLoad, int slot)
        {
            this.selectionTemporalPersistent = selectionTemporalPersistent;
            this.selectionSaveLoad = selectionSaveLoad;
            this.slot = slot;
        }
    }
}
