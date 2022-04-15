function getData(selector, url, columns, params = []) {

    var table = $(selector).DataTable({
        processing: true,
        serverSide: true,
        searching: true,
        lengthChange: true,
        info: true,
        ordering: true,
        filter: true,
        order: [],
        autoWidth: true,
        ajax: {
            url: url,
            async: true,
            global: false,
            type: "Post",
            data: {
                parameters: params
            },
            dataSrc: function (json) {
                return json.data;
            }
        },
        columns: columns
    });

    return table;
}