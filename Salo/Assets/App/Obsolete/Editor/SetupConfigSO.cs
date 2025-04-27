using UnityEditor;
using UnityEngine;

namespace Salo.Infrastructure.EditorExtensions
{
    /// <summary>
    /// Editor-only asset to store data for post-installation setup
    /// </summary>
    [CreateAssetMenu(fileName = "SetupConfig", menuName = "Salo/Config/Setup Config (Editor)")]
    public class SetupConfigSO : ConfigSOBase
    {
        [Tooltip("The folder to move out of Packages into Assets")]
        [SerializeField] private DefaultAsset userModifiableFolder;
        public DefaultAsset UserModifiableFolder => userModifiableFolder;

        [Tooltip("Folder name under Assets where the Framework content are moved to")]
        [SerializeField] private string frameworkFolderName;
        public string FrameworkFolderName => frameworkFolderName;
    }
}
