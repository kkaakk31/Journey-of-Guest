using System.Collections.Generic;
using UnityEngine;

namespace JoG.ObjectPool {

    public class UObjectPool<T> : ScriptableObject where T : Object {
        protected readonly Stack<T> pool = new(16);
        [field: SerializeField] public T Prefab { get; private set; }

        public T Rent() {
            while (pool.TryPop(out var result)) {
                if (result != null) {
                    return result;
                }
            }
            return Instantiate(Prefab);
        }

        public void Return(T uobj) => pool.Push(uobj);
    }
}