using ArtificeToolkit.Attributes;
using BuildingSystem.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem.UI
{
    public class BuildingsUI : MonoBehaviour
    {
        [Header("Buildings")]
        [SerializeField, SerializeReference, ForceArtifice] IBuilding[] _buildings;

        [Header("References")]
        [SerializeField] private BuildController _buildController;
        [SerializeField] private List<BuildingSlot> _slots;

        private void Start()
        {
            for (int i = 0; i < _buildings.Length; i++)
            {
                if (i > _slots.Count - 1)
                    _slots.Add(Instantiate(_slots[0], _slots[0].transform.parent));

                int index = i;
                _slots[index].Setup(_buildings[index], () => _buildController.BeginBuilding(_buildings[index]));
            }
        }

        private void Update()
        {
            for (int i = 0; i < _buildings.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    _buildController.CancelBuilding();
                    _buildController.BeginBuilding(_buildings[i]);
                }
            }
        }
    }
}
