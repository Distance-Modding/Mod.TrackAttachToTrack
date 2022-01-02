using Centrifuge.Distance.EditorTools.Attributes;
using LevelEditorActions;
using LevelEditorTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mod.TrackAttachToTrack.Harmony
{
    [EditorTool, KeyboardShortcut("SHIFT+ALT+A")]
    public class TrackAttachToTrackToolS : InstantTool
    {
        internal static ToolInfo info_ => new ToolInfo("TrackAttachToTrackTool+", "Attaches selected objects to the first Track Segment selected.\nNever detatches.\nObjects containing track CAN be attached to the track.", ToolCategory.Track, ToolButtonState.Button, true, 1120);

        public override ToolInfo Info_ => info_;

        // Required by distance itself
        public static void Register()
        {
            if (!G.Sys.LevelEditor_.registeredToolsNamesToTypes_.ContainsKey(info_.Name_))
                G.Sys.LevelEditor_.RegisterTool(info_);
        }

        public override bool Run()
        {
            List<GameObject> possibleAttachments;
            List<TrackSegment> possibleParents;
            this.DetermineSelectedSegmentsAndObjects(out possibleAttachments, out possibleParents);
            if (possibleAttachments.Count == 0)
            {
                LevelEditorTool.PrintErrorMessage("Need to select at least 1 non-track object to attach to the track");
                return false;
            }
            if (possibleParents.Count == 0)
            {
                LevelEditorTool.PrintErrorMessage("No segment found to attach to.");
                return false;
            }
            List<ILevelEditorAction> levelEditorActionList = new List<ILevelEditorAction>();
            foreach (GameObject gameObject in possibleAttachments)
            {
                int nearestSubsegmentIndex;
                TrackSegment nearestSegment = this.FindNearestSegment(possibleParents, gameObject.transform.position, out nearestSubsegmentIndex);
                if ((UnityEngine.Object)nearestSegment != (UnityEngine.Object)null)
                    levelEditorActionList.Add((ILevelEditorAction)new AttachToTrackAction(nearestSegment, gameObject, nearestSubsegmentIndex));
            }
            ILevelEditorAction ifNecessary = CompositeAction.CreateIfNecessary((ICollection<ILevelEditorAction>)levelEditorActionList);
            if (ifNecessary != null)
            {
                ifNecessary.Redo();
                ifNecessary.FinishAndAddToLevelEditorActions();
            }
            LevelEditorTool.PrintFormattedCountMessage("{0} object{1} attached.", levelEditorActionList.Count);
            return true;
        }

        private void DetermineSelectedSegmentsAndObjects(
          out List<GameObject> possibleAttachments,
          out List<TrackSegment> possibleParents)
        {
            possibleAttachments = new List<GameObject>();
            possibleParents = new List<TrackSegment>();
            List<TrackSegment> trackSegmentList = new List<TrackSegment>();
            int trackcount = 0;
            foreach (GameObject selectedObject in G.Sys.LevelEditor_.SelectedObjects_)
            {
                trackSegmentList.Clear();
                TrackLinkParent component = selectedObject.GetComponent<TrackLinkParent>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                    component.GetContainedSegments((ICollection<TrackSegment>)trackSegmentList);
                if (trackSegmentList.Count > 0 && trackcount != 1)
                {
                    possibleParents.AddRange((IEnumerable<TrackSegment>)trackSegmentList);
                    trackcount = 1;
                }
                else if (true)
                    possibleAttachments.Add(selectedObject);
            }
            if (possibleAttachments.Count == 0 || possibleParents.Count != 0)
                return;
            foreach (TrackSegment trackSegment in UnityEngine.Object.FindObjectsOfType<TrackSegment>())
                possibleParents.Add(trackSegment);
        }

        private static bool CanAttachToTrack(GameObject gameObject) => !gameObject.HasComponent<TrackManipulatorNode>() && !gameObject.HasComponent<TrackAttachment>();

        private bool TryDetachSelectedObjects()
        {
            List<ILevelEditorAction> levelEditorActionList = new List<ILevelEditorAction>();
            foreach (GameObject selectedObject in G.Sys.LevelEditor_.SelectedObjects_)
            {
                TrackAttachment component = selectedObject.GetComponent<TrackAttachment>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                    levelEditorActionList.Add((ILevelEditorAction)new DetachFromTrackAction(component));
            }
            ILevelEditorAction ifNecessary = CompositeAction.CreateIfNecessary((ICollection<ILevelEditorAction>)levelEditorActionList);
            if (ifNecessary == null)
                return false;
            ifNecessary.Redo();
            ifNecessary.FinishAndAddToLevelEditorActions();
            LevelEditorTool.PrintFormattedCountMessage("{0} object{1} detached.", levelEditorActionList.Count);
            return true;
        }

        private TrackSegment FindNearestSegment(
          List<TrackSegment> segments,
          Vector3 pos,
          out int nearestSubsegmentIndex)
        {
            TrackSegment trackSegment = (TrackSegment)null;
            nearestSubsegmentIndex = -1;
            float num = float.MaxValue;
            foreach (TrackSegment segment in segments)
            {
                float estDistSq;
                int nearestSubsegmentIndex1 = segment.GetNearestSubsegmentIndex(pos, out estDistSq);
                if ((double)estDistSq < (double)num)
                {
                    nearestSubsegmentIndex = nearestSubsegmentIndex1;
                    trackSegment = segment;
                    num = estDistSq;
                }
            }
            return trackSegment;
        }
    }
}
