using System;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

public class ProgramLogic : MonoBehaviour
{
    [Inject] private ResourceLoader _resourceLoader;
    [Inject] private IGameStagePresenter _gameStagePresenter;
    [Inject] private ResourceCreator _resourceCreator;

    [Inject] private LoadingMenu.Factory _loadingMenuFac;
    [Inject] private PlayMenu.Factory _playMenuFac;
    [Inject] private GameOverMenu.Factory _gameOverMenuFac;
    private SpineData[] _spineData;
    private string[] _dataIds;
    private string _playerDataId;

    // Start is called before the first frame update
    async void Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        Debug.Log("[ProgramLogic] downloading data...");
        //download and cache data
        _dataIds = new[] {"3999", "4000"};
        _playerDataId = "3999";
        _loadingMenuFac.Create()
            .Open();
        _spineData = await _resourceLoader.LoadCharacterData(_dataIds);

        Debug.Log("[ProgramLogic] --> Done.");

        Debug.Log("[ProgramLogic] creating skeleton data asset ...");
        //create spine asset and cache
        _spineData.ToObservable()
            .Subscribe(data => data.SkeletonDataAsset = _resourceCreator.CreateSkeletonDataAsset(data));
        Debug.Log("[ProgramLogic] --> Done.");

        InitGame();

        var playMenu = _playMenuFac.Create();
        playMenu.Open();
        playMenu.OnClickBtnPlayAsObservable()
            .Subscribe(_ =>
            {
                playMenu.Close();
                Play();
            });

        //logic when game over
        _gameStagePresenter.OnGameOverAsObservable()
            .Subscribe(isWin =>
            {
                var _gameOverMenu = _gameOverMenuFac.Create();
                _gameOverMenu.Open(isWin);
                _gameOverMenu.OnClickBtnPlayAsObservable()
                    .Subscribe(_ =>
                    {
                        _gameStagePresenter.Clear();
                        InitGame();
                        Play();
                        _gameOverMenu.Close();
                    });
            });
    }

    private void InitGame()
    {
        //create character views and present the game stage
        Debug.Log("[ProgramLogic] Present ...");
        _gameStagePresenter
            .SetSpineData(_spineData)
            .SetDataIds(_dataIds,
                _playerDataId)
            .SetConfig(new GameConfig
            {
                PlayerCharacterNumber = 5,
                EnemyCharacterNumber = 5
            })
            .Init()
            .UpdateView();
    }

    private void Play()
    {
        //listen input from player
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Space))
            .Subscribe(_ =>
            {
                //attack enemies
                _gameStagePresenter.InvokePlayerAttack();
            });
    }
}