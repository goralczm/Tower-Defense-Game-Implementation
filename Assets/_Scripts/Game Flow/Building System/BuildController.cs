using ArtificeToolkit.Attributes;
using Core;
using System.Linq;
using UnityEngine;
using Utilities;

namespace GameFlow
{
    public class BuildController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, SerializeReference, ForceArtifice] private IBuilding _building;
        [SerializeField] private Color _validColor;
        [SerializeField] private Color _invalidColor;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private int _ghostSortingOrder = 10;

        [Header("References")]
        [SerializeField] private Grid _grid;
        [SerializeField] private LineOfSight _lineOfSight;

        private SpriteRenderer _buildingGhost;

        private void Update()
        {
            if (_buildingGhost)
            {
                MoveBuildingGhost();

                bool buildingRequirementsMet = _building.CanBuild(out var cannotBuildReason);
                if (IsBuildingGhostColliding() || !buildingRequirementsMet)
                {
                    if (buildingRequirementsMet)
                        cannotBuildReason = "Building must not collide";

                    _buildingGhost.color = _invalidColor;

                    if (Input.GetMouseButtonDown(0))
                        Debug.Log(cannotBuildReason);
                }
                else
                {
                    _buildingGhost.color = _validColor;

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
            _buildingGhost.sortingOrder = _ghostSortingOrder;
            _lineOfSight.transform.SetParent(_buildingGhost.transform);
            _lineOfSight.transform.localPosition = Vector2.zero;
            _lineOfSight.SetRadius(_building.LineOfSightRadius);
            _lineOfSight.gameObject.SetActive(true);
        }

        private void AcceptBuilding()
        {
            GameObject newBuilding = Instantiate(_building.BuildingPrefab, _buildingGhost.transform.position, _buildingGhost.transform.rotation);
            _building.Build(newBuilding);
            CancelBuilding();
        }

        private void CancelBuilding()
        {
            _lineOfSight.transform.SetParent(null);
            _lineOfSight.gameObject.SetActive(false);
            Destroy(_buildingGhost.gameObject);
        }
    }
}
