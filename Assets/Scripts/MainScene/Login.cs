using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton;
    public Button playGameButton;
    public Text messageBoardText;
    public Player player;

    public void OnLoginButtonClicked()
    {
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);
        yield return Helper.GetPlayerInfo();
        //yield return RegistrarLogin();
        messageBoardText.text += "\nWelcome " + player.Id + ". You are logged in!";
        loginButton.interactable = false;
        logoutButton.interactable = true;
        playGameButton.interactable = true;
    }

    private IEnumerator RegistrarLogin()
    {
        LoginModel loginModel = new LoginModel();
        loginModel.Id = player.Id;
        loginModel.Name = player.name;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Login/RegisterLogin", "POST"))
        {
            string bodyJson = JsonUtility.ToJson(loginModel);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);

            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("RegistrarLogin > Error: " + httpClient.error);
            }
            else
            {
                Debug.Log("RegistrarLogin > Info: " + httpClient.responseCode);
            }
        }

    }

}
