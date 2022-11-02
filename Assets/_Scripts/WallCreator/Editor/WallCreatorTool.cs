using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Wall Creator")]
internal class WallCreatorTool : EditorTool
{
    [SerializeField] Texture2D m_ToolIcon;
    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Wall Creator",
            tooltip = "Wall Creator"
        };
    }
    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    private Vector3 firstPosition, secondPosition;
    private bool firstClick = true;
    private bool draw = false;

    public override void OnActivated()
    {
        base.OnActivated();

        draw = false;
        firstClick = true;
        firstPosition = secondPosition = Vector3.zero;
    }

    public override void OnToolGUI(EditorWindow window)
    {
        HandleMouse(SceneView.currentDrawingSceneView);
    }

    private void HandleMouse(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        EditorGUIUtility.AddCursorRect(sceneView.position, MouseCursor.Link);

        Event e = Event.current;

        Handles.color = Color.green;

        if (draw)
            Handles.DrawLine(firstPosition, GUIToGroundPosition(e.mousePosition), 3f);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (firstClick)
                    {
                        firstPosition = GUIToGroundPosition(e.mousePosition);
                        draw = true;
                        firstClick = false;
                    }
                    else
                    {
                        secondPosition = GUIToGroundPosition(e.mousePosition);
                        CreateCollider();

                        if (e.control)
                            firstPosition = secondPosition;
                        else
                        {
                            draw = false;
                            firstClick = true;
                            firstPosition = secondPosition = Vector3.zero;
                        }
                    }
                }
                break;
            case EventType.MouseMove:
                if (draw)
                    sceneView.Repaint();
                break;
        }
    }

    private void CreateCollider()
    {
        GameObject g = new GameObject("Wall", typeof(BoxCollider));
        BoxCollider collider = g.GetComponent<BoxCollider>();

        Vector3 middlePoint = (firstPosition + secondPosition) / 2;
        float length = Vector3.Distance(firstPosition, secondPosition);

        Quaternion rotation = Quaternion.LookRotation(firstPosition - secondPosition, Vector3.up);
        rotation *= Quaternion.Euler(0f, 90f, 0f);

        g.transform.SetParent(Selection.activeTransform);
        g.transform.position = middlePoint;
        g.transform.rotation = rotation;

        collider.size = new Vector3(length, 10f, 1f);
    }

    private Vector3 GUIToGroundPosition(Vector3 position)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(position);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            position = hit.point;

        position.y = 0;

        return position;
    }
}
