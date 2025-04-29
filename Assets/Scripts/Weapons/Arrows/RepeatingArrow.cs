using System.Collections;
using UnityEngine;

public class RepeatingArrow : Arrow
{
    [Header("Repeating Settings")]
    [SerializeField] private int additionalShots = 2;
    [SerializeField] private float delayBetweenShots = 0.2f;

    //Properties
    public int AdditionalShots { get => additionalShots; set => additionalShots = value; }
    public float DelayBetweenShots { get => delayBetweenShots; set => delayBetweenShots = value; }
}