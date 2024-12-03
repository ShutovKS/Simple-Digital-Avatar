using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private DependencyInstaller dependencyInstaller;

    private void Awake()
    {
        dependencyInstaller.OnDependenciesInstalled += LoadScene;
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}