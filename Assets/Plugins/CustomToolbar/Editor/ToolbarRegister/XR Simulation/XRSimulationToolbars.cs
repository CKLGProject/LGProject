#if XRSimulation

using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Simulation;

namespace NKStudio
{
    public class XRSimulationToolbars
    {
        public static void OnToolbarGUI()
        {
            // 왼쪽 마진을 3정도 적용함.
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            EditorGUILayout.LabelField("XR Simulation", GUILayout.Width(80));
            bool currentXRSimulationEnable = CheckXRSimulation();
            bool simulationEnableToggle = EditorGUILayout.Toggle(currentXRSimulationEnable, GUILayout.Width(16));
            SetXRSimulation(simulationEnableToggle);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// XR Simulation의 활성화 여부를 설정합니다.
        /// </summary>
        /// <param name="active"></param>
        private static void SetXRSimulation(bool active)
        {
            var generalSettings =
                XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);

            if (generalSettings != null && generalSettings.AssignedSettings != null)
            {
                var activeLoaders = generalSettings.AssignedSettings.activeLoaders;
                bool simActive = activeLoaders != null && activeLoaders.Any(loader =>
                {
                    if (loader == null)
                        return false;

                    return loader.GetType() == typeof(SimulationLoader);
                });

                if (simActive != active)
                {
                    if (active)
                    {
                        // Add the loader
                        XRPackageMetadataStore.AssignLoader(generalSettings.AssignedSettings,
                            typeof(SimulationLoader).FullName,
                            BuildTargetGroup.Standalone);
                    }
                    else
                    {
                        // Remove the loader
                        XRPackageMetadataStore.RemoveLoader(generalSettings.AssignedSettings,
                            typeof(SimulationLoader).FullName,
                            BuildTargetGroup.Standalone);
                    }
                }
            }
        }

        /// <summary>
        /// XR Simulation이 활성화되어 있는지 확인합니다.
        /// </summary>
        /// <returns></returns>
        private static bool CheckXRSimulation()
        {
            XRGeneralSettings generalSettings =
                XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);

            if (generalSettings != null && generalSettings.AssignedSettings != null)
            {
                var activeLoaders = generalSettings.AssignedSettings.activeLoaders;
                bool simActive = activeLoaders != null && activeLoaders.Any(loader =>
                {
                    if (loader == null)
                        return false;

                    return loader.GetType() == typeof(SimulationLoader);
                });

                return simActive;
            }

            return false;
        }
    }
}
#endif