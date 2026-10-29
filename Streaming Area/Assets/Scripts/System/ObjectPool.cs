using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private const int DEFAULT_POSITION = 999;

    private Queue<T> _queue = new Queue<T>();
    private Transform _parent;
    private T _prefab;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="obj">生成したいオブジェクト</param>
    /// <param name="poolSize">最初にいくつ生成するか</param>
    /// <param name="parent">どのオブジェクトの子として生成するか</param>
    public ObjectPool(T obj, int poolSize, Transform parent)
    {
        _parent = parent;
        _prefab = obj;
        Init(_prefab, poolSize);
    }

    /// <summary>
    /// Poolからオブジェクトを生成
    /// </summary>
    /// <param name="obj">生成したいオブジェクト</param>
    public T SpawnObject(Vector3 spawnPos)
    {
        T instObj = GetObject(_prefab);
        instObj.transform.position = spawnPos;
        instObj.gameObject.SetActive(true);

        return instObj;
    }

    /// <summary>
    /// Poolにオブジェクトを返却
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnObject(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("存在しないオブジェクトが返却されました、指定のオブジェクトが破壊されていないか確認してください");
            return;
        }

        if (!obj.gameObject.activeSelf)
        {
            #if UNITY_EDITOR
            Debug.LogWarning($"{obj.name}はすでに返却されています");
            #endif
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_parent);
        obj.transform.position = new Vector3(DEFAULT_POSITION, DEFAULT_POSITION, DEFAULT_POSITION);
        _queue.Enqueue(obj);
    }

    /// <summary>
    /// Poolの初期化
    /// </summary>
    /// <param name="obj">生成したいオブジェクト</param>
    /// <param name="size"></param>
    private void Init(T obj, int size)
    {
        for (int i = 0; i < size; i++)
        {
            T instObj = Object.Instantiate(obj, _parent);
            instObj.transform.position = new Vector3(DEFAULT_POSITION, DEFAULT_POSITION, DEFAULT_POSITION);
            instObj.gameObject.SetActive(false);
            _queue.Enqueue(instObj);
        }
    }

    /// <summary>
    /// Poolからオブジェクトを取得
    /// </summary>
    /// <param name="instObj"></param>
    /// <returns></returns>
    private T GetObject(T instObj)
    {
        T obj;

        if (_queue.Count > 0)
        {
            obj = _queue.Dequeue();
        }
        else
        {
            obj = Object.Instantiate(instObj, _parent);
            #if UNITY_EDITOR
            Debug.LogWarning("オブジェクトが不足していたため、新規に生成しました");
            #endif
        }

        return obj;
    }
}
