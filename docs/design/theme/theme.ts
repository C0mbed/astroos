/**
 * theme.ts
 * ─────────────────────────────────────────────────────────────────────────────
 * DARK SKY — Theme Toggle Module
 *
 * PURPOSE: Manages all theme state for the Dark Sky application.
 *          Single source of truth for light/dark/night mode.
 *
 * RESPONSIBILITIES:
 *   - Reads the user's stored theme preference from localStorage on init
 *   - Falls back to the OS-level prefers-color-scheme if no stored preference
 *   - Sets [data-theme] on <html> as the CSS contract anchor
 *   - Handles the `N` keyboard shortcut for Night Mode toggle
 *   - Handles the `☾` titlebar button click
 *   - Handles the light/dark mode toggle
 *   - Persists all preference changes to localStorage
 *   - Exposes a subscribe() API for UI components that need to react to changes
 *     (e.g., the titlebar ☾ button updating its icon fill state)
 *
 * DATA ATTRIBUTE CONTRACT (agreed with Canvas):
 *   <html data-theme="dark">      → Dark theme (default)
 *   <html data-theme="light">     → Light theme
 *   <html data-theme="night">     → Night Mode (red-channel-only, observatory)
 *   <html data-mode="observatory"> → Observatory layout
 *   <html data-mode="travel">     → Travel layout
 *
 * STORAGE KEY: 'darksky-theme-preference'
 *   Stored value shape: { theme: 'dark' | 'light' | 'night', mode: 'observatory' | 'travel' }
 *
 * NIGHT MODE TRANSITION:
 *   - Full-UI transition in --duration-slow (400ms) at --ease-standard
 *   - This is the one permitted full-UI colour transition in Dark Sky
 *   - CRITICAL: Transition must go dark→red ONLY. Never dark→white→red.
 *     CSS handles this via interpolation of CSS custom properties — the
 *     values are all dark or red, so no intermediate white frame is possible.
 *   - Reduce Motion: class ds-theme-transitioning is NOT added; swap is instant
 *
 * KEYBOARD SHORTCUT GUARD:
 *   `N` is guarded to prevent firing when a text input has focus.
 *   This prevents Night Mode toggling while the user is typing a sequence name.
 *
 * USAGE:
 *   import { ThemeManager } from './theme';
 *   ThemeManager.init();   // Call once at app startup, before first render
 *
 *   // Toggle Night Mode (e.g., from ☾ button click handler):
 *   ThemeManager.toggleNightMode();
 *
 *   // Switch between light/dark:
 *   ThemeManager.setTheme('light');
 *
 *   // Subscribe to theme changes:
 *   const unsubscribe = ThemeManager.subscribe((state) => {
 *     moonButton.classList.toggle('active', state.theme === 'night');
 *   });
 *
 * ─────────────────────────────────────────────────────────────────────────────
 */

// ── Types ────────────────────────────────────────────────────────────────────

type Theme = 'dark' | 'light' | 'night';
type AppMode = 'observatory' | 'travel';

interface ThemeState {
  /** The active colour theme. 'dark' is the default. */
  theme: Theme;
  /** The active layout mode. 'observatory' is the default. */
  mode: AppMode;
}

type ThemeChangeListener = (state: ThemeState) => void;

// ── Constants ────────────────────────────────────────────────────────────────

const STORAGE_KEY = 'darksky-theme-preference' as const;

/**
 * CSS class added to <html> during the Night Mode transition.
 * Applies the transition property so the swap animates.
 * Removed after the transition completes to avoid animating unrelated changes.
 */
const TRANSITION_CLASS = 'ds-theme-transitioning' as const;

/** Duration in ms — must match --duration-slow in tokens.semantic.css (400ms) */
const NIGHT_TRANSITION_DURATION_MS = 400 as const;

// ── Input Focus Guard ─────────────────────────────────────────────────────────

/**
 * Returns true if the currently focused element is a text input.
 * Used to prevent `N` shortcut from firing mid-typing.
 */
