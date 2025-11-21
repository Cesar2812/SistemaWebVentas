//Modulo de Usuarios
//ListarUsuario

const MODELO_BASE =
{
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    descripcion: "",
    idCategoria: 0,
    nombreCategoria: "",
    stock: 0,
    urlImagen: "",
    precio: 0,
    esActivo: 1,
}

let tablaData;

//documento listo

$(document).ready(function () {

    $("#txtPrecio").on("input", function () {
        this.value = this.value.replace(/[^0-9.]/g, '');

        if ((this.value.match(/\./g) || []).length > 1) {
            this.value = this.value.substring(0, this.value.length - 1);
        }
    });




    //para hacer solcitudes cargando categorias en el combo
    fetch("/Inventario/ListaCategorias").then(response => {
        return response.ok ? response.json() : Promise.reject(response)//si se devuelve la info de forma correcta retorna el json si no cancela la promesa
    }).then(responseJson => {
        //si existen elementos en el Json
        if (responseJson.data.length > 0) {
            $("<option>").attr({ "value": "0", "disabled": "true" }).text("Seleccione Una").appendTo("#cboCategoria");
            responseJson.data.forEach((item) => {
                $("#cboCategoria").append(
                    $("<option>").val(item.idCategoria).text(item.descripcion)
                )

            })

        }
    })



    //tabla de Productos
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Inventario/ListaProductos',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block" />`

                }
            },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "stock" },
            { "data": "precio", render: function (data) { return `${data.toFixed(2)}`; } },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1) {
                        return '<span class="badge badge-info"> Activo </span>';
                    } else {
                        return '<span class="badge badge-danger"> No Activo </span>';
                    }

                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],

        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})




//crearUsuario y Editar Usuario

//valor por defecto si esta vacio el modelo
function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idProducto)
    $("#txtCodigoBarra").val(modelo.codigoBarra)
    $("#txtMarca").val(modelo.marca)
    $("#txtDescripcion").val(modelo.descripcion),
    $("#cboCategoria").val(modelo.idCategoria)
    $("#txtStock").val(modelo.stock)
    $("#txtPrecio").val(modelo.precio)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtImagen").val("")
    $("#imgProducto").attr("src", modelo.urlImagen)

    $("#modalData").modal("show")
}


$("#btnNuevo").click(function () {
    MostrarModal();
})


$("#btnGuardar").click(function () {
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

    //validando rol
    const categoriaSeleccionada = $("#cboCategoria").val();

    if (categoriaSeleccionada === "0" || categoriaSeleccionada === null) {
        const mensaje = "Debe seleccionar una Categoria ";
        toastr.options.timeOut = 1300;
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje);
        $("#cboCategoria").focus();
        return;
    }

    //validando campo foto
    const fotoInput = $("#txtImagen")[0];
    if (!fotoInput.files || fotoInput.files.length === 0) {
        const mensaje = "Debe seleccionar una foto";
        toastr.options.timeOut = 1300;
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje);
        $("#txtImagen").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE)//copiando el modelobase

    modelo["idProducto"] = parseInt($("#txtId").val())
    modelo["codigoBarra"] = $("#txtCodigoBarra").val()
    modelo["marca"] = $("#txtMarca").val()
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["idCategoria"] = $("#cboCategoria").val()
    modelo["stock"] = $("#txtStock").val()
    modelo["precio"] = $("#txtPrecio").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtImagen") 

    console.log(modelo);

    const formData = new FormData();


    formData.append("imagen", inputFoto.files[0])//agregando la foto a la data
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    //creando un recurso 
    if (modelo.idProducto == 0) {
        fetch("/Inventario/CrearProducto", {
            method: "POST",
            body: formData
        }).then(response => {

            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
        }).then(responseJson => {
            if (responseJson.estado) {

                tablaData.row.add(responseJson.objeto).draw(false)

                $("#modalData").modal("hide")
                swal("Listo!", "Producto Creado", "success")
            } else {
                swal("Error!", responseJson.mensaje, "error")
            }
        })

    } else {
        fetch("/Inventario/EditarProducto", {
            method: "PUT",
            body: formData
        }).then(response => {

            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
        }).then(responseJson => {
            if (responseJson.estado) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                filaSeleccionada = null;
                $("#modalData").modal("hide")
                swal("Listo!", "Produdcto Editado", "success")
            } else {
                swal("Error!", responseJson.mensaje, "error")
            }
        })

    }


})


let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();

    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();

    MostrarModal(data);


})


//eliminarUsuario
$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();

    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "Esta Seguro?",
        text: `Eliminar al Producto: ${data.descripcion}`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si,eliminar",
        cancelButtonText: "No,Cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (respuesta) {
        if (respuesta == true) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`/Inventario/EliminarProducto?idProducto=${data.idProducto}`, {
                method: "DELETE",
            }).then(response => {

                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw()

                    swal("Listo!", "Producto Eliminado", "success")
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
        }
    }
    )
}) 