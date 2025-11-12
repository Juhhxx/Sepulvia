using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField, BoxGroup("Generation Settings")] private List<RoomBlockInfo> _roomBlocks;
    [SerializeField, BoxGroup("Generation Settings")] private RoomBlockInfo _endRoom;
    [SerializeField, BoxGroup("Generation Settings")] private int _maxPropagation;

    private List<Node> _openPoints;

    public struct Node
    {
        public Transform Point { get; private set; }
        public int PropagationLevel { get; private set; }

        public Node(Transform point, int propagationLevel)
        {
            Point = point;
            PropagationLevel = propagationLevel;
        }
    }

    private List<GameObject> _generatedBlocks;

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void GenerateDungeon()
    {
        _openPoints = new List<Node>();

        if (_generatedBlocks?.Count > 0)
        {
            foreach (GameObject go in _generatedBlocks) DestroyImmediate(go);
            _generatedBlocks.Clear();
        }
        else _generatedBlocks = new List<GameObject>();

        (GameObject block, RoomBlockInfo blockInfo) = ChooseRandomBlock();

        _generatedBlocks.Add(block);

        AddPoints(blockInfo.ConnectionPoints, 0);

        while (_openPoints.Count > 0)
        {
            Node connectFrom = _openPoints[0];

            _openPoints.Remove(connectFrom);

            GameObject newBlock;
            RoomBlockInfo newBlockInfo;

            Debug.Log($"PROPAGATION LEVEL : {connectFrom.PropagationLevel}", this);

            if (connectFrom.PropagationLevel == _maxPropagation)
            {
                newBlock = Instantiate(_endRoom.RoomPrefab, Vector3.zero, Quaternion.identity);
                newBlockInfo = newBlock.GetComponent<RoomBlockInfo>();
            }
            else
                (newBlock, newBlockInfo) = ChooseRandomBlock();

            _generatedBlocks.Add(newBlock);

            List<Transform> points = new List<Transform>(newBlockInfo.ConnectionPoints);

            int rnd = Random.Range(0, points.Count);

            Transform connectTo = points[rnd];

            points.Remove(connectTo);

            AddPoints(points, connectFrom.PropagationLevel + 1);

            Vector3 position = GetPositionOffset(connectFrom.Point, connectTo);

            newBlock.transform.Translate(position);

            float angle = GetRotationOffset(connectFrom.Point, connectTo);

            newBlock.transform.RotateAround(connectTo.position, Vector3.up, angle);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void ClearDungeon()
    {
        if (_generatedBlocks?.Count > 0)
        {
            foreach (GameObject go in _generatedBlocks) DestroyImmediate(go);
            _generatedBlocks.Clear();
        }
    }

    private (GameObject, RoomBlockInfo) ChooseRandomBlock()
    {
        GameObject block = null;
        RoomBlockInfo blockInfo = null;
        bool ok = false;

        int safety = 100;

        while (!ok)
        {
            int rnd = Random.Range(0, _roomBlocks.Count);

            block = Instantiate(_roomBlocks[rnd].RoomPrefab, Vector3.zero, Quaternion.identity);
            blockInfo = block.GetComponent<RoomBlockInfo>();

            if (safety == 0) break;

            ok = !blockInfo.IsOverlapping;

            if (!ok)
            {
                Debug.LogWarning($"WAS NOT OK, OVERLAPPING : {blockInfo.IsOverlapping}");
                DestroyImmediate(block);
            }

            safety--;
        }

        return (block, blockInfo);
    }

    private void AddPoints(List<Transform> points, int propagationLevel)
    {
        List<Node> nodes = new List<Node>();

        foreach (Transform point in points) nodes.Add(new Node(point, propagationLevel));

        _openPoints.AddRange(nodes);
    }

    private Vector3 GetPositionOffset(Transform fromPoint, Transform toPoint)
    {
        Vector3 r = Vector3.zero;

        r.x = fromPoint.position.x - toPoint.position.x;
        r.z = fromPoint.position.z - toPoint.position.z;

        Debug.Log($"POSITION OFFSET : {r}");

        return r;
    }
    
    private float GetRotationOffset(Transform fromPoint, Transform toPoint)
    {
        float r = 0;
        float diff = Mathf.DeltaAngle(fromPoint.eulerAngles.y, toPoint.eulerAngles.y);


        if (Mathf.Abs(diff) != 180)
        {
            r = Mathf.Round((180 - diff)/ 90f) * 90f % 360f;
        }

        Debug.Log($"ROTATION OFFSET : {r}");

        return r;
    }

}
