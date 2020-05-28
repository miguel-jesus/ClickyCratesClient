using System;
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

    public DateTime _birthday;
    public DateTime BirthDay
    {
        get { return _birthday; }
        set { _birthday = value; }
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

    public bool _isOnline;
    public bool IsOnline
    {
        get { return _isOnline; }
        set { _isOnline = value; }
    }

    public DateTime _lastLogin;
    public DateTime LastLogin
    {
        get { return _lastLogin; }
        set { _lastLogin = value; }
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

    void OnApplicationQuit()
    {
        //Debug.Log("Application ending after " + Time.time + " seconds");
        if (!(String.Equals(this.Token, "")))
        {
            StartCoroutine(Helper.UpdateInfoPlayer(false));
        }
    }
}
