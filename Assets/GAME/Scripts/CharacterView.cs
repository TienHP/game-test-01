using Spine.Unity;
using UnityEngine;

public class CharacterView : MonoBehaviour,
    IView<CharacterModel>
{
    [SerializeField] private SkeletonAnimation _skeletonAnimation;

    public void UpdateView(SkeletonDataAsset skeletonDataAsset)
    {
        _skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
        skeletonDataAsset.Clear();
        skeletonDataAsset.GetSkeletonData(true);
        _skeletonAnimation.Initialize(true);
    }

    public void UpdateView(CharacterModel model)
    {
    }
}