using UnityEngine;

using AmazingAssets.AdvancedDissolve;
using System;
using System.Collections;

namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class AnimateCutout : MonoBehaviour
    {
        [SerializeField] private float lerpTimerSpawn = 3.0f;
        [SerializeField] private float lerpTimerDeath = 5.0f;
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
            while (timeElapsedDissolve < lerpTimerDeath)
            {
                float propertyValue = Mathf.Lerp(0, 1, timeElapsedDissolve / lerpTimerDeath);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedDissolve += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator SpawnEffectCoroutine()
        {
            while (timeElapsedSpawn < lerpTimerSpawn)
            {
                float propertyValue = Mathf.Lerp(1, 0, timeElapsedSpawn / lerpTimerSpawn);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedSpawn += Time.deltaTime;
                yield return null;
            }
        }
    }
}