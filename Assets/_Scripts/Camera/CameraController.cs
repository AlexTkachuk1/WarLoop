using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Edge Scroll")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float screenEdgeSize = 20f;

        [Header("Drag")]
        [SerializeField] private int dragMouseButton = 0;
        [SerializeField] private float dragSpeed = 1f;

        private Camera _camera;

        private bool _isDragging;

        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            CacheBounds();
            CenterOnMapCenter();
            ClampPosition();
        }

        private void Update()
        {
            HandleDrag();

            if (!_isDragging)
            {
                HandleEdgeScroll();
            }

            ClampPosition();
        }

        private void CacheBounds()
        {
            _minBounds = MapGenerator.Instance.MinBounds;
            _maxBounds = MapGenerator.Instance.MaxBounds;
        }

        public void CenterOnWorldPosition(Vector3 worldPosition)
        {
            Vector3 newPosition = worldPosition;
            newPosition.z = _camera.transform.position.z;
        
            _camera.transform.position = newPosition;
            ClampPosition();
        }
        
        public void CenterOnMapCenter()
        {
            if (MapGenerator.Instance == null) return;
            
            Vector2 center = (_minBounds + _maxBounds) / 2f;
            
            CenterOnWorldPosition(center);
        }
        
        public void CenterOnCell(int cellX, int cellY, float cellSize = 1f)
        {
            Vector3 worldPosition = new Vector3(
                cellX * cellSize,
                cellY * cellSize,
                0f
            );
            
            CenterOnWorldPosition(worldPosition);
        }

        #region Edge Scroll

        private void HandleEdgeScroll()
        {
            Vector3 dir = Vector3.zero;
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x <= screenEdgeSize)
                dir.x -= 1f;
            else if (mousePos.x >= Screen.width - screenEdgeSize)
                dir.x += 1f;

            if (mousePos.y <= screenEdgeSize)
                dir.y -= 1f;
            else if (mousePos.y >= Screen.height - screenEdgeSize)
                dir.y += 1f;

            if (dir.sqrMagnitude > 0f)
            {
                transform.position += dir.normalized * moveSpeed * Time.deltaTime;
            }
        }

        #endregion

        #region Drag

        private void HandleDrag()
        {
            if (Input.GetMouseButtonDown(dragMouseButton))
            {
                _isDragging = true;
            }

            if (Input.GetMouseButtonUp(dragMouseButton))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                float camHeight = _camera.orthographicSize * 2f;
                float camWidth = camHeight * _camera.aspect;

                Vector3 move = new Vector3(
                    -mouseX * camWidth / Screen.width,
                    -mouseY * camHeight / Screen.height,
                    0f
                );

                transform.position += move * dragSpeed;
            }
        }

        #endregion

        #region Clamp

        private void ClampPosition()
        {
            float camHeight = _camera.orthographicSize;
            float camWidth = camHeight * _camera.aspect;

            float minX = _minBounds.x + camWidth;
            float maxX = _maxBounds.x - camWidth;
            float minY = _minBounds.y + camHeight;
            float maxY = _maxBounds.y - camHeight;

            Vector3 pos = transform.position;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            transform.position = pos;
        }

        #endregion
    }
}