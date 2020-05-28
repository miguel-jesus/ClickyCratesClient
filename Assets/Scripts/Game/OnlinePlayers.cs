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
        StartCoroutine(Refresh());
    }

    private IEnumerator Refresh()
    {
        while (true)
        {
            yield return GetOnlinePlayerInfo(playersOnlineText);
            yield return new WaitForSeconds(3);
        }

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
            TimeSpan timeSpan ;
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"myList\":" + jsonResponse + "}";
            PlayerSerializableList lista = JsonUtility.FromJson<PlayerSerializableList>(response);

            foreach(PlayerSerializable ps in lista.myList)
            {
                int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                int birthDate = int.Parse(Convert.ToDateTime(ps.BirthDay).ToString("yyyyMMdd"));
                int age = (now - birthDate) / 10000;
                DateTime lastLogin = Convert.ToDateTime(ps.LastLogin);
                timeSpan = (DateTime.Now - lastLogin);

                playersOnline += ps.FirstName + " " + "("+age+ ") \n" + timeSpan.Minutes + "' " + timeSpan.Seconds + "'' \n";
            }

            playersOnlineText.text = playersOnline;
        }
        httpClient.Dispose();
    }

   
}
