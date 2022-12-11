using System;
using UnityEngine;

namespace Utils {
    public static class Transforms {
        public static Transform FindChildTransformByName(this Transform transform, string name) {
            var result = transform.Find(name);
            if (result is null) {
                throw new NullReferenceException(name + " is inactive or does not exist");
            }

            return result;
        }

        public static GameObject FindChildGameObjectByName(this Transform transform, string name) {
            return transform.FindChildTransformByName(name).gameObject;
        }
    }
}
