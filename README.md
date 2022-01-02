# Distance TrackAttachToTrack

## Features:
- TrackAttachToTrackTool (Shortcut <kbd>Ctrl+Shift+A</kbd>.), which attaches all selected objects to the first spline selected. Even other splines. Detatches when an attached object is selected.
- TrackAttachToTrackTool+ (Shortcut <kbd>Ctrl+Alt+A</kbd>.), which attaches all selected objects to the first spline selected. Even other splines. Never detaches, so can be used to attach 2 splines to eachother, for example.
- An option to enable the ability to group track attachments to objects that aren't the track they are attached to (But it is disabled by default.). Normally, you can only group track attachments with the track they are attached to, but enabling this removes this restriction. The reason this is an option and not a default feature is that it can produce some weird effects under certain circumstances. Say you attach an object to a track, group that object, and then delete the track, and then ungroup the object. The object will then have a track attachment to a nonexistant track, which can cause the editor to freak out and crash under certain circumstances (Though you can remove it just fine if you don't try to do anything else with it first, like duplicating the object or something.). It's not unstable so long as you don't put it in such a position though.


