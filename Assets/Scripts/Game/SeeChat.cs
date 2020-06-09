using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeChat : MonoBehaviour
{
    public GameObject chatScreen;
    public GameObject objectsScreen;

    public void OnSeeChatButtonClick()
    {
        objectsScreen.SetActive(false);
        chatScreen.SetActive(true);
    }
}
