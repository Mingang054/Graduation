using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUIPoolManager : MonoBehaviour
{

    //싱글톤패턴 적용
    public static ItemUIPoolManager Instance;
    
    //ItemInstanceUI Prefab 명시
    [SerializeField]
    private GameObject itemUIPrefab;
    //최대 생성되고 존재하는 아이템의 수는 96+96+n개 , n(무기3+방어구2+...), 표시될 아이템들의 수에 따라 유기적으로 표시
    [SerializeField]
    private int poolAmount = 200;
    private Queue<GameObject> poolQueue; 
    private ItemInstanceUI a;

    private void Awake()
    {
        //싱글톤 적용 및 풀링
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolQueue = new Queue<GameObject>();

        for (int i = 0; i < poolAmount; i++)
        {
            GameObject obj = Instantiate(itemUIPrefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }


    //ItemInstance를 받아 설정하기 (불러와서 UI에 적용 시키는 역할)
    public GameObject GetItemUI(ItemInstance setItemInstance)
    {
        //풀 잔량 확인
        if (poolQueue.Count != 0)
        {
            GameObject obj = poolQueue.Dequeue();
            ItemInstanceUI itemInstanceUI = obj.GetComponent<ItemInstanceUI>();
            itemInstanceUI.itemInstance = setItemInstance;
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(itemUIPrefab, transform);
            newObj.SetActive(false);
            ItemInstanceUI newItemInstanceUI = newObj.GetComponent<ItemInstanceUI>();
            newItemInstanceUI.itemInstance = setItemInstance;
            return newObj;
        }
    }
    

    //풀링 큐로 UI 반환 및 itemInstance, 이미지, 스트링 초기화
    public void ReturnItemUI(GameObject item)
    {
        ItemInstanceUI itemUI = item.GetComponent<ItemInstanceUI>();
        if (itemUI == null) { Debug.Log("itemInstanceUI가 아닙니다"); return; }
        itemUI.itemInstance = null;
        itemUI.UpdateUI();      //itemInstance == null이면 이미지 및 스트링 제거
        item.SetActive(false);
        poolQueue.Enqueue(item);
        
    }

}
