"use strict";
var util = require("gulp-util");
var helix = {};

helix.header = function header(line1, line2) {
    util.log("O---o   ______     _______               _______ ");
    util.log(" O-o   | (  \\  )  | (    \\/  | )   ( |  | (   ) |"); 
    util.log("  O    | |   ) |  | (__      | | _ | |  | (___) |");
    util.log(" o-O   | |   | |  |  __)     | |( )| |  |  ___  |");
    util.log("o---O  | |   ) |  | (        | || || |  | (   ) |");
    util.log("O---o  | (__/  )  | (____/\\  | () () |  | )   ( |");
    util.log(" O-o   (______/   (_______/  (_______)  |/     \\|");
    util.log("  O    ------------ www.dewa.gov.ae.-------------");
    util.log(" o-O   "); 
    util.log("o---O  " + line1);
    util.log("O---o  " + line2);
    util.log(" O-o   "); 
    util.log("  O   -------------------------------------------");
    util.log(" o-O   ");
    util.log("o---O  ");
};

module.exports = helix;
