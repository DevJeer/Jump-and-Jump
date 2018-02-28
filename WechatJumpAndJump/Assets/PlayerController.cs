using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public enum GameState
{
    Begin,
    Playing,
    Over,
    Test
}


public class PlayerController : MonoBehaviour
{
    public GameState state = GameState.Begin;

    private Rigidbody playerRigidbody;

    private float powerTime;

    public float maxPowerValue;

    public GameObject firstCube;

    public GameObject cubePrefab;
    //保存当前的物体
    private GameObject currentCube;

    //得到Player的第一个位置
    private Vector3 firstPlayerPosition;

    public float distance;
    //得到主摄像机
    public Camera mainCamera;
    //生成游戏物体以及障碍的距离
    private Vector3 deltaDistance;
    private Vector3 dir = new Vector3(1, 0, 0);

    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject scoreShow;

    private int scoreCount = 0;
    private int maxScore = 0;

    //得到分数的Text
    public Text playingScoreText;
    public Text nowScoreText;
    public Text maxScoreText;


    void Start()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        scoreShow.SetActive(false);
        firstPlayerPosition = this.transform.position;
        playerRigidbody = this.GetComponent<Rigidbody>();
        currentCube = firstCube;
        deltaDistance = mainCamera.transform.position - this.transform.position;
    }

    void Update()
    {
        if (state != GameState.Playing)
        {
            scoreShow.SetActive(false);
            return;
        }
        //当鼠标按下的时候开始蓄力  鼠标松开的时候 停止蓄力 并且跳跃
        scoreShow.SetActive(true);
        if (Input.GetMouseButton(0) || Input.GetTouch(0).phase == TouchPhase.Began)
        {
            powerTime += Time.deltaTime * 5.0f;
        }
        if (Input.GetMouseButtonUp(0) || Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (powerTime > maxPowerValue)
            {
                powerTime = maxPowerValue;
            }
            PlayerJump();
            powerTime = 0;
        }

    }
    //Player跳跃的方法
    void PlayerJump()
    {
        playerRigidbody.AddForce((dir + new Vector3(0, 1, 0)) * powerTime, ForceMode.Impulse);
    }

    //生成Cube的方法
    void SpwanCube()
    {
        GameObject cube = GameObject.Instantiate(cubePrefab);
        distance = Random.Range(2.0f, 3.5f);
        cube.transform.position = currentCube.transform.position + dir * distance;
    }

    //当物体跳到第二个Cube上面的时候  生成第三个
    void OnCollisionEnter(Collision collision)
    {
        if (state != GameState.Playing)
            return;

        if (currentCube != collision.gameObject)
        {
            if (collision.gameObject.tag == "Ground")
            {
                if (scoreCount > maxScore)
                {
                    maxScore = scoreCount;
                }
                nowScoreText.text = scoreCount.ToString();
                maxScoreText.text = maxScore.ToString();
                //显示游戏结束的Panel
                gameOverPanel.SetActive(true);
                state = GameState.Over;
                currentCube = firstCube;
                return;
            }
            currentCube = collision.gameObject;
            SpwanRandomDir();
            SpwanCube();
            MoveCameraToPlayer();
            //加分  并且显示在上面
            scoreCount++;
            //更新UI
            playingScoreText.text = scoreCount.ToString();
            print(scoreCount);

        }
    }

    //移动主角的镜头
    void MoveCameraToPlayer()
    {
        mainCamera.transform.position = this.transform.position + deltaDistance;

    }

    //随机产生物体生成的方向
    void SpwanRandomDir()
    {
        int a = Random.Range(0, 2);
        dir = a == 0 ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
    }

    //当玩家点击开始游戏的时候  首先切换游戏状态
    public void OnPlayGameButtonClick()
    {
        state = GameState.Playing;
        //隐藏开始游戏界面
        startPanel.SetActive(false);
    }

    public void OnPlayAgainButtonClick()
    {
        //将分数清零
        scoreCount = 0;
        //更新UI
        playingScoreText.text = scoreCount.ToString();
        //将角色重置到原来的位置
        this.transform.position = firstPlayerPosition;
        MoveCameraToPlayer();
        dir = new Vector3(1, 0, 0);
        //删除掉多余的物体
        GameObject[] go = GameObject.FindGameObjectsWithTag("floor");
        foreach (GameObject nowGo in go)
        {
            Destroy(nowGo);
        }
        gameOverPanel.SetActive(false);
        state = GameState.Playing;
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

}
