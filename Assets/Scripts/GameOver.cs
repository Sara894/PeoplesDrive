using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ezereal
{
    public class GameOver : MonoBehaviour
    {
        [Header("Game Over Settings")]
        [SerializeField] private float upsideDownThreshold = 0.2f;
        [SerializeField] private float timeUpsideDown = 0.3f;

        private float upsideDownTimer = 0f;
        private bool gameOverTriggered = false;

        private void Update()
        {
            // Check if car is upside down
            if (Vector3.Dot(transform.up, Vector3.up) < -upsideDownThreshold)
            {
                Debug.Log("Car is upside down!");
                upsideDownTimer += Time.deltaTime;
                if (upsideDownTimer >= timeUpsideDown && !gameOverTriggered)
                {
                    gameOverTriggered = true;
                    SceneManager.LoadScene("GameOver");
                }
            }
            else
            {
                upsideDownTimer = 0f;
            }
            if (transform.position.y < -10f && !gameOverTriggered)
            {
                gameOverTriggered = true;
                SceneManager.LoadScene("GameOver");
            }
        }
    }
}