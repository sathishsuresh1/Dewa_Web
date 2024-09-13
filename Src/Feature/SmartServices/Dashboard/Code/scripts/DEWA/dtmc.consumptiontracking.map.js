var onLoadInitaited = false;
var map;
var start;
var end;
var markerList = [];

var fromPos;
var zoomLevel;

var directionsService;
var directionsRenderer;

var geocoder;
var infowindow;

var _currentArrivalTime = "";

var styles = {
    default: [],
    style: [
        {
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#fcfcfc"
                }
            ]
        },
        {
            "elementType": "labels",
            "stylers": [
                {
                    "visibility": "on"
                }
            ]
        },
        {
            "elementType": "labels.icon",
            "stylers": [
                {
                    "visibility": "off"
                }
            ]
        },
        {
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#616161"
                }
            ]
        },
        {
            "elementType": "labels.text.stroke",
            "stylers": [
                {
                    "color": "#f5f5f5"
                }
            ]
        },
        {
            "featureType": "administrative",
            "elementType": "geometry",
            "stylers": [
                {
                    "visibility": "off"
                }
            ]
        },
        {
            "featureType": "administrative.land_parcel",
            "stylers": [
                {
                    "visibility": "on"
                }
            ]
        },
        {
            "featureType": "administrative.land_parcel",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#bdbdbd"
                }
            ]
        },
        {
            "featureType": "administrative.neighborhood",
            "stylers": [
                {
                    "visibility": "on"
                }
            ]
        },
        {
            "featureType": "poi",
            "stylers": [
                {
                    "visibility": "on"
                }
            ]
        },
        {
            "featureType": "poi",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#fcfcfc"
                }
            ]
        },
        {
            "featureType": "poi",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#757575"
                }
            ]
        },
        {
            "featureType": "poi.park",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#fcfcfc"
                }
            ]
        },
        {
            "featureType": "poi.park",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#fcfcfc"
                }
            ]
        },
        {
            "featureType": "road",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#eff0f0"
                }
            ]
        },
        {
            "featureType": "road",
            "elementType": "labels.icon",
            "stylers": [
                {
                    "visibility": "on"
                }
            ]
        },
        {
            "featureType": "road.arterial",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#757575"
                }
            ]
        },
        {
            "featureType": "road.highway",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#eff0f0"
                }
            ]
        },
        {
            "featureType": "road.highway",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#eff0f0"
                }
            ]
        },
        {
            "featureType": "road.local",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#ffffff"
                }
            ]
        },
        {
            "featureType": "transit",
            "stylers": [
                {
                    "visibility": "off"
                }
            ]
        },
        {
            "featureType": "transit.line",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#e5e5e5"
                }
            ]
        },
        {
            "featureType": "transit.station",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#eeeeee"
                }
            ]
        },
        {
            "featureType": "water",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#c9c9c9"
                }
            ]
        },
        {
            "featureType": "water",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#9e9e9e"
                }
            ]
        }
    ]
}


var dataInput;
var inputStartLat;
var inputStartLang;
var inputEndLat;
var inputEndLang;
var _currentNav;


var _errormsg;
var _originmarker;
var _desmarker;
var _DefaultTimetext;
var _dataloaderinterval;
var notif;
var _storedvalue;
var _guidtext;
var _imageUrl;

