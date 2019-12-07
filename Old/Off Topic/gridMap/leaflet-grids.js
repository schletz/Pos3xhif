/* jshint strict:global */
/* globals $, L, console */

"use strict";
/*
 *
 * Inspired by Leaflet.Grid: https://github.com/jieter/Leaflet.Grid
 * https://github.com/trailbehind/leaflet-grids
 */

L.Grids = L.LayerGroup.extend({
    options: {
        redraw: 'move', // or moveend depending when the grids is refreshed
        groups: [],
        lineStyle: {
            stroke: true,
            color: '#111',
            opacity: 0.6,
            weight: 1,
            clickable: false
        },
        zoneStyle: {
            stroke: true,
            color: '#333',
            opacity: 0.6,
            weight: 4,
            clickable: false
        },
    },

    initialize: function (options) {
        L.LayerGroup.prototype.initialize.call(this);
        L.Util.setOptions(this, options);
    },

    onAdd: function (map) {
        this._map = map;
        var grid = this.redraw();

        // Create a listener to redraw the map when it's moving
        this._map.on('viewreset ' + this.options.redraw, function () {
            grid.redraw();
        });
    },

    onRemove: function (map) {
        this._map = map;

        // Remove listener and grids
        this._map.off('viewreset ' + this.options.redraw);
        this.eachLayer(this.removeLayer, this);
    },

    redraw: function () {
        this._lngCoords = [];
            this._latCoords = [];
            this._gridLabels = [];
            this._mapZoom = this._map.getZoom();
        this._bounds = this._map.getBounds(); //.pad(0.5);
        this._gridSize = this._gridSpacing();

        var gridLines = this._gridLines();
        var gridGroup = L.layerGroup();

        for (var i in gridLines) {
            try {
                gridGroup.addLayer(gridLines[i]);
            }
            catch (err) {
                console.log(err);
                console.log("*******");
                console.log(gridLines[i]);
            }
        }

        for (i in this._gridLabels) {
            gridGroup.addLayer(this._gridLabels[i]);
        }
        // First, remove old layer before drawing the new one
        this.eachLayer(this.removeLayer, this);
        // Second, add the new grid
        gridGroup.addTo(this);
        return this;
    },

    _gridSpacing: function () {
        return this.options.coordinateGridSpacing / 3600.0;
    },

    _gridLines: function () {
        var lines = [];
        var labelPt, labelText;
        var labelBounds = this._map.getBounds().pad(-0.03);
        var labelNorth = labelBounds.getNorth();
        var labelSouth = labelBounds.getSouth();
        var labelWest = labelBounds.getWest();
        var latCoord = this._snap(this._bounds.getSouth());
        var northBound = this._bounds.getNorth();
        while (latCoord < northBound) {
            lines.push(this._horizontalLine(latCoord));
            labelPt = L.latLng(latCoord, labelWest);
            labelText = this._labelFormat(latCoord, 'lat');
            this._gridLabels.push(this._label(labelPt, labelText, 'lat'));
            latCoord += this._gridSize;
        }
        var lngCoord = this._snap(this._bounds.getWest());
        var eastBound = this._bounds.getEast();

        while (lngCoord < eastBound) {
            lines.push(this._verticalLine(lngCoord));
            labelPt = L.latLng(labelSouth, lngCoord);
            labelText = this._labelFormat(lngCoord, 'lng');
            this._gridLabels.push(this._label(labelPt, labelText, 'lng'));
            lngCoord += this._gridSize;
        }
        return lines;
    },

    _snap: function (num) {
        return Math.floor(num / this._gridSize) * this._gridSize;
    },

    _snapTo: function (num, snap) {
        return Math.floor(num / snap) * snap;
    },

    _verticalLine: function (lng, options) {
        var upLimit,
            downLimit,
            style;
        if (options) {
            upLimit = options.upLimit ? options.upLimit : this._bounds.getNorth();
            downLimit = options.downLimit ? options.downLimit : this._bounds.getSouth();
            style = options.style ? options.style : this.options.lineStyle;
        } else {
            upLimit = this._bounds.getNorth();
            downLimit = this._bounds.getSouth();
            style = this.options.lineStyle;
        }
        return L.polyline([
            [upLimit, lng],
            [downLimit, lng]
        ], style);
    },

    _horizontalLine: function (lat, options) {
        return L.polyline([
            [lat, this._bounds.getWest()],
            [lat, this._bounds.getEast()]
        ], options ? options : this.options.lineStyle);
    },

    _label: function (latLng, labelText, cssClass) {
        return L.marker(latLng, {
            icon: L.divIcon({
                className: 'leaflet-grids-label',
                html: '<div class="grid-label ' + cssClass + '">' + labelText + '</div>'
            })
        });
    }
});

L.grids = {};

/*
  DECIMAL DEGREE GRIDS
 */
