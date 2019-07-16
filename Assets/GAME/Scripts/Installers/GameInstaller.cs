﻿using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private GameObject _stagePrefab;
    [SerializeField] private Transform _root;

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
        Container.BindInterfacesAndSelfTo<GameStageView>()
            .AsSingle();
        Container.BindIFactory<CharacterView>()
            .FromComponentInNewPrefab(_characterPrefab)
            .UnderTransform(_root)
            .AsCached();
        Container.BindIFactory<Stage>()
            .FromComponentInNewPrefab(_stagePrefab)
            .UnderTransform(_root)
            .AsCached();
    }
}