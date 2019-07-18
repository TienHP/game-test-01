using UnityEngine;
using UnityEngine.Events;

public class AttackingLogicStrategy : IAttackingLogicStrategy
{
    private Character _attackerView;
    private Character _targetView;
    private CharacterModel _attackerModel;
    private CharacterModel _targetModel;
    private UnityAction _onCompleted;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private Vector3 GetStartMovingPos()
    {
        return _attackerView.Transform.position;
    }

    private Vector3 GetStopMovingPos()
    {
        return _targetView.Transform.position +
               _targetView.Transform.TransformDirection(Vector3.right) * -2.0f;
    }

    void OnTargetBitten()
    {
        //play hit anim on target
        _targetView.HitByAttack(new BittenParameters
        {
            OnBittenFinished = () => { Debug.Log("[AttackingLogic] --> Finished bitten anim"); }
        });
        //apply dam
        _targetModel.HP.Value -= _attackerModel.Damage;
    }

    void OnAttackingFinished()
    {
        Debug.Log("[GameStagePresenter] -- attacking finished");
        _attackerView.ComeBack(new MovingParameters
        {
            MoveDuration = Constant.GameStagePresenter.MoveDuration,
            StartPos = _endPos,
            TargetPos = _startPos,
            OnActionFinished = OnTargetFightBack
        });
    }

    void OnTargetFightBack()
    {
        var tempView = _attackerView;
        _attackerView = _targetView;
        _targetView = tempView;
        var tempModel = _attackerModel;
        _attackerModel = _targetModel;
        _targetModel = tempModel;
        Attack();
    }


    public IAttackingLogicStrategy SetAttackerView(Character attacker)
    {
        _attackerView = attacker;
        return this;
    }

    public IAttackingLogicStrategy SetTargetView(Character target)
    {
        _targetView = target;
        return this;
    }

    public IAttackingLogicStrategy SetAttackerModel(CharacterModel attacker)
    {
        _attackerModel = attacker;
        return this;
    }

    public IAttackingLogicStrategy SetTargetModel(CharacterModel target)
    {
        _targetModel = target;
        return this;
    }

    public IAttackingLogicStrategy OnCompleted(UnityAction action)
    {
        _onCompleted = action;
        return this;
    }

    public void Attack()
    {
        _startPos = GetStartMovingPos();
        _endPos = GetStopMovingPos();
        var maxSortingOrder = Mathf.Max(_attackerView.GetSortingOrder(),
            _targetView.GetSortingOrder());
        var minSortingOrder = Mathf.Min(_attackerView.GetSortingOrder(),
            _targetView.GetSortingOrder());
        _attackerView.SetSortingOrder(maxSortingOrder);
        _targetView.SetSortingOrder(minSortingOrder);
        Debug.LogWarning($"{_attackerModel.Id} will attack {_targetModel.Id} from pos {_startPos} to pos: {_endPos}");
        //logic when attacking
        _attackerModel.Damage = Random.Range(Constant.CharacterModel.MinDam,
            Constant.CharacterModel.MaxDam + 1);
        _attackerView.Attack(new AttackParameters
        {
            MoveDuration = Constant.GameStagePresenter.MoveDuration,
            StartPos = _startPos,
            TargetPos = _endPos,
            OnTargetBitten = OnTargetBitten,
            OnActionFinished = OnAttackingFinished
        });
    }
}

public interface IAttackingLogicStrategy
{
    IAttackingLogicStrategy SetAttackerView(Character attacker);

    IAttackingLogicStrategy SetTargetView(Character target);

    IAttackingLogicStrategy SetAttackerModel(CharacterModel attacker);

    IAttackingLogicStrategy SetTargetModel(CharacterModel target);

    IAttackingLogicStrategy OnCompleted(UnityAction action);

    void Attack();
}