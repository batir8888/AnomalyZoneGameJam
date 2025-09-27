using UnityEngine;
using UnityEngine.VFX;

public class MagnetVfx : MonoBehaviour
{
    public VisualEffect Vfx { get; private set; }

    private void Awake()
    {
        Vfx = GetComponent<VisualEffect>();
        Vfx.Stop();
    }
}