L.Grids.DD = L.Grids.extend({
    options: {
        coordinateGridSpacing: 3600
    },

    _labelFormat: function (coord, dir) {
        if (coord < -180) { coord += 360; }
        if (coord >= 180) { coord -= 360; }
        var coordSec = Math.round(Math.abs(coord) * 3600.0);
        var deg = Math.floor(coordSec / 3600.0);
        var min = Math.floor((coordSec - 3600 * deg) / 60.0);
        var sec = coordSec - 3600 * deg - 60 * min;
        var coordLabel = deg + "°";
        if (min !== 0 || sec !== 0) coordLabel += min + "'";
        if (sec !== 0) coordLabel += sec + "'";
        if (dir == "lat") {
            coordLabel += (coord < 0 ? "S" : "N");
        }
        if (dir == "lng") {
            coordLabel += (coord < 0 ? "W" : "E");
        }        
        return coordLabel;
    }
});

L.grids.dd = function (options) {
    return new L.Grids.DD(options);
};

/*
  DISTANCE GRIDS BASE CLASS
 */

L.Grids.Distance = L.Grids.extend({

    _gridSpacing: function () {
        return this.options.coordinateGridSpacing;
    },

    _gridLines: function () {
        var lines = [];
        var i = 0;
        var zoom = this._map.getZoom();
        if (zoom < 3) {
            return null;
        }
        var metersAtEquator = metersPerPixel(0, zoom);
        var metersAtLat = metersPerPixel(this._map.getCenter().lat, zoom);
        var gridSize = this._gridSize * metersAtEquator / metersAtLat;

        var latCoord = LLtoSM(this._map.getCenter()).y;
        var latCoordUp = latCoord;
        var latCoordDown = latCoord;
        var eastBound = LLtoSM(this._bounds.getSouthEast()).x;
        var westBound = LLtoSM(this._bounds.getSouthWest()).x;
        var northBound = LLtoSM(this._bounds.getNorthWest()).y;
        var southBound = LLtoSM(this._bounds.getSouthWest()).y;
        // draw center horizontal line
        var leftPointCenter = SMtoLL(L.point(westBound, latCoord));
        var rightPointCenter = SMtoLL(L.point(eastBound, latCoord));
        lines.push(L.polyline([leftPointCenter, rightPointCenter], this.options.lineStyle));
        // draw horizontal lines from center out
        console.log(this._gridSize, latCoordUp, northBound);     
        while (latCoordUp < northBound) {
            latCoordUp += gridSize;
            latCoordDown -= gridSize;
            var latCoords = [latCoordUp, latCoordDown];
            for (i = 0; i < 2; i++) {
                var leftPoint = SMtoLL(L.point(westBound, latCoords[i]));
                var rightPoint = SMtoLL(L.point(eastBound, latCoords[i]));
                lines.push(L.polyline([leftPoint, rightPoint], this.options.lineStyle));
            }
        }
        // draw center vertical line
        var lngCoord = LLtoSM(this._bounds.getCenter()).x;
        var lngCoordRight = lngCoord;
        var lngCoordLeft = lngCoord;
        var topPointCenter = SMtoLL(L.point(lngCoord, northBound));
        var bottomPointCenter = SMtoLL(L.point(lngCoord, southBound));
        lines.push(L.polyline([topPointCenter, bottomPointCenter], this.options.lineStyle));
        // draw vertical lines from center out
        while (lngCoordRight < eastBound) {
            lngCoordRight += gridSize;
            lngCoordLeft -= gridSize;
            var lngCoords = [lngCoordLeft, lngCoordRight];
            for (i = 0; i < 2; i++) {
                var topPoint = SMtoLL(L.point(lngCoords[i], northBound));
                var bottomPoint = SMtoLL(L.point(lngCoords[i], southBound));
                lines.push(L.polyline([topPoint, bottomPoint], this.options.lineStyle));
            }
        }
        return lines;
    },
});

L.grids.distance = {};

/*
  METRIC DISTANCE GRIDS
 */

L.Grids.Distance.Metric = L.Grids.Distance.extend({
    options: {
        coordinateGridSpacing: 1000
    }
});

L.grids.distance.metric = function (options) {
    return new L.Grids.Distance.Metric(options);
};


// per  http://stackoverflow.com/questions/17664327/leaflet-panto-web-mercator-coordinates-epsg-3857
// and https://en.wikipedia.org/wiki/Earth_radius#Mean_radius

var EARTH_RADIUS = 6371000;

var SMtoLL = function (point) { // Spherical Mercator -> LatLng
    var projectionPoint = L.point(point);
    return L.Projection.SphericalMercator.unproject(projectionPoint);

};

var LLtoSM = function (point) { // LatLng -> Spherical Mercator 
    return L.Projection.SphericalMercator.project(point);

};

// per http://stackoverflow.com/questions/27545098/leaflet-calculating-meters-per-pixel-at-zoom-level/31266377#31266377

var metersPerPixel = function (lat, zoom) {
    return EARTH_RADIUS * Math.abs(Math.cos(lat / 180 * Math.PI)) / Math.pow(2, zoom + 8);
};

// get the column of a 2d array, from
// http://stackoverflow.com/questions/7848004/get-column-from-a-two-dimensional-array-in-javascript

function arrayColumn(arr, n) {
    return arr.map(x => x[n]);
}
