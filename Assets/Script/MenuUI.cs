using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        // Charge la sc�ne "GameScene"
        SceneManager.LoadScene("Demo", LoadSceneMode.Single);

    }
}
