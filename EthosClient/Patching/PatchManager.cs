﻿using EthosClient.Settings;
using EthosClient.Utils;
using EthosClient.Wrappers;
using Harmony;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppSystem.Security.Cryptography;
using MelonLoader;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.UI;
using VRCSDK2;
using static VRC.SDKBase.VRC_EventHandler;

namespace EthosClient.Patching
{
    public static class PatchManager
    {
        private static HarmonyMethod GetLocalPatch(string name) { return new HarmonyMethod(typeof(PatchManager).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)); }

        private static List<Patch> RetrievePatches()
        {
            var ConsoleWriteLine = AccessTools.Method(typeof(Il2CppSystem.Console), "WriteLine", new Type[] { typeof(string) });
            List <Patch> patches = new List<Patch>()
            {
                new Patch("Ethos_Extras", AccessTools.Method(typeof(VRC_EventHandler), "InternalTriggerEvent", null, null), GetLocalPatch("TriggerEvent"), null),
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("KickUserRPC"), GetLocalPatch("AntiKick"), null),
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_1"), GetLocalPatch("CanEnterPublicWorldsPatch"), null),
                new Patch("Ethos_Moderation", typeof(ModerationManager).GetMethod("BlockStateChangeRPC"), GetLocalPatch("AntiBlock"), null),
                new Patch("Ethos_Extras", typeof(UserInteractMenu).GetMethod("Update"), GetLocalPatch("CloneAvatarPrefix"), null),
                new Patch("Ethos_Extras", ConsoleWriteLine, GetLocalPatch("IL2CPPConsoleWriteLine"), null),
                new Patch("Ethos_Extras", typeof(ImageDownloader).GetMethod("DownloadImage"), GetLocalPatch("AntiIpLogImage"), null),
                new Patch("Ethos_Extras", typeof(VRCSDK2.VRC_SyncVideoPlayer).GetMethod("AddURL"), GetLocalPatch("AntiVideoPlayerHijacking"), null),
            };
            return patches;
        }

        public static void ApplyPatches()
        {
            var patches = RetrievePatches();
            foreach (var patch in patches) patch.ApplyPatch();
            ConsoleUtil.Info("All Patches have been applied successfully.");
        }

        #region Patches
        private static bool TriggerEvent(ref VrcEvent __0, ref VrcBroadcastType __1, ref int __2, ref float __3)
        {
            if (GeneralUtils.WorldTriggers) __1 = VrcBroadcastType.Always; // really scuffed yaekith we need to fix this. lol - 404
            else if (Configuration.GetConfig().AntiWorldTriggers && __1 == VrcBroadcastType.Always) return false; //Anti World triggers lol
            return true;
        }

        private static bool AntiKick(ref string __0, ref string __1, ref string __2, ref string __3, ref Player __4)
        {
            //to-do; add support for moderation logging
            var target = GeneralWrappers.GetPlayerManager().GetPlayer(__0);
            var them = __4.GetAPIUser();
            if (target.GetAPIUser().id == PlayerWrappers.GetCurrentPlayer().GetVRC_Player().GetAPIUser().id)
                if (Configuration.GetConfig().LogModerations) GeneralUtils.InformHudText(Color.red, $"You were attempt kicked by {them.displayName}");
            else
                if (Configuration.GetConfig().LogModerations) GeneralUtils.InformHudText(Color.red, $"{target.GetAPIUser().displayName} has been kicked by {them.displayName}");

            return !Configuration.GetConfig().AntiKick;
        }

        private static bool AntiBlock(ref string __0, ref bool __1, ref Player __2)
        {
            //to-do; add support for moderation logging
            var target = GeneralWrappers.GetPlayerManager().GetPlayer(__0);
            var them = __2.GetAPIUser();
            if (target.GetAPIUser().id == PlayerWrappers.GetCurrentPlayer().GetVRC_Player().GetAPIUser().id)
                if (Configuration.GetConfig().LogModerations) GeneralUtils.InformHudText(Color.red, $"You were {(__1 ? "blocked" : "unblocked")} by {them.displayName}");
            else
                if (Configuration.GetConfig().LogModerations) GeneralUtils.InformHudText(Color.red, $"{target.GetAPIUser().displayName} has been {(__1 ? "blocked" : "unblocked")} by {them.displayName}");

            return !Configuration.GetConfig().AntiBlock;
        }

        private static void NonExistentPrefix() { }

        private static bool CloneAvatarPrefix(ref UserInteractMenu __instance)
        {
            bool result = true;
            if (GeneralUtils.ForceClone)
            {
                if (__instance.menuController.activeAvatar.releaseStatus != "private")
                {
                    bool flag2 = !__instance.menuController.activeUser.allowAvatarCopying;
                    if (flag2)
                    {
                        __instance.cloneAvatarButton.gameObject.SetActive(true);
                        __instance.cloneAvatarButton.interactable = true;
                        __instance.cloneAvatarButtonText.color = new Color(0.8117647f, 0f, 0f, 1f);
                        result = false;
                    }
                    else
                    {
                        __instance.cloneAvatarButton.gameObject.SetActive(true);
                        __instance.cloneAvatarButton.interactable = true;
                        __instance.cloneAvatarButtonText.color = new Color(0.470588237f, 0f, 0.8117647f, 1f);
                        result = false;
                    }
                }
            }
            return result;
        }

        private static bool IL2CPPConsoleWriteLine(string __0) { return !Configuration.GetConfig().CleanConsole; }

        private static bool AntiIpLogImage(string __0)
        {
            if (__0.StartsWith("https://api.vrchat.cloud/api/1/file/") || __0.StartsWith("https://api.vrchat.cloud/api/1/image/") || __0.StartsWith("https://d348imysud55la.cloudfront.net/thumbnails/") || __0.StartsWith("https://files.vrchat.cloud/thumbnails/")) return true;
            return !Configuration.GetConfig().PortalSafety;
        }

        private static bool AntiVideoPlayerHijacking(ref string __0)
        {
            if (Configuration.GetConfig().VideoPlayerSafety && GeneralUtils.SuitableVideoURL(__0)) __0 = "";
            return true;
        }

        private static bool CanEnterPublicWorldsPatch(ref bool __result, ref string __0, ref string __1, ref string __2)
        {
            if (Configuration.GetConfig().AntiPublicBan)
            {
                __result = false;
                return false;
            } else
            { return true; }
        }
    }
    #endregion
}
