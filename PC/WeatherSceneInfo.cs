﻿using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Weather
{
	public class WeatherSceneInfo : MonoBehaviour
    {
        public static Scene CurrentScene;
        private static bool _hasFullSetRefs;

        public void SetRefs()
        {
            _hasFullSetRefs = true;
            Plugin.Log.Info("SetRefs " + CurrentScene.name);
            var mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            //Plugin.Log.Info("SetRefs 1");
            if (CurrentScene.name == Plugin.Menu) BundleLoader.WeatherPrefab.SetActive(PluginConfig.Instance.EnabledInMenu);
            if (CurrentScene.name == Plugin.Game) BundleLoader.WeatherPrefab.SetActive(PluginConfig.Instance.EnabledInGameplay);
            BundleLoader.Effects.Clear();
            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                //Plugin.Log.Info(i.ToString());
                var child = gameObject.transform.GetChild(i);
                child.gameObject.SetActive(true);

                var efd = child.gameObject.GetComponent<EffectDescriptor>();
                var nameToUse = EffectModel.GetNameWithoutSceneName(efd.effectName);
                child.gameObject.GetComponentsInChildren<AudioSource>().ToList().ForEach(s => { s.volume = PluginConfig.Instance.AudioSfxVolume; });
                efd.gameObject.SetActive(true);
                efd.transform.GetChild(0).gameObject.SetActive(true);
                var eff = new Effect(efd, child.gameObject, PluginConfig.Instance.EnabledEffects.Any(str => str == efd.effectName));
                
                if (MiscConfig.HasObject(nameToUse))
                {
                    //Plugin.Log.Info("Misc Config has Object! " + NameToUse);
                    var @object = MiscConfig.ReadObject(nameToUse);
                    eff.ShowInMenu = @object.ShowInMenu;
                    eff.ShowInGame = @object.ShowInGame;
                }
                else
                {
                    MiscConfig.Add(new MiscConfigObject(nameToUse, eff.ShowInMenu, eff.ShowInGame));
                    MiscConfig.Write();
                }
                eff.SetActiveRefs();
                BundleLoader.Effects.Add(eff);
                //Plugin.Log.Info("Replacing " + mrs.Length.ToString    ());
                foreach (var mr in mrs)
                {
                    if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                    {
                        eff.TrySetNoteMaterial(mr);
                    }
                }
            }
        }

        public void SetActiveRefs()
        {
            if (!_hasFullSetRefs) { SetRefs(); return; }
            var mrs = Resources.FindObjectsOfTypeAll<MeshRenderer>();
            foreach (var eff in BundleLoader.Effects)
            {
                var nameToUse = EffectModel.GetNameWithoutSceneName(eff.Desc.effectName);
                if (MiscConfig.HasObject(nameToUse))
                {
                    //Plugin.Log.Info("Misc Config has Object! " + NameToUse);
                    var @object = MiscConfig.ReadObject(nameToUse);
                    eff.ShowInMenu = @object.ShowInMenu;
                    eff.ShowInGame = @object.ShowInGame;
                }
                else
                {
                    MiscConfig.Add(new MiscConfigObject(nameToUse, eff.ShowInMenu, eff.ShowInGame));
                }
                foreach (var mr in mrs)
                {
                    if (mr.material.name.Contains("Note") || mr.gameObject.name.Contains("building") || mr.gameObject.name.Contains("speaker"))
                    {
                        eff.TrySetNoteMaterial(mr);
                    }
                }
                eff.SetActiveRefs();
            }
        }
    }
}