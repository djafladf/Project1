using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressablesLoader
{
    public static async Task InitAssets<T>(string label, List<T> createdObjs, Transform parent)
        where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        foreach (var location in locations)
        {
            createdObjs.Add(await Addressables.InstantiateAsync(location, parent).Task as T);
        }
    }
    public static async Task InitAssets<T>(string[] LoadNames,string label, List<T> createdObjs, Transform parent)
        where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        for(int i = 0; i < locations.Count; i++)
        {
            if (LoadNames.Contains(locations[i].PrimaryKey)) createdObjs.Add(await Addressables.InstantiateAsync(locations[i], parent).Task as T);
        }
    }
    // 왠진 모르겠지만 Image는 상위 Object가 필요 없음.
    public static async Task InitAssets<T>(string[] LoadNames, string label,bool IsIm, List<T> createdObjs) where T : Object
    {
        var handle = await Addressables.LoadResourceLocationsAsync(label).Task;
        int i = 0;
        foreach(var k in handle)
        {
            if (IsIm && i++ % 2 == 0) continue;
            if (LoadNames.Contains(k.PrimaryKey)) createdObjs.Add(await Addressables.LoadAssetAsync<T>(k).Task);
        }
    }
}