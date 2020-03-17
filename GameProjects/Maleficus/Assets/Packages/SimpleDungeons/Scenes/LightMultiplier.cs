using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SimpleDungeon
{
    public class LightMultiplier : MonoBehaviour
    {
        [SerializeField] private float factor = 1.0f;

        public void MultiplyLight()
        {
            foreach (Light light in GetComponentsInChildren<Light>())
            {
                if (light)
                {
                    light.intensity *= factor;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LightMultiplier))]
    public class LightMultiplierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LightMultiplier myTarget = (LightMultiplier)target;

            if (GUILayout.Button("Multiply Light Intesity"))
            {
                myTarget.MultiplyLight();
            }
        }
    }
#endif
}
