using UnityEngine;

public class WorkspaceController : MonoBehaviour
{

    public GameObject workspaceVisual;
    public GameObject workspaceHolder;

    private Direction direction;

    private void Update()
    {
        if (MainManager.Instance.CurrentMode == MainManager.Mode.Building)
        {
            direction = CInput.GetRightAxisDirection();
            if(direction != Direction.None)
            {
                RotateWorkspace(direction);
            }

            if (CInput.rightStick)
                ResetRotation();
        }

        else if (MainManager.Instance.CurrentMode == MainManager.Mode.Moving)
        {
            direction = CInput.GetLeftAxisDirection();
            if (direction != Direction.None)
            {
                MoveWorkspace(direction);
            }

            if (CInput.leftStick)
                ResetPosition();

            if (CInput.yHold)
                MoveWorkspace(Direction.Forward);

            if (CInput.xHold)
                MoveWorkspace(Direction.Backward);

            if (CInput.aUp)
                MainManager.Instance.CurrentMode = MainManager.Mode.Building;
        }

        else if (MainManager.Instance.CurrentMode == MainManager.Mode.Scaling)
        {
            if (CInput.dpadX > 0.5)
                ScaleWorkspace(Direction.Up);
            else if (CInput.dpadX < -0.5)
                ScaleWorkspace(Direction.Down);

            if (CInput.aUp)
                MainManager.Instance.CurrentMode = MainManager.Mode.Building;
        }
    }

    private void RotateWorkspace(Direction direction)
    {
        if (direction == Direction.Up)
            transform.Rotate(45, 0, 0, Space.World);
        else if (direction == Direction.Down)
            transform.Rotate(-45, 0, 0, Space.World);
        else if (direction == Direction.Left)
            transform.Rotate(0, 45, 0, Space.World);
        else if (direction == Direction.Right)
            transform.Rotate(0, -45, 0, Space.World);
    }

    private void MoveWorkspace(Direction direction)
    {
        Vector3 translation = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                translation = -Camera.main.transform.right;
                break;
            case Direction.Right:
                translation = Camera.main.transform.right;
                break;
            case Direction.Up:
                translation = Camera.main.transform.up;
                break;
            case Direction.Down:
                translation = -Camera.main.transform.up;
                break;
            case Direction.Backward:
                translation = -Camera.main.transform.forward;
                break;
            case Direction.Forward:
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

    private void ScaleWorkspace(Direction direction)
    {
        if (direction == Direction.Up)
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        else if (direction == Direction.Down)
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
