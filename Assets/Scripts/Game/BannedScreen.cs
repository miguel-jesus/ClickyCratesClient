using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BannedScreen : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject bannedScreen;
    public Button sendMessageButton;
    Player player;
    void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(RefreshInfoPlayer());
    }

    public IEnumerator RefreshInfoPlayer()
    {
        while (true)
        {
            yield return Helper.GetPlayerInfo();
            if (player.IsBanned)
            {
                titleScreen.SetActive(false);
                bannedScreen.SetActive(true);
                sendMessageButton.interactable = false;
                break;
            }
        }
    }

    public void OnQuitBannedButtonClick()
    {
        StartCoroutine(Logout());
    }

    public IEnumerator Logout()
    {
        yield return LogoutGameScene.TryLogout();
    }
}
