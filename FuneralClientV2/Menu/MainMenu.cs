﻿using FuneralClientV2.Settings;
using FuneralClientV2.Utils;
using FuneralClientV2.Wrappers;
using Il2CppSystem.Threading;
using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FuneralClientV2.Menu
{
    public class MainMenu : QMNestedButton
    {
        public MainMenu() : base("ShortcutMenu", Configuration.GetConfig().MainMenuButtonX, Configuration.GetConfig().MainMenuButtonY, "Funeral\nV2", "A client for vrchat's il2cpp system, hopefully just an updated version of my old publicly sold client, with more features and fixed bugs of course.", Color.red, Color.white, Color.red, Color.cyan)
        {
            new QMSingleButton(this, 1, 0, "GitHub", new Action(() =>
            {
                Process.Start("https://github.com/Yaekith/Funeral_ClientV2");
            }), "Open the github repository in a new browser window", Color.red, Color.white);
            new QMSingleButton(this, 2, 0, "Discord", new Action(() =>
            {
                Process.Start("https://discord.gg/8fwurVW");
            }), "Join the official discord", Color.red, Color.white);
            new QMSingleButton(this, 3, 0, "Daily\nNotice", new Action(() =>
            {
                new System.Threading.Thread(() =>
                {
                    var information = new WebClient().DownloadString("https://pastebin.com/raw/BjsgVdQp");
                    GeneralUtils.InformHudText(Color.cyan, information);
                })
                { IsBackground = true }.Start();
            }), "Gather information about the latest notice in the Discord", Color.red, Color.white);
            new QMSingleButton(this, 4, 0, "Credits", new Action(() =>
            {
                GeneralUtils.InformHudText(Color.yellow, "Yaekith - Developer\n404 - Developer");
            }), "Displays who made this cheat", Color.red, Color.white);
            new UtilsVRMenu(this);
            new FunVRMenu(this);
            new ProtectionsVRMenu(this);
            new TargetVRMenu();
            new FavoritesVRMenu(this);
            if (GeneralUtils.IsDevBranch) new DeveloperVRMenu();
            new QMToggleButton(this, 1, 2, "Clear\nConsole", delegate
            {
                Configuration.GetConfig().CleanConsole = true;
                Configuration.SaveConfiguration();
            }, "Don't Clear\nConsole", delegate
            {
                Configuration.GetConfig().CleanConsole = false;
                Configuration.SaveConfiguration();
            }, "Decide whether you want your console to be spammed by useless game information or not.", Color.red, Color.white).setToggleState(Configuration.GetConfig().CleanConsole);
            new QMSingleButton(this, 3, 2, "Select\nYourself", new Action(() =>
            {
                GeneralWrappers.GetQuickMenu().SelectPlayer(PlayerWrappers.GetCurrentPlayer());
            }), "Select your own current player and do some stuff to yourself, I don't know lol.", Color.red, Color.white);
            new QMToggleButton(this, 4, 2, "Hide\nYourself", new Action(() =>
            {
                PlayerWrappers.GetCurrentPlayer().prop_VRCAvatarManager_0.gameObject.SetActive(false);
            }), "Unhide\nYourself", new Action(() =>
            {
                PlayerWrappers.GetCurrentPlayer().prop_VRCAvatarManager_0.gameObject.SetActive(true);
            }), "Hide/Unhide yourself, for safety reasons maybe, who knows.", Color.red, Color.white);
            new QMToggleButton(this, 2, 2, "Enable\nMenu Music", delegate
            {
                Configuration.GetConfig().CustomMenuMusic = true;
                Configuration.SaveConfiguration();
            }, "Disable\nMenu Music", delegate
            {
                Configuration.GetConfig().CustomMenuMusic = false;
                Configuration.SaveConfiguration();
            }, "(EXPERIMENTAL)\nEnable custom menu music to play whenever you open your UI.", Color.red, Color.white).setToggleState(Configuration.GetConfig().CustomMenuMusic);
        }
    }
}
