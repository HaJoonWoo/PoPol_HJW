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
    [SerializeField] PoolType poolPrefab;               // Ǯ�� ������Ʈ ������.
    [SerializeField] int createPoolCount;               // ���� ���� ����.

    Stack<PoolType> storage = new Stack<PoolType>();
    Transform storageParent;                        // ����� �θ� ������Ʈ.

    protected new void Awake()
    {
        base.Awake();

        // ����� ������Ʈ ���� �� ��Ȱ��ȭ.
        storageParent = new GameObject("StorageParent").transform;
        storageParent.SetParent(transform);
        storageParent.gameObject.SetActive(false);

        // Ǯ�� ������Ʈ ����.
        
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

        PoolType pop = storage.Pop();               // ����ҿ��� �ϳ� ������.
        pop.transform.SetParent(transform);         // ������Ʈ�� �θ� ����.

        return pop;                                 // ������Ʈ ��ȯ.
    }
    public void OnReturnPool(PoolType pool)
    {
        pool.transform.SetParent(storageParent);    // ���ƿ� Ǯ�� ������Ʈ�� �θ� ����.
        storage.Push(pool);                         // stack ��⿭�� �߰�.
    }
}
