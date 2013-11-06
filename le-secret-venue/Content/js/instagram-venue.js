

$(document).ready(function () {

    console.log('venue id: ', venue_id);


    $.ajax({
        url: "https://api.instagram.com/v1/locations/" + venue_id + "/media/recent?client_id=d00aa6e4a8744e72ad267ea8d09b905f",
        jsonpCallback: 'jsonCallback',
        dataType: 'jsonp'
    }).done(function (result) {
        result.data.forEach(function (element, index, array) {
            var img_html = ich.img_temp(element);
            $('#img-container').append(img_html);
        });
        $('#venue-name').text(result.data[0].location.name);
    });
});

