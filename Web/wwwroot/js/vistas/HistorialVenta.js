//para activar los distintos tipos de busqueda en el historial de ventas
const VistaBusqueda = {
    //muestra la busqueda por fecha
    busquedaFecha: () => {
        $("#txtFechaInicio").val("");
        $("#txtFechaFin").val("");
        $("#txtNumeroVenta").val("");

        $(".busqueda-fecha").show();
        $(".busqueda-venta").hide();

    },
    //muestra la busqueda por numero de venta
    busquedaVenta: () => {
        $("#txtFechaInicio").val("");
        $("#txtFechaFin").val("");
        $("#txtNumeroVenta").val("");

        $(".busqueda-fecha").hide();
        $(".busqueda-venta").show();
    }
}

$(document).ready(function () {

    VistaBusqueda["busquedaFecha"]();
    //cofigirar el idioma del calendario
    $.datepicker.setDefaults($.datepicker.regional["es"]);

    //configurar caja de texto para la fecha se muestre el calendario
    $("#txtFechaInicio").datepicker({ dateFormat:"dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
})


//el combo al cambiar de valor
$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VistaBusqueda["busquedaFecha"]();
    } else {
        VistaBusqueda["busquedaVenta"]();
    }
})


$("#btnBuscar").click(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        //validando que las cajas de texto no sean vacias
        if ($("#txtFechaInicio").val().trim == "" || $("#txtFechaFin").val().trim == "") {
            const mensaje = `Debe ingresar las fechas para buscar`;
            toastr.options.timeOut = 1300;          // 1.5 segundos
            toastr.options.extendedTimeOut = 500;
            toastr.warning("", mensaje)
            return;
        }
    } else {
        if ($("#txtNumeroVenta").val().trim == "") {
            const mensaje = `Debe ingresar el numero de la venta para buscar`;
            toastr.options.timeOut = 1300;          // 1.5 segundos
            toastr.options.extendedTimeOut = 500;
            toastr.warning("", mensaje)
            return;
        }
    }

    let numeroVenta = $("#txtNumeroVenta").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()


    $(".card-body").find("div.row").LoadingOverlay("show");
    
    //para hacer solcitudes del historial
    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`).then(response => {
        $(".card-body").find("div.row").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {

        $("#tbventa tbody").html("")
        if (responseJson.length > 0) {
            responseJson.forEach((venta) => {
                $("#tbventa tbody").append(
                    $("<tr>").append(
                        $("<td>").text(venta.fechaRegistro),
                        $("<td>").text(venta.numeroVenta),
                        $("<td>").text(venta.tipoDocumentoVenta),
                        $("<td>").text(venta.documentoCliente),
                        $("<td>").text(venta.nombreCliente),
                        $("<td>").text(venta.total),
                        $("<td>").append(
                            $("<button>").addClass("btn btn-info btn-sm").append(
                                $("<i>").addClass("fas fa-eye")
                            ).data("venta",venta)
                        )
                    )
                )
            })
        }
       
    })
}) 



$("#tbventa tbody").on("click",".btn-info",function () {

    let d = $(this).data("venta")//obteniendo la informacion de data venta

    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtNumVenta").val(d.numeroVenta)
    $("#txtUsuarioRegistro").val(d.usuario)
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtSubTotal").val(d.subTotal)
    $("#txtIGV").val(d.impuestoTotal)
    $("#txtTotal").val(d.total)


    $("#tbProductos tbody").html("")

    d.detalleVenta.forEach((item) => {
        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),
               
            )
        )
    })

    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${d.numeroVenta}`)

    $("#modalData").modal("show")
}) 




