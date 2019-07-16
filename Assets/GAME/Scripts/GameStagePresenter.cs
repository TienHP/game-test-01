using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IGameStagePresenter : IPresenter
{
    IGameStagePresenter SetCharacterModels(CharacterModel[] characterModels);
    IGameStagePresenter SetSpineData(SpineData[] spineData);
    IGameStagePresenter SetStageModel(StageModel stageModel);
    void UpdateView();
    void InvokePlayerAttack();
}

public class GameStagePresenter : IGameStagePresenter
{
    [Inject] private IFactory<CharacterView> _characterViewFac;
    [Inject] private IFactory<Stage> _stageFac;

    private CharacterModel[] _characterModels;
    private StageModel _stageModel;
    private SpineData[] _spineDataArray;
    private Dictionary<string, CharacterView> _characterViews;
    private Stage _stage;

    private int _maxOrder = 0;

    private int IncMax()
    {
        return ++_maxOrder;
    }


    public IGameStagePresenter SetCharacterModels(CharacterModel[] characterModels)
    {
        _characterModels = characterModels;
        return this;
    }

    public IGameStagePresenter SetSpineData(SpineData[] spineData)
    {
        _spineDataArray = spineData;
        return this;
    }

    public IGameStagePresenter SetStageModel(StageModel model)
    {
        _stageModel = model;
        return this;
    }

    //render the stage
    public void UpdateView()
    {
        if (_stage == null)
        {
            _stage = _stageFac.Create();
            _stage.UpdateView(_stageModel);
        }

        if (_characterViews == null || _characterViews.Count == 0)
        {
            CreateCharacterViews();
        }
    }

    public void InvokePlayerAttack()
    {
        Debug.Log("[GameStagePresenter] --> Invoking player attack ...");
        var playerCharacters = from pair in _characterViews
            join model in _characterModels.Where(model => model.IsPlayer) on pair.Key equals model.Id
            select (pair.Value, model);

        var enemiesCharacters = from pair in _characterViews
            join model in _characterModels.Where(model => !model.IsPlayer) on pair.Key equals model.Id
            select (pair.Value, model);

        InvokeAttack(playerCharacters,
            enemiesCharacters);
    }

    private void InvokeAttack(IEnumerable<(CharacterView view, CharacterModel model)> attackers,
        IEnumerable<(CharacterView view, CharacterModel model)> targets)
    {
        attackers.ToObservable()
            .Subscribe(pair =>
            {
                pair.view.OnAttackAnimStop = () =>
                {
                    Debug.Log($"[GameStagePresenter] --> {pair.model.Id} finish attack!");
                };
                pair.view.Attack();
            });
        targets.ToObservable()
            .Subscribe(pair =>
            {
                pair.view.OnDefenseAnimStop = () =>
                {
                    Debug.Log($"[GameStagePresenter] --> {pair.model.Id} finish defense!");
                };
                pair.view.HitByAttack();
            });
    }

    private void CreateCharacterViews()
    {
        _characterViews = new Dictionary<string, CharacterView>();
        _characterModels.ToObservable()
            .Subscribe(model => Debug.Log($"model id: {model.Id}"));
        _spineDataArray.ToObservable()
            .Subscribe(data => Debug.Log($"data id: {data.Id}"));
        var characters = from model in _characterModels
            join spineData in _spineDataArray on model.Id equals spineData.Id
            select (spineData, model);
        if (!characters.Any())
        {
            Debug.LogWarning("[GameStagePresenter] CreateCharacterViews : No matched Id!");
            return;
        }

        Debug.Log("query is null " + (characters == null));

        characters.ToObservable()
            .Subscribe(characterData =>
            {
                Debug.Log("factory is null " + (_characterViewFac == null));
                var characterView = _characterViewFac.Create();
                Debug.Log("skeleton is null" + (characterData.spineData.SkeletonDataAsset == null));
                characterView.UpdateView(characterData.spineData.SkeletonDataAsset);
                _characterViews.Add(characterData.model.Id,
                    characterView);
                //and set the view at right position
                characterView.transform.position = characterData.model.IsPlayer
                    ? _stage.PlayerPos
                    : _stage.EnemyPos;
                characterView.SetSortingOrder(IncMax());
                //set scale to right value
                var localScale = characterView.transform.localScale;
                localScale.x = characterData.model.IsPlayer
                    ? Constant.CharacterModel.ScaleXPlayer
                    : 1;
                characterView.transform.localScale = localScale;
            });
    }
}