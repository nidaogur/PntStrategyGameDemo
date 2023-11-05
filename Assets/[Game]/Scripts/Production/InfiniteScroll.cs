using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game_.Scripts.Production
{
    public class InfiniteScroll : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float outOfBoundsThreshold = 40f;
        [SerializeField] private float childWidth = 125f;
        [SerializeField] private float childHeight = 125f;
        [SerializeField] private float itemSpacing = 30f;
        [SerializeField] private GameObject prefab;

        private Vector2 _lastDragPosition;
        private bool _positiveDrag;
        private int _childCount = 0;
        private float _height = 0f;

        private const int ChildOffset = 175;

        private void Start()
        {
            InitializeScrollRect();
        }

        private void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(HandleScrollRectValueChanged);
        }

        private void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(HandleScrollRectValueChanged);
        }

        private void InitializeScrollRect()
        {
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            _height = Screen.height;

            int numItems = Mathf.CeilToInt(scrollRect.content.rect.height / childHeight);
            float initialY = -ChildOffset;

            for (int i = 0; i < numItems; i++)
            {
                CreateScrollItem(initialY);
                initialY -= ChildOffset;
            }

            _childCount = scrollRect.content.childCount;
            scrollRect.content.localPosition = Vector3.zero;

            for (int i = 0; i < _childCount; i++)
            {
                HandleScrollRectValueChanged(Vector2.up * -3);
            }
        }

        private void CreateScrollItem(float yPos)
        {
            var newItem = Instantiate(prefab, scrollRect.content);
            newItem.transform.localPosition = new Vector3(100, yPos);
            newItem.transform.SetSiblingIndex(0);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 newPosition = eventData.position;
            _positiveDrag = newPosition.y > _lastDragPosition.y;
            _lastDragPosition = newPosition;
        }

        private bool ReachedThreshold(Transform item)
        {
            var position = scrollRect.transform.position;
            float positiveYThreshold = position.y + _height * 0.5f + outOfBoundsThreshold;
            float negativeYThreshold = position.y - _height * 0.5f - outOfBoundsThreshold;

            return _positiveDrag
                ? item.position.y - childWidth * 0.5f > positiveYThreshold
                : item.position.y + childWidth * 0.5f < negativeYThreshold;
        }

        private void HandleScrollRectValueChanged(Vector2 value)
        {
            int currentItemIndex = _positiveDrag ? _childCount - 1 : 0;
            var currentItem = scrollRect.content.GetChild(currentItemIndex);

            if (!ReachedThreshold(currentItem))
            {
                return;
            }

            int endItemIndex = _positiveDrag ? 0 : _childCount - 1;
            var endItem = scrollRect.content.GetChild(endItemIndex);
            var newPosition = endItem.localPosition;

            float offset = childHeight * 1.5f - itemSpacing;

            if (_positiveDrag)
            {
                newPosition.y = endItem.localPosition.y - offset;
            }
            else
            {
                newPosition.y = endItem.localPosition.y + offset;
            }

            currentItem.localPosition = newPosition;
            currentItem.SetSiblingIndex(endItemIndex);
        }
    }
}
