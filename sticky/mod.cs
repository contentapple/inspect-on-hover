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
        private LimbStatusViewBehaviour cachedInspector;
        private PersonBehaviour currentPerson;
        private readonly List<LimbBehaviour> limbBuffer = new List<LimbBehaviour>(16);

        private void Start()
        {
            cachedInspector = FindObjectOfType<LimbStatusViewBehaviour>(true);
        }

        private void Update()
        {
            if (!EnsureInspector())
                return;

            PersonBehaviour hovered = GetHoveredPerson();

            if (hovered == null || hovered == currentPerson)
                return;

            if (TryCollectLimbs(hovered))
            {
                cachedInspector.Limbs = limbBuffer;
                cachedInspector.gameObject.SetActive(true);
                currentPerson = hovered;
            }
        }

        private bool EnsureInspector()
        {
            if (cachedInspector != null)
                return true;

            cachedInspector = FindObjectOfType<LimbStatusViewBehaviour>(true);
            return cachedInspector != null;
        }

        private PersonBehaviour GetHoveredPerson()
        {
            Collider2D hit = Physics2D.OverlapPoint(Global.main.MousePosition);
            if (hit == null)
                return null;

            LimbBehaviour limb = hit.GetComponentInParent<LimbBehaviour>();
            if (limb == null)
                return null;

            PersonBehaviour person = limb.Person;
            return person != null && person.gameObject != null ? person : null;
        }

        private bool TryCollectLimbs(PersonBehaviour person)
        {
            limbBuffer.Clear();

            LimbBehaviour[] limbs = person.Limbs;
            if (limbs == null)
                return false;

            for (int i = 0; i < limbs.Length; i++)
            {
                LimbBehaviour limb = limbs[i];
                if (limb != null && limb.gameObject.activeInHierarchy)
                    limbBuffer.Add(limb);
            }

            return limbBuffer.Count > 0;
        }
    }
}
