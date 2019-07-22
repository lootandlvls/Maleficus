using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float hoverSpeed = 1.0f;
    [SerializeField] private float hoverIntensity = 1.0f;
    [SerializeField] private float hoverInterpolationFactor = 1.0f;
    


    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float heightHover = Mathf.Sin(Time.time * hoverSpeed) * hoverIntensity;
        if (ARManager.Instance != null)
        {
            heightHover *= ARManager.Instance.SizeFactor;
        }
        Vector3 hoveredPosition = transform.position;
        hoveredPosition.y += heightHover;
        transform.position = Vector3.Lerp(transform.position, hoveredPosition, hoverInterpolationFactor * Time.deltaTime);
    }
}
