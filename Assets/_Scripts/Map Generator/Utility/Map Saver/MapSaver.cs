using System.Threading.Tasks;
using UnityEngine;

public class MapSaver : MonoBehaviour
{
    [SerializeField] private MapGenerator.Core.Map.MapGenerator _mapGenerator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Save();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Load();
    }

    public void Save()
    {
        SaveSystem.SaveData(_mapGenerator.GetGenerationContext(), "MapData");
    }

    public async Task Load()
    {
        MapGenerationContext context = SaveSystem.LoadData<MapGenerationContext>("MapData") as MapGenerationContext;
        if (context == null)
        {
            Debug.LogWarning("No map saved");
            return;
        }
        
        _mapGenerator.SetGenerationContext(context);
            
        await _mapGenerator.GenerateMapByContext();
    }
}
