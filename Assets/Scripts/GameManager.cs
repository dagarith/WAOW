﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameMode
    {
        Battle,
        Tag
    }

    public GameMode gameMode;
    public Timer timer;
    public Countdown countdown;
    public Player bluePlayer;
    public Player redPlayer;
    public Text gameOverText;

    // Tag
    private Player taggedPlayer;
    private static float tagCooloffDuration = 2;
    private static float tagTimer = tagCooloffDuration + 1;
    private int tagWinScore = 0;

    // Battle
    private int battleWinScore = 10;

    // Use this for initialization
    void Start () {
        timer.gameObject.SetActive(false);
        countdown.gameObject.SetActive(true);

        switch (gameMode)
        {
            case GameMode.Battle:
                break;
            case GameMode.Tag:
                {
                    redPlayer.SetScore(3);
                    bluePlayer.SetScore(3);

                    // Tag a random player
                    int random = Random.Range(0, 2);
                    if (random == 0)
                        TagPlayer(redPlayer);
                    else
                        TagPlayer(bluePlayer);
                }
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (gameMode)
        {
            case GameMode.Battle:
                { 
                    // Score
                    Player winner = (bluePlayer.score == battleWinScore) ? bluePlayer : (redPlayer.score == battleWinScore) ? redPlayer : null;
                    if (winner != null)
                        GameOver(winner);
                }
                break;
            case GameMode.Tag:
                {
                    // Score
                    Player winner = (bluePlayer.score == tagWinScore) ? redPlayer : (redPlayer.score == tagWinScore) ? bluePlayer : null;
                    if (winner != null)
                    {
                        timer.Stop();
                        GameOver(winner);
                    }
                    // Timer
                    else
                    {
                        tagTimer += Time.deltaTime;
                        if (countdown.IsExpired())
                            timer.gameObject.SetActive(true);
                        if (timer.IsExpired())
                            GameOver(taggedPlayer.otherPlayer);
                    }
                }
                break;
        }
    }

    public void GameOver(Player winner)
    {
        gameOverText.enabled = true;
        gameOverText.text = "Game Over\n" + winner.color + " Wins!";
        bluePlayer.Stop();
        redPlayer.Stop();
        Invoke("RestartLevel", 5);
    }
    
    public void RestartTagTimer()
    {
        tagTimer = 0;
    }

    public void TagPlayer(Player player)
    {
        if (tagTimer < tagCooloffDuration)
            return;

        if (player != taggedPlayer)
        {
            RestartTagTimer();
            if (taggedPlayer != null)
                taggedPlayer.Glow(false);
            player.Glow(true);
            player.Pulse(tagCooloffDuration);
            taggedPlayer = player;
        }
    }

    public void PlayerDied(Player player)
    {
        switch (gameMode)
        {
            case GameMode.Battle:
                {
                    player.otherPlayer.IncrementScore();
                }
                break;
            case GameMode.Tag:
                {
                    player.DecrementScore();
                }
                break;
        }
    }
    
    void RestartLevel()
    {
        SceneManager.LoadScene("Level_Lava");
    }
}
