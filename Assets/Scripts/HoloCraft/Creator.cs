using HoloToolkit.Sharing.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    public GameObject objectToPlace;
    private Block currentObject;
    public Block firstBlock;

    public Block _hoveredObject;

    public Block HoveredObject
    {
        get { return _hoveredObject; }

        private set
        {
            if (_hoveredObject != null)
                _hoveredObject.RestoreDefaultColor();

            if (value != null)
            {
                currentObject.Hide();
                value.SetMaterialColor(Color.yellow);
            }
            else
            {
                currentObject.UnHide();
            }

            _hoveredObject = value;
        }
    }

    //Position and rotation informations
    private Vector3 currentPosition;
    private Quaternion previousRot;
    private Direction direction;

    private bool _isValid;

    private Creation creation;

    public GameObject workspaceHolder;

    bool IsValid
    {
        get { return _isValid; }

        set
        {
            if (value == true)
                currentObject.GetComponent<Block>().SetMaterialColor(Color.green);
            else
                currentObject.GetComponent<Block>().SetMaterialColor(Color.red);

            _isValid = value;
        }
    }

    private void Update()
    {
        if (MainManager.Instance.currentMode != MainManager.Mode.Building) return;

        direction = CInput.GetDpadDirection();
        if (direction != Direction.None)
            Translate(direction);
        if (CInput.yDown)
            Translate(Direction.Forward);
        if (CInput.xDown)
            Translate(Direction.Backward);

        direction = CInput.GetLeftAxisDirection();
        if (direction != Direction.None)
            Rotate(direction);

        if (CInput.aDown)
        {
            if (creation.GetBlock(currentPosition) == null && IsValid == true)
            {
                Validate(currentPosition, currentObject.gameObject);
                PlaceNext();
            }
        }
        if (CInput.bDown)
        {
            if (_hoveredObject != null && HoveredObject != firstBlock)
            {
                creation.RemoveBlock(HoveredObject.transform.localPosition);
                PlaceNext();
            }
        }
    }

    public void StartPlacing(Vector3 initialPos, Creation creation, GameObject workspaceHolder)
    {
        this.creation = creation;
        this.workspaceHolder = workspaceHolder;
        this.currentPosition = initialPos;

        firstBlock = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspaceHolder).GetComponent<Block>();
        firstBlock.transform.localPosition = currentPosition;
        Validate(currentPosition, firstBlock.gameObject);

        PlaceNext();
        Translate(Direction.Up);
    }

    public void PlaceNext()
    {
        if (currentObject != null)
            Destroy(currentObject.gameObject);

        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspaceHolder).GetComponent<Block>();
        currentObject.transform.localPosition = currentPosition;
        currentObject.transform.localRotation = previousRot;
        CheckValid();
    }

    private bool CheckPosition(Vector3 position)
    {
        if (position.x < 0 || position.x >= creation.maxWidth ||
            position.y < 0 || position.y >= creation.maxHeight ||
            position.z < 0 || position.z >= creation.maxDepth)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Rotate(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                currentObject.transform.Rotate(0, -90, 0, Space.Self);
                break;
            case Direction.Right:
                currentObject.transform.Rotate(0, 90, 0, Space.Self);
                break;
            case Direction.Up:
                currentObject.transform.Rotate(90, 0, 0, Space.Self);
                break;
            case Direction.Down:
                currentObject.transform.Rotate(-90, 0, 0, Space.Self);
                break;
        }
    }

    public void Translate(Direction direction)
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

        Vector3 newTranslation = workspaceHolder.transform.InverseTransformDirection(translation);

        newTranslation.x = Utility.Round(newTranslation.x);
        newTranslation.y = Utility.Round(newTranslation.y);
        newTranslation.z = Utility.Round(newTranslation.z);

        Vector3 newPosition = currentObject.transform.localPosition + newTranslation;


        if (!CheckPosition(newPosition)) return;

        currentPosition += newTranslation;
        currentObject.transform.localPosition = currentPosition;

        CheckValid();
    }

    private void CheckValid()
    {
        IsValid = false;
        HoveredObject = creation.GetBlock(currentPosition);
    }

    public void ChangeObject(GameObject newObject)
    {
        previousRot = currentObject.transform.localRotation;
        Destroy(currentObject.gameObject);
        objectToPlace = newObject;
        PlaceNext();
    }

    public void Validate(Vector3 position, GameObject block)
    {
        previousRot = block.transform.localRotation;
        creation.AddBlock(position, block.GetComponent<Block>());
        block.GetComponent<Block>().RestoreDefaultColor();
        block.GetComponent<Block>().DisableSnapPoints();
        currentObject = null;
    }

    public void SnapColliding()
    {
        if (HoveredObject == null && IsValid == false)
        {
            IsValid = true;
        }
    }

    public void StopCreation()
    {
        Destroy(currentObject.gameObject);
        if (HoveredObject != null)
            HoveredObject.RestoreDefaultColor();
    }
}
