namespace Fusee.Xirkit
{
    public interface IMemberAccessor<T>
    {
        void Set(object o, T val);
        T Get(object o);
    } 
}
