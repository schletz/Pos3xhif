var SearchForm = {
    container: "",
    mapInstance: null,
    get $container() { return $(container); },
    formData: {
        get ort() { return $(SearchForm.container + " input[name=ort]").val(); },
        set ort(val) { $(SearchForm.container + " input[name=ort]").val(val); }
    },

    searchLocation() {
        var self = this;
        var result = {};
        $.getJSON("https://nominatim.openstreetmap.org/search", { q: self.formData.ort, format: "json" })
            .done((data) => {
                if (Array.isArray(data) && data.length > 0) {
                    result.lon = data[0].lon;
                    result.lat = data[0].lat;
                    self.centerMap(result);  
                }
                else {
                    self.formData.ort = "Not found."
                }
            })
            .fail((e) => self.formData.ort = "Request failed. " + JSON.stringify(e));
    },
    /*
        searchLocation: function () {
            var self = this;
            var result = {};
            if (!self.formData.ort) { return false; }
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': self.formData.ort }, function (results, status) {
                try {
                    if (status === 'OK') {
                        result.lon = results[0].geometry.location.toJSON().lng;
                        result.lat = results[0].geometry.location.toJSON().lat;
                        self.centerMap(result);                
                    }
                    else {
                        self.formData.ort = status;
                    }
                }
                catch (e) {
                    self.formData.ort = e.message;
                }
            });
        },*/

    centerMap: function (pos) {
        this.mapInstance.setView(pos, 11);
    }

};