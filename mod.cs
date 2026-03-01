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

    public class HoverInspectSticky : MonoBehaviour
    {
        private PersonBehaviour lastPerson;

        private void Update()
        {
            Vector2 mouse = Global.main.MousePosition;
            Collider2D hit = Physics2D.OverlapPoint(mouse);

            PersonBehaviour p = null;

            if (hit != null)
            {
                LimbBehaviour limb = hit.GetComponentInParent<LimbBehaviour>();
                if (limb != null)
                {
                    p = limb.Person;
                }
            }

            var inspector = Object.FindObjectOfType<LimbStatusViewBehaviour>(true);
            if (inspector == null) return;

            if (p == null)
                return;

            List<LimbBehaviour> validLimbs = new List<LimbBehaviour>();
            foreach (var limb in p.Limbs)
            {
                if (limb != null)
                    validLimbs.Add(limb);
            }

            if (validLimbs.Count == 0)
                return;

            if (p != lastPerson)
            {
                inspector.gameObject.SetActive(true);
                inspector.Limbs = validLimbs;
                lastPerson = p;
            }
        }
    }
}