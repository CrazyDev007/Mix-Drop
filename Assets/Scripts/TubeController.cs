using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public enum TubeState
{
    None,
    Normal,
    Pouring,
    Filling
}

public class TubeController : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private Vector2 rotationPointOffset = new Vector2(0.5f, -1f);
    [SerializeField] private Vector2 liquidDropPointOffset = new Vector2(0.5f, -1f);
    [SerializeField] private SpriteRenderer liquidRenderer;
    public bool IsLocked { get; set; } = false;

    internal int[] colors = new int[4] { -1, -1, -1, -1 };
    internal int currentTopIndex = 0;
    private SpriteRenderer tubeRenderer;
    private Material tubeMaterial;
    private Vector3 initialPosition;
    private int selectedLayerId;
    private int defaultLayerId;

    public TubeState CurrentTubeState { get; set; } = TubeState.None;
    public bool IsEmpty => currentTopIndex <= 0;
    public bool IsFull => currentTopIndex >= 4 && TopColorLevelCount == 4;
    public int TopColor => colors[currentTopIndex - 1];

    public Vector2 GetLiquidDropPoint(int direction) => (Vector2)transform.position +
                                                        new Vector2(direction * liquidDropPointOffset.x,
                                                            liquidDropPointOffset.y);

    public int TopColorLevelCount
    {
        get
        {
            int topColor = TopColor;
            int levelCount = 0;
            for (int i = currentTopIndex - 1; i >= 0 && colors[i] == topColor; i--)
                levelCount++;
            return levelCount;
        }
    }

    public int TopEmptyLevelCount => 4 - currentTopIndex;

    private void Awake()
    {
        selectedLayerId = SortingLayer.NameToID("Selected");
        defaultLayerId = SortingLayer.NameToID("Default");
        //
        tubeRenderer = GetComponent<SpriteRenderer>();
        tubeMaterial = tubeRenderer.material;
        initialPosition = transform.position;
    }
    public void UpdateTubesInitialPosition()
    {
        initialPosition = transform.position;
    }
    public void SetupTube(List<int> colors)
    {
        CurrentTubeState = TubeState.Normal;
        this.colors = new int[4];
        for (int i = 0; i < colors.Count; i++)
            this.colors[i] = colors[i];

        currentTopIndex = colors.Count;
        UpdateTubeMaterial();
    }

    internal void UpdateTubeMaterial()
    {
        for (int i = 0; i < currentTopIndex; i++)
            tubeMaterial.SetColor($"_Color{i}", GameManager.Instance.Colors[colors[i]]);

        tubeMaterial.SetFloat("_Mask", GameManager.Instance.TubeData[currentTopIndex].fillAmount);
    }

    public void OnSelectTube()
    {
        tubeRenderer.sortingLayerID = selectedLayerId;
        MoveTube(GameManager.Instance.TubeUpOffset);
    }

    public void OnDeSelectTube()
    {
        tubeRenderer.sortingLayerID = defaultLayerId;
        MoveTube(0);// Move back to initial position (offset 0)
    }

    private void MoveTube(float offset)
    {
        // Use initial position + offset
        transform.DOMove(initialPosition + Vector3.up * offset, GameManager.Instance.TimeToMove);
    }

    public async Task PourLiquid(TubeController secondTube, int direction)
    {
        var task =  OnPourLiquid(secondTube, direction);
        activeTasks.Add(task);
        
        if (activeTasks.Count <= 1)
        {
           await WaitForAllTasks();
           CurrentTubeState = TubeState.Normal;
           //Debug.Log(">>>>> TC " + CurrentTubeState);
        }
    }

    public async Task OnPourLiquid(TubeController secondTube, int direction)
    {
        // Change Tubes State
        secondTube.CurrentTubeState = TubeState.Pouring;
        CurrentTubeState = TubeState.Filling;
        //
        //float currentFillAmount = GameManager.Instance.TubeData[currentTopIndex].fillAmount;
        var topColorLevelCount = secondTube.TopColorLevelCount;
        var tubeModel = GameManager.Instance.TubeData[secondTube.currentTopIndex - topColorLevelCount];

        await secondTube.MoveTubeAsync(GetLiquidDropPoint(-direction));
        
        // Setup Liquid Renderer
        liquidRenderer.color = GameManager.Instance.Colors[secondTube.TopColor];
        liquidRenderer.transform.rotation = Quaternion.identity;
        var pos= liquidRenderer.transform.localPosition;
        pos.x = -direction * 0.5f;
        liquidRenderer.transform.localPosition = pos;
        const float baseScaleY = 250f;
        const float extraScaleY = 61f;
        var liquidScale = liquidRenderer.transform.localScale;
        var fillAmount = tubeMaterial.GetFloat("_Mask");
        liquidScale.y = (Mathf.Approximately(fillAmount, 0f) ? baseScaleY : baseScaleY * (1-fillAmount)) + extraScaleY;
        liquidRenderer.transform.localScale = liquidScale;
        
        await Task.WhenAll(
            secondTube.RotateTubeAsync(0, direction * tubeModel.tubeRotationAngle, direction),
            secondTube.RotateLiquidAsync(direction * tubeModel.liquidRotationAngle),
            secondTube.FillMask(tubeModel.fillAmount),
            FillLiquid(secondTube.TopColor, topColorLevelCount)
        );
        
        liquidRenderer.gameObject.SetActive(false);
        
        await Task.WhenAll(
            secondTube.RotateTubeAsync(direction * tubeModel.tubeRotationAngle, 0, direction),
            secondTube.RotateLiquidAsync(0)
        );
        await secondTube.MoveTubeAsync(secondTube.initialPosition);
        secondTube.currentTopIndex -= topColorLevelCount;
        secondTube.UpdateTubeMaterial();
        secondTube.tubeRenderer.sortingLayerID = secondTube.defaultLayerId;
        // Change Tubes State
        secondTube.CurrentTubeState = TubeState.Normal;
        //CurrentTubeState = TubeState.Normal;
        //Debug.Log(">>>>> TC " + CurrentTubeState);
        
        // Check if we should count moves (only for Moves mode, not Normal or Timer modes)
        if (GameManager.Instance.CurrentLevelType == LevelType.Moves)
        {
            GameManager.Instance.RemainingMoves--;
            //Debug.Log("Remaining Moves: " + GameManager.Instance.RemainingMoves);
            var _movesLeft = $"{GameManager.Instance.RemainingMoves} Moves left";
            GameManager.Instance.gamePlayScreenUIref.UpdateMoves(_movesLeft);
            if (GameManager.Instance.RemainingMoves <= 0)
            {
                Debug.Log("No Moves Left");
                GameManager.Instance.OnLevelFailed();
            }
        }
        else
        {
            // Don't decrement moves for Normal or Timer modes
            // Keep showing "∞ Moves" in the UI
            GameManager.Instance.gamePlayScreenUIref.UpdateMoves("∞ Moves");
        }
    }

    private async Task<bool> MoveTubeAsync(Vector2 targetPosition)
    {
        await transform.DOMove(targetPosition, GameManager.Instance.TimeToMove).AsyncWaitForCompletion();
        return true;
    }

    private async Task<bool> RotateTubeAsync(float fromAngle, float toAngle, int direction)
    {
        Vector3 rotationPoint = transform.position +
                                transform.rotation * new Vector3(direction * rotationPointOffset.x,
                                    rotationPointOffset.y, 0);
        Vector3 mDirection = transform.position - rotationPoint;
        Quaternion mRotation = Quaternion.Euler(0, 0, -transform.eulerAngles.z);
        Vector3 rotatedDirection = mRotation * mDirection;
        Vector3 newPosition = rotationPoint + rotatedDirection;
        Vector3 initialOffset = newPosition - rotationPoint;

        await DOTween.To(() => fromAngle, value =>
        {
            Quaternion rotation = Quaternion.AngleAxis(value, Vector3.forward);
            transform.SetPositionAndRotation(rotationPoint + rotation * initialOffset, rotation);
        }, toAngle, GameManager.Instance.TimeToRotate).AsyncWaitForCompletion();

        return true;
    }

    private async Task<bool> RotateLiquidAsync(float toAngle)
    {
        await DOTween
            .To(() => tubeMaterial.GetFloat("_RotationAngle"), value => tubeMaterial.SetFloat("_RotationAngle", value),
                toAngle, GameManager.Instance.TimeToRotate).AsyncWaitForCompletion();
        return true;
    }

    private async Task<bool> FillMask(float toMask)
    {
        await DOTween
            .To(() => tubeMaterial.GetFloat("_Mask"), value => tubeMaterial.SetFloat("_Mask", value), toMask,
                GameManager.Instance.TimeToRotate).AsyncWaitForCompletion();
        return true;
    }

    private async Task<bool> FillLiquid(int fillColor, int fillLevel)
    {
        for (int i = currentTopIndex; i < currentTopIndex + fillLevel; i++)
        {
            colors[i] = fillColor;
            tubeMaterial.SetColor($"_Color{i}", GameManager.Instance.Colors[fillColor]);
        }

        currentTopIndex += fillLevel;
        var tubeModel = GameManager.Instance.TubeData[currentTopIndex];
        liquidRenderer.gameObject.SetActive(true);
        await FillMask(tubeModel.fillAmount);
        return true;
    }

    public void ShakeTube() => transform.DOShakePosition(shakeDuration, shakeStrength);

    private List<Task> activeTasks = new List<Task>();

    private async Task WaitForAllTasks()
    {
        while (activeTasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(activeTasks);
            activeTasks.Remove(completedTask);
        }
    }

    private void OnDrawGizmos()
    {
        //
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(-liquidDropPointOffset.x, liquidDropPointOffset.y, 0), 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(liquidDropPointOffset.x, liquidDropPointOffset.y, 0), 0.1f);
        //for rotation
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(
            transform.position + transform.rotation * new Vector3(-rotationPointOffset.x, rotationPointOffset.y, 0),
            0.1f);
        Gizmos.DrawSphere(transform.position + transform.rotation * rotationPointOffset, 0.1f);
    }
}