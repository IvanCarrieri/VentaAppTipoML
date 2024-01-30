

$(document).ready(function () {
    $('#tabla').DataTable({
        "paging": true,  // Habilitar paginación
        "searching": true,  // Habilitar búsqueda
        "info": true,  // Mostrar información de paginación
        "lengthMenu": [10, 25, 50, 75, 100],  // Opciones de cantidad de registros por página


        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Página",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar page _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },


    });
});
