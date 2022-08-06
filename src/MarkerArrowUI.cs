using UnityEngine;

[ExecuteAlways]
public class MarkerArrowUI : MonoBehaviour
{
    private const float MIN_DISTANCE = 32f;

    [SerializeField]
    private Vector2 _centerOffset;
    private Vector2 Center => (Vector2)transform.parent.position + _centerOffset;
    [SerializeField]
    private float _radius;

    [SerializeField]
    private float _forwardAngle;
    [SerializeField]
    private float _defaultAngle;

    private MarkerUI _marker;

    [SerializeField]
    private float _lerpT = 0.1f;

    private void SetMarker(MarkerUI marker)
    {
        _marker = marker;
        _marker.OnUpdate += OnMarkerUpdate;
    }
    private void DeleteMarker()
    {
        _marker.OnUpdate -= OnMarkerUpdate;
        _marker = null;  
    }

    private void OnEnable()
    {
        if (_marker == null)
            SetMarker(GetComponentInParent<MarkerUI>());
    }
    private void OnDisable()
    {
        DeleteMarker();
    }

    private void OnMarkerUpdate(MarkerUI obj)
    {
        Camera camera = Camera.main;
        Transform cameraT = Camera.main.transform;
        Transform target = _marker.Target;

        float dot = Vector3.Dot(cameraT.forward, (target.position - cameraT.position).normalized);
        Vector2 screenPos = camera.WorldToScreenPoint(target.position, Camera.MonoOrStereoscopicEye.Mono);
        if (dot <= 0)
            screenPos = -screenPos;

        float oldAngleDeg = transform.eulerAngles.z;
        float angleDeg;
        float distance = Vector2.Distance(screenPos, (Vector2)_marker.transform.position - _marker.Bounds.center);

        if (distance <= MIN_DISTANCE)
        {
            angleDeg = _defaultAngle + _forwardAngle;
        }
        else
        {
            Vector2 direction = Center - screenPos;
            angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _forwardAngle;
        }

        angleDeg = Mathf.LerpAngle(oldAngleDeg, angleDeg, _lerpT);

        transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);

        angleDeg -= 90;
        float radius = _radius * transform.lossyScale.x;
        transform.position = Center + new Vector2(radius * Mathf.Cos(angleDeg * Mathf.Deg2Rad), radius * Mathf.Sin(angleDeg * Mathf.Deg2Rad));
    }
}

