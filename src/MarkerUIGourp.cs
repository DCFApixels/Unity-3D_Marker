using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class MarkerUIGourp : MonoBehaviour
{
    private List<MarkerUI> markers = new List<MarkerUI>();
    [SerializeField]
    private bool _isSortByDistance = true;

    public void RegisterMarker(MarkerUI marker)
    {
        if (marker.transform.parent != transform)
        {
#if DEBUG
            Debug.LogWarning("This marker is not a direct heir");
#endif
            return;
        }
        if (!markers.Contains(marker))
        {
            markers.Add(marker);
        }
#if DEBUG
        else
            Debug.LogWarning("This marker is already contained in the marker group");
#endif
    }

    public void UnregisterMarker(MarkerUI marker)
    {
        if (markers.Contains(marker))
        {
            markers.Remove(marker);
        }
#if DEBUG
        else
            Debug.LogWarning("This marker is not in the marker group");
#endif
        
    }

    public void InstantiateMarker(MarkerUI markerPrefab, Transform target)
    {
        MarkerUI marker = Instantiate(markerPrefab, transform);
        marker.Target = target;
    }

    private void LateUpdate()
    {
        if (_isSortByDistance)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            var sorted = markers.OrderByDescending(m => Vector3.Distance(cameraPos, m.Target.position));
            int i = 0;
            foreach (var marker in sorted)
                marker.transform.SetSiblingIndex(i++);
        }
    }
}
