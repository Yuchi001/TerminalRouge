﻿using System;
using System.Linq;
using UnityEngine;

namespace UIElements
{
    public abstract class FocusableUIElement : MonoBehaviour
    {
        [SerializeField] protected string uiName;
        protected int ID => GetInstanceID();

        public virtual void LoseFocus(bool recursive)
        {
            Destroy(gameObject);

            if (transform.parent == null || !recursive) return;

            if (!transform.parent.TryGetComponent<FocusableUIElement>(out var ui))
                return;
            
            ui.LoseFocus(true);
        }

    }
}