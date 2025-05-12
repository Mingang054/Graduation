using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadSaveFileButton : MonoBehaviour
{
    public Button loadButton;
    public Button deleteButton;
    public TMP_Text text;

    public string timestamp;  // 예: "2025-05-11_2134"

    private void Awake()
    {
        // 한 번만 리스너 등록
        loadButton.onClick.AddListener(loader);
        deleteButton.onClick.AddListener(deletefile);
    }

    public void loader()
    {
        SceneLoader.instance.GoToBaseFromTitle(timestamp);
    }

    public void deletefile()
    {
        string dir = Path.Combine(Application.persistentDataPath, "SaveData");
        string filePath = Path.Combine(dir, $"{timestamp}.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"🗑️ 삭제 완료: {filePath}");
        }
        else
        {
            Debug.LogWarning($"❗ 삭제 실패: 파일 없음 - {filePath}");
        }

        Destroy(gameObject);  // 버튼 오브젝트 제거
    }
}
