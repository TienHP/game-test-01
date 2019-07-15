using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IGameStagePresenter : IPresenter
{
    IGameStagePresenter SetCharacterModels(CharacterModel[] characterModels);
    IGameStagePresenter SetSpineData(SpineData[] spineData);
    void UpdateView();
}

public class GameStagePresenter : IGameStagePresenter
{
    private CharacterModel[] _characterModels;
    private SpineData[] _spineDataArray;
    private Dictionary<string, CharacterView> _characterViews;
    [Inject] private IFactory<CharacterView> _characterViewFac;

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

    //render the stage
    public void UpdateView()
    {
        if (_characterViews == null || _characterViews.Count == 0)
        {
            CreateCharacterViews();
        }
    }

    private void CreateCharacterViews()
    {
        _characterViews = new Dictionary<string, CharacterView>();
        _characterModels.ToObservable()
            .Subscribe(model => Debug.Log($"model id: {model.Id}"));
        _spineDataArray.ToObservable()
            .Subscribe(data => Debug.Log($"data id: {data.Id}"));
        var query = from model in _characterModels
            join spineData in _spineDataArray on model.Id equals spineData.Id
            select spineData;
        if (!query.Any())
        {
            Debug.LogWarning("[GameStagePresenter] CreateCharacterViews : No matched Id!");
            return;
        }

        Debug.Log("query is null " + (query == null));

        query.ToObservable()
            .Subscribe(data =>
            {
                Debug.Log("factory is null " + (_characterViewFac == null));
                var characterView = _characterViewFac.Create();
                Debug.Log("skeleton is null" + (data.SkeletonDataAsset == null));
                characterView.UpdateView(data.SkeletonDataAsset);
                _characterViews.Add(data.Id,
                    characterView);
            });
    }
}