using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class DataLoader : MonoBehaviour
{
    private void Start() {
        Load();
    }
    async void Load()
    {
        IList<IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync("Texture").Task;

        foreach(IResourceLocation loc in locations)
        {
            Debug.Log(loc);
            Sprite data = await Addressables.LoadAssetAsync<Sprite>(loc).Task;
        }
    }
}
