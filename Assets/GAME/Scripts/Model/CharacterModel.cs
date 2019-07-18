using UniRx;

public struct CharacterModel : IModel
{
    public string Id;
    public ReactiveProperty<int> HP;
    public int Damage;
    public bool IsPlayer;
}


public static partial class Constant
{
    public static class CharacterModel
    {
        public const float ScaleXPlayer = 1.0f;
        public const float RotationYPlayer = 180;
        public const int MaxHP = 100;
        public const int MinDam = 5;
        public const int MaxDam = 10;
    }
}