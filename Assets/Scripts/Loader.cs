using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        GameScene,
        Loading,
        MainMenuWindow,
    }
    private static Scene targetScene;
    public static void load(Scene scene)
    {
        SceneManager.LoadScene(Scene.Loading.ToString());
        targetScene = scene;
    }

    public static void LoadTargetScene()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
