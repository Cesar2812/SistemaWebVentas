const MODELO_BASE =
{
    idCategoria: 0,
    descripcion: "",
    esActivo: 1,
} 


let tablaData;

//documento listo
$(document).ready(function () {
    
    //tabla de Catgeorias
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Inventario/ListaCategorias',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
           
            { "data": "descripcion" },
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
                filename: 'Reporte Categorias',
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



function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idCategoria)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#cboEstado").val(modelo.esActivo)
   

    $("#modalData").modal("show")
}

$("#btnNuevo").click(function () {
    MostrarModal();
})



$("#btnGuardar").click(function () {
   
    if ($("#txtDescripcion").val().trim() == "")
    {
        const mensaje = `Debe Completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.options.timeOut = 1300;          // 1.5 segundos
        toastr.options.extendedTimeOut = 500;
        toastr.error("", mensaje)
        $("#txtDescripcion").focus();
        return;
    }
    

    const modelo = structuredClone(MODELO_BASE)//copiando el modelobase

    modelo["idCategoria"] = parseInt($("#txtId").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["esActivo"] = $("#cboEstado").val()

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    //creando un recurso 
    if (modelo.idCategoria == 0) {
        fetch("/Inventario/CrearCategoria", {
            method: "POST",
            headers: {"Content-Type":"application/json;charset=utf-8"},
            body: JSON.stringify(modelo),
        }).then(response => {

            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
        }).then(responseJson => {
            if (responseJson.estado) {

                tablaData.row.add(responseJson.objeto).draw(false)

                $("#modalData").modal("hide")
                swal("Listo!", "Categoria Creada", "success")
            } else {
                swal("Error!", responseJson.mensaje, "error")
            }
        })

    } else {
        fetch("/Inventario/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo),
        }).then(response => {

            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
        }).then(responseJson => {
            if (responseJson.estado) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                filaSeleccionada = null;
                $("#modalData").modal("hide")
                swal("Listo!", "Categoria Editada", "success")
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



//eliminarCategoria
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
        text: `Eliminar la Categoria: ${data.descripcion}`,
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

            fetch(`/Inventario/Eliminar?idCategoria=${data.idCategoria}`, {
                method: "DELETE",
            }).then(response => {

                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response)//si se devuelve la respuesta retorna el json si no cancela la promesa
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw()

                    swal("Listo!", "Categoria Eliminada", "success")
                } else {
                    swal("Error!", responseJson.mensaje, "error")
                }
            })
        }
    }
    )
}) 