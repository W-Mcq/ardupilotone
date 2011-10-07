/*
 * TankGeneric.h
 *
 *  Created on: Sep 26, 2011
 *      Author: jgoppert
 */

#ifndef TANKGENERIC_H_
#define TANKGENERIC_H_

// vehicle options
static const apo::vehicle_t vehicle = apo::VEHICLE_TANK;
static const apo::halMode_t halMode = apo::MODE_LIVE;
static const apo::board_t board = apo::BOARD_ARDUPILOTMEGA_1280;
static const uint8_t heartBeatTimeout = 3;

// algorithm selection
#define CONTROLLER_CLASS ControllerTank
#define GUIDE_CLASS MavlinkGuide
#define NAVIGATOR_CLASS DcmNavigator
#define COMMLINK_CLASS MavlinkComm

// hardware selection
#define ADC_CLASS AP_ADC_ADS7844
#define COMPASS_CLASS AP_Compass_HMC5843
#define BARO_CLASS APM_BMP085_Class
#define RANGE_FINDER_CLASS AP_RangeFinder_MaxsonarXL
#define DEBUG_BAUD 57600
#define TELEM_BAUD 57600
#define GPS_BAUD 38400
#define HIL_BAUD 57600

// optional sensors
static bool gpsEnabled = false;
static bool baroEnabled = false;
static bool compassEnabled = false;

static bool rangeFinderFrontEnabled = false;
static bool rangeFinderBackEnabled = false;
static bool rangeFinderLeftEnabled = false;
static bool rangeFinderRightEnabled = false;
static bool rangeFinderUpEnabled = false;
static bool rangeFinderDownEnabled = false;

// loop rates
static const float loop0Rate = 150;
static const float loop1Rate = 100;
static const float loop2Rate = 10;
static const float loop3Rate = 1;
static const float loop4Rate = 0.1;

// gains
const float steeringP = 1.0;
const float steeringI = 0.0;
const float steeringD = 0.0;
const float steeringIMax = 0.0;
const float steeringYMax = 3.0;

const float throttleP = 0.0;
const float throttleI = 0.0;
const float throttleD = 0.0;
const float throttleIMax = 0.0;
const float throttleYMax = 0.0;
const float throttleDFCut = 3.0;

#include "ControllerTank.h"

#endif /* TANKGENERIC_H_ */