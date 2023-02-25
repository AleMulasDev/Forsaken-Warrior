using UnityEngine;

using AmazingAssets.AdvancedDissolve;
using System;
using System.Collections;

namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class AnimateCutout : MonoBehaviour
    {
        [SerializeField] private float lerpTimer = 0.0f;
        private Material _material;
        private Health _health;
        private float timeElapsedSpawn = 0.0f;
        private float timeElapsedDissolve = 0.0f;

        private void Start()
        {
            GetComponent<Renderer>();

            _material = GetComponent<Renderer>().material;
            _health = GetComponentInParent<Health>();

            SpawnEffect();
        }

        public void Dissolve()
        {
            StartCoroutine(DissolveCoroutine());
        }

        public void SpawnEffect()
        {
            StartCoroutine(SpawnEffectCoroutine());
        }

        private IEnumerator DissolveCoroutine()
        {
            while (timeElapsedDissolve < lerpTimer)
            {
                float propertyValue = Mathf.Lerp(0, 1, timeElapsedDissolve / lerpTimer);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedDissolve += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator SpawnEffectCoroutine()
        {
            while (timeElapsedSpawn < lerpTimer)
            {
                float propertyValue = Mathf.Lerp(1, 0, timeElapsedSpawn / lerpTimer);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedSpawn += Time.deltaTime;
                yield return null;
            }
        }
    }
}