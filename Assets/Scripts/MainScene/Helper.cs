using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Helper : MonoBehaviour
{
    public static bool isAdmin = false;
    internal static IEnumerator InitializeToken(string email, string password)
    {
        if (email.Equals("admin@fmail.com"))
        {
            isAdmin = true;
        }
        else
        {
            isAdmin = false;
        }
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "Token", "POST");

            // application/x-www-form-urlencoded
            WWWForm dataToSend = new WWWForm();
            dataToSend.AddField("grant_type", "password");
            dataToSend.AddField("username", email);
            dataToSend.AddField("password", password);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Accept", "application/json");
            httpClient.certificateHandler = new BypassCertificate();

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Helper > InitToken: " + httpClient.error);
            }
            else
            {
                string jsonResponse = httpClient.downloadHandler.text;
                AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);
                player.Token = authToken.access_token;
            }
            httpClient.Dispose();
        }
    }

    internal static IEnumerator GetPlayerId()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Account/UserId", "GET");

        byte[] bodyRaw = Encoding.UTF8.GetBytes("Nothing");
        httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.certificateHandler = new BypassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerId: " + httpClient.error);
        }
        else
        {
            player.Id = httpClient.downloadHandler.text.Replace("\"", "");
        }

        httpClient.Dispose();
    }

    internal static IEnumerator GetPlayerInfo()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Player/GetPlayerInfo", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            PlayerSerializable playerSerializable = JsonUtility.FromJson<PlayerSerializable>(httpClient.downloadHandler.text);
            player.Id = playerSerializable.Id;
            player.FirstName = playerSerializable.FirstName;
            player.LastName = playerSerializable.LastName;
            player.NickName = playerSerializable.NickName;
            player.City = playerSerializable.City;
            player.BirthDay = DateTime.Parse(playerSerializable.BirthDay);
            player.IsOnline = playerSerializable.IsOnline;
            player.IsBanned = playerSerializable.IsBanned;
            if (playerSerializable.IsBanned)
            {
               player.BannedHour = DateTime.Parse(playerSerializable.BannedHour);
            }
            
        }

        httpClient.Dispose();
    }

    internal static IEnumerator UpdateInfoPlayer(bool isOnline,DateTime hourInGame)
    {
        Player player = FindObjectOfType<Player>();
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.FirstName = player.FirstName;
        playerSerializable.LastName = player.LastName;
        playerSerializable.BirthDay = player.BirthDay.ToString();
        playerSerializable.NickName = player.NickName;
        playerSerializable.City = player.City;
        playerSerializable.IsOnline = isOnline;
        if(player.LastLogin == DateTime.MinValue)
        {
            playerSerializable.LastLogin = DateTime.Now.ToString();
            player.LastLogin = DateTime.Now;
        }
        else
        {
            playerSerializable.LastLogin = player.LastLogin.ToString();
        }
        
        playerSerializable.HourGameScene = hourInGame.ToString();
        player.HourGameScene = DateTime.Parse(playerSerializable.HourGameScene);
        playerSerializable.IsBanned = player.IsBanned;
        playerSerializable.BannedHour = player.BannedHour.ToString();



        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/UpdatePlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);

            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);

            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new BypassCertificate();

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new System.Exception("UpdateInfoPlayer > Error: " + httpClient.responseCode + ", Info: " + httpClient.error);
            }
            else
            {
                Debug.Log("UpdateInfoPlayer > Info: " + httpClient.responseCode);
                player.Id = playerSerializable.Id;
                player.FirstName = playerSerializable.FirstName;
                player.LastName = playerSerializable.LastName;
                player.NickName = playerSerializable.NickName;
                player.City = playerSerializable.City;
                player.BirthDay = DateTime.Parse(playerSerializable.BirthDay);
              
            }
            httpClient.Dispose();
        }

    }

    internal static IEnumerator UpdateBan()
    {
        Player player = FindObjectOfType<Player>();
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.FirstName = player.FirstName;
        playerSerializable.LastName = player.LastName;
        playerSerializable.BirthDay = player.BirthDay.ToString();
        playerSerializable.NickName = player.NickName;
        playerSerializable.City = player.City;
        playerSerializable.IsOnline = player.IsOnline;
        playerSerializable.LastLogin = player.LastLogin.ToString();
        playerSerializable.HourGameScene = player.HourGameScene.ToString();
        playerSerializable.IsBanned = false;
        player.IsBanned = false;


        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/UpdatePlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);

            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);

            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new BypassCertificate();

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new System.Exception("UpdateBan > Error: " + httpClient.responseCode + ", Info: " + httpClient.error);
            }
            else
            {
                Debug.Log("UpdateBan > Info: " + httpClient.responseCode);
                player.Id = playerSerializable.Id;
                player.FirstName = playerSerializable.FirstName;
                player.LastName = playerSerializable.LastName;
                player.NickName = playerSerializable.NickName;
                player.City = playerSerializable.City;
                player.BirthDay = DateTime.Parse(playerSerializable.BirthDay);

            }
            httpClient.Dispose();
        }

    }

    /*
     * ______________________________________________________________________________________________
     * 
     * Vamos a crear este bloque de comentario para separar entre las llamadas al Player y al Objects
     * ______________________________________________________________________________________________
     */

    internal static IEnumerator GetObjectsInfo()
    {
        Player player = FindObjectOfType<Player>();
        Objects objects = FindObjectOfType<Objects>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Objects/GetObjectsInfo", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetObjectsInfo: " + httpClient.error);
        }
        else
        {
            ObjectsSerializable objectsSerializable = JsonUtility.FromJson<ObjectsSerializable>(httpClient.downloadHandler.text);
            objects.Id = objectsSerializable.Id;
            objects.Synti = int.Parse(objectsSerializable.Synti);
            objects.Box = int.Parse(objectsSerializable.Box);
            objects.Barrel = int.Parse(objectsSerializable.Barrel);
            objects.Skull = int.Parse(objectsSerializable.Skull);
        }

        httpClient.Dispose();
    }

    internal static IEnumerator UpdateInfoObjects(int synti,int barrel,int box,int skull)
    {
        Player player = FindObjectOfType<Player>();
        Objects objects = FindObjectOfType<Objects>();

        ObjectsSerializable objectsSerializable = new ObjectsSerializable();
        objectsSerializable.Id = objects.Id;
        objectsSerializable.Synti = (objects.Synti + synti).ToString();
        objectsSerializable.Box = (objects.Box + box).ToString();
        objectsSerializable.Barrel = (objects.Barrel + barrel).ToString();
        objectsSerializable.Skull = (objects.Skull + skull).ToString();
     
        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Objects/UpdateObjects", "POST"))
        {
            string objectsData = JsonUtility.ToJson(objectsSerializable);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(objectsData);

            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);

            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new BypassCertificate();

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new System.Exception("UpdateInfoObjects > Error: " + httpClient.responseCode + ", Info: " + httpClient.error);
            }
            else
            {
                Debug.Log("UpdateInfoPlayer > Info: " + httpClient.responseCode);
            }
            httpClient.Dispose();
        }

    }
}
