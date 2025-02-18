using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolAmount = 100;

    private Queue<GameObject> poolQueue;

    private void Awake()
    {
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
            GameObject obj = Instantiate(projectilePrefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public GameObject GetProjectile(Faction faction, float returnTime, float speed, float damage, float penetration, float colliderSize)
    {
        if (poolQueue.Count == 0)
        {
            Debug.LogWarning("⚠ 풀에 사용 가능한 프로젝타일이 없습니다!");
            return null;
        }

        GameObject obj = poolQueue.Dequeue();
        obj.SetActive(true);

        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetForEnable(faction, speed, damage, penetration, colliderSize);
        }

        StartCoroutine(SetDeleteTime(obj, returnTime));
        return obj;
    }

    private IEnumerator SetDeleteTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnProjectile(obj);
    }

    public void ReturnProjectile(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
