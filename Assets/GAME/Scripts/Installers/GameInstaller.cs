using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameState>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<GameConfig>()
            .AsSingle();
    }
}