﻿// Copyright (C) 2019 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltimateClean
{
    /// <summary>
    /// The base button component used in the kit that provides the ability to
    /// (optionally) play sounds when the user rolls over/presses it.
    /// </summary>
    public class CleanButton : Button
    {
        private ButtonSounds buttonSounds;
        private bool pointerWasUp;

        protected override void Awake()
        {
            base.Awake();
            buttonSounds = GetComponent<ButtonSounds>();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (buttonSounds != null && interactable)
            {
                buttonSounds.PlayPressedSound();
            }
            base.OnPointerClick(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            pointerWasUp = true;
            base.OnPointerUp(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (pointerWasUp)
            {
                pointerWasUp = false;
                base.OnPointerEnter(eventData);
            }
            else
            {
                if (buttonSounds != null && interactable)
                {
                    buttonSounds.PlayRolloverSound();
                }
                base.OnPointerEnter(eventData);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            pointerWasUp = false;
            base.OnPointerExit(eventData);
        }
    }
}