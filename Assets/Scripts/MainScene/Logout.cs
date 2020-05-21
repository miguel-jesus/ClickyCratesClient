using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Logout : MonoBehaviour
{
    public Button loginButton;
    public Button logoutButton;
    public Button playGameButton;
    public Text messageBoardText;
    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnLogoutButtonClicked()
    {
        TryLogout();
    }

    private void TryLogout()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Account/Logout", "POST");
        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
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
            messageBoardText.text += $"\n{httpClient.responseCode} Bye bye {player.Id}.";
            loginButton.interactable = true;
            logoutButton.interactable = false;
            playGameButton.interactable = false;
        }
    }

}
