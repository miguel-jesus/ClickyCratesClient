using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OnlinePlayers : MonoBehaviour
{
    public Text playersOnlineText;
    private void Start()
    {
        StartCoroutine(GetOnlinePlayerInfo(playersOnlineText));
    }

    internal static IEnumerator GetOnlinePlayerInfo(Text playersOnlineText)
    {
        string playersOnline = "";
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Player/GetPlayersInfo", "GET");

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
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"myList\":" + jsonResponse + "}";
            PlayerSerializableList lista = JsonUtility.FromJson<PlayerSerializableList>(response);
            foreach(PlayerSerializable ps in lista.myList)
            {
                //Debug.Log(ps.FirstName);
                playersOnline += ps.FirstName + "\n";
            }
            playersOnlineText.text = playersOnline;
        }
        httpClient.Dispose();
    }

   
}
