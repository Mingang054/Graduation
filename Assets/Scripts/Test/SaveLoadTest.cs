using UnityEngine;

/// <summary>
/// SaveLoader 테스트용 스크립트
/// (인스펙터에서 버튼 클릭으로 저장/로드 확인)
/// </summary>
public class SaveLoadTest : MonoBehaviour
{
    [ContextMenu("Save With Timestamp")]
    public void SaveWithTimestamp()
    {
        SaveLoader.SaveWithTimestamp();
    }

    [ContextMenu("Save Checkpoint")]
    public void SaveCheckpoint()
    {
        SaveLoader.SaveCheckpoint();
    }

    [ContextMenu("Load Checkpoint")]
    public void LoadCheckpoint()
    {
        SaveLoader.LoadCheckpoint();
    }
}
