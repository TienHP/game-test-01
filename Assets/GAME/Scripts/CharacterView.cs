using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public static partial class Constant
{
    public static class CharacterView
    {
        public const string AttackByRanged = "attack/ranged/cast-fly";
        public const string DefenseByRanged = "defense/hit-by-ranged-attack";
    }
}

public class CharacterView : MonoBehaviour,
    IView<CharacterModel>
{
    [SerializeField] private SkeletonAnimation _skeletonAnimation;
    public UnityAction OnAttackAnimStop { get; set; }
    public UnityAction OnDefenseAnimStop { get; set; }

    private void Start()
    {
        _skeletonAnimation.AnimationState.Complete += entry =>
        {
            Debug.Log($"[CharacterView] --> finish animation : {entry.animation.name}");
            if (entry.animation.name.Equals(Constant.CharacterView.AttackByRanged))
            {
                OnAttackAnimStop?.Invoke();
            }
            else if (entry.animation.name.Equals(Constant.CharacterView.DefenseByRanged))
            {
                OnDefenseAnimStop?.Invoke();
            }
        };
        _skeletonAnimation.AnimationState.Start += entry => { };
    }

    public void UpdateView(SkeletonDataAsset skeletonDataAsset)
    {
        _skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
        skeletonDataAsset.Clear();
        skeletonDataAsset.GetSkeletonData(true);
        _skeletonAnimation.Initialize(true);
    }

    public void Attack(float moveDuration = 1.0f)
    {
        
        _skeletonAnimation.AnimationState.AddAnimation(0,
            Constant.CharacterView.AttackByRanged,
            false,
            0.0f);
    }

    public void HitByAttack()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.DefenseByRanged,
            false);
    }

    public void SetSortingOrder(int order)
    {
        GetComponent<Renderer>()
            .sortingOrder = order;
    }

    public void UpdateView(CharacterModel model)
    {
    }
}