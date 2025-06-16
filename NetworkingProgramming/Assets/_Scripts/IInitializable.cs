using UnityEngine;

public interface IInitializable<T>
{
    public void Init(T data);
}
