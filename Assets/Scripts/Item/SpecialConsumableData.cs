using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecialConsumable", menuName = "Items/SpecialConsumable")]
public class SpecialConsumableData : ConsumableData
{
    public string specialEffectDescription;  // 특수 효과 설명
    public string triggerCode;               // 트리거 이벤트 코드
}

//이벤트 트리거용