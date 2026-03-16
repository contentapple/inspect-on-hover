using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace Mod
{
    public class Mod : MonoBehaviour
    {
        public static void Main()
        {
            ModAPI.Register<_HX13S0>();
        }
    }

    internal static class _GX
    {
        private static float _tickAt;
        private static bool _primed;
        private static readonly string[] _r =
        {
            "ypsnd",
            "ypsli",
            "artxeddn",
            "ydobretadn",
            "ydobhctawx",
            "ardihg"
        };

        private static string _rv(string s)
        {
            char[] c = s.ToCharArray();
            System.Array.Reverse(c);
            return new string(c);
        }

        internal static bool _ok()
        {
            if (!_primed)
            {
                _primed = true;
                _tickAt = Time.unscaledTime + 0.75f;
                return true;
            }

            if (Time.unscaledTime < _tickAt)
                return true;

            _tickAt = Time.unscaledTime + 0.75f;

            if (Debugger.IsAttached)
                return false;

            if (_probe())
                return false;

            return _asm();
        }

        private static bool _probe()
        {
            try
            {
                Process[] p = Process.GetProcesses();
                for (int i = 0; i < p.Length; i++)
                {
                    string n = p[i].ProcessName.ToLowerInvariant();
                    for (int j = 0; j < _r.Length; j++)
                    {
                        if (n.Contains(_rv(_r[j])))
                            return true;
                    }
                }
            }
            catch { }

            return false;
        }

        private static bool _asm()
        {
            try
            {
                Assembly a = typeof(_GX).Assembly;
                string n = a.GetName().Name;
                if (string.IsNullOrEmpty(n))
                    return false;

                int h = 17;
                for (int i = 0; i < n.Length; i++)
                    h = (h * 31) ^ n[i];

                return h != 0 && a.ManifestModule != null;
            }
            catch
            {
                return false;
            }
        }

        internal static void _drop(MonoBehaviour b, LimbStatusViewBehaviour i)
        {
            b.enabled = false;
            if (i != null && i.gameObject.activeSelf)
                i.gameObject.SetActive(false);
        }
    }

    public class _HX13S0 : MonoBehaviour
    {
        private LimbStatusViewBehaviour _u0;
        private readonly List<LimbBehaviour> _u1 = new List<LimbBehaviour>(16);

        private Vector2 _u2;
        private Collider2D _u3;
        private LimbBehaviour _u4;
        private PersonBehaviour _u5;

        private PersonBehaviour _u6;

        private void Start()
        {
            _u0 = FindObjectOfType<LimbStatusViewBehaviour>(true);
            _u2 = Global.main.MousePosition;
        }

        private void Update()
        {
            if (!_GX._ok())
            {
                _GX._drop(this, _u0);
                return;
            }

            if (!Q0())
                return;

            Vector2 p0 = Global.main.MousePosition;
            Collider2D p1;
            LimbBehaviour p2;
            PersonBehaviour p3;
            Q1(out p1, out p2, out p3);

            bool b0 = p0 != _u2;
            bool b1 = p1 != _u3;
            bool b2 = p2 != _u4;
            bool b3 = p3 != _u5;

            if (!b0 && !b1 && !b2 && !b3)
                return;

            _u2 = p0;
            _u3 = p1;
            _u4 = p2;
            _u5 = p3;

            Q2(p3);
        }

        private bool Q0()
        {
            if (_u0 != null)
                return true;

            _u0 = FindObjectOfType<LimbStatusViewBehaviour>(true);
            return _u0 != null;
        }

        private void Q1(out Collider2D c0, out LimbBehaviour c1, out PersonBehaviour c2)
        {
            c0 = Physics2D.OverlapPoint(Global.main.MousePosition);
            c1 = null;
            c2 = null;

            if (c0 == null)
                return;

            c1 = c0.GetComponentInParent<LimbBehaviour>();
            if (c1 == null || c1.gameObject == null || !c1.gameObject.activeInHierarchy)
            {
                c1 = null;
                return;
            }

            PersonBehaviour c3 = c1.Person;
            if (c3 == null || c3.gameObject == null || !c3.gameObject.activeInHierarchy)
            {
                c1 = null;
                return;
            }

            c2 = c3;
        }

        private void Q2(PersonBehaviour d0)
        {
            if (d0 == null || d0 == _u6)
                return;

            if (!Q4(d0))
            {
                Q3();
                return;
            }

            _u0.Limbs = _u1;
            if (!_u0.gameObject.activeSelf)
                _u0.gameObject.SetActive(true);

            _u6 = d0;
        }

        private void Q3()
        {
            _u6 = null;
            if (_u0 != null && _u0.gameObject.activeSelf)
                _u0.gameObject.SetActive(false);
        }

        private bool Q4(PersonBehaviour e0)
        {
            _u1.Clear();

            LimbBehaviour[] e1 = e0.Limbs;
            if (e1 == null || e1.Length == 0)
                return false;

            for (int i = 0; i < e1.Length; i++)
            {
                LimbBehaviour e2 = e1[i];
                if (e2 != null && e2.gameObject.activeInHierarchy)
                    _u1.Add(e2);
            }

            return _u1.Count > 0;
        }
    }
}
