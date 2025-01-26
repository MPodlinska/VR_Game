using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 15;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;

    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Whiteboard _tempboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;

    private List<Point> _points = new List<Point>();
    public List<Gesture> gestures = new List<Gesture>();
    public TextAsset[] gestureFiles;

    private List<string> requiredSequence = new List<string> { "line", "N" };
    private int currentIndex = 0;

    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y;

        foreach(var file in gestureFiles)
        {
            gestures.Add(GestureIO.ReadGestureFromXML(file.text));
        }
    }

    void Update()
    {
        Draw();
      //  if(Input.GetKeyDown(KeyCode.Space))
       // {
       //     RecognizeGesture();
       // }
    }

    private void Draw()
    {
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
        {
            if(_touch.transform.CompareTag("Whiteboard"))
            {
                if(_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                    _tempboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                var x = (int)((_touchPos.x * _whiteboard.textureSize.x) - (_penSize / 2));
                var y = (int)((_touchPos.y * _whiteboard.textureSize.y) - (_penSize / 2));


                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;

                if(_touchedLastFrame)
                {
                    _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                    for(float f = 0.01f; f<1.00f;f+=0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                    }

                    transform.rotation = _lastTouchRot;

                    _whiteboard.texture.Apply();

                    _points.Add(new Point(x, y, 0));
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        

        if(_points.Count > 0 && !_touchedLastFrame)
        {
            RecognizeGesture();
            
            _points.Clear();
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }

    private void RecognizeGesture()
    {
        if(_points.Count == 0) return;

        Gesture candidate = new Gesture(_points.ToArray());
        Result result = PointCloudRecognizer.Classify(candidate, gestures.ToArray());
        Debug.Log($"Recognized gesture: {result.GestureClass} with score: {result.Score}");

        if (result.GestureClass == requiredSequence[currentIndex])
        {
            Debug.Log("Correct gesture!");
            currentIndex++;

            if (currentIndex >= requiredSequence.Count)
            {
                Debug.Log("Sequence completed!");
                currentIndex = 0;
            }
        }
        else
        {
            Debug.Log("Wrong gesture! Try again.");
           // currentIndex = 0;
        }

        if (_tempboard != null)
        {
            _tempboard.ClearBoard();
            _tempboard = null;
        }

        _points.Clear();
    }
}
