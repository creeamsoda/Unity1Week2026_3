using UnityEngine;

public class DollManager : MonoBehaviour
{
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject leftArm;
    [SerializeField] public GameObject rightArm;
    [SerializeField] public GameObject leftLeg;
    [SerializeField] public GameObject rightLeg;
    
    public MaskController bodyMaskController;
    public MaskController leftArmMaskController;
    public MaskController rightArmMaskController;
    public MaskController leftLegMaskController;
    public MaskController rightLegMaskController;
    
    public SpriteRenderer bodySpriteRenderer;
    public SpriteRenderer leftArmSpriteRenderer;
    public SpriteRenderer rightArmSpriteRenderer;
    public SpriteRenderer leftLegSpriteRenderer;
    public SpriteRenderer rightLegSpriteRenderer;

    void Start()
    {
        bodyMaskController = body.GetComponent<MaskController>();
        leftArmMaskController = leftArm.GetComponent<MaskController>();
        rightArmMaskController = rightArm.GetComponent<MaskController>();
        leftLegMaskController = leftLeg.GetComponent<MaskController>();
        rightLegMaskController = rightLeg.GetComponent<MaskController>();

        bodySpriteRenderer = body.GetComponentInChildren<SpriteRenderer>();
        leftArmSpriteRenderer = leftArm.GetComponentInChildren<SpriteRenderer>();
        rightArmSpriteRenderer = rightArm.GetComponentInChildren<SpriteRenderer>();
        leftLegSpriteRenderer = leftLeg.GetComponentInChildren<SpriteRenderer>();
        rightLegSpriteRenderer = rightLeg.GetComponentInChildren<SpriteRenderer>();
    }
}
