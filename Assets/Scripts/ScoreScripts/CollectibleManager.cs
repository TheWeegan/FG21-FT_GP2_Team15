using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR //use if statement to avoid problems when making the build. UnityEditor needs to be excluded. 
#endif

namespace ScoreScripts
{
    public class CollectibleManager : MonoBehaviour
    {
        public static List<CollectScript> AllTheCollectibles = new List<CollectScript>();
        
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (CollectScript collectibles in AllTheCollectibles)
            {   
                Vector3 managerPos = transform.position;
                Vector3 collectPos = collectibles.transform.position;
                float halfHeight = (managerPos.y - collectPos.y) * .5f;
                Vector3 offset = Vector3.up * halfHeight;
                
                
                //used to located all objects when editing scene. Can't be included in build.
                Handles.DrawBezier(managerPos, collectPos, managerPos - offset,
                    collectPos + offset, Color.blue, EditorGUIUtility.whiteTexture, 1f);
            }
        }
        #endif
    }
}
