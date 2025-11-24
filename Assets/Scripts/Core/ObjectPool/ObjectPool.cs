using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour
{
    public GameObject prefab;
    public int amount = 5;

    private List<T> instances;
    private int lastInstanceIndex = 0;

    public List<T> PoolList
    {
        get
        {
            if (instances == null)
                SetInstances();

            return instances;
        }
    }

    private void Awake()
    {
        if (instances != null)
            return;

        SetInstances();
    }

    private void SetInstances()
    {
        GameObject conteiner = new GameObject();
        conteiner.name = "ObjectPool_" + prefab.name;

        instances = new List<T>();

        for (int i = 0; i < amount; i++)
        {
            var obj = Instantiate(prefab, conteiner.transform);
            obj.SetActive(false);
            instances.Add(obj.GetComponent<T>());
        }
    }

    public T GetInstance()
    {
        var instance = instances[lastInstanceIndex];

        lastInstanceIndex++;
        if (lastInstanceIndex >= instances.Count)
            lastInstanceIndex = 0;

        return instance;
    }
}

