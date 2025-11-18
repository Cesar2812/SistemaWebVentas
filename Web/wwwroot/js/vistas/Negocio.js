$(document).ready(function () {

    $(".card-body").LoadingOverlay("show");
    //eventos de Negocio
    //para hacer solcitudes
    fetch("/Administracion/GetNegocio").then(response => {
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        $(".card-body").LoadingOverlay("hide");
        //si existen elementos en el Json
        if (responseJson.estado) {

            const objNegocio = responseJson.objeto;

            $("#txtNumeroDocumento").val(objNegocio.numeroDocumento);
            $("#txtRazonSocial").val(objNegocio.razonSocial);
            $("#txtCorreo").val(objNegocio.correo);
            $("#txtDireccion").val(objNegocio.direccion);
            $("#txTelefono").val(objNegocio.telefono);
            let impuestoDecimal = parseFloat(objNegocio.porcentajeImpuesto).toFixed(2);
            $("#txtImpuesto").val(impuestoDecimal);
            $("#txtSimboloMoneda").val(objNegocio.simboloMoneda);
            $("#imgLogo").attr("src", objNegocio.urlLogo);
        } else {
            swal("Error!", responseJson.mensaje, "error")
        }
    })


})



$("#btnGuardarCambios").click(function () {

    //validando campos
    const inputs = $("input.input-validar").serializeArray();//imputs con clase validar
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")//obtiene los inputs que estan vacios

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe Completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    } 

    //validando campo logo
    const fotoInput = $("#txtLogo")[0];
    if (!fotoInput.files || fotoInput.files.length === 0) {
        const mensaje = "Debe seleccionar un logo";
        toastr.options.timeOut = 1300;
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje);
        $("#txtLogo").focus();
        return;
    }

    const modeloNegocio = {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        razonSocial: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val(),
    }  

    const inputLogo = document.getElementById("txtLogo");
    const formData = new FormData();

    formData.append("logo", inputLogo.files[0]);
    formData.append("model", JSON.stringify(modeloNegocio));

    $(".card-body").LoadingOverlay("show");


    fetch("/Administracion/SaveChangesNegocio", {
        method: "POST",
        body: formData
    }).then(response => {
        $(".card-body").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        if (responseJson.estado) {
            swal("Listo!", "Datos Editados Correctamente", "success");
            const obj = responseJson.objeto;

            $("#imgLogo").attr("src", obj.urlLogo);


           
        } else {
            swal("Error!", responseJson.mensaje, "error");
        }
    });

})
