using Core;
using System.Linq;
using UnityEngine;
using Utilities;

namespace GameFlow
{
    public class BuildController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, SerializeReference] private IBuilding _building;
        [SerializeField] private GameObject _lineOfSight;
        [SerializeField] private Color _noCollisionColor;
        [SerializeField] private Color _collisionColor;
        [SerializeField] private LayerMask _obstacleLayer;

        [Header("References")]
        [SerializeField] private Grid _grid;

        private SpriteRenderer _buildingGhost;

        private void Update()
        {
            if (_buildingGhost)
            {
                MoveBuildingGhost();

                if (IsBuildingGhostColliding())
                    _buildingGhost.color = _collisionColor;
                else
                {
                    _buildingGhost.color = _noCollisionColor;

                    if (Input.GetMouseButtonDown(0))
                        AcceptBuilding();
                }

                if (Input.GetMouseButtonDown(1))
                    CancelBuilding();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.B))
                    BeginBuilding();
            }
        }

        private void MoveBuildingGhost()
        {
            Vector2 cellPos = _grid.GetCellCenterWorld(_grid.WorldToCell(MouseInput.GetMouseWorldPosition()));

            _buildingGhost.transform.position = cellPos;
        }

        private bool IsBuildingGhostColliding()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_buildingGhost.transform.position, .1f);

            return hits.Any(h => Helpers.IsInLayerMask(h.gameObject.layer, _obstacleLayer));
        }

        private void BeginBuilding()
        {
            _buildingGhost = new GameObject("Building Ghost").AddComponent<SpriteRenderer>();
            _buildingGhost.sprite = _building.Sprite;
            _buildingGhost.transform.position = new(50f, 50f);
            _lineOfSight.SetActive(true);
        }

        private void AcceptBuilding()
        {
            GameObject newBuilding = Instantiate(_building.BuildingPrefab, _buildingGhost.transform.position, _buildingGhost.transform.rotation);
            _building.Build(newBuilding);
            CancelBuilding();
        }

        private void CancelBuilding()
        {
            _lineOfSight.SetActive(false);
            Destroy(_buildingGhost);
        }
    }
}
