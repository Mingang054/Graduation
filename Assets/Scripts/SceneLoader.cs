using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour
{

    public static SceneLoader instance;

    public void Awake()
    {
       if (instance == null)
       instance = this;
    }

    private void Start()
    {
        SaveLoader.LoadCheckpoint();
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void GoToBaseFromRaid()
    {
        SaveLoader.SaveCheckpoint();
        SceneManager.LoadScene("Base");
    }

    public void GoToBaseFromTitle(string filename)
    {
        // 🔥 1. 파일 복사 (filename.json → checkpoint.json)
        CopySaveFileToCheckpoint(filename);

        // 🔥 2. Base 씬으로 이동
        SceneManager.LoadScene("Base");
    }


    // "PrologueRaid"
    public void GoToRaid(string raidSceneName)
    {
        SaveLoader.SaveCheckpoint();
        SceneManager.LoadScene(raidSceneName);
    }

    // 🔥 파일 복사 기능 추가
    private void CopySaveFileToCheckpoint(string filenameWithoutExtension)
    {
        string saveDir = Path.Combine(Application.persistentDataPath, "SaveData");
        string sourcePath = Path.Combine(saveDir, filenameWithoutExtension + ".json");
        string checkpointPath = Path.Combine(saveDir, "checkpoint.json");

        if (!File.Exists(sourcePath))
        {
            Debug.LogError($"❗ 복사 실패: {sourcePath} 파일이 존재하지 않습니다.");
            return;
        }

        File.Copy(sourcePath, checkpointPath, overwrite: true);
        Debug.Log($"✅ {filenameWithoutExtension}.json → checkpoint.json 복사 완료");
    }
}
