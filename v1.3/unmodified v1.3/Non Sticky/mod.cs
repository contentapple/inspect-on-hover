using System.Collections.Generic;
using UnityEngine;

namespace Mod
{
    public class Mod : MonoBehaviour
    {
        public static void Main()
        {
            ModAPI.Register<HoverInspectNonStickyV13>();
        }
    }

    public class HoverInspectNonStickyV13 : MonoBehaviour
    {
        private LimbStatusViewBehaviour cachedInspector;
        private readonly List<LimbBehaviour> limbBuffer = new List<LimbBehaviour>(16);

        private Vector2 lastMousePos;
        private Collider2D lastCollider;
        private LimbBehaviour lastLimb;
        private PersonBehaviour lastPerson;

        private PersonBehaviour inspectedPerson;

        private void Start()
        {
            cachedInspector = FindObjectOfType<LimbStatusViewBehaviour>(true);
            lastMousePos = Global.main.MousePosition;
        }

        private void Update()
        {
            if (!EnsureInspector())
                return;

            Vector2 mousePos = Global.main.MousePosition;
            Collider2D hoveredCollider;
            LimbBehaviour hoveredLimb;
            PersonBehaviour hoveredPerson;
            GetHoveredState(out hoveredCollider, out hoveredLimb, out hoveredPerson);

            bool mouseChanged = mousePos != lastMousePos;
            bool colliderChanged = hoveredCollider != lastCollider;
            bool limbChanged = hoveredLimb != lastLimb;
            bool personChanged = hoveredPerson != lastPerson;

            if (!mouseChanged && !colliderChanged && !limbChanged && !personChanged)
                return;

            lastMousePos = mousePos;
            lastCollider = hoveredCollider;
            lastLimb = hoveredLimb;
            lastPerson = hoveredPerson;

            HandleHoverChange(hoveredPerson);
        }

        private bool EnsureInspector()
        {
            if (cachedInspector != null)
                return true;

            cachedInspector = FindObjectOfType<LimbStatusViewBehaviour>(true);
            return cachedInspector != null;
        }

        private void GetHoveredState(out Collider2D collider, out LimbBehaviour limb, out PersonBehaviour person)
        {
            collider = Physics2D.OverlapPoint(Global.main.MousePosition);
            limb = null;
            person = null;

            if (collider == null)
                return;

            limb = collider.GetComponentInParent<LimbBehaviour>();
            if (limb == null || limb.gameObject == null || !limb.gameObject.activeInHierarchy)
            {
                limb = null;
                return;
            }

            PersonBehaviour candidate = limb.Person;
            if (candidate == null || candidate.gameObject == null || !candidate.gameObject.activeInHierarchy)
            {
                limb = null;
                return;
            }

            person = candidate;
        }

        private void HandleHoverChange(PersonBehaviour hoveredPerson)
        {
            if (hoveredPerson == null)
            {
                HideInspector();
                return;
            }

            if (hoveredPerson == inspectedPerson)
                return;

            if (!TryCollectLimbs(hoveredPerson))
            {
                HideInspector();
                return;
            }

            cachedInspector.Limbs = limbBuffer;
            if (!cachedInspector.gameObject.activeSelf)
                cachedInspector.gameObject.SetActive(true);

            inspectedPerson = hoveredPerson;
        }

        private void HideInspector()
        {
            inspectedPerson = null;
            if (cachedInspector != null && cachedInspector.gameObject.activeSelf)
                cachedInspector.gameObject.SetActive(false);
        }

        private bool TryCollectLimbs(PersonBehaviour person)
        {
            limbBuffer.Clear();

            LimbBehaviour[] limbs = person.Limbs;
            if (limbs == null || limbs.Length == 0)
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
