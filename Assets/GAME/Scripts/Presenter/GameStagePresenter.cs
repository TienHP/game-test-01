using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public static partial class Constant
{
    public static class GameStagePresenter
    {
        public const float HPBarHeightOnScreen = 250.0f;
        public const float MoveDuration = 1.0f;
    }
}


public class GameStagePresenter : IGameStagePresenter
{
    [Inject] private IFactory<Character> _characterViewFac;
    [Inject] private IFactory<Stage> _stageFac;
    [Inject] private IFactory<HPBarView> _hpBarViewFac;
    [Inject(Id = Constant.ZenjectId.MainCamera)] private Camera _mainCamera;
    [Inject(Id = Constant.ZenjectId.WorldUIRoot)] private Transform _worldUIRoot;
    [Inject] private IAttackingLogicStrategy _attackingLogic;

    private CharacterModel[] _characterModels;
    private StageModel _stageModel;
    private SpineData[] _spineDataArray;
    private readonly Dictionary<string, Character> _characterViews = new Dictionary<string, Character>();
    private Dictionary<string, HPBarView> _hpBarViews = new Dictionary<string, HPBarView>();

    private Stage _stage;
    private int _maxOrder = 0;
    private CancellationTokenSource _updatingHPBarToken;

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
            InitViews();
        }
    }

    private void InitViews()
    {
        CreateViews();

        //loop: update hp bar position
        _updatingHPBarToken = new CancellationTokenSource();
        UpdateHPBarPosAsync(_updatingHPBarToken.Token)
            .SuppressCancellationThrow();

        //subscribe to reactive properties to update views
        _characterModels.Join(_hpBarViews,
                model => model.Id,
                hpBar => hpBar.Key,
                (model,
                    hpBar) => (model, hpBar.Value))
            .ToObservable()
            .Subscribe(pair =>
            {
                var (model, value) = pair;
                model.HP.Subscribe(hp => value.UpdateView(hp));
            });
        //first push to change views
        _characterModels.ToObservable()
            .Subscribe(model => model.HP = model.HP);
    }

    private async UniTask UpdateHPBarPosAsync(CancellationToken cancellationToken)
    {
        var characterBarPair = _hpBarViews.Join(_characterViews,
            hpBar => hpBar.Key,
            character => character.Key,
            (hpBar,
                character) => new {HpBar = hpBar.Value, Character = character.Value});
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            characterBarPair
                .ToObservable()
                .Subscribe(pair => pair.HpBar.UpdatePos(_worldUIRoot.GetComponent<RectTransform>()
                    .GetWorldPosInsideRectTransformWithSameScreenPosition(_mainCamera,
                        _mainCamera,
                        pair.Character.Transform.position,
                        0.0f,
                        Constant.GameStagePresenter.HPBarHeightOnScreen)));
            await UniTask.Yield();
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

    private void InvokeAttack(IEnumerable<(Character view, CharacterModel model)> attackers,
        IEnumerable<(Character view, CharacterModel model)> targets)
    {
        attackers.ToObservable()
            .Subscribe(attacker =>
            {
                //random an enemy
                var target = targets.ElementAt(Random.Range(0,
                    targets.Count()));

                _attackingLogic.SetAttackerView(attacker.view)
                    .SetAttackerModel(attacker.model)
                    .SetTargetView(target.view)
                    .SetTargetModel(target.model)
                    .Attack();
            });
    }

    private void CreateViews()
    {
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

        var incMax = IncMax();
        characters.ToObservable()
            .Subscribe(characterData =>
            {
                //create and add character view
                var characterView = CreateCharacterView(characterData,
                    incMax);
                _characterViews.Add(characterData.model.Id,
                    characterView);
                //create and add hp bar view
                var hpBarView = _hpBarViewFac.Create();
                _hpBarViews.Add(characterData.model.Id,
                    hpBarView);
            });
    }


    private Character CreateCharacterView((SpineData spineData, CharacterModel model) characterData,
        int sortingOrder)
    {
        var characterView = _characterViewFac.Create();
        //update view by data asset and model
        characterView.UpdateView(characterData.spineData.SkeletonDataAsset);

        //and set the view at right position
        characterView.transform.position = characterData.model.IsPlayer
            ? _stage.PlayerPos
            : _stage.EnemyPos;

        characterView.SetSortingOrder(characterData.model.IsPlayer
            ? sortingOrder + 1
            : sortingOrder);

        //set scale to right value
        var localScale = characterView.transform.localScale;
        localScale.x = characterData.model.IsPlayer
            ? Constant.CharacterModel.ScaleXPlayer
            : 1;
        characterView.Transform.rotation = Quaternion.Euler(0,
            characterData.model.IsPlayer
                ? Constant.CharacterModel.RotationYPlayer
                : 0,
            0);
        characterView.transform.localScale = localScale;
        return characterView;
    }
}