using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (Keyboard.current.upArrowKey.isPressed) 
        {
            Debug.Log("ShootUp");
        }
        if (Keyboard.current.downArrowKey.isPressed) 
        {
            Debug.Log("ShootDown");
        }
        if (Keyboard.current.leftArrowKey.isPressed) 
        {
            Debug.Log("ShootLeft");
        }
        if (Keyboard.current.rightArrowKey.isPressed) 
        {
            Debug.Log("ShootRight");
        }
    }

}
