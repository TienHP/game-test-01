using UnityEngine;
using Zenject;

public interface IGameConfig
{
    int MaxHP { get; }
}

public class GameConfig : IInitializable, IGameConfig
{
    public int MaxHP => 100;

    public void Initialize()
    {
        Debug.Log("[GameConfig] --> Initialized");
    }
}