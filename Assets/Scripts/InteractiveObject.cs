using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {
    [NonSerialized] public Vector3 pos;

    protected virtual void Awake() {
        pos = transform.position;
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
