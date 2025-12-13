using UnityEditor;

namespace Editor
{
    public class InspectorLock
    {
        [MenuItem("Tools/Toggle Inspector Lock %SPACE")]
        static void ToggleLock()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}