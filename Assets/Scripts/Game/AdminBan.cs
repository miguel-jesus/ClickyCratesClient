using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdminBan : MonoBehaviour
{
    PlayerSerializable playerBanned = new PlayerSerializable();
    public InputField messageInputField;
    public Button sendMessageButton;
    public Button banButton;
    public bool isPlayerReadyToSend = false;

    void Start()
    {
        if (Helper.isAdmin)
        {
            sendMessageButton.gameObject.SetActive(false);
            banButton.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onOkButtonClick()
    {
        StartCoroutine(BannPlayer());
    }

    public IEnumerator GetPlayersList(string id)
    {
        id = id.Substring(4, 3);
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Player/GetPlayersList", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("AdminBan > GetPlayersList: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"myList\":" + jsonResponse + "}";
            PlayerSerializableList lista = JsonUtility.FromJson<PlayerSerializableList>(response);

            foreach (PlayerSerializable ps in lista.myList)
            {
                var idPlayer = ps.Id.Substring(0, 3);
                if (id.Equals(idPlayer))
                {
                    playerBanned.Id = ps.Id;
                    playerBanned.FirstName = ps.FirstName;
                    playerBanned.LastName = ps.LastName;
                    playerBanned.BirthDay = ps.BirthDay;
                    playerBanned.NickName = ps.NickName;
                    playerBanned.City = ps.City;
                    playerBanned.IsOnline = ps.IsOnline;
                    playerBanned.LastLogin = ps.LastLogin;
                    playerBanned.HourGameScene = ps.HourGameScene;
                    playerBanned.IsBanned = ps.IsBanned;
                    playerBanned.BannedHour = ps.BannedHour;
                    isPlayerReadyToSend = true;
                }

            }

        }
        httpClient.Dispose();
    }

    public IEnumerator BannPlayer()
    {
        string banCommand = messageInputField.text;
        messageInputField.text = string.Empty;
        if (banCommand.Length < 7)
        {
            banCommand += "       ";
        }
        if (banCommand.Substring(0, 3).Equals("ban"))
        {
            Player player = FindObjectOfType<Player>();
            if (banCommand.Substring(4, 3).Trim().Length > 2)
            {
                yield return GetPlayersList(banCommand);
            }


            if (isPlayerReadyToSend)
            {
                playerBanned.IsBanned = true;
                playerBanned.BannedHour = DateTime.Now.ToString();
                Debug.Log(playerBanned.FirstName);
                //Debug.Log(player.Id.Substring(0, 3));
                //yield return null;
                using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/UpdatePlayer", "POST"))
                {
                    string playerData = JsonUtility.ToJson(playerBanned);

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
                        playerBanned.Id = string.Empty;
                        playerBanned.FirstName = string.Empty;
                        playerBanned.LastName = string.Empty;
                        playerBanned.BirthDay = string.Empty;
                        playerBanned.NickName = string.Empty;
                        playerBanned.City = string.Empty;
                        playerBanned.IsOnline = false;
                        playerBanned.LastLogin = string.Empty;
                        playerBanned.HourGameScene = string.Empty;
                        playerBanned.IsBanned = false;
                        playerBanned.BannedHour = string.Empty;
                        isPlayerReadyToSend = false;
                    }
                    httpClient.Dispose();
                }
            }
            else
            {
                yield return null;
            }

        }
        else
        {
            yield return null;
        }


    }
}
