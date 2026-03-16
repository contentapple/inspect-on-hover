using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mod
{
    public class Mod : MonoBehaviour
    {
        private static bool booted;

        public static void Main()
        {
            if (booted)
                return;

            booted = true;

            try
            {
                const string dllName = "InspectOnHover.NonSticky.Protected.v2.dll";
                string root = Directory.GetCurrentDirectory();
                string dllPath = Directory.GetFiles(root, dllName, SearchOption.AllDirectories).FirstOrDefault();

                if (string.IsNullOrEmpty(dllPath))
                {
                    ModAPI.Notify("Non-sticky loader: DLL not found");
                    return;
                }

                Assembly asm = Assembly.LoadFrom(dllPath);
                Type entryType = asm.GetType("Mod.Mod", false);
                if (entryType == null)
                {
                    ModAPI.Notify("Non-sticky loader: entry type missing");
                    return;
                }

                MethodInfo entry = entryType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
                if (entry == null)
                {
                    ModAPI.Notify("Non-sticky loader: entry method missing");
                    return;
                }

                entry.Invoke(null, null);
            }
            catch (Exception ex)
            {
                ModAPI.Notify("Non-sticky loader: " + ex.Message);
            }
        }
    }
}
