using UnityEngine;
using Zenject;

public interface IGameState
{
    CharacterModel[] CharacterModels { get; set; }
}

public class GameState : IInitializable, IGameState
{
    public CharacterModel[] CharacterModels { get; set; }

    public void Initialize()
    {
        Debug.Log("[GameState] --> Initialized");
    }
}