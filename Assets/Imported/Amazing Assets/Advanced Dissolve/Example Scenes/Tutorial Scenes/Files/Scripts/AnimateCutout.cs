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
        private float timeElapsedSpawn = 0.0f;
        private float timeElapsedDissolve = 0.0f;

        private void Start()
        {
            GetComponent<Renderer>();

            _material = GetComponent<Renderer>().material;

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
            yield return new WaitForSeconds(2f);

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
            GetComponentInParent<AIController>().SetEnemyState(EEnemyState.EES_Spawning);

            while (timeElapsedSpawn < lerpTimerSpawn)
            {
                float propertyValue = Mathf.Lerp(1, 0, timeElapsedSpawn / lerpTimerSpawn);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedSpawn += Time.deltaTime;
                yield return null;
            }

            GetComponentInParent<AIController>().SetEnemyState(EEnemyState.EES_Inoccupied);
        }
    }
}