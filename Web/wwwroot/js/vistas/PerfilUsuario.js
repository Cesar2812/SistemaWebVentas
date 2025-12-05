$(document).ready(function () {

    $(".container-fluid").LoadingOverlay("show");


    //para hacer solcitudes
    fetch("/Administracion/ObtenerUsuario").then(response => {
        $(".container-fluid").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        
        //si existen elementos en el Json
        if (responseJson.estado) {

            const d = responseJson.objeto;

            $("#imgFoto").attr("src", d.urlFoto);
            $("#txtNombre").val(d.nombre);
            $("#txtCorreo").val(d.correo);
            $("#txTelefono").val(d.telefono);
            $("#txtRol").val(d.nombreRol);
           
        } else {
            swal("Error!", responseJson.mensaje, "error")
        }
    })
})


$("#btnGuardarCambios").click(function () {

    if ($("#txtCorreo").val().trim() == "") {
        const mensaje = `Debe Completar el campo Correo:`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje)
        $("#txtCorreo").focus();
        return;
    }

    if ($("#txTelefono").val().trim() == "") {
        const mensaje = `Debe Completar el campo Telefono:`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje)
        $("#txTelefono").focus();
        return;
    } 


    swal({
        title: "Desea Guardar los Cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (respuesta) {
        if (respuesta == true) {
            $(".showSweetAlert").LoadingOverlay("show");

            let modelo = {
                correo: $("#txtCorreo").val(),
                telefono: $("#txTelefono").val()
            }

            fetch("/Administracion/GuardarPerfil", {
                method: "POST",
                headers: { "Content-Type": "application/json;charset=utf-8" },
                body: JSON.stringify(modelo),
            }).then(response => {

                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
            }).then(responseJson => {
                if (responseJson.estado) {
                    swal("Listo!", "Los cambios en su perfil se guardaron", "success")
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
        }
    }
    )
})


$("#btnCambiarClave").click(function () {
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

    if ($("#txtClaveNueva").val().trim() != $("#txtConfirmarClave").val().trim()) {
        const mensaje = `Las claves no coinciden:`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje)
        return;
       
    }

    let modelo = {
        claveActual:$("#txtClaveActual").val().trim(),
        claveNueva: $("#txtClaveNueva").val().trim(),
    }


    fetch("/Administracion/CambiarClave", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(modelo),
    }).then(response => {

        $(".showSweetAlert").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
    }).then(responseJson => {
        if (responseJson.estado) {
            swal("Listo!", "La Clave fue cambiada correctamente", "success")
            $("input.input-validar").val("")
        } else {
            swal("Error!", responseJson.mensaje, "error")
        }
    }) 






})


