using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton;
    public Button playGameButton;
    public Text messageBoardText;
    Player player;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        //Debug.Log(player.Token);
        if (!(String.Equals(player.Token, "")))
        {
            loginButton.interactable = false;
            logoutButton.interactable = true;
            playGameButton.interactable = true;
        }
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
       
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);
        yield return Helper.GetPlayerInfo(); 
        Debug.Log(player.IsBanned);
        if (player.IsBanned)
        {
            TimeSpan timeSpan = DateTime.Now - player.BannedHour;
            if (timeSpan.TotalMinutes > 10)
            {
                yield return Helper.UpdateBan();
                yield return Helper.UpdateInfoPlayer(true, DateTime.MinValue);
                messageBoardText.text += "\nWelcome " + player.FirstName + ". You are logged in!";
                loginButton.interactable = false;
                logoutButton.interactable = true;
                playGameButton.interactable = true;
            }
            else
            {
                yield return Logout.TryLogout(messageBoardText, loginButton, logoutButton, playGameButton);
            }
           
        }
        else
        {
            yield return Helper.UpdateInfoPlayer(true,DateTime.MinValue);
            messageBoardText.text += "\nWelcome " + player.FirstName + ". You are logged in!";
            loginButton.interactable = false;
            logoutButton.interactable = true;
            playGameButton.interactable = true;
        }
        
    }
   


}
