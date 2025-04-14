using Unity.Cinemachine;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ZoomCursor : MonoBehaviour
{
    [SerializeField] PlayerShooter shooter;
    //-- 타겟그룹제어 --//
    [SerializeField]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    float zoomRadius = 0.5f;
    [SerializeField]
    float zoomWeight = 1f;
    private void Awake()
    {

    }

    void Update()               // FixedUpdate → Update 로 이동
    {
        RefreshCursor();
    }

    private void RefreshCursor()
    {
        Vector3 target = shooter.mouseWorld;

        // 위치가 바뀐 경우에만 갱신 (불필요한 dirty flag 방지)
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
