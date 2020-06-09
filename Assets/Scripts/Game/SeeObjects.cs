using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeObjects : MonoBehaviour
{
    public GameObject chatScreen;
    public GameObject objectsScreen;

    public void OnSeeObjectsButtonClick()
    {
        chatScreen.SetActive(false);
        objectsScreen.SetActive(true);
    }
}
