using UnityEngine;
using UnityEditor;

public static class Shortcuts
{
    const string FindSourceAssetString      = "GameObject/Find Source Asset %e";            // ctrl-e / command-e
    const string HideSelectionString        = "View/Hide Selection %h";                     // ctrl-h / command-h
    const string ViewTopDownString          = "View/Toggle View Top-Down %F1";              // ctrl-F1 / command-F1
    const string ViewLeftRightString        = "View/Toggle View Left-Right %F2";            // ctrl-F2 / command-F2
    const string ViewFrontBackString        = "View/Toggle View Front-Back %F3";            // ctrl-F3 / command-F3
    const string ToggleOrthogonalString     = "View/Toggle View Perspective-Orthogonal %`"; // ctrl-` / command-`

    // Hide/show selected objects
    #region Hide Selection
    [MenuItem(HideSelectionString)]
    static void HideSelection()
    {
        int hidden_objects = 0;
        int shown_objects = 0;

        foreach (var obj in Selection.gameObjects)
        {
            if (obj.activeSelf)
                shown_objects++;
            else
                hidden_objects++;
        }

        if (hidden_objects == 0 && shown_objects == 0)
            return;

        bool toggle = (hidden_objects != 0);
        foreach (var obj in Selection.gameObjects)
        {
            obj.SetActive(toggle);
        }
    }
    #endregion


    // Find the source asset in the project that is currently selected
    #region Find Source Asset
    [MenuItem(FindSourceAssetString, true)]
    public static bool ValidateFindSourceAsset()
    {
        var gameObject = Selection.activeGameObject;
        if (gameObject == null) return false;

        if (PrefabUtility.GetPrefabParent(gameObject) != null) return true;
        var meshFilter = gameObject.GetComponent<MeshFilter>();

        return (meshFilter != null && meshFilter.sharedMesh != null && AssetDatabase.Contains(meshFilter.sharedMesh));
    }
    [MenuItem(FindSourceAssetString)]
    public static void FindSourceAsset()
    {
        var gameObject = Selection.activeGameObject;
        if (gameObject == null)
            return;

        var parent = PrefabUtility.GetPrefabParent(gameObject);
        if (parent != null)
        {
            Selection.activeObject = parent;
            EditorGUIUtility.PingObject(gameObject);
            return;
        }

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            return;

        var mesh = meshFilter.sharedMesh;
        if (mesh == null)
            return;
        
        Selection.activeObject = mesh;
    }
    #endregion


    // Toggle between top and down view by pressing ctrl-F1 or command-F1
    #region Set View Top-Down
    [MenuItem(ViewTopDownString, true)]
    public static bool ValidateSetViewTopDown() { return SceneView.lastActiveSceneView != null; }
    [MenuItem(ViewTopDownString)]
    public static void SetViewTopDown()
    {
        var view = SceneView.lastActiveSceneView;
        if (view == null)
            return;

        Quaternion topDirection     = kDirectionRotations[1];
        Quaternion downDirection = kDirectionRotations[4];
        ToggleBetweenViewDirections(view, topDirection, downDirection);
    }
    #endregion


    // Toggle between left and right view by pressing ctrl-F2 or command-F2
    #region Set View Left-Right
    [MenuItem(ViewLeftRightString, true)]
    public static bool ValidateSetViewLeftRight() { return SceneView.lastActiveSceneView != null; }
    [MenuItem(ViewLeftRightString)]
    public static void SetViewLeftRight()
    {
        var view = SceneView.lastActiveSceneView;
        if (view == null)
            return;

        Quaternion leftDirection    = kDirectionRotations[3];
        Quaternion rightDirection    = kDirectionRotations[0];
        ToggleBetweenViewDirections(view, leftDirection, rightDirection);
    }
    #endregion


    // Toggle between front and back view by pressing ctrl-F3 or command-F3
    #region Set View Front-Back
    [MenuItem(ViewFrontBackString, true)]
    public static bool ValidateSetFrontBack() { return SceneView.lastActiveSceneView != null; }
    [MenuItem(ViewFrontBackString)]
    public static void SetViewFrontBack()
    {
        var view = SceneView.lastActiveSceneView;
        if (view == null)
            return;

        Quaternion frontDirection    = kDirectionRotations[2];
        Quaternion backDirection    = kDirectionRotations[5];
        ToggleBetweenViewDirections(view, frontDirection, backDirection);
    }
    #endregion


    // Toggle between perspective and orthogonal view by pressing ctrl-` or command-`
    #region Toggle Orthogonal
    [MenuItem(ToggleOrthogonalString, true)]
    public static bool ValidateToggleOrthogonal() { return SceneView.lastActiveSceneView != null; }
    [MenuItem(ToggleOrthogonalString)]
    public static void ToggleOrthogonal()
    {
        var view = SceneView.lastActiveSceneView;
        if (view == null)
            return;

        view.LookAt(view.pivot, view.rotation, view.size, !view.orthographic);
    }
    #endregion



    #region Toggle Between View Directions (helper function)
    static Quaternion[] kDirectionRotations = {
        Quaternion.LookRotation (new Vector3 (-1, 0, 0)),    // right
        Quaternion.LookRotation (new Vector3 ( 0,-1, 0)),    // top
        Quaternion.LookRotation (new Vector3 ( 0, 0,-1)),    // front
        Quaternion.LookRotation (new Vector3 ( 1, 0, 0)),    // left
        Quaternion.LookRotation (new Vector3 ( 0, 1, 0)),    // down
        Quaternion.LookRotation (new Vector3 ( 0, 0, 1)),    // back
    };

    const float kCompareEpsilon = 0.0001f;
    static void ToggleBetweenViewDirections(SceneView view, Quaternion primaryDirection, Quaternion alternativeDirection)
    {
        Vector3 direction = primaryDirection * Vector3.forward;
        float dot = Vector3.Dot(view.camera.transform.forward, direction);
        if (dot < 1.0f - kCompareEpsilon) { view.LookAt(view.pivot, primaryDirection,     view.size, view.orthographic); }
        else                              { view.LookAt(view.pivot, alternativeDirection, view.size, view.orthographic); }
    }
    #endregion
}
