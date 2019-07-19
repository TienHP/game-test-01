using UniRx;
using UnityEngine;
using Zenject;

public class ProgramLogic : MonoBehaviour
{
    [Inject] private ResourceLoader _resourceLoader;
    [Inject] private IGameStagePresenter _gameStagePresenter;
    [Inject] private GameState _gameState;
    [Inject] private ResourceCreator _resourceCreator;

    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("[ProgramLogic] downloading data...");
        //download and cache data
        var dataIds = new[] {"3999", "4000"};
        var playerDataId = "3999";
        _gameState.SpineData = await _resourceLoader.LoadCharacterData(dataIds);
        Debug.Log("[ProgramLogic] --> Done.");

        Debug.Log("[ProgramLogic] creating skeleton data asset ...");
        //create spine asset and cache
        _gameState.SpineData.ToObservable()
            .Subscribe(data => data.SkeletonDataAsset = _resourceCreator.CreateSkeletonDataAsset(data));
        Debug.Log("[ProgramLogic] --> Done.");

        //create character views and present the game stage
        Debug.Log("[ProgramLogic] Present ...");
        _gameStagePresenter
            .SetSpineData(_gameState.SpineData)
            .SetDataIds(dataIds,
                playerDataId)
            .SetConfig(new GameConfig
            {
                PlayerCharacterNumber = 5,
                EnemyCharacterNumber = 5
            })
            .Init()
            .UpdateView();

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