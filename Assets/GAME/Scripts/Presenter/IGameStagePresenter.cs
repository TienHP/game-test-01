public interface IGameStagePresenter : IPresenter
{
    IGameStagePresenter SetCharacterModels(CharacterModel[] characterModels);
    IGameStagePresenter SetSpineData(SpineData[] spineData);
    IGameStagePresenter SetStageModel(StageModel stageModel);
    void UpdateView();
    void InvokePlayerAttack();
}