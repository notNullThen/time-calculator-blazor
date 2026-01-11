// Theme helper for TimeCalculator
// Supports mode: system | dark | light
// Stores the mode in localStorage under key: tc.theme

(function () {
  const storageKey = 'tc.theme';
  const modeOrder = ['system', 'dark', 'light'];

  /** @type {MediaQueryList|null} */
  const mql = (typeof window !== 'undefined' && window.matchMedia)
    ? window.matchMedia('(prefers-color-scheme: dark)')
    : null;

  /** @type {Set<any>} */
  const subscribers = new Set();

  let currentMode = 'system';
  let currentTheme = 'light';
  let listening = false;

  function normalizeMode(mode) {
    return (mode === 'dark' || mode === 'light' || mode === 'system') ? mode : 'system';
  }

  function computeSystemTheme() {
    return (mql && mql.matches) ? 'dark' : 'light';
  }

  function applyTheme(theme) {
    currentTheme = theme === 'dark' ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', currentTheme);
    document.documentElement.setAttribute('data-bs-theme', currentTheme);
  }

  function getState() {
    return { mode: currentMode, theme: currentTheme };
  }

  function notify() {
    const state = getState();
    subscribers.forEach(ref => {
      try {
        ref.invokeMethodAsync('NotifyThemeChanged', state.mode, state.theme);
      } catch {
        // ignore
      }
    });
  }

  function onSystemThemeChanged() {
    if (currentMode !== 'system') return;
    applyTheme(computeSystemTheme());
    notify();
  }

  function addMqlListener() {
    if (!mql || listening) return;
    listening = true;

    if (typeof mql.addEventListener === 'function') {
      mql.addEventListener('change', onSystemThemeChanged);
    } else if (typeof mql.addListener === 'function') {
      mql.addListener(onSystemThemeChanged);
    }
  }

  function removeMqlListener() {
    if (!mql || !listening) return;
    listening = false;

    if (typeof mql.removeEventListener === 'function') {
      mql.removeEventListener('change', onSystemThemeChanged);
    } else if (typeof mql.removeListener === 'function') {
      mql.removeListener(onSystemThemeChanged);
    }
  }

  function getStoredMode() {
    try {
      const v = localStorage.getItem(storageKey);
      // Back-compat: previously stored theme was 'dark'|'light'
      if (v === 'dark' || v === 'light' || v === 'system') return v;
    } catch {
      // ignore
    }
    return null;
  }

  function setStoredMode(mode) {
    try {
      localStorage.setItem(storageKey, mode);
    } catch {
      // ignore
    }
  }

  function setModeInternal(mode, persist) {
    currentMode = normalizeMode(mode);
    if (persist) setStoredMode(currentMode);

    if (currentMode === 'system') {
      addMqlListener();
      applyTheme(computeSystemTheme());
    } else {
      removeMqlListener();
      applyTheme(currentMode);
    }

    notify();
    return getState();
  }

  // Public API for Blazor JS interop
  window.tcTheme = {
    init: function () {
      const stored = getStoredMode();
      // If nothing stored, default to system without writing localStorage.
      const mode = stored ?? 'system';
      return setModeInternal(mode, stored !== null);
    },
    getState: function () {
      return getState();
    },
    setMode: function (mode) {
      return setModeInternal(mode, true);
    },
    cycleMode: function () {
      const idx = modeOrder.indexOf(currentMode);
      const next = modeOrder[(idx >= 0 ? idx + 1 : 0) % modeOrder.length];
      return setModeInternal(next, true);
    },
    subscribe: function (dotNetRef) {
      if (!dotNetRef) return;
      subscribers.add(dotNetRef);
      // Push current state immediately
      try {
        dotNetRef.invokeMethodAsync('NotifyThemeChanged', currentMode, currentTheme);
      } catch {
        // ignore
      }
    },
    unsubscribe: function (dotNetRef) {
      if (!dotNetRef) return;
      subscribers.delete(dotNetRef);
    }
  };

  // Run as early as possible
  if (typeof document !== 'undefined') {
    try { window.tcTheme.init(); } catch { /* ignore */ }
  }
})();
