﻿using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInstanceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    public Image itemImage;
    public Text itemCountText;

    public ItemInstance itemInstance;
    private RectTransform rectTransform;
    private Canvas canvas;

    // 드래그 전 상태
    private Transform originalParent;
    private Vector2Int originLocation;
    private Vector2 originalPosition;

    private float Cell => BagInventoryManager.Instance.CellSize;
    private void OnEnable()
    {
        if (itemInstance != null && itemInstance.data!=null) 
        {
            itemImage.enabled = true;
            itemImage.sprite = itemInstance.data.itemSprite; }
        UpdateUI();
    }
    private void Awake()
    {
        itemInstance = null;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (itemInstance != null)
        {
            UpdateUI();
            UpdateSize();
        }
    }

    // init 기능 일부 ItemUIPoolManager로 이관
    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
        UpdateUI();
    }

    public void UpdateUI()
    {

        if (itemInstance != null)
        {
            this.gameObject.SetActive(true);
            UpdateSize();
            if (itemInstance.count <= 0) { 
                RemoveItemOnUI();
                ItemUIPoolManager.Instance.ReturnItemUI(this.gameObject);
            }
            if (GetComponentInParent<HealSlotUI>() != null)
            {

                itemImage.sprite = itemInstance.data.itemSprite;
                itemImage.enabled = true;
                // 현재 오브젝트의 부모 Transform을 직접 얻습니다.
                if (transform.parent == null)
                    return;

                // 부모와 자식의 RectTransform을 가져옵니다.
                RectTransform parentRect = transform.parent as RectTransform;
                RectTransform selfRect = transform as RectTransform;
                if (parentRect == null || selfRect == null)
                    return;
                // 1. 부모의 중앙 기준으로 정렬하도록 anchor와 pivot을 중앙으로 설정합니다.
                selfRect.anchorMin = new Vector2(0.5f, 0.5f);
                selfRect.anchorMax = new Vector2(0.5f, 0.5f);
                selfRect.pivot = new Vector2(0.5f, 0.5f);
                // 2. 부모의 중앙으로 실제 위치 이동
                selfRect.anchoredPosition = Vector2.zero;
                Debug.Log("heal");
            }
            else if(itemInstance.currentEquipSlotType == EquipSlotType.none) {
                itemImage.sprite = itemInstance.data.itemSprite;


                // 부모와 자식의 RectTransform을 가져옵니다.
                RectTransform parentRect = transform.parent as RectTransform;
                RectTransform selfRect = transform as RectTransform;
                if (parentRect == null || selfRect == null)
                    return;
                // 1. 부모의 중앙 기준으로 정렬하도록 anchor와 pivot을 중앙으로 설정합니다.
                // 앵커: 좌측 상단 (0,1)
                selfRect.anchorMin = new Vector2(0f, 1f);
                selfRect.anchorMax = new Vector2(0f, 1f);
                // 피봇: 좌측 상단 (0,1)
                selfRect.pivot = new Vector2(0f, 1f);

                UpdatePosition(itemInstance.location);
                UpdateSize();

                itemImage.enabled = true;
                itemCountText.text = itemInstance.count > 1 ? itemInstance.count.ToString() : "";
                Debug.Log("item");
            }
            else
            {
                itemImage.sprite = itemInstance.data.itemSprite;
                itemImage.enabled = true;
                // 현재 오브젝트의 부모 Transform을 직접 얻습니다.
                if (transform.parent == null)
                    return;

                // 부모와 자식의 RectTransform을 가져옵니다.
                RectTransform parentRect = transform.parent as RectTransform;
                RectTransform selfRect = transform as RectTransform;
                if (parentRect == null || selfRect == null)
                    return;
                // 1. 부모의 중앙 기준으로 정렬하도록 anchor와 pivot을 중앙으로 설정합니다.
                selfRect.anchorMin = new Vector2(0.5f, 0.5f);
                selfRect.anchorMax = new Vector2(0.5f, 0.5f);
                selfRect.pivot = new Vector2(0.5f, 0.5f);
                // 2. 부모의 중앙으로 실제 위치 이동
                selfRect.anchoredPosition = Vector2.zero;
            }
        }
        else
        {   //UI풀로 Return 등에 적용
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemCountText.text = "";
        }


    }

    public void UpdatePosition(Vector2Int location)
    {
        if (rectTransform == null) return;
        // 1칸=Cell, (1,1) => anchoredPosition=(0,0) 방식
        rectTransform.anchoredPosition =
            new Vector2(location.x * Cell - Cell, -location.y * Cell + Cell);
    }

    public void UpdateSize()
    {
        if (rectTransform == null || itemInstance == null) return;
        rectTransform.sizeDelta =
            new Vector2(itemInstance.data.size.x * Cell, itemInstance.data.size.y * Cell);
    }
    //============ 드래그 & 드롭 기능 ============//

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            // 🔧 canvas가 null이면 한 번 찾아줌
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("[OnBeginDrag] canvas를 찾을 수 없습니다!");
                    return;
                }
            }

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            Debug.Log($"[OnBeginDrag] 드래그 시작: {itemInstance?.data.itemName ?? "null"} (현재 위치: {itemInstance?.location})");

            // 1) 드래그 전 상태 저장
            originalParent = transform.parent;
            originLocation = itemInstance.location;
            originalPosition = rectTransform.anchoredPosition;
            // 1) 피벗을 오른쪽‑아래로
            rectTransform.pivot = new Vector2(1f, 0f);

            // 2) 마우스 위치 → 월드 좌표
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                    out Vector3 globalMousePos))
            {

                globalMousePos.z = 0f;         // ★ 추가
                Debug.Log(Cell);
                // 3) 셀 절반만큼 보정 (x = -Cell/2, y = +Cell/2)
                Vector3 offset = new Vector3(Cell * 0.5f, -Cell * 0.5f, 0f);
                rectTransform.position = globalMousePos + offset;
            }

            // 4) 부모를 최상단 Canvas로 옮기고 Raycast 차단
            rectTransform.SetParent(canvas.transform, true);
            canvasGroup.blocksRaycasts = false;
        }

    }


    public void OnDrag(PointerEventData eventData)
    {
        // 기존처럼 delta 기반으로 UI 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //Debug.Log($"[OnDrag] 드래그 중... 마우스 위치: {eventData.position}, " +
        //          $"UI 위치: {rectTransform.anchoredPosition}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[OnEndDrag] 드래그 종료: {itemInstance.data.itemName}");

        // 다시 RaycastTarget 활성화

        //Raycast실시(25-04-01 수정) 마지막으로 Hover한 좌료 재계산

        GameObject foundObject = eventData.pointerCurrentRaycast.gameObject;

        
        if (foundObject != null) {
            SlotUI foundSlotUI = foundObject.GetComponent<SlotUI>();
            if (foundSlotUI != null)
            {
                BagInventoryManager.Instance.currentPointedSlot = foundSlotUI.location;
                BagInventoryManager.Instance.currentPointedSlotIsMySlot = foundSlotUI.isMySlot;

                Vector2Int targetSlotLocation = CaculateTargetSlot();
                bool isTargetMySlot = BagInventoryManager.Instance.currentPointedSlotIsMySlot;

                //사전에 위치 해제
                if (itemInstance.currentEquipSlotUI == null)
                BagInventoryManager.Instance.FreeItemSlots(itemInstance);

                if (CanPlaceItem(targetSlotLocation, isTargetMySlot))
                {
                    //배치 가능할 때(배치되는 위치가 EquipSlot이 아닐때) 배치 후 currentEquipSlot을 none으로 초기화
                    PlaceItem(targetSlotLocation, isTargetMySlot);
                    itemInstance.currentEquipSlotType = EquipSlotType.none;
                    Debug.Log($"[OnEndDrag] 배치 성공");
                }
                else
                {
                    // 배치에 실패하였고 ItemSlot에 있던 것이 아니라면
                    if (itemInstance.currentEquipSlotUI == null)
                    {
                        
                        if (BagInventoryManager.Instance.myItems.Contains(itemInstance))
                        {
                            BagInventoryManager.Instance.OccupySlots(originLocation, itemInstance.data.size, BagInventoryManager.Instance.mySlots);
                        }
                        else if (BagInventoryManager.Instance.opponentItems.Contains(itemInstance))
                        {
                            BagInventoryManager.Instance.OccupySlots(originLocation, itemInstance.data.size, BagInventoryManager.Instance.opponentSlots);
                        }
                    }
                    //원래 위치로 복귀??????????????
                    //if (itemInstance.currentEquipSlotType == EquipSlotType.none) {
                        //원래 위치가 착용구간이 아닐때
                        itemInstance.location = originLocation;
                        rectTransform.SetParent(originalParent, true);
                        rectTransform.anchoredPosition = originalPosition;
                        Debug.Log($"[OnEndDrag] 배치 실패 → 원래 위치로 복귀");
                    //}

                }

                canvasGroup.blocksRaycasts = true;
                rectTransform.pivot = new Vector2(0f, 1f);
                UpdateUI();
                return;
            }



            EquipmentSlotUI foundEquipUI = foundObject.GetComponent<EquipmentSlotUI>();
            if (foundEquipUI != null) {

                BagInventoryManager.Instance.currentPointedEquipSlot = foundEquipUI;//

                BagInventoryManager.Instance.currentPointedSlotIsMySlot = true;
                BagInventoryManager.Instance.currentPointedSlotIsEquip = true;

                //배치가능여부 계산 및 배치
                if (CanEquipItem(foundEquipUI))
                {
                    EquipItem(foundEquipUI);
                }
                else
                {

                    itemInstance.location = originLocation;
                    rectTransform.SetParent(originalParent, true);
                    rectTransform.anchoredPosition = originalPosition;
                    canvasGroup.blocksRaycasts = true;
                    rectTransform.pivot = new Vector2(0f, 1f);
                }
                //foundEquipUI로 옮기는 코드


                //UPDATEUI 외 다른 방식 적용
                canvasGroup.blocksRaycasts = true;
                
                BagInventoryManager.Instance.currentPointedSlotIsEquip = false;
                BagInventoryManager.Instance.currentPointedEquipSlot = null;
                UpdateUI();
                return;
            }
            
            HealSlotUI foundHealUI = foundObject.GetComponent<HealSlotUI>();
            if (foundHealUI != null)
            {

                if (CanEquipHealItem(foundHealUI))
                {
                    EquipHealItem(foundHealUI);

                    BagInventoryManager.Instance.currentPointedSlotIsEquip = false;
                    BagInventoryManager.Instance.currentPointedEquipSlot = null;
                }
                canvasGroup.blocksRaycasts = true;       // ★ 추가
                rectTransform.pivot = new Vector2(0f, 1f);   // ★ 추가
                UpdateUI();
                return;
            }

        }


        //완전실패
        
        canvasGroup.blocksRaycasts = true;
        rectTransform.SetParent(originalParent, true);
        rectTransform.pivot = new Vector2(0f, 1f);
        UpdateUI();
    }

    //============ CaculateTargetSlot 계산 ============//

    /// <summary>
    /// 마우스가 마지막으로 Hover한 슬롯(currentPointedSlot)에
    /// 아이템의 실제 좌측상단이 들어갈 좌표를 계산
    /// ex) size=(2,2) 이면 currentPointedSlot-(1,1) 
    /// </summary>
    /// 
    private Vector2Int CaculateTargetSlot()
    {
        var manager = BagInventoryManager.Instance;
        Vector2Int hoveredSlot = manager.currentPointedSlot;

        // 아이템 크기가 (3,2)라면, 우측하단이 hoveredSlot 위치에 닿았을 때
        // 좌측상단은 (3-1, 2-1)만큼 왼쪽/위로 보정
        Vector2Int finalPos = hoveredSlot - (itemInstance.data.size - new Vector2Int(1, 1));

        Debug.Log($"[CaculateTargetSlot] hovered={hoveredSlot}, finalPos={finalPos}");
        return finalPos;
    }


    private bool CanEquipItem(EquipmentSlotUI foundEquipUI)
    {
        //슬롯이 비어있다면 false 반환
        if (foundEquipUI.equipedItem != null) { return false; } 
        ItemType slotItemType = foundEquipUI.GetValidItemType();
        
        // 타입 불일치 시 false 반환
        if (slotItemType != itemInstance.data.itemType)
            return false;

        EquipSlotType equipSlotType = foundEquipUI.GetEquipSlotType();

        // 무기
        if (slotItemType == ItemType.Weapon)
        {
            //캐스팅
            WeaponData weapon = itemInstance.data as WeaponData;
            if (weapon == null) return false;

            // 3번 무기 슬롯일 때 권총만 허용
            if (equipSlotType == EquipSlotType.thirdWeapon && weapon.category != WeaponCategoryLegacy.Pistol)
                return false;

            //Equip
        }

        // 방어구
        else if (slotItemType == ItemType.Armor)
        {
            //캐스팅
            ArmorData armor = itemInstance.data as ArmorData;
            if (armor == null) return false;

            if (equipSlotType == EquipSlotType.head && armor.armorSlot != ArmorSlot.Head)
                return false;

            if (equipSlotType == EquipSlotType.body && armor.armorSlot != ArmorSlot.Body)
                return false;
        }

        return true;
    }

    public void EquipItem(EquipmentSlotUI foundEquipUI) {
        //게임 로드 시에도 사용가능

        //기존 위치에 대한 점유해제 및 리스트 내 삭제
        var manager = BagInventoryManager.Instance;

        // (1) myItems 쪽에 있었던 경우
        if (manager.myItems.Contains(itemInstance))
        {
            manager.FreeItemSlots(itemInstance); // 슬롯 점유 해제
            manager.myItems.Remove(itemInstance);
        }
        // (2) opponentItems 쪽에 있었던 경우
        else if (manager.opponentItems != null && manager.opponentItems.Contains(itemInstance))
        {
            manager.FreeItemSlots(itemInstance); // 슬롯 점유 해제
            manager.opponentItems.Remove(itemInstance);
        }
        else if (itemInstance.currentEquipSlotType != EquipSlotType.none)   
        {
            UnEquip();
            //기존 Slot에서 제거...
            //EquipSlot간 이동
        }
        rectTransform.SetParent(foundEquipUI.transform, true);
        foundEquipUI.equipedItem = this;
        itemInstance.currentEquipSlotUI = foundEquipUI;
        itemInstance.currentEquipSlotType = foundEquipUI.GetEquipSlotType();

        Weapon weapon = itemInstance as Weapon;
        WeaponData weaponData = weapon.data as WeaponData;

        GameObject ob = Instantiate(weaponData.WeaponPrefab, VestInventory.Instance.hand);
        ob.SetActive(false);
        
        switch (foundEquipUI.GetEquipSlotType())
        {
            case EquipSlotType.firstWeapon:
                if (VestInventory.Instance.weaponOnHand1 != null) 
                { Destroy(VestInventory.Instance.weaponOnHand1); }

                VestInventory.Instance.weaponOnHand1 = ob.GetComponent<WeaponOnHand>();
                VestInventory.Instance.weaponOnHand1.currentWeapon = itemInstance as Weapon;
                manager.firstWeapon = weapon;
                VestInventory.Instance.firstWeaponOnVest.IsEquiped = true;
                VestInventory.Instance.thirdWeaponOnVest.IsUsing = false;
                break;

            case EquipSlotType.secondWeapon:
                if (VestInventory.Instance.weaponOnHand2 != null)
                { Destroy(VestInventory.Instance.weaponOnHand2); }

                VestInventory.Instance.weaponOnHand2 = ob.GetComponent<WeaponOnHand>();
                VestInventory.Instance.weaponOnHand2.currentWeapon = itemInstance as Weapon;

                manager.secondWeapon = weapon;
                VestInventory.Instance.secondWeaponOnVest.IsEquiped = true;
                VestInventory.Instance.thirdWeaponOnVest.IsUsing = false;
                break;

            case EquipSlotType.thirdWeapon:
                if (VestInventory.Instance.weaponOnHand3 != null)
                { Destroy(VestInventory.Instance.weaponOnHand3); }

                VestInventory.Instance.weaponOnHand3 = ob.GetComponent<WeaponOnHand>();
                VestInventory.Instance.weaponOnHand3.currentWeapon = itemInstance as Weapon;

                manager.thirdWeapon = weapon;
                VestInventory.Instance.thirdWeaponOnVest.IsEquiped = true;
                VestInventory.Instance.thirdWeaponOnVest.IsUsing = false;
                break;

            default:
                break;
        }
        return;
    }

    private void UnEquip()
    {
        var vest = VestInventory.Instance;

        switch (itemInstance.currentEquipSlotType)
        {
            case EquipSlotType.firstWeapon:
                if (vest.weaponOnHand1 != null)
                {
                    if (vest.weaponOnHand == vest.weaponOnHand1)
                    {
                        vest.weaponOnHand = null;
                        vest.shooter.SetNoWeapon();   // 🛠️ PlayerShooter 무기 해제 호출
                    }
                    Destroy(vest.weaponOnHand1.gameObject);
                }
                BagInventoryManager.Instance.firstWeapon = null;
                vest.firstWeaponOnVest.IsEquiped = false;
                vest.firstWeaponOnVest.UpdateUI();
                break;

            case EquipSlotType.secondWeapon:
                if (vest.weaponOnHand2 != null)
                {
                    if (vest.weaponOnHand == vest.weaponOnHand2)
                    {
                        vest.weaponOnHand = null;
                        vest.shooter.SetNoWeapon();
                    }
                    Destroy(vest.weaponOnHand2.gameObject);
                }
                BagInventoryManager.Instance.secondWeapon = null;
                vest.secondWeaponOnVest.IsEquiped = false;
                vest.secondWeaponOnVest.UpdateUI();
                break;

            case EquipSlotType.thirdWeapon:
                if (vest.weaponOnHand3 != null)
                {
                    if (vest.weaponOnHand == vest.weaponOnHand3)
                    {
                        vest.weaponOnHand = null;
                        vest.shooter.SetNoWeapon();
                    }
                    Destroy(vest.weaponOnHand3.gameObject);
                }
                BagInventoryManager.Instance.thirdWeapon = null;
                vest.thirdWeaponOnVest.IsEquiped = false;
                vest.thirdWeaponOnVest.UpdateUI();
                break;

            default:
                break;
        }

        if (itemInstance.currentEquipSlotUI != null)
        {
            itemInstance.currentEquipSlotUI.equipedItem = null;
            itemInstance.currentEquipSlotUI = null;
            itemInstance.currentEquipSlotType = EquipSlotType.none;
        }

        if (transform.parent != null)
        {
            RectTransform parentRect = transform.parent as RectTransform;
            RectTransform selfRect = transform as RectTransform;
            if (parentRect != null && selfRect != null)
            {
                selfRect.anchorMin = new Vector2(0f, 1f);
                selfRect.anchorMax = new Vector2(0f, 1f);
                selfRect.pivot = new Vector2(0f, 1f);
            }
        }
    }




    private bool CanEquipHealItem(HealSlotUI foundHealUI)
    {
        if (foundHealUI.equipedItem != null) { return false; }

        if (itemInstance.data is ConsumableData consume)
        {
            if (consume.isMedicine)
            {
                return true;
            }
        }
        return false;
    }

    public void EquipHealItem(HealSlotUI foundHealUI)
    {
        var mgr = BagInventoryManager.Instance;

        /* ① 인벤토리(내/상대)에서 제거 + 슬롯 점유 해제 */
        if (mgr.myItems.Contains(itemInstance))
        {
            mgr.FreeItemSlots(itemInstance);
            mgr.myItems.Remove(itemInstance);
        }
        else if (mgr.opponentItems != null && mgr.opponentItems.Contains(itemInstance))
        {
            mgr.FreeItemSlots(itemInstance);
            mgr.opponentItems.Remove(itemInstance);
        }
        else if(true)  // 이미 다른 힐 슬롯에 있었던 경우 list contain으로...
        {
            UnEquipHealItem();
        }

        var hmgr = HealItemManager.instance;
        hmgr.consumables[foundHealUI.index] = itemInstance as Consumable;

        /* ② UI 부모 변경 */
        rectTransform.SetParent(foundHealUI.transform, true);
        rectTransform.anchoredPosition = Vector2.zero;   // 중앙 고정
        rectTransform.localScale = Vector3.one;

        foundHealUI.equipedItem = this;
        
        UpdateUI();
    }

    private void UnEquipHealItem()
    {
        var hmgr = HealItemManager.instance;
        if (hmgr == null) return;
        
        // 0~5 슬롯을 모두 검색
        for (int i = 0; i < hmgr.consumables.Length; i++)
        {
            if (hmgr.consumables[i] == (itemInstance as Consumable))
            {
                hmgr.consumables[i] = null;                 // 배열 해제
                hmgr.healSlot[i].equipedItem = null;        // 슬롯‑UI 해제
                break;
            }
        }


        // ③ 원래 Canvas(혹은 풀)로 이동
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null) transform.SetParent(rootCanvas.transform, true);

        // ④ 피벗/앵커 복원
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
    }


    /// <summary>
    /// 실제로 해당 targetPosition에 배치할 수 있는지 검사만 (점유 안함)
    /// </summary>
    private bool CanPlaceItem(Vector2Int targetPosition, bool isTargetMySlot)
    {
        //인자는 이동의 목적지 고려

        var manager = BagInventoryManager.Instance;

        // 1) 어느 인벤토리로 갈지에 따라 딕셔너리와 사이즈 결정
        Dictionary<Vector2Int, Slot> targetSlots;
        if (isTargetMySlot)
        {
            targetSlots = manager.mySlots;
        }
        else
        { targetSlots = manager.opponentSlots;
        }

        Vector2Int inventorySize;
        if (isTargetMySlot)
        {
            inventorySize = manager.myInventoryVector;
        }
        else
        {
            inventorySize = manager.opponentInventoryVector;
        }

        // 2) ValidSlots로 검사
        return manager.ValidSlots(targetPosition, itemInstance.data.size, targetSlots, inventorySize);
    }

    /// <summary>
    /// 배치 가능하다면, 실제로 Occupy + 인벤토리 리스트 이동 처리
    /// (현재 내/상대 인벤토리 중 어느 리스트에 이 아이템이 있었는지 파악 후 이동)
    /// 비효올적이지만 자주하는 연산 아님
    /// </summary>
    public void PlaceItem(Vector2Int targetPosition, bool isTargetMySlot)
    {
        //inventoryManager와 연동
        var manager = BagInventoryManager.Instance;

        //itemInstance 원래 위치 분기 부분 작성 필요
        
        //Equip된 아이템이 아니라면 기존에 포함되었던 item List에서 삭제
        if (itemInstance.currentEquipSlotUI == null)
        {
            // (A) 기존 인벤토리에서 이 아이템 제거만 (FreeItemSlots는 필요 없음!)
            if (manager.myItems.Contains(itemInstance))
            {
                manager.myItems.Remove(itemInstance);
                // manager.FreeItemSlots(itemInstance); // ★삭제
            }
            else if (manager.opponentItems != null && manager.opponentItems.Contains(itemInstance))
            {
                manager.opponentItems.Remove(itemInstance);
                // manager.FreeItemSlots(itemInstance); // ★삭제
            }
            else if (HealItemManager.instance != null &&
                 System.Array.IndexOf(HealItemManager.instance.consumables,
                                      itemInstance as Consumable) >= 0)
            {
                UnEquipHealItem();
            }
        }
        else
        {
            UnEquip();
        }


        //...


        // (B) 새 위치 점유
        // PlaceItemInSlot() 안에서 OccupySlots()가 자동으로 호출됨
        if (isTargetMySlot)
        {
            manager.PlaceItemInSlot(
                itemInstance,
                targetPosition,
                manager.mySlots,
                manager.myItems,
                manager.myInventoryVector
            );
            rectTransform.SetParent(manager.myInventory, true);
        }
        else
        {
            manager.PlaceItemInSlot(
                itemInstance,
                targetPosition,
                manager.opponentSlots,
                manager.opponentItems,
                manager.opponentInventoryVector
            );
            rectTransform.SetParent(manager.opponentInventory, true);
        }

        // (C) 아이템 location 갱신 + UI 갱신
        itemInstance.location = targetPosition;
        UpdatePosition(itemInstance.location);
        UpdateUI();
    }



    /// <summary>
    /// 특정 딕셔너리 내에서 (location ~ location+size-1) 영역을 Occupied 상태로 세팅
    /// </summary>
    private void OccupySlotRange(Dictionary<Vector2Int, Slot> slots,
                                 Vector2Int location,
                                 Vector2Int size,
                                 bool state)
    {
        for (int y = location.y; y < location.y + size.y; y++)
        {
            for (int x = location.x; x < location.x + size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (slots.TryGetValue(pos, out Slot slot))
                {
                    slot.SetOccupied(state);
                }
            }
        }
    }

    private void OnDisable()
    {
        // 드래그 도중 UI가 꺼지면 Raycast 복원
        itemImage.raycastTarget = true;
    }


    public void TryUseItem()
    {
        if (itemInstance!=null&&itemInstance is Consumable consumable)
        {
            if (consumable.Use() == true)
            {
                // item 또는 힐 칸에서 제거 로직 (미구현)
                RemoveItemOnUI();
                ItemUIPoolManager.Instance.ReturnItemUI(this.gameObject);
            }
            else
            {
                UpdateUI();
            }
        }
    }

    public void RemoveItemOnUI()
    {
        var bag = BagInventoryManager.Instance;

        /* ① 인벤토리에 있었던 경우 → 리스트에서 빼고 슬롯 점유 해제 */
        if (bag.myItems.Remove(itemInstance))
            bag.FreeItemSlots(itemInstance);
        else if (bag.opponentItems != null && bag.opponentItems.Remove(itemInstance))
            bag.FreeItemSlots(itemInstance);

        /* ② 힐‑슬롯에 들어 있던 경우 → HealItemManager 정리 */
        var hMgr = HealItemManager.instance;
        if (hMgr != null && itemInstance is Consumable con)
        {
            for (int i = 0; i < hMgr.consumables.Length; i++)
            {
                if (hMgr.consumables[i] == con)
                {
                    hMgr.consumables[i] = null;                 // 배열 비움
                    hMgr.healSlot[i].equipedItem = null;                 // 슬롯‑UI 비움
                    break;
                }
            }
        }

        /* ③ 착용 중이었으면 해제 */
        if (itemInstance.currentEquipSlotUI != null)
            UnEquip();
    }
}
