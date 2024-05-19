namespace TaskManager.App.UWP.DependencyInjection
{
    public interface INamedServiceProvider<T>
    {
        T GetService(string name);
    }
}
