using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    
    public string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public int _synti;
    public int Synti
    {
        get { return _synti; }
        set { _synti = value; }
    }

    public int _box;
    public int Box
    {
        get { return _box; }
        set { _box = value; }
    }

    public int _barrel;
    public int Barrel
    {
        get { return _barrel; }
        set { _barrel = value; }
    }

    public int _skull;
    public int Skull
    {
        get { return _skull; }
        set { _skull = value; }
    }

    void Awake()
    {
        int count = FindObjectsOfType<Objects>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
