using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MonitorSetup: MonoBehaviour {

    [SerializeField]
    float m_inchi = 50;
    [SerializeField]
    Vector2 m_aspect = new Vector2(16, 9);

	// Update is called once per frame
	void Update () 
    {
        float theta = Mathf.Atan2(m_aspect.y, m_aspect.x);
        float diagonal = InchiToMeter(m_inchi);

        // calc global scale
        Vector3 size = new Vector3();
        size.x = diagonal * Mathf.Cos(theta);
        size.y = diagonal * Mathf.Sin(theta);
        size.z = transform.lossyScale.z;

        // calc local scale
        transform.localScale = Vector3.one;
        Vector3 localSize = new Vector3(size.x / transform.lossyScale.x, size.y / transform.lossyScale.y, size.z / transform.lossyScale.z);

        transform.localScale = localSize;
    }

    float InchiToMeter(float inchi)
    {
        return inchi * 0.0254f;
    }
}
