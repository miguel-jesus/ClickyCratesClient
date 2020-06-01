using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    private float spawnRate = 1.0f;
    public TextMeshProUGUI scoreText;
    private int score;
    public TextMeshProUGUI gameOverText;
    public bool isGameActive;
    public Button restartButton;
    public GameObject titleScreen;
    public Text playerNameText;
    public Text syntiScoreText;
    public Text boxScoreText;
    public Text barrelScoreext;
    public Text skullScoreText;
    private Player player;
    private Objects objects;
    int synti = 0;
    int box = 0;
    int barrel = 0;
    int skull = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        objects = FindObjectOfType<Objects>();
        titleScreen.gameObject.SetActive(true);
        playerNameText.text = player.FirstName.ToString();

        StartCoroutine(ObjectsInfo());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }

    }
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }
    public void GameOver()
    {
       
        gameOverText.gameObject.SetActive(true);
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        
    }
    public void RestartGame()
    { StartCoroutine(UpdateObjectsInfo());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void StartGame(int difficulty)
    {
        spawnRate /= difficulty;
        isGameActive = true;
        StartCoroutine(SpawnTarget());
        score = 0;
        scoreText.text = "Score: " + score;
        UpdateScore(0);
        restartButton.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(false);
    }
    public void UpdateSyntiObjects()
    {
        synti++;
        Debug.Log(synti);
    }

    public void UpdateBarrelObjects()
    {
        barrel++;
        Debug.Log(barrel);
    }

    public void UpdateBoxObjects()
    {
        box++;
        Debug.Log(box);
    }

    public void UpdateSkullObjects()
    {
        skull++;
        Debug.Log(skull);
    }
    private IEnumerator ObjectsInfo()
    {
        yield return Helper.GetObjectsInfo();
        syntiScoreText.text = objects.Synti.ToString();
        boxScoreText.text = objects.Box.ToString();
        barrelScoreext.text = objects.Barrel.ToString();
        skullScoreText.text = objects.Skull.ToString();
    }
    private IEnumerator UpdateObjectsInfo()
    {
        yield return Helper.UpdateInfoObjects(synti,barrel,box,skull);
    }
}
