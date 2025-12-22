using UnityEngine;

namespace Ezereal
{
    public class EzerealSoundController : MonoBehaviour // This system plays tire and engine sounds.
    {
        [Header("References")]
        [SerializeField] bool useSounds = false;
        [SerializeField] EzerealCarController ezerealCarController;
        [SerializeField] AudioSource tireAudio;
        [SerializeField] AudioSource engineAudio;

        [Header("Settings")]
        public float maxVolume = 0.5f; // Maximum volume for high speeds

        [Header("Debug")]
        [SerializeField] bool alreadyPlaying;

        void Start()
        {
            if (useSounds)
            {
                alreadyPlaying = false;

                if (ezerealCarController == null || ezerealCarController.vehicleRB == null || tireAudio == null || engineAudio == null)
                {
                    Debug.LogWarning("ezerealSoundController is missing some references. Ignore or attach them if you want to have sound controls.");
                }

                if (tireAudio != null)
                {
                    tireAudio.volume = 0f; // Start with zero volume
                    tireAudio.Stop();
                }
            }
        }

        public void TurnOnEngineSound()
        {
            if (useSounds)
            {
                if (engineAudio != null)
                {
                    engineAudio.Play();
                }
            }
        }

        public void TurnOffEngineSound()
        {
            if (useSounds)
            {
                if (engineAudio != null)
                {
                    engineAudio.Stop();
                }
            }
        }

        void Update()
        {
            if (useSounds)
            {
#if UNITY_6000_0_OR_NEWER
                if (ezerealCarController != null && ezerealCarController.vehicleRB != null && tireAudio != null && engineAudio != null)
                {
                    float carSpeed = ezerealCarController.vehicleRB.linearVelocity.magnitude;
#else
                if (ezerealCarController != null && ezerealCarController.vehicleRB != null && tireAudio != null && engineAudio != null)
                {
                    float carSpeed = ezerealCarController.vehicleRB.velocity.magnitude;
#endif
                    bool isStationary = carSpeed < 0.5f;

                    if (!isStationary && !alreadyPlaying && !ezerealCarController.InAir())
                    {
                        tireAudio.Play();
                        alreadyPlaying = true;
                    }
                    else if (isStationary || ezerealCarController.InAir())
                    {
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
    }
}