function isTextInputFocused(): boolean {
  const el = document.activeElement;
  if (!el) return false;

  const tag = el.tagName.toLowerCase();

  if (tag === 'input') {
    // Guard all text-like input types. Number/range inputs are fine to pass through.
    const inputEl = el as HTMLInputElement;
    const blockedTypes = ['text', 'search', 'email', 'url', 'password', 'tel'];
    return blockedTypes.includes(inputEl.type.toLowerCase());
  }

  if (tag === 'textarea') return true;
  if ((el as HTMLElement).isContentEditable) return true;

  return false;
}

// ── Storage Helpers ───────────────────────────────────────────────────────────

/** Reads the stored ThemeState from localStorage. Returns null if absent or malformed. */
function readStoredState(): ThemeState | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as Partial<ThemeState>;

    // Validate shape — don't trust localStorage blindly
    const validThemes: Theme[] = ['dark', 'light', 'night'];
    const validModes: AppMode[] = ['observatory', 'travel'];

    if (!validThemes.includes(parsed.theme as Theme)) return null;
    if (!validModes.includes(parsed.mode as AppMode)) return null;

    return { theme: parsed.theme as Theme, mode: parsed.mode as AppMode };
  } catch {
    // Corrupted storage — fall through to defaults
    return null;
  }
}

/** Writes the current ThemeState to localStorage. */
function writeState(state: ThemeState): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  } catch {
    // localStorage may be unavailable in some environments.
    // This is non-fatal — state will be lost on reload but the UI still works.
    console.warn('[DarkSky] ThemeManager: localStorage unavailable, preference not persisted.');
  }
}

// ── DOM Application ───────────────────────────────────────────────────────────

/**
 * Applies the given theme to <html> by setting data-theme.
 *
 * For Night Mode activation/deactivation, adds the transition class first,
 * then removes it after the animation completes to keep transitions scoped.
 *
 * CRITICAL: Never allows an intermediate state with white backgrounds.
 * The token system guarantees dark/red values throughout — this function
 * should not modify any colour values directly.
 *
 * @param theme     - The theme to apply.
 * @param animate   - Whether to apply the CSS transition class. False for
 *                    Reduce Motion and for programmatic no-animation swaps.
 */
function applyThemeToDom(theme: Theme, animate: boolean): void {
  const html = document.documentElement;
  const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const shouldAnimate = animate && !prefersReducedMotion;

  if (shouldAnimate) {
    // Add the transition class BEFORE changing the attribute so the browser
    // picks it up in time to animate the transition.
    html.classList.add(TRANSITION_CLASS);

    // Remove the class after the transition completes to avoid animating
    // subsequent unrelated property changes (hover states, etc.).
    setTimeout(() => {
      html.classList.remove(TRANSITION_CLASS);
    }, NIGHT_TRANSITION_DURATION_MS + 50); // +50ms buffer for staggered sub-elements
  }

  html.setAttribute('data-theme', theme);
}

/** Applies the layout mode to <html> by setting data-mode. */
function applyModeToDom(mode: AppMode): void {
  document.documentElement.setAttribute('data-mode', mode);
}

// ── ThemeManager ─────────────────────────────────────────────────────────────

/**
 * ThemeManager — singleton controller for all theme state in Dark Sky.
 *
 * Call ThemeManager.init() once at app startup, before the first render.
 * All other methods can be called at any time after init().
 */
