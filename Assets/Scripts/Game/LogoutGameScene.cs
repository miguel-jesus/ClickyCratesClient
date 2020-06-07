using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LogoutGameScene : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnLogoutButtonClicked()
    {
        StartCoroutine(TryLogout());

    }

    private IEnumerator TryLogout()
    {
        yield return Helper.UpdateInfoPlayer(false);
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Account/Logout", "POST");
        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.certificateHandler = new BypassCertificate();
        httpClient.SendWebRequest();
        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Login > TryLogout: " + httpClient.error);
        }
        else
        {
            player.Token = string.Empty;
            player.Id = string.Empty;
            player.FirstName = string.Empty;
            player.LastName = string.Empty;
            player.NickName = string.Empty;
            player.City = string.Empty;
            SceneManager.LoadScene(0);
        }
    }
}
