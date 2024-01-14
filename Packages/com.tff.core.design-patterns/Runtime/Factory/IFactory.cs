namespace TFF.Core.DesignPatterns.Factory
{
    public interface IFactory<out T>
    {
        public T Create();
    }
}