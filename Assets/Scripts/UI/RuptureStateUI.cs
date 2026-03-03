using UnityEngine;

public class RuptureStateUI : MonoBehaviour
{
    [SerializeField] AimingLine aimingLine;
    [SerializeField] SpriteRenderer endPointRenderer;
    [SerializeField] Sprite collisionSprite;
    [SerializeField] Sprite noCollisionSprite;

    Player player;
    RuptureTarget target;
    RuptureLaunchParams ruptureLaunchParams;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (target != null)
        {
            aimingLine.gameObject.SetActive(true);
            aimingLine.transform.position = target.GetCenter();
            aimingLine.transform.forward = target.GetLaunchDirection(player.transform.position, ruptureLaunchParams, out bool collision);

            endPointRenderer.enabled = true;
            endPointRenderer.sprite = collision ? collisionSprite : noCollisionSprite;
            endPointRenderer.transform.position = aimingLine.GetEndPoint();
        }
        else
        {
            aimingLine.gameObject.SetActive(false);
            endPointRenderer.enabled = false;
        }
    }

    public void SetTarget(RuptureTarget target)
    {
        this.target = target;
    }

    public void SetLaunchParams(RuptureLaunchParams launchParams)
    {
        this.ruptureLaunchParams = launchParams;
        aimingLine.maxLineLength = launchParams.maxLaunchDistance;
    }
}