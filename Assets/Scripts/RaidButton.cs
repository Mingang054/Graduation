using UnityEngine;
using UnityEngine.UI;

public class RaidButton : MonoBehaviour
{
    public string raidName;  // 이동할 레이드 씬 이름

    private Button button;   // 버튼 컴포넌트

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnRaidButtonClicked);
        }
        else
        {
            Debug.LogError("❗ 버튼 컴포넌트를 찾을 수 없습니다.", this);
        }
    }

    private void OnRaidButtonClicked()
    {
        if (string.IsNullOrEmpty(raidName))
        {
            Debug.LogWarning("❗ 레이드 이름이 설정되지 않았습니다.");
            return;
        }

        SceneLoader.instance.GoToRaid(raidName);
        Debug.Log($"🚀 {raidName} 씬으로 이동합니다.");
    }
}
