using UnityEngine;

/// <summary>
/// SaveLoader �׽�Ʈ�� ��ũ��Ʈ
/// (�ν����Ϳ��� ��ư Ŭ������ ����/�ε� Ȯ��)
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
