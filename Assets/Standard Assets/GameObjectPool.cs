using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : MonoBehaviour {
    private readonly T m_Prefab;
    private readonly Transform m_FreeItemContainer;

    private readonly List<T> freeItems = new List<T>();

    public int Count { get; private set; }

    public GameObjectPool(T prefab, Transform freeItemContainer) {
        m_Prefab = prefab;
        m_FreeItemContainer = freeItemContainer;
    }

    public T GetItem(Transform parent) {
        Count++;
        T result;
        if (freeItems.Count > 0) {
            result = freeItems[freeItems.Count - 1];
            result.gameObject.SetActive(true);
            result.transform.SetParent(parent, false);
            freeItems.RemoveAt(freeItems.Count - 1);
        } else {
            result = CreateItem(parent);
        }
        return result;
    }

    private T CreateItem(Transform parent) {
        return Object.Instantiate(m_Prefab, parent);
    }

    public void DisposeItem(T item) {
        Count--;
        item.gameObject.SetActive(false);
        item.transform.SetParent(m_FreeItemContainer, false);
        freeItems.Add(item);
    }
}
