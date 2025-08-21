using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Scriptable/LevelDefinition")]
public class LevelDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public int levelIndex;
    public Scene scene;

    public int numberOfLaps;

}
