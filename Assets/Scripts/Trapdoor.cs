using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trapdoor : MonoBehaviour {
    [Tooltip("If false, the trapdoor is visible but closed.")]
    public bool isOpen = true;

    private void Awake() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isOpen && collision.CompareTag("Player")) {
            Debug.Log("Player entered trapdoor! Moving to next level...");
            LevelManager.Instance.AdvanceLevel();
        }
    }
}