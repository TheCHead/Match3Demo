using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Match3 : MonoBehaviour 
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 origin = Vector3.zero;
    [SerializeField] private bool debug = true;
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private GemTypeSO[] gemTypes;
    [SerializeField] private ParticleSystem explosionVFX;
    [SerializeField] private Tilemap tilemap;

    private Grid2D<GridObject<Gem>> _grid;
    private InputReader _inputReader;
    private Vector2Int _selectedGem = Vector2Int.one * -1;
    private AudioManager _audioManager;
    

    private void Awake() 
    {
        _inputReader = GetComponent<InputReader>();
        _audioManager = GetComponent<AudioManager>();
    }

    private void Start() 
    {
        InitializeGrid();
        _inputReader.FireEvent += OnSelectGem;
    }

    private void OnDestroy() 
    {
        _inputReader.FireEvent -= OnSelectGem;
    }


    private void InitializeGrid()
    {
        _grid = Grid2D<GridObject<Gem>>.TilemapGrid(tilemap, debug);

        for (int x = 0; x < tilemap.size.x; x++)
        {
            for (int y = 0; y < tilemap.size.y; y++)
            {
                if (_grid.IsEmpty(x, y))
                {
                    CreateGem(x, y);
                }
            }
        }
    }

    private void CreateGem(int x, int y)
    {
        Gem gem = Instantiate(gemPrefab, _grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);

        List<GemTypeSO> forbiddenTypes = new();
        
        if  (_grid.IsValid(x - 1, y) && !_grid.IsEmpty(x - 1, y))
        {
            forbiddenTypes.Add(_grid.GetValue(x - 1, y).GetValue().GetGemType());
        }
        if  (_grid.IsValid(x, y - 1) && !_grid.IsEmpty(x, y - 1))
        {
            forbiddenTypes.Add(_grid.GetValue(x, y - 1).GetValue().GetGemType());
        }
        GemTypeSO[] allowedTypes = gemTypes.Except(forbiddenTypes).ToArray();
        gem.SetType(allowedTypes[Random.Range(0, allowedTypes.Length)]);

        var gridObject = new GridObject<Gem>(_grid, x, y);
        gridObject.SetValue(gem);

        _grid.SetValue(x, y, gridObject);
    }

    private void OnSelectGem()
    {
        var gridPos = _grid.GetXY(Camera.main.ScreenToWorldPoint(_inputReader.Selected));

        if (!_grid.IsValid(gridPos.x, gridPos.y) || _grid.IsEmpty(gridPos.x, gridPos.y))
            return;

        if (_selectedGem == gridPos)
        {
            DeselectGem();
        }
        else if (_selectedGem == Vector2Int.one * -1)
        {
            SelectGem(gridPos);
        }
        else
        {
            StartCoroutine(SwapSequence(_selectedGem, gridPos));
        }
    }

    private void DeselectGem() => _selectedGem = new Vector2Int(-1, -1);
    private void SelectGem(Vector2Int gridPos) => _selectedGem = gridPos;

    IEnumerator SwapSequence(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        DeselectGem();

        yield return SwapGems(gridPosA, gridPosB);

        List<Vector2Int> matches = FindMatches();
        
        do 
        {
            yield return ExplodeGems(matches);

            //yield return new WaitForSeconds(0.5f);

            yield return MakeGemsFall();

            yield return new WaitForSeconds(1f);

            matches = FindMatches();
        }

        while (matches.Count > 0);
        
        yield return FillEmptySpots();
        
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        var gridObjectA = _grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = _grid.GetValue(gridPosB.x, gridPosB.y);
        
        gridObjectA.GetValue().transform
            .DOLocalMove(_grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f)
            .SetEase(Ease.InSine);
        
        gridObjectB.GetValue().transform
            .DOLocalMove(_grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f)
            .SetEase(Ease.InSine);

        _grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
        _grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ExplodeGems(List<Vector2Int> matches)
    {
        foreach (var match in matches)
        {
            var gem = _grid.GetValue(match.x, match.y).GetValue();
            _grid.SetValue(match.x, match.y, null);

            gem.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f, 1);
            
            ExplodeVFX(match);

            yield return new WaitForSeconds(0.1f);

            gem.DestroyGem();
        }
    }

    private void ExplodeVFX(Vector2Int match)
    {
        // TODO: pool
        var fx = Instantiate(explosionVFX, transform);
        fx.transform.position = _grid.GetWorldPositionCenter(match.x, match.y);
        Destroy(fx.gameObject, 2f);
    }

    IEnumerator MakeGemsFall()
    {
        for (var x = 0; x < width; x++) 
        {
            for (var y = 0; y < height; y++) 
            {
                if (_grid.IsValid(x, y) && _grid.IsEmpty(x, y)) 
                {
                    for (var i = y + 1; i < height; i++) 
                    {
                        if (!_grid.IsValid(x, i))
                        {
                            break;
                        }

                        if (!_grid.IsEmpty(x, i)) 
                        {
                            var gem = _grid.GetValue(x, i).GetValue();
                            _grid.SetValue(x, y, _grid.GetValue(x, i));
                            _grid.SetValue(x, i, null);
                            gem.transform
                                .DOLocalMove(_grid.GetWorldPositionCenter(x, y), 1f)
                                .SetEase(Ease.OutBounce);
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator FillEmptySpots() 
    {
        for (var x = 0; x < width; x++) 
        {
            for (var y = 0; y < height; y++) 
            {
                if (_grid.IsValid(x, y) && _grid.IsEmpty(x, y)) 
                {
                    CreateGem(x, y);
                    yield return new WaitForSeconds(0.1f);;
                }
            }
        }
    }

    private List<Vector2Int> FindMatches()
    {
        HashSet<Vector2Int> matches = new();

        // Horizontal
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                var gemA = _grid.GetValue(x, y);
                var gemB = _grid.GetValue(x + 1, y);
                var gemC = _grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null) continue; 

                if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        // Vertical
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                var gemA = _grid.GetValue(x, y);
                var gemB = _grid.GetValue(x, y + 1);
                var gemC = _grid.GetValue(x, y + 2);

                if (gemA == null || gemB == null || gemC == null) continue; 

                if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        if (matches.Count == 0)
        {
            //_audioManager.PlayeNoMatch();
        }
        else
        {
            //_audioManager.PlayMatch();
        }

        return new List<Vector2Int>(matches);
    }    
}
