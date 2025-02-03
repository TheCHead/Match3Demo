using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour
{
    [SerializeField] private ParticleSystem explodeVfx;
    private GemTypeSO _type;

    public void SetType(GemTypeSO type)
    {
        _type = type;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }

    public GemTypeSO GetGemType() => _type;

    public void DestroyGem()
    {
        ExplodeVFX(transform.position);
        Destroy(gameObject);
    }

    private void ExplodeVFX(Vector3 position)
    {
        // TODO: pool
        var fx = GameObject.Instantiate(explodeVfx);
        fx.transform.position = position;
        GameObject.Destroy(fx.gameObject, 2f);
    }
}
