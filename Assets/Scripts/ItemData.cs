using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite itemImage;
    public Vector2Int gridSize;
    public bool[] slotsItem;
    public Color color = Color.cyan; // <-- thêm trường chọn màu
}