﻿using HarmonyLib;
using UnityEngine;

namespace Weather
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    public class NotePatch
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Postfix(Component instance)
        {
            var obj = instance.gameObject;
            var mrs = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (var t in BundleLoader.Effects)
            {
                foreach (var t1 in mrs)
                {
                    t.TrySetNoteMaterial(t1);
                }
            }
        }
    }
}