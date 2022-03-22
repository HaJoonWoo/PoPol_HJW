using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : class
{
    private static T instance;
    public static T Instance => instance;

    protected void Awake()
    {
        instance = this as T;
    }
}
