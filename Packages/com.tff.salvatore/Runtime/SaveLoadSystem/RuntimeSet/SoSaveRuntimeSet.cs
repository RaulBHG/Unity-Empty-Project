using TFF.Core.DesignPatterns.RuntimeSets;
using TFF.Salvatore.SaveLoadSystem.Core;
using UnityEngine;

namespace TFF.Salvatore.SaveLoadSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_SaveSystem_RuntimeSet", menuName = "TFF/Save System/RuntimeSet")]
    public class SoSaveRuntimeSet : ASoRuntimeSet<string, ASaveElement>
    {
    }
}
