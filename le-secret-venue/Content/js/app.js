

$(document).ready(function () {

    $('#btn-search').click(function () {
        console.log('search button clicked...');
        //var data, data_html;

        //data = {
        //  "name": "Tripel",
        //  "address": "Playa Vista, CA",
        //  "rating": "9.1",
        //  "num_of_reviews": "175",
        //  "num_of_checkins": "97"
        //};

        //data_html = ich.data_temp(data);

        //$('#data-table-body').append(data_html);


        // new logic
        var near = $('#near').val();
        $.ajax({
            url: "http://localhost:52382/data/explore/" + near
        }).done(function (data) {
            //console.log('made ajax call: ', data);
            $('#data-table-body').html('');


            data.response.groups[0].items.forEach(function(element, index, array){
                data_html = ich.data_temp(element);
                $('#data-table-body').append(data_html);
            });
        });
    });

});