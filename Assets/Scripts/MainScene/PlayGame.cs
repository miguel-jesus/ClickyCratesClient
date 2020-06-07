using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    public void OnPlayGameButtonClicked()
    {
        SceneManager.LoadScene(1);
        StartCoroutine(Helper.UpdateInfoPlayer(true, DateTime.Now));
    }
}
