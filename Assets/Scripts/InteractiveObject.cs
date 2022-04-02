using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour {
    [NonSerialized] public Vector3 pos;

    protected void Awake() {
        pos = transform.position;
        InteractiveObjectManager.Instance.RegisterObject(this);
    }

    private void OnDestroy() {
        if (InteractiveObjectManager.Instance != null) {
            InteractiveObjectManager.Instance.UnregisterObject(this);
        }
    }

    public virtual string InteractText => "";
    public virtual bool Interactable => false;
}
