using UnityEngine;

public class WorkspaceController : MonoBehaviour
{

    public GameObject workspaceVisual;
    public GameObject workspaceHolder;

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.CurrentMode == MainManager.Mode.Building)
        {
            if (obj.button == ControllerConfig.LEFTSTICKUP)
                RotateWorkspace(MainManager.Direction.Up);
            else if (obj.button == ControllerConfig.LEFTSTICKDOWN)
                RotateWorkspace(MainManager.Direction.Down);
            else if (obj.button == ControllerConfig.LEFTSTICKLEFT)
                RotateWorkspace(MainManager.Direction.Left);
            else if (obj.button == ControllerConfig.LEFTSTICKRIGHT)
                RotateWorkspace(MainManager.Direction.Right);

            if (obj.button == ControllerConfig.RIGHTSTICK)
                ResetRotation();
        }

        else if (MainManager.Instance.CurrentMode == MainManager.Mode.Moving)
        {
            if (obj.button == ControllerConfig.LEFTSTICKX)
            {
                if (obj.value > 0)
                    MoveWorkspace(MainManager.Direction.Right);
                else
                    MoveWorkspace(MainManager.Direction.Left);
            }
            if (obj.button == ControllerConfig.LEFTSTICKY)
            {
                if (obj.value > 0)
                    MoveWorkspace(MainManager.Direction.Up);
                else
                    MoveWorkspace(MainManager.Direction.Down);
            }
            if (obj.button == ControllerConfig.LEFTSTICK)
                ResetPosition();

            if (obj.button == ControllerConfig.Y)
                MoveWorkspace(MainManager.Direction.Forward);

            if (obj.button == ControllerConfig.X)
                MoveWorkspace(MainManager.Direction.Backward);

            if (obj.button == ControllerConfig.A)
                MainManager.Instance.CurrentMode = MainManager.Mode.Building;
        }

        else if (MainManager.Instance.CurrentMode == MainManager.Mode.Scaling)
        {
            if (obj.button == ControllerConfig.UP)
                ScaleWorkspace(MainManager.Direction.Up);
            if (obj.button == ControllerConfig.DOWN)
                ScaleWorkspace(MainManager.Direction.Down);
            if (obj.button == ControllerConfig.A)
                MainManager.Instance.CurrentMode = MainManager.Mode.Building;
        }
    }

    private void RotateWorkspace(MainManager.Direction direction)
    {
        if (direction == MainManager.Direction.Up)
            transform.Rotate(45, 0, 0, Space.World);
        else if (direction == MainManager.Direction.Down)
            transform.Rotate(-45, 0, 0, Space.World);
        else if (direction == MainManager.Direction.Left)
            transform.Rotate(0, 45, 0, Space.World);
        else if (direction == MainManager.Direction.Right)
            transform.Rotate(0, -45, 0, Space.World);
    }

    private void MoveWorkspace(MainManager.Direction direction)
    {
        Vector3 translation = Vector3.zero;
        switch (direction)
        {
            case MainManager.Direction.Left:
                translation = -Camera.main.transform.right;
                break;
            case MainManager.Direction.Right:
                translation = Camera.main.transform.right;
                break;
            case MainManager.Direction.Up:
                translation = Camera.main.transform.up;
                break;
            case MainManager.Direction.Down:
                translation = -Camera.main.transform.up;
                break;
            case MainManager.Direction.Backward:
                translation = -Camera.main.transform.forward;
                break;
            case MainManager.Direction.Forward:
                translation = Camera.main.transform.forward;
                break;
        }

        Vector3 newTranslation = Vector3.zero;

        newTranslation.x = Utility.Round(translation.x);
        newTranslation.y = Utility.Round(translation.y);
        newTranslation.z = Utility.Round(translation.z);

        transform.position += newTranslation / 4;
    }

    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void ResetPosition()
    {
        transform.position = Camera.main.transform.forward;
    }

    private void ScaleWorkspace(MainManager.Direction direction)
    {
        if (direction == MainManager.Direction.Up)
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        else if (direction == MainManager.Direction.Down)
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void Validate()
    {
        MainManager.Instance.CurrentMode = MainManager.Mode.Building;
    }

    public void ToggleVisual(bool state)
    {
        workspaceVisual.SetActive(state);
    }
}
