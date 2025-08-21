using UnityEngine;

public class LapCounterController : MonoBehaviour
{
    public LevelCatalogue LevelCatalogue;

    public LevelDefinition thisLevel;

    int numberOfLaps;

    private void Awake()
    {
       numberOfLaps = thisLevel.numberOfLaps;
    }

}
