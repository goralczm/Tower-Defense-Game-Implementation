using Core;
using System;
using UnityEngine;
using Utilities;

namespace Towers
{
    public class TowerSelectionController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _towersLayer;

        private GenericCache<TowerBehaviour> _towersCache = new();

        public static event Action<TowerBehaviour> OnTowerSelected;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !Helpers.IsMouseOverUI())
            {
                RaycastHit2D hit = Physics2D.Raycast(MouseInput.GetMouseWorldPosition(), Vector2.zero, Mathf.Infinity, _towersLayer);
                if (hit.collider != null)
                {
                    if (_towersCache.TryGetKey(hit.collider, out var tower))
                        OnTowerSelected?.Invoke(tower);
                }
                else
                    OnTowerSelected?.Invoke(null);
            }
        }
    }
}
