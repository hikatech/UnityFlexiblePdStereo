using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoCameraGenerator : MonoBehaviour {

    [SerializeField]
    GameObject m_cameraPrefab;

    GameObject m_leftCamera;
    GameObject m_rightCamera;

    [SerializeField]
    float m_pupillaryDistance = 0.06f;

    [SerializeField]
    GameObject m_monitorQuad;


    void OnEnable () 
    {
        // Instantiate camera prefab and setup stereo mode and PD
        m_leftCamera = Instantiate(m_cameraPrefab, transform);
        m_leftCamera.name = "LeftCamera";
        m_leftCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
        m_leftCamera.transform.localPosition = new Vector3( -m_pupillaryDistance / 2, 0);

        m_rightCamera = Instantiate(m_cameraPrefab, transform);
        m_rightCamera.name = "RightCamera";
        m_rightCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
        m_rightCamera.transform.localPosition = new Vector3( m_pupillaryDistance / 2, 0);

        // Setup Frustum
        SetProjectonMatFromQuad(m_leftCamera.GetComponent<Camera>(), m_monitorQuad);
        SetProjectonMatFromQuad(m_rightCamera.GetComponent<Camera>(), m_monitorQuad);
    }

    void OnDisable()
    {
        Destroy(m_leftCamera);
        m_leftCamera = null;
        Destroy(m_rightCamera);
        m_rightCamera = null;
    }

    void Update () 
    {
        // Setup PD
        m_leftCamera.transform.localPosition = new Vector3(-m_pupillaryDistance / 2, 0);
        m_rightCamera.transform.localPosition = new Vector3(m_pupillaryDistance / 2, 0);

        // Setup Frustum
        SetProjectonMatFromQuad(m_leftCamera.GetComponent<Camera>(), m_monitorQuad);
        SetProjectonMatFromQuad(m_rightCamera.GetComponent<Camera>(), m_monitorQuad);
    }


    //GameObject CreateCameraObject(string name, Transform parent)
    //{
    //    if(!m_cameraPrefab){
    //        return CreateEmptyGameObject("Error", parent);
    //    }
    //    GameObject go = Instantiate(m_cameraPrefab, parent);
    //    go.name = name;

    //    return go;
    //}
    
    //GameObject CreateEmptyGameObject(string name, Transform parent)
    //{
    //    GameObject go = new GameObject(name);
    //    go.transform.parent = parent;
    //    go.transform.localPosition = Vector3.zero;
    //    go.transform.localRotation = Quaternion.identity;

    //    return go;
    //}

    void SetProjectonMatFromQuad(Camera cam, GameObject quad)
    {
        float left = -1;
        float right = 1;
        float top = 1;
        float bottom = -1;
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        // quads Z rotation (roll) is locked
        quad.transform.localEulerAngles = new Vector3(quad.transform.localEulerAngles.x, quad.transform.localEulerAngles.y, 0);

        // set camera rotation
        cam.transform.forward = quad.transform.forward;

        //set left right bottom top
        {
            Matrix4x4 worldToCamMat = cam.worldToCameraMatrix;
            Mesh quadMesh = quad.GetComponent<MeshFilter>().sharedMesh;

            //transform Quad corner-vertices position from local space to world space
            Vector3 vertexLeftBottom = quad.transform.localToWorldMatrix.MultiplyPoint(quadMesh.vertices[0]);
            Vector3 vertexRightTop = quad.transform.localToWorldMatrix.MultiplyPoint(quadMesh.vertices[1]);

            //transform from world space to camera space
            Vector3 leftBottom = worldToCamMat.MultiplyPoint(vertexLeftBottom);
            Vector3 rightTop = worldToCamMat.MultiplyPoint(vertexRightTop);

            float scale = near / GetDistanceFromQuad(cam, quad);

            left = leftBottom.x * scale;
            bottom = leftBottom.y * scale;
            right = rightTop.x * scale;
            top = rightTop.y * scale;

        }
        //set projection matrix
        cam.projectionMatrix = Matrix4x4.Frustum(left, right, bottom, top, near, far);
    }

    float GetDistanceFromQuad(Camera cam, GameObject quad)
    {
        Vector3 qNormal = quad.transform.forward.normalized;
        Vector3 qPos = cam.transform.position - quad.transform.position;

        float distance = Mathf.Abs(Vector3.Dot(qNormal, qPos));

        return distance;
    }
}
