using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;
    public List<GameObject> linesList;
    [SerializeField] private int metroMatrixSize;   //determines BOTH the width and height of the metro matrix
    [SerializeField] private int linesQuantity;     //amount of lines in the map
    [SerializeField] private int maxLineLength;     //determine the max number a line can have
    private int lineIDCounter = 0;
    [SerializeField] private int minIntersectionPoints;              //Each lines' min intersection points
    [SerializeField] private int maxIntersectionPoints;              //Each line's max intersection points
    [SerializeField] private GameObject cameraParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < linesQuantity; i++)
        {
            CreateLine();
        }
        foreach (GameObject lineObj in linesList)
        {
            Line line = lineObj.GetComponent<Line>();
            Debug.Log(line.lineVertexList);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void CreateLine()
    {
        // Instantiate the line object from prefab
        GameObject lineObj = Instantiate(linePrefab);
        lineObj.transform.SetParent(cameraParent.transform);           
        linesList.Add(lineObj);                                 // Add it to the list of lines

        // Get the Line component
        Line line = lineObj.GetComponent<Line>();               
        line.lineID = lineIDCounter++;                          // Assign a unique ID for this line

        Vector2 currentPoint; // Will store the current position of the line

        // Determine starting point
        if (lineIDCounter == 1) // First line created
        {
            // Pick a random starting point within the metro matrix
            int startingX = Random.Range(0, metroMatrixSize + 1);
            int startingY = Random.Range(0, metroMatrixSize + 1);
            Vector2 startingPoint = new Vector2(startingX, startingY);
            currentPoint = startingPoint;

            // Add starting point to this line's vertex list
            line.lineVertexList.Add(startingPoint);
        }
        else // Not the first line
        {
            // Filter existing lines that have at least one vertex
            List<GameObject> linesWithVertices = linesList.FindAll(l => l.GetComponent<Line>().lineVertexList.Count > 0);

            if (linesWithVertices.Count > 0)
            {
                // Pick a random existing line with vertices
                GameObject selectedLineObj = linesWithVertices[Random.Range(0, linesWithVertices.Count)];
                Line selectedLine = selectedLineObj.GetComponent<Line>();

                // Pick a random vertex from that line as starting point
                Vector2 startingPoint = selectedLine.lineVertexList[Random.Range(0, selectedLine.lineVertexList.Count)];

                currentPoint = startingPoint;

                // Add the starting point to this line's vertex list
                line.lineVertexList.Add(startingPoint);
            }
            else
            {
                // Fallback: if no other line has vertices, pick a random point
                int startingX = Random.Range(0, metroMatrixSize + 1);
                int startingY = Random.Range(0, metroMatrixSize + 1);
                Vector2 startingPoint = new Vector2(startingX, startingY);
                currentPoint = startingPoint;
                line.lineVertexList.Add(startingPoint);
            }
        }

        // Track intersections
        int currentIntersections = 0; // Counter for how many intersections this line has
        GameObject lastIntersectedLine = null; // Track the last line intersected to avoid sequential intersections
        int attempts = 0; // Safeguard counter to prevent infinite loops

        // Generate the rest of the points for this line
        for (int i = 0; i < maxLineLength - 1 ; i++) //Remove in between parentheses for full line length
        {
            attempts++;
            if (attempts > 300) break; // Stop trying after 300 failed attempts to prevent infinite loops

            // Randomly create a vector translating direction for the line to grow
            Vector2 nextDirection = new Vector2(
                Random.Range(-1, 2) * (Random.Range(0, 2) * 2 - 1),
                Random.Range(-1, 2) * (Random.Range(0, 2) * 2 - 1)
            );

            Vector2 potentialPoint = currentPoint + nextDirection; // The point we want to add next

            // Skip if this point already exists in this line
            if (line.lineVertexList.Contains(potentialPoint)) continue;

            // Check if the point intersects with any other line
            GameObject intersectedLine = null; // Will hold the line we intersect (if any)
            foreach (GameObject otherLineObj in linesList)
            {
                if (otherLineObj == lineObj) continue; // Skip itself

                Line otherLine = otherLineObj.GetComponent<Line>();
                if (otherLine.lineVertexList.Contains(potentialPoint))
                {
                    intersectedLine = otherLineObj; // Found an intersection
                    break; // Stop checking other lines
                }
            }

            bool intersects = intersectedLine != null; // True if the point intersects another line

            // Determine whether intersections are needed or maxed
            bool needIntersection = currentIntersections < minIntersectionPoints; // Still need intersections
            bool maxIntersectionReached = currentIntersections >= maxIntersectionPoints; // Reached max allowed intersections

            // Skip this point if it violates intersection rules
            if ((needIntersection && !intersects) || (maxIntersectionReached && intersects)) continue;

            // Skip if intersecting the same line as the previous point
            if (intersects && intersectedLine == lastIntersectedLine) continue;

            // Valid point: add to the line
            currentPoint = potentialPoint; // Update current point
            line.lineVertexList.Add(currentPoint); // Add it to the line's vertex list

            if (intersects)
            {
                currentIntersections++; // Increment intersections counter
                lastIntersectedLine = intersectedLine; // Track which line was intersected
            }
            else
            {
                lastIntersectedLine = null; // Reset if no intersection
            }

            i++; // Move to next point in line
        }
    }






}
