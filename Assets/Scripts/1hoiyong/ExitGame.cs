using UnityEngine;
using UnityEngine.SceneManagement;

public class GameExitButton : MonoBehaviour
{

    public void QuitGame()
    {

        // ���� ������ "Base"�̸� ����
        if (SceneManager.GetActiveScene().name == "Base")
        {
            SaveLoader.SaveWithTimestamp();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
