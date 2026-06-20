using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FinishLevel : MonoBehaviour
{
    [Tooltip("If false, the trapdoor is visible but closed.")]
    public bool isOpen = true;

    [Header("Drop Animation Settings")]
    [Tooltip("How long it takes to fall into the hole and shrink to zero")]
    public float fallDuration = 0.5f;
    [Tooltip("How far down on the Y axis the player goes while shrinking")]
    public float fallDepth = 0.5f;
    public float delayBeforeLoad = 0.2f;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen && collision.CompareTag("Player"))
        {
            isOpen = false;
            StartCoroutine(DropPlayerRoutine(collision.gameObject));
        }
    }

    private IEnumerator DropPlayerRoutine(GameObject player)
    {
        Debug.Log("Player entered trapdoor! Falling in...");

        if (player.TryGetComponent(out Rigidbody2D playerRb))
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (player.TryGetComponent(out Collider2D playerCol))
        {
            playerCol.enabled = false;
        }

        Vector3 startPos = player.transform.position;
        Vector3 startScale = player.transform.localScale;

        Vector3 targetPos = transform.position + new Vector3(0, -fallDepth, 0);

        float elapsed = 0;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / fallDuration; 

            player.transform.position = Vector3.Lerp(startPos, targetPos, percent);

            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, percent);

            yield return null;
        }

        player.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(delayBeforeLoad);

        GameManager.Instance.AdvanceLevel();
    }
}