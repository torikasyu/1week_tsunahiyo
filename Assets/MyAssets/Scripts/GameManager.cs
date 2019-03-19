using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : torikasyu.SingletonMonoBehaviour<GameManager>
{
    public AudioClip[] clips;

    AudioSource audio;
    public int TopLimitPos { get; set; } = 4;
    public int BottomLimitPos { get; set; } = -4;

    public float RightLimitPos { get; set; } = 10f;
    public float LefLimitPos { get; set; } = -10f;

    public float EnemyBlockSpeed { get; set; } = 2f;
    public float ShootBlockSpeed { get; set; } = 15f;
    public float SpawnDistance { get; set; } = 5f;

    public GameObject enemyA;
    public GameObject DummyBlock;
    public GameObject BlockParent;
    public GameObject wallPrefab;
    public bool canShoot = true;
    bool canSpawn = false;
    float distance = -1f;
    float distanceWall = 1.0f;

    //public Text ScoreText;
    public TMPro.TextMeshProUGUI ScoreText;
    public TMPro.TextMeshProUGUI HighScoreText;
    public TMPro.TextMeshProUGUI LevelText;
    public TMPro.TextMeshProUGUI InfoText;
    public Button TweetButton;
    int score = 0;
    int level = 1;
    int exp = 0;

    enum enumGameState
    {
        None,
        WaitforStart,
        Playing,
        GameOver
    }

    enumGameState gameState;
    enumGameState nextGameState;

    void Start()
    {
        gameState = enumGameState.None;
        nextGameState = enumGameState.WaitforStart;
        audio = GetComponent<AudioSource>();
        audio.Play();

    }

    public void PlaySound(int type)
    {
        audio.PlayOneShot(clips[type]);
    }

    int highScore;
    void updateHighScore()
    {
        try
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        catch
        {
            highScore = 0;
        }
    }

    public void OnClick_Tweet()
    {
        //本文＋ハッシュタグ＊２ツイート
        naichilab.UnityRoomTweet.Tweet("tsunahiyo", string.Format("つなひよでレベル{0}/{1}点達成！", level, score), "tsunahiyo", "unity1week");
    }

    bool isRunning = false;
    public void AddScore(int increment)
    {
        PlaySound(1);
        score += increment * level * 10;

        exp++;
        if (exp % 3 == 0)
        {
            level++;
            //SpawnDistance -= 1f;
            EnemyBlockSpeed += 0.15f;
        }

        ScoreText.text = "SCORE<br>" + score.ToString();
        LevelText.text = "LEVEL<br>" + level.ToString();

        if (!isRunning)
        {
            float currentSpeed = EnemyBlockSpeed;
            EnemyBlockSpeed = 0;
            StartCoroutine(SetSpeed(currentSpeed));
            //isRunning = false;
        }
    }

    IEnumerator SetSpeed(float speed)
    {
        //if (isRunning) { yield break; }
        isRunning = true;

        yield return new WaitForSeconds(1f);
        EnemyBlockSpeed = speed;
        isRunning = false;
    }

    public void GameOverProcess()
    {
        nextGameState = enumGameState.GameOver;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case enumGameState.WaitforStart:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    nextGameState = enumGameState.Playing;
                }
                break;
            case enumGameState.Playing:
                BlockSpawn();
                break;
            case enumGameState.GameOver:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    //nextGameState = enumGameState.WaitforStart;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                }
                break;
            default:
                break;
        }

        switch (nextGameState)
        {
            case enumGameState.WaitforStart:
                updateHighScore();
                score = 0;
                level = 1;
                //EnemyBlockSpeed = 1.0f;

                TweetButton.gameObject.SetActive(false);
                InfoText.gameObject.SetActive(true);
                InfoText.text = "<color=yellow>TSUNAHIYO</color><br><br>HIT ENTER KEY TO START<br><br>UP / DOWN<br>SPACE : Shoot";
                ScoreText.text = "SCORE<br>" + score.ToString();
                LevelText.text = "LEVEL<br>" + level.ToString();
                HighScoreText.text = "<color=red>HIGH<br> SCORE</color><br>" + highScore.ToString();
                break;
            case enumGameState.Playing:
                InfoText.gameObject.SetActive(false);
                score = 0;
                level = 1;

                break;
            case enumGameState.GameOver:
                PlaySound(2);
                string highScroreMsg = "GAME OVER";
                //int high = PlayerPrefs.GetInt("highscore");
                if (highScore < score)
                {
                    PlayerPrefs.SetInt("highscore", score);
                    PlayerPrefs.Save();
                    highScroreMsg = "<color=red>HIGH SCORE</color>";
                }
                EnemyBlockSpeed = 0;
                TweetButton.gameObject.SetActive(true);
                InfoText.gameObject.SetActive(true);
                InfoText.text = highScroreMsg + "<br>HIT ENTER KEY";

                // Type == Number の場合
                naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
                break;
            default:
                break;
        }

        if (nextGameState != enumGameState.None)
        {
            gameState = nextGameState;
            nextGameState = enumGameState.None;
        }
    }

    int dummyCountMax = 3;
    int dummyCountMin = 0;

    bool CheckDict(Dictionary<int, int> d)
    {
        if (level == 1)
        {
            dummyCountMax = 1;
            dummyCountMin = 1;
        }
        else if (level == 2)
        {
            dummyCountMax = 2;
            dummyCountMin = 2;
        }
        else if (level < 5)
        {
            dummyCountMax = 3;
            dummyCountMin = 3;
        }
        else if (level < 10)
        {
            dummyCountMax = 4;
            dummyCountMin = 1;
        }
        else if (level < 20)
        {
            dummyCountMax = 3;
            dummyCountMin = 1;
        }
        else
        {
            dummyCountMax = 3;
            dummyCountMin = 0;
        }

        int cnt = 0;
        foreach (KeyValuePair<int, int> p in d)
        {
            if (p.Value == 0) cnt++;
        }

        return dummyCountMin <= cnt && cnt <= dummyCountMax;
    }

    void BlockSpawn()
    {
        if (canSpawn)
        {
            canSpawn = false;

            var parentObj = Instantiate(BlockParent, new Vector3(RightLimitPos, 0, 0), Quaternion.identity);
            int LuckCount = 0;

            Dictionary<int, int> blocks;
            do
            {
                blocks = new Dictionary<int, int>();
                for (int i = BottomLimitPos; i <= TopLimitPos; i++)
                {
                    blocks.Add(i, Random.Range(0, 10) > 1 ? 1 : 0);
                }
            }
            while (!CheckDict(blocks));

            /*
            List<int> numbers = new List<int>();
            Dictionary<int, int> blocks = new Dictionary<int, int>();

            for (int i = BottomLimitPos; i <= TopLimitPos; i++)
            {
                blocks.Add(i, 1);
                numbers.Add(i);
            }

            while (numbers.Count > 2)
            {
                int index = Random.Range(0, numbers.Count);
                int ransu = numbers[index];
                Debug.Log(ransu);
                blocks[ransu] = 0;
                numbers.RemoveAt(index);
            }
            */

            foreach (KeyValuePair<int, int> p in blocks)
            {
                if (p.Value == 1)
                {
                    Instantiate(enemyA, new Vector3(RightLimitPos, p.Key, 0), Quaternion.identity).transform.SetParent(parentObj.transform);
                }
                else
                {
                    LuckCount++;
                    Instantiate(DummyBlock, new Vector3(RightLimitPos, p.Key, 0), Quaternion.identity).transform.SetParent(parentObj.transform);
                }
            }

            /*
            for (int i = BottomLimitPos; i <= TopLimitPos; i++)
            {
                if (Random.Range(0, 10) > 1)
                {
                    Instantiate(enemyA, new Vector3(RightLimitPos, i, 0), Quaternion.identity).transform.SetParent(parentObj.transform);
                }
                else
                {
                    LuckCount++;
                    Instantiate(DummyBlock, new Vector3(RightLimitPos, i, 0), Quaternion.identity).transform.SetParent(parentObj.transform);
                }
            }
            */

            if (LuckCount == 0)
            {
                Destroy(parentObj);
            }
            else
            {
                parentObj.GetComponent<BlockParent>().LuckCount = LuckCount;
            }

        }

        if (!canSpawn)
        {
            distance += EnemyBlockSpeed * Time.deltaTime;
            //print(distance);
            if (distance >= SpawnDistance)
            {
                canSpawn = true;
                distance = 0;
            }
        }
    }
}
