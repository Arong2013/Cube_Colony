// Copyright (C) 2019 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateClean
{
    /// <summary>
    /// This button extends the base CleanButton button component and smoothly fades
    /// the button in/out as the user rolls over/presses it.
    /// </summary>
    [RequireComponent(typeof(FadeConfig))]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeButton : CleanButton
    {
        private FadeConfig config;
        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            base.Awake();
            config = GetComponent<FadeConfig>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerEnter(eventData);
                StopAllCoroutines();
                StartCoroutine(Utils.FadeOut(canvasGroup, config.onHoverAlpha, config.fadeTime));
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerExit(eventData);
                StopAllCoroutines();
                StartCoroutine(Utils.FadeIn(canvasGroup, 1.0f, config.fadeTime));
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerDown(eventData);
                canvasGroup.alpha = config.onClickAlpha;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (interactable)
            {
                base.OnPointerUp(eventData);
                canvasGroup.alpha = 1.0f;
            }
        }
    }
}
