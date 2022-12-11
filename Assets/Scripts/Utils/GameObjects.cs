using System;
using UnityEngine;

namespace Utils {
    public static class GameObjects {
        public static bool Equals(GameObject a, GameObject b) {
            if (a is null && b is null) {
                return true;
            }

            if (a is null || b is null) {
                return false;
            }

            return a.Equals(b);
        }

        public static Transform FindChildTransformByName(this GameObject gameObject, string name) {
            return gameObject.transform.FindChildTransformByName(name);
        }

        public static GameObject FindChildGameObjectByName(this GameObject gameObject, string name) {
            return gameObject.FindChildTransformByName(name).gameObject;
        }
    }
}
