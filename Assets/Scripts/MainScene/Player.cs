using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const string _httpServerAddress = "https://localhost:44302/";
    //private const string _httpServerAddress = "http://localhost:51605/";
    public string HttpServerAddress
    {
        get
        {
            return _httpServerAddress;
        }
    }

    public string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    public string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string _firstName;
    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; }
    }

    public string _lastName;
    public string LastName
    {
        get { return _lastName; }
        set { _lastName = value; }
    }

    public string _nickName;
    public string NickName
    {
        get { return _nickName; }
        set { _nickName = value; }
    }

    public string _city;
    public string City
    {
        get { return _city; }
        set { _city = value; }
    }
    void Awake()
    {
        int count = FindObjectsOfType<Player>().Length;
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
