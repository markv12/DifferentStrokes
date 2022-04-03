using System;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {
    [NonSerialized] public Transform t;

    protected virtual void Awake() {
        t = transform;
        InteractiveObjectManager.Instance.RegisterObject(this);
    }

    private void OnDestroy() {
        if (InteractiveObjectManager.Instance != null) {
            InteractiveObjectManager.Instance.UnregisterObject(this);
        }
    }

    public abstract string InteractText { get; }
    public abstract bool Interactable { get; }

    public abstract void OnNearChanged(bool isNear);
}
