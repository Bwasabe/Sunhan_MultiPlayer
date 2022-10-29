using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName ="SO/Item")]
public class ItemSO : ScriptableObject
{
    public int code;
    public AssetReferenceSprite _assetSprite;
    public Sprite sprite;
}
