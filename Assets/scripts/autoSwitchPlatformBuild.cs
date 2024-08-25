using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AutoSwitchPlatform
{
    static AutoSwitchPlatform()
    {
        
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            
            Debug.Log("Switching to iOS platform...");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        }
    }
}
