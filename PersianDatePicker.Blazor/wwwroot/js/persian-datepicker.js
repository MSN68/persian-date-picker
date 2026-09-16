// Focus-management helpers for the Persian DatePicker Blazor component.
// Loaded once per component via JS module import.

const FOCUSABLE =
  'button:not([disabled]):not([tabindex="-1"]), [href]:not([tabindex="-1"]), input:not([disabled]):not([tabindex="-1"]), select:not([disabled]), textarea:not([disabled])';

export function focusBySelector(selector) {
  const el = document.querySelector(selector);
  if (el) el.focus();
}

export function focusIn(container) {
  if (!container) return;
  const el = container.querySelector(
    'button.dp-selected:not([disabled]), button[aria-current="date"]:not([disabled]), button[tabindex="0"]:not([disabled])'
  );
  const target = el || container.querySelector(FOCUSABLE);
  if (target) target.focus();
}

// WAI-ARIA APG dialog focus trap: returns the element Tab should move to when the user
// presses Tab/Shift+Tab at an edge, or null if no trap is needed.
export function trapFocus(container, shiftKey) {
  if (!container) return null;
  const focusable = Array.from(container.querySelectorAll(FOCUSABLE)).filter(e => e.offsetParent !== null);
  if (focusable.length === 0) return null;
  const first = focusable[0];
  const last = focusable[focusable.length - 1];
  const active = document.activeElement;
  if (shiftKey && (active === first || active === container)) return last;
  if (!shiftKey && active === last) return first;
  return null;
}

export function isOutside(ancestor, eventTarget) {
  return !ancestor.contains(eventTarget);
}

let outsideHandler = null;

// Registers a one-shot document-level click listener. When a click lands outside
// `ancestor`, `onOutside` (a DotNetObjectReference) is invoked. Used to close the
// dropdown when the user clicks elsewhere on the page.
export function attachOutsideClick(ancestor, onOutside) {
  detachOutsideClick();
  if (!ancestor || !onOutside) return;
  outsideHandler = (e) => {
    if (e.target && !ancestor.contains(e.target)) {
      onOutside.invokeMethodAsync('OnOutsideClick');
    }
  };
  document.addEventListener('click', outsideHandler);
}

export function detachOutsideClick() {
  if (outsideHandler) {
    document.removeEventListener('click', outsideHandler);
    outsideHandler = null;
  }
}
