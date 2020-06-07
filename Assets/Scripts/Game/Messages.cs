using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Messages : MonoBehaviour
{
    public Text messageBoardText;
    public InputField messageInputField;
    void Start()
    {
        StartCoroutine(RefreshGetMessages());
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    private IEnumerator RefreshGetMessages()
    {
        while (true)
        {
            yield return GetMessages(messageBoardText);
        }
    }
    public void onSendButtonClick()
    {
        StartCoroutine(InsertMessage());
        messageInputField.text = string.Empty;
    }

    private IEnumerator InsertMessage()
    {
        Player player = FindObjectOfType<Player>();
        MessagesSerializable messagesSerializable = new MessagesSerializable();
        messagesSerializable.IdPlayer = player.Id;
        messagesSerializable.Messages = messageInputField.text;
        messagesSerializable.MessageHour = DateTime.Now.ToString();
        

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Messages/InsertNewMessage", "POST"))
        {
            string messageData = JsonUtility.ToJson(messagesSerializable);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(messageData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new BypassCertificate();

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Messages > InsertNewMessage: " + httpClient.error);
            }
        }

    }

    internal static IEnumerator GetMessages(Text messageBoardText)
    {
        string messages = "";
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Messages/GetMessages", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Messages > GetMessages: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"messagesList\":" + jsonResponse + "}";
            MessagesSerializableList messagesSerializableList = JsonUtility.FromJson<MessagesSerializableList>(response);

            foreach (MessagesSerializable ms in messagesSerializableList.messagesList)
            {
                DateTime messageHour = Convert.ToDateTime(ms.MessageHour);
                if(player.HourGameScene < messageHour)
                {
                    messages += ms.IdPlayer.Substring(0,3) + ">" + ms.Messages + "\n";
                }
               // Debug.Log(messageHour + "\n" + player.HourGameScene);

            }

            messageBoardText.text = messages;
        }
        httpClient.Dispose();
    }
}
