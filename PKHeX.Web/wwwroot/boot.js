// Starts Blazor only in a browser that can run it, and turns a failed start into a retry screen instead of a stuck loading message.
// A classic script, because the CSP allows no inline script. Messages live in index.html; this only chooses which one to show.
// It must still parse in the old browsers it exists to turn away, so it keeps to ES2015 syntax: no optional chaining, nullish coalescing,
// optional catch binding or async functions (a Unit test checks this).
(() => {
    'use strict';

    // Smallest modules using the WebAssembly features the .NET 10 runtime is built with.
    // SIMD: a function returning i8x16.popcnt(i8x16.splat(0)).
    const simdModule = new Uint8Array([0, 97, 115, 109, 1, 0, 0, 0, 1, 5, 1, 96, 0, 1, 123, 3, 2, 1, 0, 10, 10, 1, 8, 0, 65, 0, 253, 15, 253, 98, 11]);
    // Exception handling: a function containing try … catch_all … end.
    const exceptionsModule = new Uint8Array([0, 97, 115, 109, 1, 0, 0, 0, 1, 4, 1, 96, 0, 0, 3, 2, 1, 0, 10, 8, 1, 6, 0, 6, 64, 25, 11, 11]);

    function validates(bytes) {
        try {
            return WebAssembly.validate(bytes);
        } catch (e) {
            return false;
        }
    }

    /** Names of required features this browser lacks; empty when it has them all. */
    function missingFeatures() {
        const missing = [];
        if (typeof WebAssembly !== 'object' || typeof WebAssembly.validate !== 'function') {
            missing.push('WebAssembly');
        } else {
            if (!validates(simdModule)) {
                missing.push('WebAssembly SIMD');
            }
            if (!validates(exceptionsModule)) {
                missing.push('WebAssembly exception handling');
            }
        }
        if (typeof BigInt64Array !== 'function') {
            missing.push('64-bit integer arrays');
        }
        if (typeof Blob !== 'function' || typeof URL !== 'function' || typeof URL.createObjectURL !== 'function') {
            missing.push('file downloads');
        }
        return missing;
    }

    /** Replaces the loading message with the failure screen `id`, and moves focus to its action if it has one. */
    function show(id) {
        // The class also keeps Blazor's error bar hidden (app.css), since later runtime errors would show it again.
        document.body.classList.add('boot-stopped');
        document.getElementById('app').hidden = true;
        document.getElementById('boot-slow').hidden = true;
        const screen = document.getElementById(id);
        screen.hidden = false;
        const action = screen.querySelector('button');
        if (action) {
            action.focus();
        }
    }

    const missing = missingFeatures();
    if (missing.length > 0) {
        document.getElementById('boot-missing').textContent = missing.join(', ');
        show('boot-unsupported');
        return;
    }

    /** Undoes show(), for a start that succeeded after all. */
    function restore() {
        document.body.classList.remove('boot-stopped');
        document.getElementById('app').hidden = false;
        document.getElementById('boot-failed').hidden = true;
        document.getElementById('boot-slow').hidden = true;
    }

    const reload = () => window.location.reload();
    document.getElementById('boot-retry').addEventListener('click', reload);
    document.getElementById('boot-slow-retry').addEventListener('click', reload);

    // In .NET 10 the promise from Blazor.start() does not settle when a runtime or assembly download fails: depending on the engine,
    // startup either hangs or throws "Failed to start platform" from an inner async function. What every engine does surface is an
    // unhandled rejection from the loader naming the failed file under _framework/ ("download '…/_framework/…' failed …"), and a healthy
    // boot raises none (the E2E boot checks require zero page errors). Only errors that name _framework/ count, so an unrelated error,
    // such as one from a browser extension, cannot turn a slow but healthy start into a failure. Once such errors have stopped for
    // failureGraceMs and start has not completed, the retry screen is shown; in testing a single aborted assembly download already
    // stopped the start for good in every engine, retry or not. If start still completes, the app replaces the retry screen.
    const failureGraceMs = 3000;
    // A download that never answers raises nothing, so after this long a non-destructive hint offers a retry while loading continues.
    const slowHintMs = 30000;
    let started = false;
    let failureTimer;

    /** True for an error or rejection reason that concerns the app's own runtime files. */
    function concernsFramework(reason) {
        if (reason === null || reason === undefined) {
            return false;
        }
        const text = typeof reason === 'object' ? String(reason.message) + ' ' + String(reason.stack) : String(reason);
        return text.indexOf('/_framework/') >= 0;
    }

    function onFrameworkError(reason) {
        if (started || !concernsFramework(reason)) {
            return;
        }
        clearTimeout(failureTimer);
        failureTimer = setTimeout(() => {
            if (!started) {
                show('boot-failed');
            }
        }, failureGraceMs);
    }
    const onRejection = event => onFrameworkError(event.reason);
    const onError = event => onFrameworkError(event.error || { message: event.message, stack: event.filename });
    const slowTimer = setTimeout(() => {
        if (!started) {
            document.getElementById('boot-slow').hidden = false;
        }
    }, slowHintMs);

    function onStarted() {
        started = true;
        clearTimeout(failureTimer);
        clearTimeout(slowTimer);
        window.removeEventListener('unhandledrejection', onRejection);
        window.removeEventListener('error', onError);
        restore();
    }
    window.addEventListener('unhandledrejection', onRejection);
    window.addEventListener('error', onError);

    // The catch covers a start() that rejects, and a blazor.webassembly.js that did not load at all.
    try {
        Blazor.start().then(onStarted, () => show('boot-failed'));
    } catch (e) {
        show('boot-failed');
    }
})();
