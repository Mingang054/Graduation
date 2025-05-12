using System.IO;
using UnityEngine;

public class LoadSaveFileUI : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public Transform buttonParent;

    private void Awake()
    {
        string dir = Path.Combine(Application.persistentDataPath, "SaveData");

        if (!Directory.Exists(dir)) return;

        string[] files = Directory.GetFiles(dir, "*.json");

        foreach (string filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath); // 예: 2025-05-11_2134

            GameObject buttonGO = Instantiate(ButtonPrefab, buttonParent);
            var btn = buttonGO.GetComponent<LoadSaveFileButton>();

            btn.text.text = fileName;
            btn.timestamp = fileName;

            // 버튼 이벤트 바인딩
            btn.loadButton.onClick.AddListener(() => btn.loader());
            btn.deleteButton.onClick.AddListener(() =>
            {
                File.Delete(filePath);
                Destroy(buttonGO);
            });
        }
    }
}
