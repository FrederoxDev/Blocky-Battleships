using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public GameObject[] smallCabs;
    public int smallCost = 1;

    public GameObject[] mediumCabs;
    public int mediumCost = 2;

    public int startingRoundBalence = 1;
    public int roundBalance;
    public int roundNumber = 0;
    public int remainingCabs = 0;

    [Header("UI Elements")]
    public TMP_Text roundName;
    public TMP_Text roundDetails;
    public Button startButton;
    public TMP_Text startText;

    private PlayerCab player;
    public bool preventSpawns = false;
    public bool gameOver = false;

    private void Awake()
    {
        roundName.enabled = false;
        roundDetails.enabled = false;
        
        if (Application.isEditor && preventSpawns)
        {
            return;
        }

        roundName.enabled = true;
        roundName.text = "Build your ship!";
        startText.text = "START WAVE";
        startButton.gameObject.SetActive(true);
        player = FindObjectOfType<PlayerCab>();
    }

    public void OnStartButton()
    {
        if (gameOver)
        {
            SceneManager.LoadScene("TitleScreen");
            return;
        }

        startButton.gameObject.SetActive(false);
        StartNextRound();
    }

    private void StartNextRound()
    {
        roundBalance = startingRoundBalence + roundNumber;
        roundNumber++;
        Debug.Log("Round " + roundNumber);

        int numSmallCabs = Random.Range(0, startingRoundBalence / smallCost + 1) + 1;
        int numMediumCabs = Random.Range(0, (startingRoundBalence - numSmallCabs * smallCost) / mediumCost + 1);

        roundName.enabled = true;
        roundDetails.enabled = true;
        roundName.text = $"Round {roundNumber}";
        roundDetails.text = $"{numSmallCabs} Small Cabs, {numMediumCabs} Medium Cabs";

        LeanTween.delayedCall(1f, () =>
        {
            roundName.enabled = false;
            roundDetails.enabled = false;
        });
        
        remainingCabs = numSmallCabs + numMediumCabs;

        for (int i = 0; i < numSmallCabs; i++)
        {
            SpawnEnemy(smallCabs[Random.Range(0, smallCabs.Length)]);
        }

        for (int i = 0; i < numMediumCabs; i++)
        {
            SpawnEnemy(mediumCabs[Random.Range(0, mediumCabs.Length)]);
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Vector2 pos = new Vector2(
            player.transform.position.x + Random.Range(-50, 50), 
            player.transform.position.y + Random.Range(-50, 50)
        );

        GameObject cab = Instantiate(prefab, pos, Quaternion.identity);
        cab.name = "EnemyCab";
    }

    public void ReportDeath()
    {
        remainingCabs--;

        Debug.Log($"{remainingCabs} Cabs Remaining");

        if (remainingCabs <= 0)
        {
            roundName.enabled = true;
            roundDetails.enabled = true;

            Block[] blocks = FindObjectsOfType<Block>();

            foreach(Block block in blocks)
            {
                block.Heal();   
            }

            roundName.text = $"Upgrade your ship!";
            roundDetails.text = $"Healed all Blocks!";
            startButton.gameObject.SetActive(true);
        }
    }

    public void PlayerDied()
    {
        roundName.enabled = true;
        roundName.text = "Game Over!";
        startText.text = "MAIN MENU";
        gameOver = true;
        startButton.gameObject.SetActive(true);
    }
}
