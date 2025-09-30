using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class Line : MonoBehaviour
{
    [ReadOnly]
    public int lineID;
    [ReadOnly]
    public List<Vector2> lineVertexList = new List<Vector2>();
    private LineRenderer lineRenderer;
    [SerializeField] List<Color> colorList = new List<Color>(); 
    private Color lineColor;
    [SerializeField] GameObject sphere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Start()
    {
        lineColor = colorList[lineID]; //Define the lineColor based on ID
        UpdateLineRender(); //Draw all the lines

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLineRender() //Lines are static but called it update just in case we want dynamic line points
    {
        if (lineVertexList.Count > 0)
        {
            lineRenderer.positionCount = lineVertexList.Count;

            // Convert Vector2 -> Vector3 (z=0)
            Vector3[] positions = new Vector3[lineVertexList.Count];
            for (int i = 0; i < lineVertexList.Count; i++)
            {
                positions[i] = new Vector3(lineVertexList[i].x, lineVertexList[i].y, 0f);
            }

            lineRenderer.SetPositions(positions);  //sets th position of each points in the line

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;

            foreach (Vector3 position in positions)
            {
                Instantiate(sphere, position, Quaternion.identity);
            }

        }
    }
}
