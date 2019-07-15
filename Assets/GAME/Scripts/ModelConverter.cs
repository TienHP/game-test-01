public abstract class ModelConverter<TSource, TTarget> where TTarget : IModel
{
    public abstract TTarget GetModel(TSource source);
}