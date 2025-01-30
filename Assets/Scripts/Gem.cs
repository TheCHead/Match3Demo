using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour
{
    private GemTypeSO _type;

    public void SetType(GemTypeSO type)
    {
        _type = type;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }

    public GemTypeSO GetGemType() => _type;

    public void DestroyGem()
    {
        Destroy(gameObject);
    }
}
