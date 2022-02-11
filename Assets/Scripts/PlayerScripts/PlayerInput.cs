using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class PlayerInput : MonoBehaviour
{
    KeyCode jumpKey = KeyCode.Space;
    KeyCode suckKey = KeyCode.Mouse0;
    KeyCode shootKey = KeyCode.Mouse1;

    private bool isActivated = true;

    public bool IsActivated { get => isActivated; set => isActivated = value; }

    public Vector3 GetMoveDirection()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Ability CheckAbilityInput()
    {
        return !isActivated ? Ability.none :
            Input.GetKeyDown(suckKey)  ? Ability.suck :
            Input.GetKeyDown(shootKey) ? Ability.shoot :
            Input.GetKeyDown(jumpKey)  ? Ability.jump : Ability.none;
    }
    public Ability CheckAbilityInputUp()
    {
        return !isActivated ? Ability.none:
            Input.GetKeyUp(suckKey)  ? Ability.suck :
            Input.GetKeyUp(shootKey) ? Ability.shoot :
            Input.GetKeyUp(jumpKey)  ? Ability.jump : Ability.none;
    }
}
