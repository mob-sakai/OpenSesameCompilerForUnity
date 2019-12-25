using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.OpenSesameCompilers
{
    internal class SettingWizard : ScriptableWizard
    {
        [SerializeField]
        bool m_OpenSesameCompiler;

        [SerializeField]
        string m_PublishFolder;

        [SerializeField]
        [HideInInspector]
        string m_Guid;

        [SerializeField]
        [HideInInspector]
        string m_AssemblyName;

        void OnEnable()
        {
            if (string.IsNullOrEmpty(m_Guid))
            {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var m_Setting = OpenSesameSetting.CreateFromAsmdef(path);
                m_PublishFolder = m_Setting.PublishFolder;
                m_OpenSesameCompiler = m_Setting.OpenSesameCompiler;
                m_Guid = AssetDatabase.AssetPathToGUID(path);
                m_AssemblyName = m_Setting.AssemblyName;
            }
            else if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(m_Guid)))
            {
                Close();
            }

            helpString = "Assembly Settings for " + m_AssemblyName
                + "\n - Open Sesame Compiler: Compile with 'OpenSesame' to access internals/privates."
                + "\n - Publish Folder: The folder path to publush (eg. Assets/Editor)";
            maxSize = new Vector2(1600, 210);
            minSize = new Vector2(450, 210);
        }

        void OnSelectionChange()
        {
            if (Selection.activeObject is AssemblyDefinitionAsset)
            {
                m_Guid = null;
                OnEnable();
            }
        }

        [MenuItem("Assets/OpenSesame Compiler/Setting", false)]
        static void OpenSettingWizard()
        {
            DisplayWizard<SettingWizard>("Assembly Settings", "Publish", "Save");
        }

        [MenuItem("Assets/OpenSesame Compiler/Setting", true)]
        static bool OpenSettingWizard_Valid()
        {
            return Selection.activeObject as AssemblyDefinitionAsset && !EditorApplication.isCompiling;
        }

        [MenuItem("Assets/OpenSesame Compiler/Publish", false)]
        static void Publish()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            OpenSesameSetting.PublishOrigin = Path.GetDirectoryName(path) + "/";
            var importer = AssetImporter.GetAtPath(path);
            if (importer == null)
                return;

            importer.SaveAndReimport();
        }

        [MenuItem("Assets/OpenSesame Compiler/Publish", true)]
        static bool Publish_Valid()
        {
            return OpenSettingWizard_Valid()
                && OpenSesameSetting.CreateFromAsmdef(AssetDatabase.GetAssetPath(Selection.activeObject)).OpenSesameCompiler;
        }

        /// <summary>
        /// This is called when the user clicks on the Create button.
        /// </summary>
        void OnWizardCreate()
        {
            var path = AssetDatabase.GUIDToAssetPath(m_Guid);
            OpenSesameSetting.PublishOrigin = Path.GetDirectoryName(path) + "/";
            Save(true);
        }

        /// <summary>
        /// Allows you to provide an action when the user clicks on the other button.
        /// </summary>
        void OnWizardOtherButton()
        {
            Save();
        }

        /// <summary>
        /// This is called when the wizard is opened or whenever the user changes something in the wizard.
        /// </summary>
        void OnWizardUpdate()
        {
            isValid = false;
            // Publish folder is empty.
            if (string.IsNullOrEmpty(m_PublishFolder.Trim()))
            {
                errorString = "Publish folder is empty";
            }
            // Publish folder does not exist.
            else if (!Directory.Exists(m_PublishFolder))
            {
                errorString = "Publish folder does not exist.";
            }
            else
            {
                isValid = !EditorApplication.isCompiling;
                errorString = "";
            }
        }

        public void Save(bool force = false)
        {
            var path = AssetDatabase.GUIDToAssetPath(m_Guid);
            var setting = OpenSesameSetting.CreateFromAsmdef(path);
            setting.PublishFolder = m_PublishFolder;
            setting.OpenSesameCompiler = m_OpenSesameCompiler;

            var importer = AssetImporter.GetAtPath(path);
            if (importer == null)
                return;

            var json = JsonUtility.ToJson(setting);
            if (importer.userData != json || force)
            {
                importer.userData = json;
                importer.SaveAndReimport();
            }
        }
    }

    [System.Serializable]
    internal class OpenSesameSetting
    {
        const string k_KeyPublishOrigin = "OpenSesame_PublishOrigin";
        static readonly Regex s_regName = new Regex("\"name\"\\s*:\\s*\"([^\"]+)\"", RegexOptions.Compiled);

        public static string PublishOrigin
        {
            get { return EditorPrefs.GetString(k_KeyPublishOrigin); }
            set { EditorPrefs.SetString(k_KeyPublishOrigin, value); }
        }

        public string PublishFolder;
        public bool OpenSesameCompiler;

        public string AssemblyName { get; private set; }

        public static OpenSesameSetting CreateFromAsmdef(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(Path.GetFullPath(path)))
                return new OpenSesameSetting();

            var importer = AssetImporter.GetAtPath(path);
            if (importer == null)
                return new OpenSesameSetting();

            OpenSesameSetting setting = null;
            try
            {
                setting = JsonUtility.FromJson<OpenSesameSetting>(importer.userData);
            }
            catch
            {
            }

            setting = setting ?? new OpenSesameSetting();

            setting.AssemblyName = s_regName.Match(File.ReadAllText(path)).Groups[1].Value;

            if (string.IsNullOrEmpty(setting.PublishFolder))
                setting.PublishFolder = Path.GetDirectoryName(path);

            return setting;
        }
    }
}
