using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicZoneTrigger : MonoBehaviour
{
    [Header("Zone Music Settings")]
    [SerializeField] private AudioClip zoneMusic;
    [SerializeField] private bool revertOnExit = true;
    [SerializeField] private MusicStateManager.MusicState revertState = MusicStateManager.MusicState.FreeRoam;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";

    private AudioClip previousMusic;
    private bool playerInZone = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerInZone)
        {
            playerInZone = true;
            OnPlayerEnterZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && playerInZone)
        {
            playerInZone = false;
            OnPlayerExitZone();
        }
    }

    private void OnPlayerEnterZone()
    {
        if (zoneMusic != null && AudioManager.instance != null)
        {
            previousMusic = AudioManager.instance.GetCurrentMusic();
            AudioHelper.PlayMusic(zoneMusic);
            Debug.Log($"MusicZoneTrigger: Entered zone, playing '{zoneMusic.name}'");
        }
    }

    private void OnPlayerExitZone()
    {
        if (revertOnExit && MusicStateManager.instance != null)
        {
            MusicStateManager.instance.SetMusicState(revertState);
            Debug.Log($"MusicZoneTrigger: Exited zone, reverting to {revertState} music");
        }
    }
}
