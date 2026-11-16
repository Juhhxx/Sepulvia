using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class OutlineManager : MonoBehaviour
{
    public enum OutlineType { MeshRenderer, SpriteRenderer, Graphic }
    [SerializeField] private OutlineType _outlineType;

    [SerializeField] private Material _outline;

    [SerializeField, ShowIf(nameof(_outlineType), OutlineType.MeshRenderer)] private MeshRenderer _meshRenderer;
    [SerializeField, ShowIf(nameof(_outlineType), OutlineType.SpriteRenderer)] private SpriteRenderer _spriteRenderer;
    [SerializeField, ShowIf(nameof(_outlineType), OutlineType.Graphic)] private Graphic _graphicRenderer;

    private Material[] _defaultMaterials;
    private Material[] _outlineMaterials;
    private bool _outlineActive = false;

    private void Start()
    {
        StoreMaterials();
    }

    private void StoreMaterials()
    {
        switch (_outlineType)
        {
            case OutlineType.MeshRenderer:
                if (_meshRenderer == null) return;

                // Create arrays to apply to mesh renderer
                _defaultMaterials = _meshRenderer.materials;
                _outlineMaterials = new Material[_defaultMaterials.Length + 1];

                // Create outline aray with all materials from original renderer + outline material
                for (int i = 0; i < _defaultMaterials.Length; i++)
                {
                    _outlineMaterials[i] = _defaultMaterials[i];
                }
                _outlineMaterials[_outlineMaterials.Length - 1] = _outline;

                break;
            
            case OutlineType.SpriteRenderer:
                if (_spriteRenderer == null) return;
                
                // Create arrays to apply to sprite renderer and add the required materials                    
                _defaultMaterials = new Material[1] { _spriteRenderer.material };
                _outlineMaterials = new Material[1] { _outline };

                break;
            
            case OutlineType.Graphic:
                if (_graphicRenderer == null) return;

                // Create arrays to apply to sprite renderer and add the required materials                    
                _defaultMaterials = new Material[1] { _graphicRenderer.material };
                _outlineMaterials = new Material[1] { _outline };

                break;
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void Toggle()
    {
        ToggleOutline(!_outlineActive);
    }

    public void ToggleOutline(bool on)
    {
        _outlineActive = on;

        switch (_outlineType)
        {
            case OutlineType.MeshRenderer:
                // Check is renderer no null and switch materials acordingly
                if (_meshRenderer == null) return;
        
                if (on)
                {
                    _meshRenderer.materials = _outlineMaterials;
                }
                else
                {
                    _meshRenderer.materials = _defaultMaterials;
                }

                break;
            
            case OutlineType.SpriteRenderer:
                // Check is renderer no null and switch materials acordingly
                if (_spriteRenderer == null) return;
        
                if (on)
                {
                    _spriteRenderer.material = _outlineMaterials[0];
                }
                else
                {
                    _spriteRenderer.material = _defaultMaterials[0];
                }

                break;
            
            case OutlineType.Graphic:
                // Check is renderer no null and switch materials acordingly
                if (_graphicRenderer == null) return;
        
                if (on)
                {
                    _graphicRenderer.material = _outlineMaterials[0];
                }
                else
                {
                    _graphicRenderer.material = _defaultMaterials[0];
                }

                break;
        }
    }
}
