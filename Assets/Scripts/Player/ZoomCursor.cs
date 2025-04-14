using Unity.Cinemachine;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ZoomCursor : MonoBehaviour
{
    [SerializeField] PlayerShooter shooter;
    //-- Ÿ�ٱ׷����� --//
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    float zoomRadius = 0.5f;
    [SerializeField]
    float zoomWeight = 1f;
    private void Awake()
    {

    }

    void Update()               // FixedUpdate �� Update �� �̵�
    {
        RefreshCursor();
    }

    private void RefreshCursor()
    {
        Vector3 target = shooter.mouseWorld;

        // ��ġ�� �ٲ� ��쿡�� ���� (���ʿ��� dirty flag ����)
        if ((transform.position - target).sqrMagnitude > 0.0001f)
            transform.position = target;
    }

    private void OnEnable()
    {
        targetGroup.AddMember(this.transform, zoomWeight, zoomRadius);
    }
    private void OnDisable()
    {
        targetGroup.RemoveMember(this.transform);
    }
}
