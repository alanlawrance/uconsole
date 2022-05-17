using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    public static void BuildWin64()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = "win64/uconsole.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.scenes = GetScenes();

        Debug.Log("Building Win64");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Built Win64");
    }

    private static string[] GetScenes()
    {
        return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
    }
}