function initMap(currentNav) {

    if (currentNav != null) {
        _currentNav = currentNav;
    }


    inputStartLat = $(dataInput).data('slat');
    inputStartLang = $(dataInput).data('slngt');
    inputEndLat = $(dataInput).data('elat');
    inputEndLang = $(dataInput).data('elngt');

    start = { lat: parseFloat(inputStartLat), lng: parseFloat(inputStartLang) };
    end = { lat: parseFloat(inputEndLat), lng: parseFloat(inputEndLang) };


    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 7,
        mapTypeControl: false,
        zoomControl: false,
        scaleControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        center: start
    });
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        polylineOptions: {
            strokeColor: "#087B36",
            strokeWeight: 2
        },
        suppressMarkers: true
    });
    geocoder = new google.maps.Geocoder();
    infowindow = new google.maps.InfoWindow();

    //var fromDetail = geocodeLatLng(geocoder, start,"#origin-input");

    //console.log(fromDetail);
    //if (fromDetail != null && fromDetail != undefined) {

    //}

    directionsRenderer.setMap(map);

    const onChangeHandler = function () {
        calculateAndDisplayRoute(directionsService, directionsRenderer);
    };

    onChangeHandler()

    map.setOptions({
        styles: styles["style"]
    });

}
function calculateAndDisplayRoute(directionsService, directionsRenderer) {
    directionsService.route(
        {
            origin: start,
            destination: end,
            travelMode: google.maps.TravelMode.DRIVING
        },
        (response, status) => {
            //debugger;
            clearMarkers();
            _currentArrivalTime = "";
            var _orgin = start;
            var _destination = end;
            var _leg = null;
            var time = null;
            eraseCookie(_storedvalue);
            if (status === "OK") {
                directionsRenderer.setDirections(response);
                _leg = response.routes[0].legs[0];
                var arrivalSec = response.routes[0].legs[0].duration.value;
                _currentArrivalTime = fnGetTime(arrivalSec);
                time = response.routes[0].legs[0].duration.text;//fnGetDiffTime(arrivalSec);// ;
                _orgin = _leg.start_location;
                _destination = _leg.end_location;
                $("#error-content-ele").hide()

                createCookie(_storedvalue, arrivalSec, 0.5);
                if ($(".arrivaltime").length > 0) {
                    $(".arrivaltime").html($(".arrivaltime").data("msg").replace("{{t}}", _currentArrivalTime));
                }
            } else {

                $("#error-content").html(_errormsg);
                $("#error-content-ele").show();
            }
            console.log(response);
            /*Orgin marker*/
            var OrginMarkerDetail = { IconPath: _originmarker, Title: '', EInfo: true, Label: null, EToolTip: true }
            GetMarkerFulldetail(geocoder, _orgin, "#origin-input", OrginMarkerDetail)

            if (time == null) {

                time = _DefaultTimetext;
               
            }
            _desmarker = _imageUrl + time;
            /*Destination marker*/
            var DestMarkerDetail = { IconPath: _desmarker, Title: '', EInfo: true, Label: null, EToolTip: true, IsLast: true }

            setTimeout(function () {
                GetMarkerFulldetail(geocoder, _destination, null, DestMarkerDetail);
            }, 100)

            onLoadInitaited = true;
        }
    );
}

/*Create marker*/
function makeMarker(position, icon, title, _label) {
    return new google.maps.Marker({
        position: position,
        map: map,
        icon: icon,
        //label: {
        //    text: _label,
        //    color: "#000000",
        //    fontSize: "10px",
        //    fontWeight: "bold"
        //},
        title: title,
        //labelAnchor: { x: 0, y: 30 },
        //labelClass: 'mymarkerlabel'

    });
}


// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    setMapOnAll(null);
    markerList = [];
}

// Sets the map on all markers in the array.
function setMapOnAll(map) {
    for (let i = 0; i < markerList.length; i++) {
        markerList[i].setMap(map);
    }
}


/*Get Loaction Formatted Address */
function geocodeLatLng(geocoder, latlong, _cntrlId) {
    //debugger;
    geocoder.geocode({ location: latlong }, (results, status) => {
        console.log("geocodeLatLng");
        console.log(results);
        console.log(status);
        if (status === "OK") {
            $(_cntrlId).val(results[0].formatted_address);
        } else {
            //window.alert("Geocoder failed due to: " + status);
        }
    });
}

//MarkerDetail = { IconPath: "smaple.png", Title: 'Hello', EInfo: true, Label: "Hello", EToolTip: true, IsLast: true }
function GetMarkerFulldetail(geocoder, latlong, _cntrlId, makerDetail) {
    geocoder.geocode({ location: latlong }, (results, status) => {
        console.log("geocodeLatLng");
        console.log(results);
        console.log(status);



        var _address = null;
        if (status === "OK") {

            _address = results[0].formatted_address;
            if (_cntrlId != null) {
                $(_cntrlId).val(_address);
            }
        } else if ($(_cntrlId).length > 0) {
            _address = $(_cntrlId).data("defaultaddress");
            $(_cntrlId).val(_address)
        }


        if (makerDetail != null) {
            var titleDescription = makerDetail.Title + (makerDetail.EInfo ? _address : "");
            var mkr = makeMarker(latlong, makerDetail.IconPath, titleDescription, makerDetail.Label);
            markerList.push(mkr);

            if (markerList != null && markerList.length > 0 && makerDetail.IsLast) {
                var bounds = new google.maps.LatLngBounds();
                bounds.extend(markerList[0].position);
                bounds.extend(markerList[1].position);
                map.fitBounds(bounds);
                //debugger;
                fromPos = markerList[0].position;
                zoomLevel = map.getZoom();

                console.log("fromPos:" + fromPos)
                console.log("zoomLevel:" + zoomLevel)
                map.setCenter(fromPos);

                var apart = (zoomLevel <= 5 ? 3 : 10);
                map.setZoom(zoomLevel - (zoomLevel / apart))
            }


            if (makerDetail.EToolTip) {
                google.maps.event.addListener(mkr, 'click', function () {
                    infowindow.setContent('<div><b>' + titleDescription + '<b></div>')
                    infowindow.open(map, this);
                });
            }
        }
    });
}

