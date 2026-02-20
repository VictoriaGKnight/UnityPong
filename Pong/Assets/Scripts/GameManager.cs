/*
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Win Settings")]
    [SerializeField] private int pointsToWin = 5;

    [Header("Ball Reset")]
    [SerializeField] private BallMovement ball;
    [SerializeField] private float resetDelay = 0.5f;
    [SerializeField] private float serveSpeed = 5f;

    private NetworkVariable<int> leftScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> rightScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> winner = new NetworkVariable<int>(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool isResetting = false; 

    public int LeftScore => leftScore.Value;
    public int RightScore => rightScore.Value;
    public bool GameOver => gameOver.Value;
    public int Winner => winner.Value;

    public void StartGame()
    {
        if (!IsServer) return;

        leftScore.Value = 0;
        rightScore.Value = 0;
        gameOver.Value = false;
        winner.Value = -1;

    
        StartGameUI ui = FindObjectOfType<StartGameUI>();
        if (ui != null) ui.HideButton();

        if (ball == null)
            ball = FindObjectOfType<BallMovement>();

        if (ball != null)
        {
            float y = Random.Range(-0.5f, 0.5f);
            if (Mathf.Abs(y) < 0.2f) y = (y >= 0 ? 0.2f : -0.2f);

            Vector2 dir = new Vector2(1f, y).normalized;

            ball.ResetToCenterAndServe(dir, serveSpeed);
        }
    }

    public void ScoreRightPoint()
    {
        if (!IsServer) return;
        if (gameOver.Value) return;
        if (isResetting) return;

        rightScore.Value++;
        CheckWinCondition();

        if (!gameOver.Value)
            StartCoroutine(ResetBallAfterScore(serveToLeft: false));
    }

    public void ScoreLeftPoint()
    {
        if (!IsServer) return;
        if (gameOver.Value) return;
        if (isResetting) return;

        leftScore.Value++;
        CheckWinCondition();

        if (!gameOver.Value)
            StartCoroutine(ResetBallAfterScore(serveToLeft: true));
    }

    private void CheckWinCondition()
    {
        if (!IsServer) return;
        if (gameOver.Value) return;

        if (leftScore.Value >= pointsToWin)
        {
            gameOver.Value = true;
            winner.Value = 0;
            StopBallOnServer();
        }
        else if (rightScore.Value >= pointsToWin)
        {
            gameOver.Value = true;
            winner.Value = 1;
            StopBallOnServer();
        }
    }

    private IEnumerator ResetBallAfterScore(bool serveToLeft)
    {
        isResetting = true;

        if (ball == null)
            ball = FindObjectOfType<BallMovement>();

        if (ball == null)
        {
            Debug.LogError("GameManager: Ball reference missing!");
            isResetting = false;
            yield break;
        }

        ball.ResetToCenterAndServe(Vector2.zero, 0f);

        yield return new WaitForSeconds(resetDelay);

        float y = Random.Range(-0.5f, 0.5f);

        if (Mathf.Abs(y) < 0.2f)
            y = Mathf.Sign(y == 0 ? 1 : y) * 0.2f;

        Vector2 dir = serveToLeft
            ? new Vector2(-1f, y).normalized
            : new Vector2(1f, y).normalized;

        ball.ResetToCenterAndServe(dir, serveSpeed);

        isResetting = false;
    }

    private void StopBallOnServer()
    {
        if (!IsServer) return;

        if (ball == null)
            ball = FindObjectOfType<BallMovement>();

        if (ball != null)
            ball.StopBall();
    }
}

*/

using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int pointsToWin = 5;
    [SerializeField] private float serveSpeed = 5f;

    [Header("References")]
    [SerializeField] private BallMovement ball;

    private NetworkVariable<int> leftScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> rightScore = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> winner = new NetworkVariable<int>(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int LeftScore => leftScore.Value;
    public int RightScore => rightScore.Value;
    public bool GameOver => gameOver.Value;
    public bool GameStarted => gameStarted.Value;
    public int Winner => winner.Value;

    public void StartGame()
    {
        if (!IsServer) return;

        leftScore.Value = 0;
        rightScore.Value = 0;
        gameOver.Value = false;
        winner.Value = -1;
        gameStarted.Value = true;

        if (ball == null) ball = FindObjectOfType<BallMovement>();

        float y = Random.Range(-0.5f, 0.5f);
        if (Mathf.Abs(y) < 0.2f) y = (y >= 0 ? 0.2f : -0.2f);

        Vector2 dir = new Vector2(1f, y).normalized;
        ball.ResetToCenterAndServe(dir, serveSpeed);
    }

    public void ScoreRightPoint()
    {
        if (!IsServer) return;
        if (!gameStarted.Value) return;
        if (gameOver.Value) return;

        rightScore.Value++;
        CheckWinCondition();

        if (!gameOver.Value)
        {
            ServeTowardRightScorer();
        }
    }

    public void ScoreLeftPoint()
    {
        if (!IsServer) return;
        if (!gameStarted.Value) return;
        if (gameOver.Value) return;

        leftScore.Value++;
        CheckWinCondition();

        if (!gameOver.Value)
        {
            ServeTowardLeftScorer();
        }
    }

    private void ServeTowardLeftScorer()
    {
        if (ball == null) ball = FindObjectOfType<BallMovement>();

        float y = Random.Range(-0.5f, 0.5f);
        if (Mathf.Abs(y) < 0.2f) y = (y >= 0 ? 0.2f : -0.2f);

        Vector2 dir = new Vector2(-1f, y).normalized; 
        ball.ResetToCenterAndServe(dir, serveSpeed);
    }

    private void ServeTowardRightScorer()
    {
        if (ball == null) ball = FindObjectOfType<BallMovement>();

        float y = Random.Range(-0.5f, 0.5f);
        if (Mathf.Abs(y) < 0.2f) y = (y >= 0 ? 0.2f : -0.2f);

        Vector2 dir = new Vector2(1f, y).normalized; 
        ball.ResetToCenterAndServe(dir, serveSpeed);
    }

    private void CheckWinCondition()
    {
        if (!IsServer) return;
        if (gameOver.Value) return;

        if (leftScore.Value >= pointsToWin)
        {
            gameOver.Value = true;
            winner.Value = 0;
            ball.StopBall();
        }
        else if (rightScore.Value >= pointsToWin)
        {
            gameOver.Value = true;
            winner.Value = 1;
            ball.StopBall();
        }
    }
}