using Core;
using System.Linq;
using UnityEngine;
using Utilities;
using Utilities.Text;

namespace BuildingSystem.Core
{
    public class BuildController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color _validColor;
        [SerializeField] private Color _invalidColor;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private int _ghostSortingOrder = 10;

        [Header("References")]
        [SerializeField] private Grid _grid;
        [SerializeField] private LineOfSight _lineOfSight;

        private IBuilding _building;
        private SpriteRenderer _buildingGhost;

        private void Update()
        {
            if (_buildingGhost)
            {
                MoveBuildingGhost();

                string cannotBuildReason = "";
                bool buildingRequirementsMet = _building.CanBuild(ref cannotBuildReason);
                if (IsBuildingGhostColliding() || !buildingRequirementsMet)
                {
                    if (buildingRequirementsMet)
                        cannotBuildReason = "Building must not collide";

                    _buildingGhost.color = _invalidColor;
                    _lineOfSight.SetColor(_invalidColor);

                    if (Input.GetMouseButtonDown(0))
                        TextBubble.CreateTextBubble(cannotBuildReason, _buildingGhost.transform.position, Color.red);
                }
                else
                {
                    _buildingGhost.color = _validColor;
                    _lineOfSight.SetColor(_validColor);

                    if (Input.GetMouseButtonDown(0))
                        AcceptBuilding();
                }

                if (Input.GetMouseButtonDown(1))
                    CancelBuilding();
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

        public void BeginBuilding(IBuilding building)
        {
            _building = building;

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
            _building.OnBuild(newBuilding);
            CancelBuilding();
        }

        public void CancelBuilding()
        {
            _lineOfSight.transform.SetParent(null);
            _lineOfSight.gameObject.SetActive(false);
            if (_buildingGhost)
                Destroy(_buildingGhost.gameObject);
        }
    }
}
