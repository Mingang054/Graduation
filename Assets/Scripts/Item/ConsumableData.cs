﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Consumable")]
public class ConsumableData : ItemData
{
    //단발성 회복 관련
    public float hp;                          // HP 즉시 회복
    public float sp;                          // SP 즉시 회복
    public float energy;                      // 에너지 즉시 회복
    public float hydration;                  // 수분 즉시 회복

    public bool hemostasis;                   // 지혈 효과 여부
    
    //지속회복 관련
    public float hpRegen;                     // HP 지속 회복
    public float spRegen;                     // SP 지속 회복
    public float energyRegen;                 // 에너지 지속 회복
    public float hydrationRegen;              // 수분 지속 회복
    public float regenDuration;               // 지속 회복 시간

    public bool isMedicine;                         //의료품 여부

    //치료 액션 종류
    public bool isInjector;
    public bool isSplint;
    public bool isTourniquet;
    
    public ConsumableType consumableType;     // 소모품 타입 (의약품, 음식)

    public bool isConsumable;                 // 실제로 소모되는지 여부 (true: 소모됨, false: 무한 사용 가능)
}