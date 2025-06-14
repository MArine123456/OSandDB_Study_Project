using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Boundary Settings")]
    public float mapWidth = 20f;
    public float mapHeight = 20f;
    public bool useBoundaryConstraints = true;

    private Camera cam;
    private float cameraHalfWidth;
    private float cameraHalfHeight;

    void Start()
    {
        cam = GetComponent<Camera>();

        // 카메라 크기 계산 (Orthographic 카메라 기준)
        if (cam.orthographic)
        {
            cameraHalfHeight = cam.orthographicSize;
            cameraHalfWidth = cameraHalfHeight * cam.aspect;
        }
        else
        {
            // Perspective 카메라의 경우 대략적인 계산
            float distance = Mathf.Abs(transform.position.z);
            cameraHalfHeight = distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            cameraHalfWidth = cameraHalfHeight * cam.aspect;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 경계 제한 적용 (활성화된 경우)
        if (useBoundaryConstraints)
        {
            desiredPosition = ApplyBoundaryConstraints(desiredPosition);
        }

        // 부드러운 카메라 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    Vector3 ApplyBoundaryConstraints(Vector3 targetPosition)
    {
        // 맵 경계 계산 (카메라가 벽을 넘지 않도록)
        float minX = -mapWidth / 2f + cameraHalfWidth;
        float maxX = mapWidth / 2f - cameraHalfWidth;
        float minY = -mapHeight / 2f + cameraHalfHeight;
        float maxY = mapHeight / 2f - cameraHalfHeight;

        // 경계가 유효한지 확인 (맵이 카메라보다 큰 경우에만)
        if (maxX > minX)
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        if (maxY > minY)
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    // Inspector에서 맵 크기를 시각적으로 확인
    void OnDrawGizmosSelected()
    {
        if (!useBoundaryConstraints) return;

        // 맵 경계 표시 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, 0));

        // 카메라 이동 가능 영역 표시 (초록색)
        if (cameraHalfWidth > 0 && cameraHalfHeight > 0)
        {
            float constrainedWidth = Mathf.Max(0, mapWidth - 2 * cameraHalfWidth);
            float constrainedHeight = Mathf.Max(0, mapHeight - 2 * cameraHalfHeight);

            if (constrainedWidth > 0 && constrainedHeight > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(constrainedWidth, constrainedHeight, 0));
            }
        }
    }

    // 런타임에 맵 크기 설정하는 함수 (GameManager에서 호출 가능)
    public void SetMapBounds(float width, float height)
    {
        mapWidth = width;
        mapHeight = height;
    }

    // 경계 제한 토글 함수
    public void SetBoundaryConstraints(bool enabled)
    {
        useBoundaryConstraints = enabled;
    }
}