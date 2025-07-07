using System;
using UnityEngine;

namespace RandomElementsSystem.Types
{
    [Serializable]
    public class WeightPropertyTransform : WeightProperty<Transform>
    {
        /// <summary>
        /// Do not use this default constructor. It is used only for serialization.
        /// </summary>
        public WeightPropertyTransform()
        {
        }

        /// <summary>
        /// Creates a new instance of WeightPropertyTransform.
        /// </summary>
        /// <param name="value">Transform value</param>
        /// <param name="weight">Weight of value</param>
        public WeightPropertyTransform(Transform value, float weight) : base(value, weight)
        {
        }
    }
}