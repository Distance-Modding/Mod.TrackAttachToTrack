using Centrifuge.Distance.Game;
using Centrifuge.Distance.GUI.Controls;
using Centrifuge.Distance.GUI.Data;
using Events.MainMenu;
using Events.QuitLevelEditor;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mod.EditorAnnihilator
{
    [ModEntryPoint("com.github.pred/Distance.TrackAttachToTrack")]
    public class Mod : MonoBehaviour
    {
        public static Mod Instance;

        public IManager Manager { get; set; }

        public static Log Logger { get; private set; }

        public static ConfigurationLogic Config { get; private set; }

        public static bool ModEnabled { get; set; }

        public void Initialize(IManager manager)
        {
            Instance = this;
            Logger = LogManager.GetForCurrentAssembly();
            Manager = manager;
            Config = gameObject.AddComponent<ConfigurationLogic>();
            //Events.MainMenu.Initialized.Subscribe(OnMainMenuInitialized);
            //Events.QuitLevelEditor.Quit.Subscribe(OnMainMenuInitialized2);
            //Events.MainMenu.Initialized.Unsubscribe(OnMainMenuInitialized);
            //Events.MainMenu.Initialized.Broadcast(new Initialized.Data());
            CreateSettingsMenu();
            //GameObject lamp = new GameObject();
            //Type lamptrans = System.Type.GetType("Transform");
            //lamp.AddComponent(lamptrans);
            RuntimePatcher.AutoPatch();
        }

        public void CreateSettingsMenu()
	    {
            MenuTree settingsMenu;
            settingsMenu = new MenuTree("menu.mod.TrackAttachToTrack", "TrackAttachToTrack Settings")
            {
            new CheckBox(MenuDisplayMode.MainMenu, "setting:enable_disable_trackgrouping", "ENABLE GROUPING TRACK ATTACHMENTS")
            .WithGetter(() => Config.gta)
            .WithSetter((x) => Config.gta = x)
            .WithDescription("Normally, you can only group track attachments with the track they are attached to, but enabling this removes this restriction. The reason this is an option and not a default feature is that it can produce some weird effects under certain circumstances.")
            };

            Menus.AddNew(MenuDisplayMode.Both, settingsMenu, "TrackAttachToTrack", "Settings for the TrackAttachToTrack mod.");
        }

        public static bool GTA => Config.gta;

        private void OnMainMenuInitialized(Initialized.Data data)
        {
            Resource.CreateLevelEditorPrefabDirInfo();
        }

        private void OnMainMenuInitialized2(Quit.Data data)
        {
            Resource.CreateResourceList();
        }
    }
}