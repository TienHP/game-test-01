using System.Linq;
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
        var idList = new[] {"3999", "4000"};
        var playerCharacterId = "3999";
        _gameState.SpineData = await _resourceLoader.LoadCharacterData(idList);
        Debug.Log("[ProgramLogic] --> Done.");

        Debug.Log("[ProgramLogic] creating skeleton data asset ...");
        //create spine asset and cache
        _gameState.SpineData.ToObservable()
            .Subscribe(data => data.SkeletonDataAsset = _resourceCreator.CreateSkeletonDataAsset(data));
        Debug.Log("[ProgramLogic] --> Done.");

        Debug.Log("[ProgramLogic] creating model objects ...");
        //init character models
        _gameState.CharacterModels = idList.Select(id =>
            {
                if (id == playerCharacterId)
                {
                    return new CharacterModel
                    {
                        Id = id,
                        HP = new ReactiveProperty<int>(100),
                        Damage = 0,
                        IsPlayer = true
                    };
                }

                return new CharacterModel
                {
                    Id = id,
                    HP = new ReactiveProperty<int>(100),
                    Damage = 0,
                    IsPlayer = false
                };
            })
            .ToArray();

        _gameState.StageModel = new StageModel
        {
            GroundRotationX = 60,
            GroundPositionY = -5.0f,
            SpriteRendererHeight = 15.6f,
            SpriteRendererWidth = 15.6f
        };

        //create character views and present the game stage
        Debug.Log("[ProgramLogic] Present ...");
        _gameStagePresenter.SetCharacterModels(_gameState.CharacterModels)
            .SetSpineData(_gameState.SpineData)
            .SetStageModel(_gameState.StageModel)
            .UpdateView();

        //listen input from player
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Space))
            .Subscribe(_ =>
            {
                //attack enemy
                _gameStagePresenter.InvokePlayerAttack();
            });
    }
}