using UnityEngine;
using Zenject;

public static partial class Constant
{
    public static class ZenjectId
    {
        public const string WorldUIRoot = "WorldUIRoot";
        public const string MainCamera = "MainCamera";
        public const string UICamera = "UICamera";
    }
}

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private GameObject _stagePrefab;
    [SerializeField] private Transform _worldRoot;
    [SerializeField] private GameObject _hpBarPrefab;
    [SerializeField] private Transform _worldUIRoot;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _uiCamera;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameState>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<GameConfig>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<WebRequestService>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<ResourceLoader>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<ResourceCreator>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<GameStagePresenter>()
            .AsSingle();
        Container.BindInterfacesAndSelfTo<AttackingLogicStrategy>()
            .AsSingle();
        Container.BindIFactory<Character>()
            .FromComponentInNewPrefab(_characterPrefab)
            .UnderTransform(_worldRoot)
            .AsSingle();
        Container.BindIFactory<Stage>()
            .FromComponentInNewPrefab(_stagePrefab)
            .UnderTransform(_worldRoot)
            .AsSingle();
        Container.BindIFactory<HPBarView>()
            .FromComponentInNewPrefab(_hpBarPrefab)
            .UnderTransform(_worldUIRoot.transform)
            .AsSingle();
        Container.Bind<Camera>()
            .WithId(Constant.ZenjectId.MainCamera)
            .FromInstance(_mainCamera);
        Container.Bind<Camera>()
            .WithId(Constant.ZenjectId.UICamera)
            .FromInstance(_uiCamera);
        Container.Bind<Transform>()
            .WithId(Constant.ZenjectId.WorldUIRoot)
            .FromInstance(_worldUIRoot);
    }
}