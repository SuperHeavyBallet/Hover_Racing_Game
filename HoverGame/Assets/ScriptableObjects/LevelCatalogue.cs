using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "LevelCatalogue", menuName = "Scriptable/LevelCatalogue")]
public class LevelCatalogue : ScriptableObject
{
    public List<LevelDefinition> levels = new();

    private Dictionary<string, LevelDefinition> byId;
    private Dictionary<int, LevelDefinition> byIndex;

    bool built;

    void OnEnable() => built = false;

    public void EnsureBuilt()
    {
        if (built) return;
        byId = new();
       

        foreach (var l in levels)
        {
            if (!l || string.IsNullOrWhiteSpace(l.id)) continue;
            byId[l.id] = l;
            byIndex[l.levelIndex] = l;
            
        }
        built = true;
    }

    public List<LevelDefinition> GetListOfAllLevels()
    {
        EnsureBuilt();
        return levels;
    }

    public LevelDefinition GetById(string id)
    {
        EnsureBuilt();

        if(byId != null)
        {
            if(byId.TryGetValue(id, out var lvl)) return lvl;
        }

        return null;
    }

    public LevelDefinition GetByIndex(int index)
    {
        if (byIndex != null)
        {
            if (byIndex.TryGetValue(index, out var lvl)) return lvl;
        }

        return null;
    }



}
