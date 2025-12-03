let valorImpuesto = 0;

$(document).ready(function () {

    $("#txtSubTotal").val("0.00");
    $("#txtIGV").val("0.00");
    $("#txtTotal").val("0.00");

    //obteniendo tipos de documentos de venta
    fetch("/Venta/ObtenerTipoDocumentos").then(response => {
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        //si existen elementos en el Json
        if (responseJson.length > 0) {
            
            responseJson.forEach((item) => {
                $("#cboTipoDocumentoVenta").append(
                    $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
                )
            })

        }
    })


    //obteniendo datos del negocio
    fetch("/Administracion/GetNegocio").then(response => {
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        if (responseJson.estado)
        {
            const d = responseJson.objeto;

            $("#inputGroupSubTotal").text(`Sub Total-${d.simboloMoneda}`);
            $("#inputGroupIGV").text(`IGV(${d.porcentajeImpuesto}%)-${d.simboloMoneda}`);
            $("#inputGroupTotal").text(`Total-${d.simboloMoneda}`);

            valorImpuesto = parseFloat(d.porcentajeImpuesto);//almacena el valor del impuesto

        }
    })


    //filtrando productos en el select2
    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Venta/ObtenerProductos",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            delay: 250,//tiempo de busqueda que demora
            data: function (params) {
                return {
                    busqueda: params.term, // search term
                };
            },
            processResults: function (data) {
                
                return {
                    results: data.map((item) =>
                    (
                        {
                            id: item.idProducto,
                            text: item.descripcion,


                            marca: item.marca,
                            categoria: item.categoria,
                            urlImagen: item.urlImagen,
                            precio: parseFloat(item.precio),


                        }
                    ))
                    
                };
            },
           
        },
        language: "es",
        placeholder: 'Buscar Producto',
        minimumInputLength: 1,
        templateResult: formatoResultados,
    });

});


function formatoResultados(data)
{
    if (data.loading)
        return data.text;

    var contenedor = $(
        `<table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="heigth:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
                </td>

                <td>
                    <p style="font-weight: bolder;margin:2px">${data.marca}</p>
                    <p style="margin:2px">${data.text}</p>
                </td>
            </tr>
        </table>`
    );
    return contenedor;
}


//cargar el foco al abrir el select2
$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})


let productosParaVenta = [];
//cuando se selecciona un producto del select
$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;//obteniendo datos del producto seleccionado
    let produdctoEncontrado = productosParaVenta.filter(p => p.idProducto === data.id);//buscando si el producto ya fue agregado al detalle de venta
    if (produdctoEncontrado.length > 0)
    {
        $("#cboBuscarProducto").val("").trigger("change");
        const mensaje = `El producto ya fue agregado al detalle de la venta`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.warning("", mensaje)
        return false;
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type:"input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese la Cantidad",
    }, function (cantidad)
    {
        if (cantidad === false) return false;
        if (cantidad === "") {
            toastr.warning("", "Necesita ingresar la cantidad");
            return false;
        }
        if (isNaN(parseInt(cantidad))) {
            toastr.warning("", "Ingrese un valor numérico válido");
            return false;
        }

        //obtjeto producto para agregar al detalle de venta osea al array productosParaVenta para la tabla y la venta
        let producto = {
            idProducto: data.id,
            marcaProducto: data.marca,
            descripcionProducto: data.text,
            categoriaProducto: data.categoria,
            cantidad: parseInt(cantidad),
            precio: data.precio.toString(),
            total: (parseFloat(cantidad) * data.precio).toString()
        }

        productosParaVenta.push(producto);

        MostrarPrecioProductos();
        $("#cboBuscarProducto").val("").trigger("change");
        swal.close(); 
    }
    )
});


function MostrarPrecioProductos() {
    let total = 0;
    let igv = 0;
    let subTotal = 0;
    let porcentajeImpuesto = valorImpuesto / 100;
    let totalIva = 0;

    $("#tbProducto tbody").html("");//limpiando el tbody

    productosParaVenta.forEach((item, index) => {
        total = total + (parseFloat(item.precio) * parseFloat(item.cantidad));
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-sm btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(parseFloat(item.precio).toFixed(2)),
                $("<td>").text(parseFloat(item.total).toFixed(2))

            )

        )
    })

    subTotal = total;
    igv = subTotal * porcentajeImpuesto;
    totalIva = subTotal + igv;

    $("#txtSubTotal").val(subTotal.toFixed(2));
    $("#txtIGV").val(igv.toFixed(2));
    $("#txtTotal").val(totalIva.toFixed(2));


}


//funcionalidad boton eliminar en el detalle de venta
$(document).on("click","button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto")

    productosParaVenta = productosParaVenta.filter(p => p.idProducto != _idProducto);//devuelve todos los productos menos el que se desea eliminar u omitir

    MostrarPrecioProductos();
})


$("#btnTerminarVenta").on("click", function ()
{
    if (productosParaVenta.length < 1)//validando que existan productos en el detalle de venta
    {
        const mensaje = `Debe de ingresar productos`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.warning("", mensaje)
        return false;
    }

    //creando el objeto para mandarlo al endpoint
    const vmDetalleVEnta = productosParaVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        detalleVenta: vmDetalleVEnta
    }
    console.log(venta);
    $("#btnTerminarVenta").LoadingOverlay("show");


    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(venta),
    }).then(response => {
        $("#btnTerminarVenta").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        if (responseJson.estado) {
            productosParaVenta = [];//limpiando el array de productos para nueva venta
            MostrarPrecioProductos();
            $("#txtDocumentoCliente").val("")
            $("#txtNombreCliente").val("")
            $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())

            swal("Venta Registrada!", `Numero Venta:${responseJson.objeto.numeroVenta}`, "success")

        } else {
            swal("Error!", "No se pudo registrar la venta", "error")
        }
    })

})

