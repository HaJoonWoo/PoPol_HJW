using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool<PoolType>
{
    void Setup(OnReturnPoolEvent<PoolType> OnReturnPool);
}
public delegate void OnReturnPoolEvent<PoolType>(PoolType type);

public class ObjectPool<ManagerType, PoolType> : Singleton<ManagerType>
    where PoolType : MonoBehaviour, IPool<PoolType>
    where ManagerType : MonoBehaviour
{    
    [SerializeField] PoolType poolPrefab;               // 풀링 오브젝트 프리팹.
    [SerializeField] int createPoolCount;               // 최초 생성 개수.

    Stack<PoolType> storage = new Stack<PoolType>();
    Transform storageParent;                        // 저장소 부모 오브젝트.

    protected new void Awake()
    {
        base.Awake();

        // 저장소 오브젝트 생성 후 비활성화.
        storageParent = new GameObject("StorageParent").transform;
        storageParent.SetParent(transform);
        storageParent.gameObject.SetActive(false);

        // 풀링 오브젝트 생성.
        
        CreatePool(createPoolCount);
    }

    private void CreatePool(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {            
            PoolType newPool = Instantiate(poolPrefab, storageParent);
            newPool.Setup(OnReturnPool);
            storage.Push(newPool);
        }
    }


    public PoolType GetPool()
    {
        if (storage.Count <= 0)
            CreatePool();

        PoolType pop = storage.Pop();               // 저장소에서 하나 꺼낸다.
        pop.transform.SetParent(transform);         // 오브젝트의 부모를 변경.

        return pop;                                 // 오브젝트 반환.
    }
    public void OnReturnPool(PoolType pool)
    {
        pool.transform.SetParent(storageParent);    // 돌아온 풀링 오브젝트의 부모 변경.
        storage.Push(pool);                         // stack 대기열에 추가.
    }
}
