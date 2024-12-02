using UnityEngine;



public abstract class AActions<T> : MonoBehaviour
{

    public abstract void Repair(int stat, T value);
    public abstract void Broken(int stat, T value, int damage);
    public abstract void ActionImplementation(int stat, T value);

}