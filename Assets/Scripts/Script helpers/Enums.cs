using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum Ability
    {
        none = -1,
        jump = 0,
        suck = 1,
        shoot= 2,
    }
    public enum CameraPerspective
    {
       thirdPerson,
       pauseMenu,
       cinematic,
       none
    }
}
