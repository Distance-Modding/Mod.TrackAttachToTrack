

using System;

using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using LevelEditorTools;
using UnityEngine;
using LevelEditorActions;

namespace Mod.EditorAnnihilator.Harmony
{
    [HarmonyPatch(typeof(GroupTool), "Run")]
    internal static class GroupTool__Run
    {
        //__instance is the class you are patching, so you can call functions on it.
        //If patching a function with paramaters, you can just add those paramaters as paramaters inside Postfix.
        //If the function you're patching has a return type, you can modify the result value with the parameter 'ref [type of return value] __result'
        [HarmonyPrefix]
        internal static bool Prefix(ref bool __result, GroupTool __instance)
        {
            if(Mod.GTA)
            {
                LevelEditor levelEditor = G.Sys.LevelEditor_;
                GameObject[] array = levelEditor.SelectedNonTrackNodeAndNonOnlyAllowOneObjects_.ToArray();
                if (array.Length < 1)
                {
                    LevelEditorTool.PrintErrorMessage("Requires at least 1 object to group.");
                    __result = false;
                    return false;
                }
                GroupAction groupAction = Group.CreateGroupAction(array, levelEditor.ActiveObject_);
                GameObject gameObject = groupAction.GroupObjects().gameObject;
                if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
                {
                    levelEditor.ClearSelectedList();
                    levelEditor.SelectObject(gameObject);
                    groupAction.FinishAndAddToLevelEditorActions();
                    LevelEditorTool.PrintFormattedCountMessage("Grouped {0} object{1}.", array.Length);
                    __result = true;
                    return false;
                }
                Debug.LogError((object)"Grouping failed");
                __result = false;
                return false;
            }
            return true;
        }
    }
}