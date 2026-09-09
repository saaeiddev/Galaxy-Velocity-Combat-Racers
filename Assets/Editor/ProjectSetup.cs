using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace GalaxyVelocity.Editor {
    [InitializeOnLoad] public static class ProjectSetup {
        static ProjectSetup(){EditorApplication.delayCall+=Ensure;}
        [MenuItem("Galaxy Velocity/Prepare Project")]public static void Ensure(){
            if(EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling)return;
            const string path="Assets/Materials/GalaxyPipeline.asset";
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            if(!pipeline){
                var renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,"Assets/Materials/GalaxyRenderer.asset");
                pipeline=UniversalRenderPipelineAsset.Create(renderer);pipeline.msaaSampleCount=4;pipeline.supportsHDR=true;pipeline.shadowDistance=150;pipeline.useSRPBatcher=true;AssetDatabase.CreateAsset(pipeline,path);
            }
            GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
            PlayerSettings.companyName="Amir Saeid Dehghan";PlayerSettings.productName="Galaxy Velocity Combat Racers";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.colorSpace=ColorSpace.Linear;
            // Keep runtime-created URP materials available after player shader stripping.
            var graphics=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            var shaders=graphics.FindProperty("m_AlwaysIncludedShaders");var lit=Shader.Find("Universal Render Pipeline/Lit");bool found=false;
            for(int i=0;i<shaders.arraySize;i++)if(shaders.GetArrayElementAtIndex(i).objectReferenceValue==lit)found=true;
            if(!found&&lit){int i=shaders.arraySize;shaders.InsertArrayElementAtIndex(i);shaders.GetArrayElementAtIndex(i).objectReferenceValue=lit;graphics.ApplyModifiedProperties();}
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/AsterionCanyon.unity",true)};
            if(!File.Exists("Assets/Prefabs/Rook.prefab")){
                for(int i=0;i<3;i++){var o=ProceduralArt.Vehicle(i);foreach(var rend in o.GetComponentsInChildren<Renderer>()){
                    var mat=rend.sharedMaterial;if(!AssetDatabase.Contains(mat))AssetDatabase.CreateAsset(mat,"Assets/Materials/"+System.Guid.NewGuid().ToString("N")+".mat");
                }PrefabUtility.SaveAsPrefabAsset(o,"Assets/Prefabs/"+new[]{"Rook","Kite","Nyx"}[i]+".prefab");Object.DestroyImmediate(o);}
            }
            AssetDatabase.SaveAssets();
        }
        [MenuItem("Galaxy Velocity/Build Windows x64")]public static void BuildWindows(){Ensure();Directory.CreateDirectory("Builds/Windows");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/AsterionCanyon.unity"},locationPathName="Builds/Windows/GalaxyVelocity.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Build failed: "+report.summary.result);}
    }
}
