using System.Collections.Generic;

namespace TFF.Salvatore.SaveLoadSystem.Core
{
    public interface ISaveElement
    {
        // Used to save information provided by this object
        List<DataChunk> OnSave();

        // Used to load information provided by this object
        void OnLoad(List<DataChunk> data);
    }
}
