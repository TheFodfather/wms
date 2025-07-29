$(function () {
    const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

    var table = $('#productsTable').DataTable({
        "dom": "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "processing": true,
        "ajax": {
            "url": "/Products/GetProducts",
            "type": "POST",
            "datatype": "json",
            "headers": {
                "RequestVerificationToken": antiForgeryToken
            }
        },
        "columns": [
            { "data": "sku", "className": "align-middle" },
            { "data": "name", "className": "align-middle" },
            { "data": "manufacturer", "className": "align-middle" },
            { "data": "category", "className": "align-middle" },
            { "data": "costPrice", "className": "text-end align-middle", "render": $.fn.dataTable.render.number(',', '.', 4, '£') },
            { "data": "sellPrice", "className": "text-end align-middle", "render": $.fn.dataTable.render.number(',', '.', 4, '£') },
            { "data": "qty", "className": "text-center align-middle" },
            {
                "data": "priceMarginPercentage",
                "className": "text-center align-middle",
                "render": function (data, type, row) {
                    if (type === 'display') {
                        let margin = parseFloat(data);
                        let textClass = 'text-danger';
                        if (margin > 25) {
                            textClass = 'text-success';
                        } else if (margin > 15) {
                            textClass = 'text-warning';
                        }
                        return `<span class="${textClass} fw-bold">${margin.toFixed(2)}%</span>`;
                    }
                    return data;
                }
            },
            {
                "data": "id",
                "className": "text-center align-middle",
                "render": function (data) {
                    var editUrl = `/Products/Edit/${data}`;
                    return `<div class="btn-group" role="group">` +
                        `<a href="${editUrl}" class="btn btn-sm btn-outline-primary" title="Edit"><i class="fas fa-edit"></i></a>` +
                        `<button class="btn btn-sm btn-outline-danger delete-btn" data-id="${data}" title="Delete"><i class="fas fa-trash"></i></button>` +
                        `</div>`;
                },
                "orderable": false,
                "searchable": false
            }
        ],
        "initComplete": function () {
            var createButton = $('<a href="/Products/Create" class="btn btn-primary ms-3">' +
                '<i class="fas fa-plus me-1"></i> Create New' +
                '</a>');
            $('#productsTable_filter').append(createButton);
        }
    });

    $('#productsTable').on('click', '.delete-btn', function () {
        var id = $(this).data('id');
        if (confirm("Are you sure you want to delete this product? This cannot be undone.")) {
            $('#deleteId').val(id);
            var form = $('#deleteForm');
            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                "headers": {
                    "RequestVerificationToken": antiForgeryToken
                },
                data: form.serialize(),
                success: function (result) {
                    if (result.success) {
                        table.ajax.reload(null, false);
                    }
                },
                error: function () {
                    alert("An error occurred while deleting the product.");
                }
            });
        }
    });
});