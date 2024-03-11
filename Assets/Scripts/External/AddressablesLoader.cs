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

    public static async Task InitAssets<T>(string[] LoadNames,string label, T[] createdObjs, Transform parent)
        where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        for(int i = 0; i < locations.Count; i++)
        {
            int j = System.Array.IndexOf(LoadNames, locations[i].PrimaryKey);
            if (LoadNames.Contains(locations[i].PrimaryKey)) createdObjs[j] = await Addressables.InstantiateAsync(locations[i], parent).Task as T;
        }
    }
    /// <summary>
    /// Load
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="LoadNames"></param>
    /// <param name="label"></param>
    /// <param name="createdObjs"> 결과를 담아 둘 List </param>
    /// <param name="S"> 해당 결과의 순서를 알려줍니다.</param>
    /// <param name="tp"> List의 타입(Sprite 오류 방지용) </param>
    /// <returns></returns>
    public static async Task InitAssets<T>(string[] LoadNames, string label, List<T> createdObjs,List<int> S, System.Type tp) where T : Object
    {
        var handle = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach(var k in handle)
        {
            if (LoadNames.Contains(k.PrimaryKey) && tp == k.ResourceType)
            {
                S.Add(System.Array.IndexOf(LoadNames,k.PrimaryKey));
                createdObjs.Add(await Addressables.LoadAssetAsync<T>(k).Task);
            }
        }
    }

    public static async Task InitAssets<T>(string[] batchName, string label, List<T> createdObjs, System.Type tp) where T : Object
    {
        var handle = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach (var k in handle)
        {
            int j = System.Array.IndexOf(batchName,k.PrimaryKey);
            if (batchName.Contains(k.PrimaryKey) && tp == k.ResourceType) createdObjs.Add(await Addressables.LoadAssetAsync<T>(k).Task);
        }
    }

    public static async Task InitAssets<T>(string[] batchName, string label, T[] createdObjs, System.Type tp) where T : Object
    {
        var handle = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach (var k in handle)
        {
            int j = System.Array.IndexOf(batchName, k.PrimaryKey);
            if (batchName.Contains(k.PrimaryKey) && tp == k.ResourceType) createdObjs[j] = await Addressables.LoadAssetAsync<T>(k).Task;
        }
    }

}