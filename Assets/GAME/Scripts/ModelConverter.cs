public abstract class ModelConverter<TSource, TTarget> where TTarget : IModel
    where TSource : IModel
{
    public abstract void Convert(ref TTarget target,
        TSource source);
}