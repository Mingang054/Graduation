using UnityEngine;
using UnityEngine.SceneManagement;

public class GameExitButton : MonoBehaviour
{

    public void QuitGameWithSave()
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
    public void QuitGameWithNoSave()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
