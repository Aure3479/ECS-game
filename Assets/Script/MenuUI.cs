using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Scenes;
using Unity.Entities;

public class MenuUI : MonoBehaviour
{
    public string subSceneName = "1stsubscne";

    public async void OnClickStartGame()
    {
        // Charge la scène "Demo" de manière asynchrone
        var asyncOperation = SceneManager.LoadSceneAsync("Demo", LoadSceneMode.Single);
        await asyncOperation;

        // Attend que la subscene soit chargée
        var world = World.DefaultGameObjectInjectionWorld;
        var sceneSystem = world.GetExistingSystem<SceneSystem>();
        var sceneEntity = SceneSystem.GetSceneEntity(world.Unmanaged, new Unity.Entities.Hash128 (subSceneName));

    }
}
