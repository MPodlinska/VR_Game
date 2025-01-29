using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using PDollarGestureRecognizer;
using UnityEngine.SceneManagement;

public class MovementRecognizer : MonoBehaviour
{
    public XRNode inputSource;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button inputButton;
    public float inputTreshold = 0.1f;
    public Transform movementSource;

    public float newPositionTresholdDistance = 0.05f;
    public GameObject debugCubePrefab;

    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    private List<Point> _points = new List<Point>();
    public List<Gesture> gestures = new List<Gesture>();
    public TextAsset[] gestureFiles;

    private List<string> requiredSequence = new List<string> { "line", "N" };
    private static int currentIndex = 0;

    public int points;

    // Start is called before the first frame update
    void Start()
    {
        points = ThrowCollision.total_points;

        foreach (var file in gestureFiles)
        {
            gestures.Add(GestureIO.ReadGestureFromXML(file.text));
        }
    }

    // Update is called once per frame
    void Update()
    {
        points = ThrowCollision.total_points;

        if (points >= 4)
        {
            UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputTreshold);
            if (!isMoving && isPressed)
            {
                StartMovement();
            }
            else if (isMoving && !isPressed)
            {
                EndMovement();
            }
            else if (isMoving && isPressed)
            {
                UpdateMovement();
            }
        }
    }

    void StartMovement()
    {
        Debug.Log("Start Movement");
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        if (debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
    }

    void EndMovement()
    {
        Debug.Log("End Movement");
        isMoving = false;

        Point[] pointArray = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture candidate = new Gesture(pointArray);
        Result result = PointCloudRecognizer.Classify(candidate, gestures.ToArray());
        Debug.Log($"Recognized gesture: {result.GestureClass} with score: {result.Score}");

        if (result.Score > 0.4)
        {
            Debug.Log("Correct gesture! " + currentIndex + " " + result.Score);
            currentIndex++;

            if (currentIndex >= 2 && result.Score > 0.4)
            {
                Debug.Log("Sequence completed!");
                currentIndex = 0;
                 SceneManager.LoadScene("EndGame");
            }
        }
        else
        {
            Debug.Log("Wrong gesture! Try again.");
            // currentIndex = 0;
        }
    }

    void UpdateMovement()
    {
        Debug.Log("Update Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionTresholdDistance)
        {
            positionsList.Add(movementSource.position);

            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
    }

}