"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const node_process_1 = require("node:process");
const proj4 = require("proj4");
const Position = require("./Position");
const Orientation = require("./Orientation");
const LTPENU = require("./WGS84ToLTPENU");
const Basic = require("./Basic");
const Advanced = require("./Advanced");
const Local = require("./Local");
const FrameTransform = require("./FrameTransform");
var source = proj4.Proj('EPSG:4326'); //source coordinates will be in Longitude/Latitude, WGS84
var dest = proj4.Proj('EPSG:3785'); //destination coordinates in meters, global spherical mercators projection, see http://spatialreference.org/ref/epsg/3785/
// transforming point coordinates
var p = proj4.toPoint([-76.0, 45.0, 11.0]); //any object will do as long as it has 'x' and 'y' properties
//var p = new proj4.Point($("#lng").val(), $("#lat").val());
let q = proj4.transform(source, dest, p);
let r = proj4.transform(dest, source, q);
console.log("X : " + p.x + " \nY : " + p.y + " \nZ : " + p.z);
console.log("X : " + q.x + " \nY : " + q.y + " \nZ : " + q.z);
console.log("X : " + r.x + " \nY : " + r.y + " \nZ : " + r.z);
let d = new LTPENU.LTP_ENU();
let from = new Position.GeodeticPosition(-1.0, 52.0, 15.0);
let origin = new Position.GeodeticPosition(-1.00005, 52.0, 15.3);
let to = new Position.CartesianPosition(0, 0, 0);
d.GeodeticToEnu(from, origin, to);
let myYPRLocal = new Basic.BasicYPR("OS_GB: BasicYPR", new Position.GeodeticPosition(51.5, -1.5, 12.3), new Orientation.YPRAngles(1, 2, 3));
let json = myYPRLocal.toJSON();
console.log(json);
let myQLocal = new Basic.BasicQuaternion("OS_GB: BasicQ", new Position.GeodeticPosition(51.5, -1.5, 23.4), new Orientation.Quaternion(0.1, 0.2, 0.3, 1.0));
json = myQLocal.toJSON();
console.log(json);
let myALocal = new Advanced.Advanced("OS_GB: Advanced", new FrameTransform.Extrinsic("epsg", "5819", "[1.5, -1.5, 23.4]"), new Orientation.Quaternion(0.1, 0.2, 0.3, 1.0));
json = myALocal.toJSON();
console.log(json);
let myLLocal = new Local.Local("OS_GB: Local", new FrameTransform.Translation(9.0, 8.7, 7.6), new Orientation.YPRAngles(1, 2, 3));
json = myLLocal.toJSON();
console.log(json);
node_process_1.stdin.read();
//# sourceMappingURL=app.js.map