using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [Header("Camera Target")]
    [Tooltip("An empty GameObject that the CinemachineCamera tracks.")]
    [SerializeField] private Transform cameraTarget;

    private void OnEnable()
    {
        Room.OnRoomEnteredGlobal += MoveCameraFocus;
        RoomManager.OnMapGenerated += HandleMapGenerated;
    }

    private void OnDisable()
    {
        Room.OnRoomEnteredGlobal -= MoveCameraFocus;
        RoomManager.OnMapGenerated -= HandleMapGenerated;
    }

    private void Start()
    {
        if (RoomManager.Instance != null)
        {
            HandleMapGenerated(RoomManager.Instance.GetStartRoomPosition());
        }
    }

    private void HandleMapGenerated(Vector2 startPos)
    {
        if (cameraTarget != null)
        {
            cameraTarget.position = new Vector3(startPos.x, startPos.y, cameraTarget.position.z);
        }
    }

    private void MoveCameraFocus(Room enteredRoom)
    {
        if (cameraTarget != null && enteredRoom != null)
        {
            cameraTarget.position = new Vector3(
                enteredRoom.transform.position.x,
                enteredRoom.transform.position.y,
                cameraTarget.position.z
            );
        }
    }
}