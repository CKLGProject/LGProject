using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using DeadMosquito.AndroidGoodies;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelUI : MonoBehaviour
{
    public UIDocument UI;

    public Location[] Locations;
    public float TriggerDistance = 25f;

    private VisualElement _NoGPSPopup;
    private VisualElement _DisalbeGPSPopup;
    private VisualElement _statePopup;

    private Label _distanceLabel;
    private Label _placeLabel;

    private float[] _results = new float[3];

    private void Start()
    {
        _NoGPSPopup = UI.rootVisualElement.Q<VisualElement>("NoGPS-Popup");
        _DisalbeGPSPopup = UI.rootVisualElement.Q<VisualElement>("DisableGPS-Popup");
        _statePopup = UI.rootVisualElement.Q<VisualElement>("State-Popup");

        _distanceLabel = UI.rootVisualElement.Q<Label>("distance-label");
        _placeLabel = UI.rootVisualElement.Q<Label>("place-label");

        var hasGPS = AGGPS.DeviceHasGPS();
        if (!hasGPS)
        {
            _DisalbeGPSPopup.style.display = DisplayStyle.Flex;
            return;
        }

        var isGPS = AGGPS.IsGPSEnabled();

        if (!isGPS)
        {
            _NoGPSPopup.style.display = DisplayStyle.Flex;
            return;
        }

        _statePopup.style.display = DisplayStyle.Flex;

        const long minTime = 200;
        const float minDistance = 1;
        AGGPS.RequestLocationUpdates(minTime, minDistance, OnLocationChangedCallback);
    }
    
    private void OnLocationChangedCallback(AGGPS.Location obj)
    {
        Location nearestLocation = FindNearestLocation(obj, Locations, out float nearestDistance);
        
        if (nearestDistance < TriggerDistance)
            EnablePopup(nearestLocation, nearestDistance);
        else
            DisablePopup(nearestDistance);

    }

    /// <summary>
    /// 가장 가까운 위치를 찾는다.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Location FindNearestLocation(AGGPS.Location a, Location[] b, out float distance)
    {
        // 가장 가까운 위치를 찾는다.
        float[] temp = new float[b.Length];

        for (int i = 0; i < b.Length; i++)
        {
            Location location = b[i];
            AGGPS.DistanceBetween(a.Latitude, a.Longitude, location.Latitude, location.Longitude, _results);
            temp[i] = _results[0];
        }

        // calcul 배열에서 가장 작은 값의 인덱스를 찾는다.
        int minIndex = Array.IndexOf(temp, temp.Min());
        distance = temp[minIndex];
        return b[minIndex];
    }

    private void EnablePopup(Location location, float distance)
    {
        _statePopup.style.backgroundColor = new StyleColor(new Color(0, 1, 0.1f, 0.5f));
        _distanceLabel.text = $"타겟 위치와 거리(Debug) : {distance:N0}";
        _placeLabel.text = location.Name;
    }

    private void DisablePopup(float distance)
    {
        _statePopup.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0.5f));
        _distanceLabel.text = $"타겟 위치와 거리(Debug) : {distance:N0}";
        _placeLabel.text = "파악 중";
    }

    private void OnDestroy()
    {
        AGGPS.RemoveUpdates();
    }
}
