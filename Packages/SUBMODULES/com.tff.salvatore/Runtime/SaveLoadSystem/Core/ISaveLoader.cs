using System.Collections.Generic;

namespace TFF.Salvatore.SaveLoadSystem.Core
{
    public interface ISaveLoader
    {
        // Used to save persistent game
        bool OnSave(int slot = 0);

        // Used to load persistent game
        Dictionary<string, List<DataChunk>> OnLoad(int slot = 1);
    }
}
