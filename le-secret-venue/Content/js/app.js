

$(document).ready(function () {

    $('#data-table').tablesorter();
    $('#myTable').tablesorter();

    $('#btn-search').click(function () {

        // new logic
        var near = $('#near').val();
        $.ajax({
            url: "http://localhost:52382/data/foursquare/" + near
        }).done(function (data) {
            $('#data-table-body').html('');


            data.response.groups[0].items.forEach(function(element, index, array){
                data_html = ich.data_temp(element);
                $('#data-table-body').append(data_html);
            });

            $('#data-table').trigger('update');
        });

        
        //$('#data-table').tablesorter();

        return false;
    });

});