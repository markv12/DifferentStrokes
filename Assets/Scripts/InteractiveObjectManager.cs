using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectManager : Singleton<InteractiveObjectManager> {
    private static readonly List<InteractiveObject> activeObjects = new List<InteractiveObject>();
    private static readonly HashSet<InteractiveObject> nearObjects = new HashSet<InteractiveObject>();

    private const float MAX_SQUARE_DISTANCE = 20f;
    private const float MAX_DOT = -0.5f;
    public InteractiveObject GetNearestInteractableObject(Vector3 pos, Vector3 faceDirection) {
        InteractiveObject result = null;
        float minDot = float.MaxValue;
        for (int i = 0; i < activeObjects.Count; i++) {
            InteractiveObject interactiveObject = activeObjects[i];
            Vector3 posDiff = pos - interactiveObject.t.position;
            float sqDistance = posDiff.sqrMagnitude;
            bool isNear = sqDistance < MAX_SQUARE_DISTANCE;
            HandleEnterExitNear(isNear, interactiveObject);
            if (isNear && interactiveObject.Interactable) {
                float dot = Vector3.Dot(posDiff.normalized, faceDirection);
                if (dot < MAX_DOT && dot < minDot) {
                    result = interactiveObject;
                    minDot = dot;
                }
            }
        }
        return result;
    }

    private void HandleEnterExitNear(bool isNear, InteractiveObject interactiveObject) {
        bool alreadyContains = nearObjects.Contains(interactiveObject);
        if (isNear && !alreadyContains) {
            nearObjects.Add(interactiveObject);
            interactiveObject.OnNearChanged(true);
        } else if (!isNear && alreadyContains) {
            nearObjects.Remove(interactiveObject);
            interactiveObject.OnNearChanged(false);
        }
    }

    public void RegisterObject(InteractiveObject interactiveObject) {
        if (!activeObjects.Contains(interactiveObject)) {
            activeObjects.Add(interactiveObject);
        }
    }

    public void UnregisterObject(InteractiveObject interactiveObject) {
        activeObjects.Remove(interactiveObject);
    }
}
