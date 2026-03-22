using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private EnteringRateChecker body;
    [SerializeField] private EnteringRateChecker leftArm;
    [SerializeField] private EnteringRateChecker rightArm;
    [SerializeField] private EnteringRateChecker leftLeg;
    [SerializeField] private EnteringRateChecker rightLeg;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    public float GetScore()
    {
        return (body.SamplingArea() + leftArm.SamplingArea() + rightArm.SamplingArea() + leftLeg.SamplingArea() + rightLeg.SamplingArea()) / 5f;
    }
    
}
