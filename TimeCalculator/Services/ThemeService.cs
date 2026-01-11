using Microsoft.JSInterop;

namespace TimeCalculator.Services;

public sealed class ThemeService : IAsyncDisposable
{
    private string _currentTheme = "light";
    private string _currentMode = "system";
    private bool _initialized;

    private IJSRuntime? _js;
    private DotNetObjectReference<ThemeService>? _dotNetRef;

    public string CurrentTheme => _currentTheme;
    public string CurrentMode => _currentMode;

    public event Action? Changed;

    private sealed record ThemeState(string Mode, string Theme);

    public async ValueTask InitializeAsync(IJSRuntime js)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        _js = js;

        var state = await js.InvokeAsync<ThemeState>("tcTheme.init");
        _currentMode = state.Mode;
        _currentTheme = state.Theme;

        _dotNetRef = DotNetObjectReference.Create(this);
        await js.InvokeVoidAsync("tcTheme.subscribe", _dotNetRef);
        Changed?.Invoke();
    }

    public async ValueTask SetModeAsync(IJSRuntime js, string mode)
    {
        var state = await js.InvokeAsync<ThemeState>("tcTheme.setMode", mode);
        _currentMode = state.Mode;
        _currentTheme = state.Theme;
        Changed?.Invoke();
    }

    public async ValueTask CycleModeAsync(IJSRuntime js)
    {
        var state = await js.InvokeAsync<ThemeState>("tcTheme.cycleMode");
        _currentMode = state.Mode;
        _currentTheme = state.Theme;
        Changed?.Invoke();
    }

    [JSInvokable]
    public void NotifyThemeChanged(string mode, string theme)
    {
        var changed = false;

        if (!string.Equals(_currentMode, mode, StringComparison.Ordinal))
        {
            _currentMode = mode;
            changed = true;
        }

        if (!string.Equals(_currentTheme, theme, StringComparison.Ordinal))
        {
            _currentTheme = theme;
            changed = true;
        }

        if (changed)
        {
            Changed?.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_js is not null && _dotNetRef is not null)
        {
            try
            {
                await _js.InvokeVoidAsync("tcTheme.unsubscribe", _dotNetRef);
            }
            catch
            {
                // ignore shutdown errors
            }
        }

        _dotNetRef?.Dispose();
        _dotNetRef = null;
        _js = null;
    }
}
