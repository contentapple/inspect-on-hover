using System.Collections.Generic;
using UnityEngine;

namespace Mod
{
    public class Mod : MonoBehaviour
    {
        public static void Main()
        {
            ModAPI.Register<HoverInspectSticky>();
        }
    }

    // Refactored function
    public class HoverInspectSticky : MonoBehaviour
    {
        private PersonBehaviour lastPerson;
        LimbStatusViewBehaviour inspector = Object.FindObjectOfType<LimbStatusViewBehaviour>(true);

        private void Update()
        {
            Vector2 mouse = Global.main.MousePosition;
            Collider2D hit = Physics2D.OverlapPoint(mouse);

            PersonBehaviour p = null;

            // Less checks being taken per frame unnecessarily than original
            if (hit != null && lastPerson != hit.GetComponentInParent<LimbBehaviour>()){
                LimbBehaviour limb = hit.GetComponentInParent<LimbBehaviour>();
                p = limb.Person;
                
                // If person or inspector is missing, cancel before causing errors
                if (inspector == null || p == null) return;

                // Gets each limb and appends to a list
                List<LimbBehaviour> validLimbs = new List<LimbBehaviour>();
                foreach (var limby in p.Limbs)
                {
                    if (limby != null)
                        validLimbs.Add(limby);
                }

                // Update inspector once per hover on a person
                inspector.gameObject.SetActive(true);
                inspector.Limbs = validLimbs;
                lastPerson = p;
            }

        }
    }
}
