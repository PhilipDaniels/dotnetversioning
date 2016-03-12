namespace SetVersion.Lib
{
    public interface IVariableResolver
    {
        string Resolve(string expression, params int?[] currentValues);
    }
}
