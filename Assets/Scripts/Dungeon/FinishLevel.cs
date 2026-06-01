using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FinishLevel : MonoBehaviour {
    [Tooltip("If false, the trapdoor is visible but closed.")]
    public bool isOpen = true;

    [Header("Drop Animation Settings")]
    public float hopDuration = 0.3f;
    public float fallDuration = 0.4f;
    public float jumpHeight = 1.2f;
    public float delayBeforeLoad = 0.2f;

    private void Awake() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isOpen && collision.CompareTag("Player")) {
            isOpen = false;
            StartCoroutine(DropPlayerRoutine(collision.gameObject));
        }
    }

    private IEnumerator DropPlayerRoutine(GameObject player) {
        Debug.Log("Player entered trapdoor! Hopping in...");

        if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRb)) {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
        }

        Vector3 startPos = player.transform.position;
        Vector3 centerPos = transform.position;
        Vector3 startScale = player.transform.localScale;

        float elapsed = 0;

        while (elapsed < hopDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / hopDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, centerPos, t);

            currentPos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            player.transform.position = currentPos;
            yield return null;
        }

        player.transform.position = centerPos;

        elapsed = 0;
        while (elapsed < fallDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;

            float easeIn = t * t;

            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, easeIn);

            player.transform.position = centerPos + new Vector3(0, -easeIn * 0.5f, 0);

            yield return null;
        }

        player.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(delayBeforeLoad);

        GameManager.Instance.AdvanceLevel();
    }
}