jQuery(window).on("load", function () {
    setTimeout(function () {
        //var loadDeafult = false;
        //if (navigator != null && navigator != undefined) {
        //    var error = null;
        //    try {
        //        navigator.geolocation.getCurrentPosition(initMap);
        //    } catch (e) {
        //        error = e;
        //        loadDeafult = true;
        //    }
        //} else {
        //    loadDeafult = true;
        //}

        //if (loadDeafult) {
        //    initMap(null);
        //}
        dataInput = $(".mapdata");
        _errormsg = $(dataInput).data("serrormsg");
        _originmarker = $(dataInput).data("soriginmarker");
        _desmarker = $(dataInput).data("sdesmarker");
        _DefaultTimetext = $(dataInput).data("sdefaulttimetext");
        _dataloaderinterval = parseFloat($(dataInput).data("dataloaderinterval"));
        _imageUrl = $(dataInput).data("homeimage_apiurl");
        notif = $("#notificationPanel").data("nid");

        _storedvalue = "_artym" + notif;

        _guidtext = $(dataInput).data("guidtext");
        initMap(null);
        if (_dataloaderinterval > 20000) {

            setInterval(function () {
                ReloadMapData()
            }, _dataloaderinterval)
        };
    }, 50);



});


function ReloadMapData() {

    dataInput = $(".mapdata");
    inputStartLat = $(dataInput).data('slat');
    inputStartLang = $(dataInput).data('slngt');
    inputEndLat = $(dataInput).data('elat');
    inputEndLang = $(dataInput).data('elngt');
    notif = $("#notificationPanel").data("nid");


    var requestData = {
        N: notif,
        lat: inputStartLat,
        lng: inputStartLang,
        g: _guidtext
    };


    var _postUrl = $(dataInput).data("mapgeoapiurl");
    requestData = AddCustomForgeryToken(requestData, ".j120-smart-response--map");
    $.ajax({
        url: _postUrl,
        type: 'POST',
        data: requestData,
        beforeSend: function () {
            $(".m66-preloader-fullpage").show();
        },
        success: function (responseData) {
            //debugger;
            console.log(responseData);

            if (responseData.success) {
                $(dataInput).attr("slat", responseData.data.geolatitude);
                $(dataInput).attr("slngt", responseData.data.geolongitude);
                $(dataInput).data("slat", responseData.data.geolatitude);
                $(dataInput).data("slngt", responseData.data.geolongitude);
                inputStartLat = $(dataInput).data('slat');
                inputStartLang = $(dataInput).data('slngt');
                start = { lat: parseFloat(inputStartLat), lng: parseFloat(inputStartLang) };
                end = { lat: parseFloat(inputEndLat), lng: parseFloat(inputEndLang) };
                calculateAndDisplayRoute(directionsService, directionsRenderer);
                //location.reload();
            }
        },
        complete: function (x) {
            console.log(x);
            setTimeout(function () {
                $(".m66-preloader-fullpage").hide();
            }, 100)
        }

    })
}



var AddCustomForgeryToken = function (data, elementId) {
    data.__RequestVerificationToken = $(elementId + ' input[name=__RequestVerificationToken]').val();
    return data;
};


function fnGetTime(_sec) {
    var timeString = "";
    try {
        var date = new Date();
        var d = new Date(date.getTime() + _sec * 1000); // specify value for SECONDS here
        timeString = d.toLocaleTimeString();
    } catch (e) {
        console.log(e);
    }
    return timeString;
}

function fnGetDiffTime(_sec) {
    var timeString = "";
    try {
        var date = new Date(0);
        date.setSeconds(_sec)
        timeString = date.toISOString().substr(11, 8);
    } catch (e) {
        console.log(e);
    }
    return timeString;
}

