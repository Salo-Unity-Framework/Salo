using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Salo.Infrastructure.EditorExtensions
{
    /// <summary>
    /// This class handles the various steps required after installation
    /// </summary>
    public static class PostInstallationSetup
    {
        // This is meant to be run on projects that install the Framework,
        // and not on the project used for Framework development.
        [MenuItem("Salo/Run first-time setup")]
        private static void setup()
        {
            moveUserModifiableFolder();
            markAddressable();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void moveUserModifiableFolder()
        {
            var setupConfig = SOLoaderEditor.GetUniqueAsset<SetupConfigSO>();

            // Full source path
            var sourcePath = AssetDatabase.GetAssetPath(setupConfig.UserModifiableFolder);
            if (!AssetDatabase.IsValidFolder(sourcePath))
            {
                Debug.LogError($"Invalid source folder: {sourcePath}");
                return;
            }

            // Full target path
            var targetParentFolderPathWithoutAssets = setupConfig.FrameworkFolderName; // Salo
            var targetParentFolderPath = Path.Combine("Assets/", targetParentFolderPathWithoutAssets); // Assets/Salo
            var targetFolderPath = Path.Combine("Assets", setupConfig.FrameworkFolderName, Path.GetFileName(sourcePath)); // Assets/Salo/UserModifiable

            // Create the target folder if needed
            if (!AssetDatabase.IsValidFolder(targetParentFolderPath))
            {
                // Use AssetDatabase.CreateFolder instead of System.IO to create folders
                // so Unity can process the folders correctly. We need to loop down
                // through each folder to create them one by one.
                var parent = "Assets";
                
                foreach (var folder in targetParentFolderPathWithoutAssets.Split(Path.DirectorySeparatorChar))
                {
                    string currentPath = Path.Combine(parent, folder);
                    if (!AssetDatabase.IsValidFolder(currentPath))
                    {
                        AssetDatabase.CreateFolder(parent, folder);
                    }
                    parent = currentPath;
                }
            }

            // Move the folder (will preserve all asset GUIDs)
            var error = AssetDatabase.MoveAsset(sourcePath, targetFolderPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Error moving folder: {error}");
                return;
            }

            Debug.Log("Moved UserModifiable folder from Packages to Assets");
        }

        private static void markAddressable()
        {
            Debug.Log("Marking assets as Addressable...");

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (null == settings)
            {
                Debug.LogError("AddressableAssetSettingsDefaultObject.Settings is null");
                return;
            }

            // Get the folder path from the DefaultAsset
            var setupConfig = SOLoaderEditor.GetUniqueAsset<SetupConfigSO>();
            string folderPath = AssetDatabase.GetAssetPath(setupConfig.UserModifiableFolder);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError($"Path '{folderPath}' is not a valid folder.");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(folderPath);

            // Check if the folder is already an Addressable
            var entry = settings.FindAssetEntry(guid);
            if (null != entry)
            {
                Debug.Log($"Folder '{folderPath}' is already an Addressable");
                return;
            }

            // Mark as Addressable. NOTE: Moving to Default group
            settings.CreateOrMoveEntry(guid, settings.DefaultGroup);

            Debug.Log("Marked UserModifiable data as Addressable");
        }
    }
}
