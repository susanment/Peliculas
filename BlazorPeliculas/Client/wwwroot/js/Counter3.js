export function mostrarAlerta(mensaje){
    return alert(mensaje + "hOLA ");
}

export function mostrarAlerta2(mensaje) {
    return mostrarAlerta3("ALERTA 3");
}
function mostrarAlerta3(mensaje) {
    return alert(mensaje);
}