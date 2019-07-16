public static partial class Constant
{
    public static class CharacterModel
    {
        public const float ScaleXPlayer = -1.0f;
    }
}

public struct CharacterModel : IModel
{
    public string Id;
    public int HP;
    public int Damage;
    public bool IsPlayer;
}