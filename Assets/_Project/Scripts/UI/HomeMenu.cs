using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    public void SelectMap(string mapSceneName)
    {
        SceneManager.LoadScene(mapSceneName);
    }
}