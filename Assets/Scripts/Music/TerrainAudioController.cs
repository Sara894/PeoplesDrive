using UnityEngine;

namespace Ezereal
{
    public class TerrainAudioController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EzerealCarController carController;
        [SerializeField] private AudioSource tireAudioSource;

        [Header("Terrain Sound Clips")]
        [SerializeField] private AudioClip roadTireSound;
        [SerializeField] private AudioClip dirtTireSound;
        [SerializeField] private AudioClip offRoadTireSound;

        [Header("Settings")]
        [SerializeField] private float transitionSpeed = 2f;
        [SerializeField] private LayerMask roadLayer;
        [SerializeField] private LayerMask dirtLayer;
        [SerializeField] private LayerMask offRoadLayer;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private TerrainType currentTerrain = TerrainType.Road;

        private TerrainType previousTerrain;
        private float targetVolume = 0f;

        public enum TerrainType
        {
            Road,
            Dirt,
            OffRoad
        }

        private void Start()
        {
            if (carController == null)
            {
                carController = GetComponent<EzerealCarController>();
            }

            if (tireAudioSource == null)
            {
                Debug.LogWarning("TerrainAudioController: Tire AudioSource not assigned!");
            }

            previousTerrain = currentTerrain;
            UpdateTireSound();
        }

        private void Update()
        {
            DetectTerrainType();

            if (currentTerrain != previousTerrain)
            {
                UpdateTireSound();
                previousTerrain = currentTerrain;

                if (debugMode)
                {
                    Debug.Log($"TerrainAudioController: Terrain changed to {currentTerrain}");
                }
            }
        }

        private void DetectTerrainType()
        {
            if (carController == null) return;

            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f))
            {
                int hitLayer = 1 << hit.collider.gameObject.layer;

                if ((roadLayer.value & hitLayer) != 0)
                {
                    currentTerrain = TerrainType.Road;
                }
                else if ((dirtLayer.value & hitLayer) != 0)
                {
                    currentTerrain = TerrainType.Dirt;
                }
                else if ((offRoadLayer.value & hitLayer) != 0)
                {
                    currentTerrain = TerrainType.OffRoad;
                }
            }
        }

        private void UpdateTireSound()
        {
            if (tireAudioSource == null) return;

            AudioClip newClip = GetClipForTerrain(currentTerrain);

            if (newClip != null && tireAudioSource.clip != newClip)
            {
                bool wasPlaying = tireAudioSource.isPlaying;
                float currentTime = tireAudioSource.time;

                tireAudioSource.clip = newClip;

                if (wasPlaying)
                {
                    tireAudioSource.time = currentTime;
                    tireAudioSource.Play();
                }

                if (debugMode)
                {
                    Debug.Log($"TerrainAudioController: Switched to {currentTerrain} tire sound");
                }
            }
        }

        private AudioClip GetClipForTerrain(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Road:
                    return roadTireSound;
                case TerrainType.Dirt:
                    return dirtTireSound;
                case TerrainType.OffRoad:
                    return offRoadTireSound;
                default:
                    return roadTireSound;
            }
        }

        public TerrainType GetCurrentTerrain()
        {
            return currentTerrain;
        }

        public void SetRoadTireSound(AudioClip clip)
        {
            roadTireSound = clip;
        }

        public void SetDirtTireSound(AudioClip clip)
        {
            dirtTireSound = clip;
        }

        public void SetOffRoadTireSound(AudioClip clip)
        {
            offRoadTireSound = clip;
        }
    }
}