export const ThemeManager = (() => {

  // Internal state
  let _state: ThemeState = { theme: 'dark', mode: 'observatory' };
  const _listeners = new Set<ThemeChangeListener>();

  /** Notifies all registered listeners of the current state. */
  function _notify(): void {
    for (const listener of _listeners) {
      listener({ ..._state });
    }
  }

  /**
   * Resolves the initial theme based on priority:
   *   1. Stored user preference (localStorage)
   *   2. OS-level prefers-color-scheme (dark or light — Night Mode is never
   *      auto-set from the OS; it requires explicit user action)
   *   3. Fallback: 'dark' (Dark Sky is dark-first)
   */
  function _resolveInitialTheme(): ThemeState {
    const stored = readStoredState();
    if (stored) return stored;

    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    return {
      theme: prefersDark ? 'dark' : 'light',
      mode: 'observatory',
    };
  }

  return {

    /**
     * Initialises the ThemeManager.
     *
     * Must be called once at app startup, before the first render.
     * Reads stored preferences, applies them to the DOM, and registers
     * the `N` keyboard shortcut handler.
     *
     * @example
     *   // In your app entry point (e.g., main.ts):
     *   import { ThemeManager } from './theme';
     *   ThemeManager.init();
     */
    init(): void {
      _state = _resolveInitialTheme();

      // Apply without animation on initial load — no transition on page load
      applyThemeToDom(_state.theme, false);
      applyModeToDom(_state.mode);

      // `N` keyboard shortcut — global Night Mode toggle
      // Guard: does not fire when a text input has focus
      document.addEventListener('keydown', (event: KeyboardEvent) => {
        if (event.key !== 'N' && event.key !== 'n') return;
        if (event.metaKey || event.ctrlKey || event.altKey) return;
        if (isTextInputFocused()) return;

        this.toggleNightMode();
      });

      // Listen for OS-level scheme changes while the app is running
      // (e.g., user switches from light to dark in system settings).
      // Only applies if there's no stored preference and Night Mode is not active.
      window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
        const stored = readStoredState();
        if (stored) return; // User has a stored preference — don't override it
        if (_state.theme === 'night') return; // Night Mode — don't interfere

        const newTheme: Theme = e.matches ? 'dark' : 'light';
        this.setTheme(newTheme, false); // No animation for system-level changes
      });
    },

    /**
     * Toggles Night Mode on/off.
     *
     * - If Night Mode is currently active → restores the previous light/dark theme
     * - If Night Mode is inactive → activates Night Mode
     *
     * Animates the transition (unless Reduce Motion is active).
     * Persists the new state to localStorage.
     *
     * Called by:
     *   - The `N` keyboard shortcut handler (via init())
     *   - The `☾` titlebar button click handler (called externally)
     */
    toggleNightMode(): void {
      if (_state.theme === 'night') {
        // Restore to 'dark' — Night Mode is a dark-first app, so we default
        // back to dark rather than trying to remember pre-Night state.
        // TODO(forge): If light mode was active before Night Mode was toggled,
        //              restore to light. Requires storing the pre-Night theme.
        _state = { ..._state, theme: 'dark' };
      } else {
        _state = { ..._state, theme: 'night' };
      }

      applyThemeToDom(_state.theme, true); // Animate this transition
      writeState(_state);
      _notify();
    },

    /**
     * Sets the light/dark theme explicitly.
     *
     * Will not activate Night Mode — use toggleNightMode() for that.
     * Deactivates Night Mode if it was previously active.
     *
     * @param theme   - 'dark' or 'light'. 'night' is rejected; use toggleNightMode().
     * @param animate - Whether to animate the transition. Default: false.
     *                  Night Mode → light/dark should animate; system changes should not.
     */
    setTheme(theme: Exclude<Theme, 'night'>, animate = false): void {
      _state = { ..._state, theme };
      applyThemeToDom(theme, animate);
      writeState(_state);
      _notify();
    },

    /**
     * Sets the layout mode (Observatory or Travel).
     *
     * @param mode - 'observatory' or 'travel'
     */
    setMode(mode: AppMode): void {
      _state = { ..._state, mode };
      applyModeToDom(mode);
      writeState(_state);
      _notify();
    },

    /**
     * Returns the current ThemeState.
     * Returns a copy — do not mutate the returned object.
     */
    getState(): ThemeState {
      return { ..._state };
    },

    /**
     * Subscribes to theme state changes.
     *
     * The listener is called with the new state immediately on any theme or
     * mode change. Returns an unsubscribe function.
     *
     * @param listener - Callback invoked with the new ThemeState on every change.
     * @returns A function that removes this listener when called.
     *
     * @example
     *   const unsubscribe = ThemeManager.subscribe((state) => {
     *     moonButton.classList.toggle('active', state.theme === 'night');
     *   });
     *   // Later, when the component unmounts:
     *   unsubscribe();
     */
    subscribe(listener: ThemeChangeListener): () => void {
      _listeners.add(listener);
      return () => _listeners.delete(listener);
    },

  };

})();
