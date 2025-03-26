using UnityEditor;

/// <summary>
/// Unity Editor menu toggle to enable/disable bootstrapping on Editor Play
/// </summary>
public static class BootstrapOnPlayMenuItem
{
    private const string MENU_NAME = "Salo/Bootstrap on Editor Play";

    [MenuItem(MENU_NAME)]
    private static void ToggleMenuItem()
    {
        // The menu item was clicked. Toggle value
        var newValue = !IsBootstrapOnPlayEnabled();
        Menu.SetChecked(MENU_NAME, newValue);
        EditorPrefs.SetBool(MENU_NAME, newValue);
    }

    [MenuItem(MENU_NAME, true)]
    private static bool ToggleMenuItemValidator()
    {
        // Set the checkbox value
        Menu.SetChecked(MENU_NAME, IsBootstrapOnPlayEnabled());
        return true;
    }

    // Set to public, to be checked during Editor bootstrapping
    public static bool IsBootstrapOnPlayEnabled()
    {
        // Default to true if setting is not set
        if (!EditorPrefs.HasKey(MENU_NAME))
        {
            EditorPrefs.SetBool(MENU_NAME, true);
            return true;
        }

        return EditorPrefs.GetBool(MENU_NAME);
    }
}
