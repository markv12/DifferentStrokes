using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectManager : Singleton<InteractiveObjectManager> {
    private static readonly List<InteractiveObject> activeObjects = new List<InteractiveObject>();

    private const float MAX_SQUARE_DISTANCE = 20f;
    private const float MAX_DOT = -0.5f;
    public InteractiveObject GetNearestObject(Vector3 pos, Vector3 faceDirection) {
        InteractiveObject result = null;
        float minDot = float.MaxValue;
        for (int i = 0; i < activeObjects.Count; i++) {
            InteractiveObject interactiveObject = activeObjects[i];
            if (interactiveObject.Interactable) {
                Vector3 posDiff = pos - interactiveObject.pos;
                float sqDistance = posDiff.sqrMagnitude;
                if (sqDistance < MAX_SQUARE_DISTANCE) {
                    float dot = Vector3.Dot(posDiff.normalized, faceDirection);
                    if (dot < MAX_DOT && dot < minDot) {
                        result = interactiveObject;
                        minDot = dot;
                    }
                }
            }
        }
        return result;
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
