﻿using EthosClient.API;
using EthosClient.Menu;
using EthosClient.Settings;
using EthosClient.Wrappers;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRCSDK2;

namespace EthosClient.Utils
{
    public static class GeneralUtils
    {
        public static bool WorldTriggers = false;

        public static bool Flight = false;

        public static bool Autism = false;

        public static bool ESP = false;

        public static bool SpinBot = false;

        public static bool ForceClone = false;

        public static List<string> Deafened = new List<string>();

        public static Vector3 SavedGravity = Physics.gravity;

        public static List<QMButtonBase> Buttons = new List<QMButtonBase>();

        private static System.Random random = new System.Random();

        public static string Version = "1.4";

        public static AudioSource Source = null;

        public static bool IsDevBranch = true;

        public static void InformHudText(Color color, string text)
        {
            var NormalColor = VRCUiManager.prop_VRCUiManager_0.hudMessageText.color;
            VRCUiManager.prop_VRCUiManager_0.hudMessageText.color = color;
            VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0($"[ETHOS] {text}");
            VRCUiManager.prop_VRCUiManager_0.hudMessageText.color = NormalColor;
        }

        public static void ToggleColliders(bool toggle)
        {
            Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
            Component component = PlayerWrappers.GetCurrentPlayer().GetComponents<Collider>().FirstOrDefault<Component>(); //Fix this later but im lazy ok

            for (int i = 0; i < array.Length; i++)
            {
                Collider collider = array[i];
                bool flag = collider.GetComponent<PlayerSelector>() != null || collider.GetComponent<VRC.SDKBase.VRC_Pickup>() != null || collider.GetComponent<QuickMenu>() != null || collider.GetComponent<VRC_Station>() != null || collider.GetComponent<VRC.SDKBase.VRC_AvatarPedestal>() != null;
                if (!flag && collider != component) collider.enabled = toggle;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void ToggleUIButton(string Name, bool state)
        {
            QMToggleButton Button = null;
            foreach(var button in Buttons)
            {
                var button2 = (QMToggleButton)button;
                if (button2.getOnText().ToLower().Contains(Name.ToLower())) Button = button2;
            }
            if (Button != null) Button.setToggleState(state);
        }

        public static FavoritedAvatar GetExtendedFavorite(string ID)
        {
            foreach(var avatar in Configuration.GetConfig().ExtendedFavoritedAvatars) if (avatar.ID == ID) return avatar;
            return null;
        }

        public static bool SuitableVideoURL(string url)
        {
            if (url.Contains("youtube.com")) return true;
            else if (url.Contains("youtu.be")) return true;
            return false;
        }

        public static EthosVRButton GetEthosVRButton(string ID)
        {
            foreach(var button in Configuration.GetConfig().Buttons)
            {
                if (button.ID == ID)
                {
                    return button;
                }
            }
            return null;
        }
    }
}
