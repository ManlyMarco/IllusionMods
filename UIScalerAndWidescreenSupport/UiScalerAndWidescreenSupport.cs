﻿using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace UIScalerAndWidescreenSupport
{
    using Debug = UnityEngine.Debug;

#if AI
    [BepInProcess("AI-Syoujyo")]
    [BepInProcess("StudioNEOV2")]
#elif HS2
    [BepInProcess("HoneySelect2")]
    [BepInProcess("StudioNEOV2")]
#elif KK
    [BepInProcess("Koikatu")]
    [BepInProcess("CharaStudio")]
#endif
    [BepInPlugin("hj." + "aihs2studio." + nameof(UIScalerAndWidescreenSupport), nameof(UIScalerAndWidescreenSupport), VERSION)]
    public class UIScalerAndWidescreenSupport : BaseUnityPlugin
    {
        public const string VERSION = "1.0.1";
        public static ConfigEntry<float> ScaleConfig { get; set; }


        public void Awake()
        {
            ScaleConfig = Config.Bind("Scale", "Scale", 1f, new ConfigDescription("Scale factor for the entire game UI. Needs a game restart to take effect.", new AcceptableValueRange<float>(0.1f, 2f)));

            if (Math.Abs(ScaleConfig.Value - 1f) > 0.01f)
            {
                Harmony.CreateAndPatchAll(typeof(UIScalerAndWidescreenSupport));
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
                enabled = false;
        }

        private static void RescaleUi(CanvasScaler canvascale)
        {
            canvascale.matchWidthOrHeight = 1;
            canvascale.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvascale.scaleFactor = UIScalerAndWidescreenSupport.ScaleConfig.Value;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {


#if AI
            if (scene.name == "Title")
            {
                GameObject[] gol = scene.GetRootGameObjects();
                for (int i = 0; i < gol.Length; i++)
                {

                    if (gol[i].name == "TitleScene")
                    {
                        FixRect(
                            gol[i].transform.Find("Canvas/Title/PressInductionCaption"),
                            new Vector2(0.4f, 0.5f),
                            new Vector2(0.6f, 0.5f),
                            Vector2.zero,
                            Vector2.zero
                        );
                        break;
                    }


                }
            }
#elif HS2
            if (scene.name == "Home")
            {
                GameObject[] gol = scene.GetRootGameObjects();
                for (int i = 0; i < gol.Length; i++)
                {

                    if (gol[i].name == "HomeScene")
                    {
                        FixRect(
                            gol[i].transform.Find("Canvas/Panel/CharaEdit/Group/SelectChara/Panel"),
                            new Vector2(0.6f, 0.5f),
                            new Vector2(0.6f, 0.5f),
                            new Vector2(-686, -480),
                            new Vector2(-200, 480)
                        );
                        FixRect(
                            gol[i].transform.Find("Canvas/Panel/CharaEdit/Group/SelectCard1/Panel"),
                            new Vector2(0.6f, 1f),
                            new Vector2(0.6f, 1f),
                            new Vector2(-185, -1018),
                            new Vector2(185, -123)
                        );
                        FixRect(
                            gol[i].transform.Find("Canvas/Panel/CharaEdit/Group/SelectCard2/Panel"),
                            new Vector2(0.6f, 1f),
                            new Vector2(0.6f, 1f),
                            new Vector2(-185, -1018),
                            new Vector2(185, -123)
                        );
                        FixRect(
                            gol[i].transform.Find("Canvas/Panel/CharaEdit/Group/SelectGroups/Panel"),
                            new Vector2(0.6f, 0.5f),
                            new Vector2(0.6f, 0.5f),
                            new Vector2(200, -480),
                            new Vector2(686, 480)
                        );
                        break;
                    }


                }
            }
#endif
            if (scene.name == "CharaCustom")
            {
                GameObject[] gol = scene.GetRootGameObjects();
                for (int i = 0; i < gol.Length; i++)
                {

                    if (gol[i].name == "CharaCustom")
                    {

                        FixRect(
                            gol[i].transform.Find("CustomControl/Canvas_PopupCheck/Panel2/Text"),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(-1000, -200),
                            new Vector2(1000, 200)
                        );

                    }


                }
            }
        }



        private static void FixRect(Transform transform, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = transform.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void CanvasScalerHook(ref CanvasScaler __instance)
        {
            if (__instance.transform.name != "FrontSpCanvas")
            {
                RescaleUi(__instance);
            }

        }

        /* Things i keep for future update -----
        
        private static List<string> canvasScalerGONames = new List<string> {
        "",
        ""
        };
        private static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        */
    }

}