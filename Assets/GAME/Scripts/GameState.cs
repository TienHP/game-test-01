using UnityEngine;
using Zenject;

public interface IGameState
{
    StageModel StageModel { get; set; }
    CharacterModel[] CharacterModels { get; set; }
    SpineData[] SpineData { get; set; }
}

public class GameState : IInitializable,
    IGameState
{
    public StageModel StageModel { get; set; }


    public CharacterModel[] CharacterModels { get; set; }

    public SpineData[] SpineData { get; set; }

    public void Initialize()
    {
        Debug.Log("[GameState] --> Initialized");
    }
}