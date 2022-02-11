using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HasCollidedScript : MonoBehaviour
{
    public bool HasCollided { get { return hasCollided; } set { hasCollided = value; } }
    bool hasCollided = false;
}
