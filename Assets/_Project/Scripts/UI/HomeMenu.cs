using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    [SerializeField] private string mapSceneName = "TestMap01";

    public void SelectMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}