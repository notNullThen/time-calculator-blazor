// Shows a small banner if Blazor Server can't connect.
// This runs even when Blazor interactivity never starts (the "buttons do nothing" symptom).

(function () {
  function ensureBanner(message, details) {
    if (document.getElementById('tc-interactive-banner')) return;

    const el = document.createElement('div');
    el.id = 'tc-interactive-banner';
    el.style.position = 'fixed';
    el.style.left = '12px';
    el.style.right = '12px';
    el.style.bottom = '12px';
    el.style.zIndex = '2147483647';
    el.style.padding = '12px 14px';
    el.style.borderRadius = '10px';
    el.style.border = '1px solid rgba(0,0,0,0.2)';
    el.style.background = 'rgba(255, 243, 205, 0.98)';
    el.style.color = '#1f1f1f';
    el.style.font = '14px/1.35 -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, Arial, sans-serif';
    el.style.boxShadow = '0 6px 24px rgba(0,0,0,0.18)';

    const title = document.createElement('div');
    title.style.fontWeight = '600';
    title.textContent = message;

    const small = document.createElement('div');
    small.style.opacity = '0.85';
    small.style.marginTop = '4px';
    small.textContent = details;

    const btn = document.createElement('button');
    btn.type = 'button';
    btn.textContent = 'Reload';
    btn.style.marginTop = '10px';
    btn.style.padding = '6px 10px';
    btn.style.borderRadius = '8px';
    btn.style.border = '1px solid rgba(0,0,0,0.25)';
    btn.style.background = '#fff';
    btn.addEventListener('click', () => location.reload());

    el.appendChild(title);
    el.appendChild(small);
    el.appendChild(btn);

    document.body.appendChild(el);
  }

  async function check() {
    // Give the app a moment to load scripts and attempt to connect.
    await new Promise(r => setTimeout(r, 2500));

    try {
      const res = await fetch('/_blazor/negotiate?negotiateVersion=1', { method: 'POST' });
      if (!res.ok) throw new Error('negotiate status ' + res.status);
      // If negotiate works, the issue is usually websocket transport blocked; don't show banner yet.
    } catch (e) {
      ensureBanner(
        'Not connected (Blazor Server)',
        'The phone can load HTML, but cannot reach /_blazor (SignalR). Check LAN IP, firewall, and that you opened http://<IP>:<port>.'
      );
    }
  }

  if (typeof window !== 'undefined' && typeof document !== 'undefined') {
    window.addEventListener('load', () => { check(); });
  }
})();
