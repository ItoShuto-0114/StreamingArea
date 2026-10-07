using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private List<T> _objects = new List<T>();
    private Queue<T> _queue = new Queue<T>();

    public ObjectPool(T obj, int poolSize, Transform parent)
    {
        Init(obj, poolSize, parent);
    }

    private void Init(T obj, int size, Transform parent)
    {
        for (int i = 0; i < size; i++)
        {
            T instObj = Object.Instantiate(obj, parent);
            _objects.Add(instObj);
            instObj.gameObject.SetActive(false);
            _queue.Enqueue(instObj);
        }
    }
}
