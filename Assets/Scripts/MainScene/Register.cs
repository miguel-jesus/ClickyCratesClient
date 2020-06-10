using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public InputField firstNameInputField;
    public InputField lastNameInputField;
    public InputField birthdateInputField;
    public InputField nickNameInputField;
    public InputField cityInputField;
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public Button registerButton;
    public Text messageBoardText;
    public Player player;

    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterNewUser());
    }

    private IEnumerator RegisterNewUser()
    {
        yield return RegisterUser();
        yield return Helper.InitializeToken(emailInputField.text, passwordInputField.text);  //Sets player.Token
        messageBoardText.text += "\nToken: " + player.Token.Substring(0, 7) + "...";
        yield return Helper.GetPlayerId();  //Sets player.Id
        messageBoardText.text += "\nId: " + player.Id;
        player.FirstName = firstNameInputField.text;
        player.LastName = lastNameInputField.text;
        player.NickName = nickNameInputField.text;
        player.City = cityInputField.text;
        player.BirthDay = DateTime.ParseExact(birthdateInputField.text, "yyyy-MM-dd", null);
        yield return InsertPlayer();
        yield return InsertObjects();
        messageBoardText.text += $"\nPlayer \"{player.FirstName}\" registered.";
        player.Id = string.Empty;
        player.Token = string.Empty;
        player.FirstName = string.Empty;
        player.LastName = string.Empty;
        player.NickName = string.Empty;
        player.City = string.Empty;
    }

    private IEnumerator RegisterUser()
    {
        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Account/Register", "POST");

        AspNetUserRegister newUser = new AspNetUserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);

        httpClient.SetRequestHeader("Content-Type", "application/json");
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            // Debug.Log(httpClient.downloadHandler.text.Substring(95,39));

            var errorMessage = httpClient.downloadHandler.text;
            if (errorMessage.Contains("Info"))
            {
            ErrorSerializable error = JsonUtility.FromJson<ErrorSerializable>(errorMessage);
            Debug.Log(error.ModelState.Info[1]);
            Register.MessageError(error.ModelState.Info[1]);
            }
            else if ("".Equals(emailInputField.text) || "".Equals(passwordInputField.text) || "".Equals(confirmPasswordInputField.text))
            {
                Register.MessageError("Email,Password and Confirm Password are required fields");
            }
            else
            {
                Register.MessageError("Password and Confirm Password not match");
            }
            
            throw new Exception("RegisterUser> Error: " + httpClient.error);
        }

        messageBoardText.text += "\nRegisterUser > Info: " + httpClient.responseCode;

        httpClient.Dispose();
    }

    private IEnumerator InsertPlayer()
    {
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.FirstName = player.FirstName;
        playerSerializable.LastName = player.LastName;
        playerSerializable.BirthDay = player.BirthDay.ToString();
        playerSerializable.NickName = player.NickName;
        playerSerializable.City = player.City;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Player/InsertNewPlayer", "POST"))
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
                throw new Exception("RegisterNewPlayer > InsertPlayer: " + httpClient.error);
            }

            messageBoardText.text += "\nRegisterNewPlayer > InsertPlayer: " + httpClient.responseCode;
        }

    }

    private IEnumerator InsertObjects()
    {
        ObjectsSerializable objectsSerializable = new ObjectsSerializable();
        objectsSerializable.Id = player.Id;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Objects/InsertNewObjects", "POST"))
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
                throw new Exception("RegisterNewPlayer > InsertObjects: " + httpClient.error);
            }

            messageBoardText.text += "\nRegisterNewPlayer > InsertObjects: " + httpClient.responseCode;
        }

    }

    public static void MessageError(string message)
    {
        Text messageBoardText = GameObject.Find("Message Board Text").GetComponent<Text>();
        messageBoardText.text = message;
    }
}
