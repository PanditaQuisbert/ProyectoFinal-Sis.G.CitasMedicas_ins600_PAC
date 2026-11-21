// SALUD GESTIÓN — Reloj y fecha en tiempo real (zona horaria America/La_Paz).
// Sin dependencias externas. Si el navegador no reconoce la zona IANA,
// se degrada a la hora local del equipo sin interrumpir la aplicación.

(function () {
    'use strict';

    var ZONA_HORARIA = 'America/La_Paz';
    var LOCALE = 'es-BO';
    var temporizador = null;

    function crearFormato(opciones) {
        try {
            var conZona = Object.assign({ timeZone: ZONA_HORARIA }, opciones);
            return new Intl.DateTimeFormat(LOCALE, conZona);
        } catch (error) {
            return new Intl.DateTimeFormat(LOCALE, opciones);
        }
    }

    var formatoHora = crearFormato({
        hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false
    });
    var formatoFecha = crearFormato({
        weekday: 'long', day: '2-digit', month: 'long', year: 'numeric'
    });

    function actualizarReloj() {
        var hora = document.getElementById('relojHora');
        var fecha = document.getElementById('relojFecha');
        if (!hora || !fecha) return;

        var ahora = new Date();
        var textoHora = formatoHora.format(ahora);
        var textoFecha = formatoFecha.format(ahora);

        if (hora.textContent !== textoHora) hora.textContent = textoHora;
        if (fecha.textContent !== textoFecha) {
            fecha.textContent = textoFecha;
            fecha.setAttribute('title', textoFecha + ' · ' + ZONA_HORARIA);
        }
    }

    function iniciarReloj() {
        actualizarReloj();
        if (temporizador === null) {
            temporizador = window.setInterval(actualizarReloj, 1000);
        }
    }

    function detenerReloj() {
        if (temporizador !== null) {
            window.clearInterval(temporizador);
            temporizador = null;
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', iniciarReloj);
    } else {
        iniciarReloj();
    }

    window.addEventListener('pagehide', detenerReloj);
})();
