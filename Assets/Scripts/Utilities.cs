using System;
using Unity.VisualScripting;
using UnityEngine;

public static class Utilities {
    public static GameObject FindChildByName(Transform transform, string name) {
        var result = transform.Find(name).gameObject;
        if (result.IsUnityNull()) {
            throw new NullReferenceException(name + " is inactive or does not exist");
        }

        return result;
    }

    public static T RequireNonNull<T>(T obj, string message) {
        if (obj == null) {
            throw new NullReferenceException(message);
        }

        return obj;
    }
}
