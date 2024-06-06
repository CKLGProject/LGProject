using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPro : Button
{
    public Graphic[] SyncTarget;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (transition != Transition.ColorTint)
        {
            base.DoStateTransition(state, instant);
            return;
        }

        // Color Tint 처리
        Color color = state switch
        {
            SelectionState.Normal => colors.normalColor,
            SelectionState.Highlighted => colors.highlightedColor,
            SelectionState.Pressed => colors.pressedColor,
            SelectionState.Selected => colors.selectedColor,
            SelectionState.Disabled => colors.disabledColor,
            _ => Color.black
        };

        if (gameObject.activeInHierarchy)
            ColorTween(color * colors.colorMultiplier, instant);
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        if (targetGraphic == null) return;
        foreach (Graphic t in SyncTarget)
        {
            if (t) 
                t.CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
        }
    }
}