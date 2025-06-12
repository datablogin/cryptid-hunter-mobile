using UnityEngine;
using UnityEngine.EventSystems;

namespace CryptidHunter.UI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [SerializeField] private float handleRange = 1f;
        [SerializeField] private float deadZone = 0.1f;
        [SerializeField] private bool fixedPosition = true;
        
        [Header("Components")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        
        private RectTransform baseRect;
        private Canvas canvas;
        private Camera cam;
        private Vector2 input = Vector2.zero;
        
        public float Horizontal => input.x;
        public float Vertical => input.y;
        public Vector2 Direction => input;
        
        private void Start()
        {
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            
            if (canvas == null)
                Debug.LogError("Joystick must be child of a Canvas");
                
            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!fixedPosition)
            {
                background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
                background.gameObject.SetActive(true);
            }
            OnDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            
            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (eventData.position - position) / (radius * handleRange);
            
            if (input.magnitude > 1f)
                input = input.normalized;
                
            if (input.magnitude < deadZone)
                input = Vector2.zero;
                
            handle.anchoredPosition = input * radius * handleRange;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            
            if (!fixedPosition)
                background.gameObject.SetActive(false);
        }
        
        private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                baseRect, 
                screenPosition, 
                cam, 
                out localPoint))
            {
                return localPoint;
            }
            return Vector2.zero;
        }
    }
}