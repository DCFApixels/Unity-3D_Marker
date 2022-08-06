using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class MarkerUI : MonoBehaviour
{
    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (_isAutoEnabled)
            {
                gameObject.SetActive(_isEnabled);
            }
        }
    }

    [SerializeField]
    private bool _isAutoEnabled = true;
    [SerializeField]
    private bool _isAutoBounds = true;
    [SerializeField]
    private Bounds2D _bounds;
    public Bounds2D Bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    private MarkerUIGourp _group;

    [SerializeField]
    private AnimationCurve _scaleCurve;
    public AnimationCurve ScaleCurve
    {
        get => _scaleCurve;
        set => _scaleCurve = value;
    }
    [SerializeField]
    private float _distanceScaleMultipler;
    public float DistanceScaleMultipler
    {
        get => _distanceScaleMultipler;
        set => _distanceScaleMultipler = value;
    }
    [SerializeField]
    private float _scale = 1f;
    public float Scale
    {
        get => _scale;
        set => _scale = value;
    }

    [SerializeField]
    private float _moveSpeed = 1f;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }
    [SerializeField]
    private float _moveCurveDistance = 100f;
    public float MoveCurveDistance
    {
        get => _moveCurveDistance;
        set => _moveCurveDistance = value;
    }

    [SerializeField]
    private AnimationCurve _moveCurve;
    public AnimationCurve MuveCurve
    {
        get => _moveCurve;
        set => _moveCurve = value;
    }

    [SerializeField]
    private Transform _target;
    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    [SerializeField]
    private bool _isClamped = true;
    public bool IsClamped
    {
        get => _isClamped;
        set => _isClamped = value;
    }

    private void OnEnable()
    {
        IsEnabled = _target != null;
        RegisterInGroup();
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Camera camera = Camera.main;
        Transform cameraT = Camera.main.transform;
        RectTransform rectTransform = transform as RectTransform;
        Vector3 pos = rectTransform.position;

        float cameraDistance = Vector3.Distance(cameraT.position, Target.position);

        float dot = Vector3.Dot(cameraT.forward, (_target.position - cameraT.position).normalized);

        float scale = _scaleCurve.Evaluate((dot + 1f) / 2f);
        scale = Mathf.Max(0f, scale - (cameraDistance - 10f) * DistanceScaleMultipler);
        scale *= _scale;

        transform.localScale = Vector3.one * scale;

        Vector3 screenPos = camera.WorldToScreenPoint(_target.position, Camera.MonoOrStereoscopicEye.Mono);

        if (dot <= 0) screenPos = -screenPos;

        Rect markerRect = new Rect(0, 0, 0, 0);
        if (_isAutoBounds)
        {
            markerRect.size = rectTransform.rect.size * scale;
            markerRect.center = screenPos;
        }
        else
        {
            markerRect.size = _bounds.size * scale;
            markerRect.center = (Vector2)screenPos - _bounds.center * scale;
        }

        Rect safeArea = Screen.safeArea;

        bool isContains = Contains(safeArea, markerRect);
        if (isContains != _isTargetInside)
        {
            if (isContains)
                onTargetEnter.Invoke();
            else
                onTargetExit.Invoke();
            _isTargetInside = isContains;
        }

        if(!_isTargetInside && _isClamped)
        {
            markerRect.position = Clamp(safeArea, markerRect);
        }


        float distance = Vector2.Distance(pos, markerRect.center);
        float speed = _moveCurve.Evaluate(distance / _moveCurveDistance) * _moveSpeed;
        if (distance < speed)
            rectTransform.position = markerRect.center;
        else
            rectTransform.position = (Vector2)pos + (markerRect.center - (Vector2)pos).normalized * speed;
        OnUpdate?.Invoke(this);
    }

    private void OnDestroy()
    {
        UnregisterInGroup();
    }

    private void OnValidate()
    {
        IsEnabled = _target != null;
    }

    private void RegisterInGroup()
    {
        if (_group == null && transform.parent.TryGetComponent(out MarkerUIGourp group))
        {
            _group = group;
            group.RegisterMarker(this);
        }
    }
    private void UnregisterInGroup()
    {
        if (_group != null)
        {
            _group.UnregisterMarker(this);
        }
    }

    private bool Contains(Rect origin, Rect other)
    {
        return origin.xMin <= other.xMin
            && origin.xMax >= other.xMax
            && origin.yMin <= other.yMin
            && origin.yMax >= other.yMax;
    }

    private Vector2 Clamp(Rect rect, Rect other)
    {
        Vector2 result = other.position;
        Vector2 center = rect.center;

        if (!(rect.xMin <= other.xMin && rect.xMax >= other.xMax))
        {
            result.x = Mathf.Clamp(other.x, 0, rect.width - other.width);
            result.y = (1f - (other.x - result.x) / (other.x - center.x)) * (other.y - center.y) + center.y;
        }

        
        if (!(rect.yMin <= other.yMin && rect.yMax >= other.yMax))
        {
            result.y = Mathf.Clamp(other.y, 0, rect.height - other.height);
            result.x = (1f - (other.y - result.y) / (other.y - center.y)) * (other.x - center.x) + center.x;
            result.x = Mathf.Clamp(other.x, 0, rect.width - other.width);
        }

        return result;
    }


    private void OnDrawGizmosSelected()
    {
        if (!_isAutoBounds)
        {
            Vector3 pos = transform.position;
            float scale = transform.localScale.x;
            Gizmos.DrawWireCube(new Vector3(pos.x, pos.y, 0), _bounds.size * scale);
        }

        if (_target != null)
        {
            Camera camera = Camera.main;
            float dot = Vector3.Dot(camera.transform.forward, (_target.position - camera.transform.position).normalized);
            Vector3 screenPos = camera.WorldToScreenPoint(_target.position, Camera.MonoOrStereoscopicEye.Mono);
            if (dot <= 0) screenPos = -screenPos;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(screenPos, 10f);
        }
    }

    private void OnTransformParentChanged()
    {
        RegisterInGroup();
    }


    private bool _isTargetInside = false;
    public bool IsTargetInside
    {
        get => _isTargetInside;
    }

    public UnityEvent onTargetExit;
    public UnityEvent onTargetEnter;

    public event System.Action<MarkerUI> OnUpdate;

    [System.Serializable]
    public struct Bounds2D
    {
        public Vector2 center;
        public Vector2 size;

        public Rect Rect => new Rect(center - size / 2, size);
    }
}