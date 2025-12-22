using UnityEngine;

namespace Ezereal
{
    public class EzerealSoundController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] bool useSounds = false;
        [SerializeField] EzerealCarController ezerealCarController;
        [SerializeField] AudioSource tireAudio;
        [SerializeField] AudioSource engineAudio;

        [Header("Settings")]
        public float maxVolume = 0.5f;

        [Header("Debug")]
        [SerializeField] bool alreadyPlaying;

        void Start()
        {
            if (useSounds)
            {
                alreadyPlaying = false;

                if (ezerealCarController == null || ezerealCarController.vehicleRB == null || tireAudio == null || engineAudio == null)
                {
                    Debug.LogWarning("EzerealSoundController is missing some references. Ignore or attach them if you want to have sound controls.");
                }

                if (tireAudio != null)
                {
                    tireAudio.volume = 0f;
                    tireAudio.Stop();
                }

                if (engineAudio != null)
                {
                    engineAudio.Play();
                }
            }
        }

        public void TurnOnEngineSound()
        {
            if (useSounds && engineAudio != null && !engineAudio.isPlaying)
            {
                engineAudio.Play();
            }
        }

        public void TurnOffEngineSound()
        {
            if (useSounds && engineAudio != null && engineAudio.isPlaying)
            {
                engineAudio.Stop();
            }
        }

        void Update()
        {
            if (!useSounds) return;
            if (ezerealCarController == null || ezerealCarController.vehicleRB == null || tireAudio == null || engineAudio == null)
                return;

#if UNITY_6000_0_OR_NEWER
            float carSpeed = ezerealCarController.vehicleRB.linearVelocity.magnitude;
#else
            float carSpeed = ezerealCarController.vehicleRB.velocity.magnitude;
#endif
            bool isStationary = carSpeed < 0.5f;

            // Play tire sound only if moving and on ground
            if (!isStationary)
            {
                if (!tireAudio.isPlaying)
                    tireAudio.Play();
                alreadyPlaying = true;
            }
            else
            {
                if (tireAudio.isPlaying)
                    tireAudio.Stop();
                alreadyPlaying = false;
            }

            // Calculate the volume based on speed
            float targetVolume = Mathf.Clamp01(carSpeed / 15f) * maxVolume;
            tireAudio.volume = targetVolume;

            // Tire Pitch
            float tireSoundPitch = 0.8f + (carSpeed / 50f);
            tireAudio.pitch = tireSoundPitch;

            // Engine Pitch
            float engineSoundPitch = 0.8f + (carSpeed / 25f);
            engineAudio.pitch = engineSoundPitch;
        }
    }
}
