using UnityEngine;

using AmazingAssets.AdvancedDissolve;
using System;
using System.Collections;

namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class AnimateCutout : MonoBehaviour
    {
        [SerializeField] private bool enableSpawnEffect = true;
        [SerializeField] private bool resetParentState = true;
        [SerializeField] private float lerpTimerSpawn = 3.0f;
        [SerializeField] private float lerpTimerDeath = 5.0f;

        private AudioSource _audioSource;
        private AudioClip _dissolveAudioClip;

        private Material _material;
        private float timeElapsedSpawn = 0.0f;
        private float timeElapsedDissolve = 0.0f;

        private void Start()
        {
            GetComponent<Renderer>();

            _material = GetComponent<Renderer>().material;

            _audioSource = GetComponentInParent<AudioSource>();

            if (GetComponentInParent<BossController>())
                _dissolveAudioClip = Resources.Load<AudioClip>("MalignoDissolveAudioClip");
            else
                _dissolveAudioClip = Resources.Load<AudioClip>("EnemyDissolveAudioClip");

            if (enableSpawnEffect)
                SpawnEffect();
        }

        public void Dissolve(float delay)
        {
            StartCoroutine(DissolveCoroutine(delay));
        }

        public void SpawnEffect()
        {
            StartCoroutine(SpawnEffectCoroutine());
        }

        private IEnumerator DissolveCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            AudioManager.Instance.PlaySoundEffect(_audioSource, _dissolveAudioClip);

            while (timeElapsedDissolve < lerpTimerDeath)
            {
                float propertyValue = Mathf.Lerp(0, 1, timeElapsedDissolve / lerpTimerDeath);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedDissolve += Time.deltaTime;
                yield return null;
            }

            timeElapsedDissolve = 0f;
        }

        private IEnumerator SpawnEffectCoroutine()
        {
            if (resetParentState)
            {
                GetComponentInParent<AIController>()?.SetEnemyState(EEnemyState.EES_Spawning);
                GetComponent<BossProp>()?.SetEnemyState(EEnemyState.EES_Spawning);
            }

            while (timeElapsedSpawn < lerpTimerSpawn)
            {
                float propertyValue = Mathf.Lerp(1, 0, timeElapsedSpawn / lerpTimerSpawn);
                AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, propertyValue);
                timeElapsedSpawn += Time.deltaTime;
                yield return null;
            }
            if (resetParentState)
            {
                GetComponentInParent<AIController>()?.SetEnemyState(EEnemyState.EES_Inoccupied);
                GetComponent<BossProp>()?.SetEnemyState(EEnemyState.EES_Inoccupied);
            }

            timeElapsedSpawn = 0f;
        }
    }
}