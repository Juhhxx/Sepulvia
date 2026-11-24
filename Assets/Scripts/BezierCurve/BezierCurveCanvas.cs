using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
public class BezierCurveCanvas : MonoBehaviour
{
    //[SerializeField] Canvas _canvas;
    [SerializeField] RectTransform _beadStringParent;
    [SerializeField] List<Transform> _PList = new List<Transform>();
    [SerializeField] List<GameObject> _BList = new List<GameObject>();
    [SerializeField] float _bezierIncrement = .1f;
    [SerializeField] GameObject _beadPrefab;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] List<Vector3> _linePoints = new List<Vector3>();
    [SerializeField] int _lineNumber = 1;
    [SerializeField] int _maxBeadRotation = 180;
    [SerializeField] int _minBeadRotation = 0;
    [SerializeField] float _beadRattleOffset;
    [SerializeField] float _rattleSpeed = .5f;
    int _lineCounter = 0;
    bool _shaking = false; //test bool 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(_shaking)
        {
            RattleBeads();
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 P0, Vector3 P1, Vector3 P2) // 0<= t <= 1
    {
        float oneMinusT = 1 - t;
        Vector3 point = oneMinusT * (oneMinusT * P0 + t * P1) + t * (oneMinusT * P1 + t * P2); 
        return point;
    }

    [Button]
    private void InstantiateBeadString() //Verificar a distancia entre cada instanciação e se houver distancia que seja maior que a sprite, instanciar uma extra
    {
        for(int l = 1; l <= _lineNumber; l++) //Por cada linha que existe
        {
            for(float t = 0; t<=1; t += _bezierIncrement) // Instancia se a coleção de beads
            {
                float rotationValue = Random.Range(_minBeadRotation, _maxBeadRotation);
                Vector3 nextPoint = CalculateBezierPoint(t, _PList[_lineCounter].position, _PList[_lineCounter + 1].position, _PList[_lineCounter + 2].position);
                

                _BList.Add(Instantiate(_beadPrefab, nextPoint, Quaternion.Euler(0, 0, rotationValue), _beadStringParent));
                _linePoints.Add(nextPoint);
            }
            //_linePoints.Add(CalculateBezierPoint(1 + _bezierIncrement, _PList[_lineCounter].position, _PList[_lineCounter + 1].position, _PList[_lineCounter + 2].position));

            _lineCounter += 2;
        }
        Vector3[] points = _linePoints.ToArray();

        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(points);

        Vector3 offset = new Vector3(0f, 0f, -.1f);
        //_beadStringParent.position = _beadStringParent.position + offset;

        _beadStringParent.localPosition = new Vector3 (_beadStringParent.localPosition.x, _beadStringParent.localPosition.y, _beadStringParent.localPosition.z - .1f);

        _shaking = true;
    }

    private void RattleBeads() //Recebe t, um valor normalizado da função seno
    {
        foreach(GameObject child in _BList)
        {
            float t = Mathf.Sin(Time.time * _rattleSpeed + child.transform.localPosition.x) * _beadRattleOffset;
            child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y + t, child.transform.localPosition.z);
        }
    }
}
