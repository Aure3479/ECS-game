using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class GameUIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private EntityManager em;
    private EntityQuery queryGameOver;
    private EntityQuery queryVictory;

    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);

        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        queryGameOver = em.CreateEntityQuery(typeof(GameOverTag));
        queryVictory = em.CreateEntityQuery(typeof(VictoryTag));
    }

    void Update()
    {
        bool isGameOver = !queryGameOver.IsEmpty;
        if (isGameOver && gameOverPanel && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);
        }

        bool isVictory = !queryVictory.IsEmpty;
        if (isVictory && victoryPanel && !victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(true);
        }
    }

    public void OnClickReplay()
    {
        SceneManager.LoadScene("Demo");
    }
}
