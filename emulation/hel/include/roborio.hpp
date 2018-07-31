#ifndef _ROBORIO_HPP_
#define _ROBORIO_HPP_

/**
 * \file roborio.hpp
 * \brief Defines internal structure of mock RoboRIO
 * This file defines the RoboRIOs structure
 */

#define ASIO_STANDALONE
#define ASIO_HAS_STD_ADDRESSOF
#define ASIO_HAS_STD_ARRAY
#define ASIO_HAS_CSTDINT
#define ASIO_HAS_STD_SHARED_PTR
#define ASIO_HAS_STD_TYPE_TRAITS

#include <array>
#include <map>
#include <memory>
#include <mutex>
#include <vector>
#include <thread>
#include <atomic>

#include <asio.hpp>

#include "accelerometer.hpp"
#include "accumulator.hpp"
#include "alarm.hpp"
#include "analog_inputs.hpp"
#include "analog_outputs.hpp"
#include "can_motor_controller.hpp"
#include "counter.hpp"
#include "digital_system.hpp"
#include "fpga_encoder.hpp"
#include "error.hpp"
#include "global.hpp"
#include "joystick.hpp"
#include "match_info.hpp"
#include "net_comm.hpp"
#include "power.hpp"
#include "pwm_system.hpp"
#include "relay_system.hpp"
#include "receive_data.hpp"
#include "robot_mode.hpp"
#include "send_data.hpp"
#include "spi_system.hpp"
#include "sync_server.hpp"
#include "sys_watchdog.hpp"

namespace hel{

    extern std::atomic<bool> hal_is_initialized;

    /**
     * \struct RoboRIO roborio.hpp
     * \brief Mock RoboRIO implementation
     *
     * This class represents the internals of the RoboRIO hardware, broken up into several sub-systems:
     * Analog Input, Analog Output, PWM, DIO, SPI, MXP, RS232, and I2C.
     */
    struct RoboRIO{
        /**
         * \var bool user_button
         * \represents the state of the user button on the roborio
         */

        bool user_button;

        Accelerometer accelerometer;
        BoundsCheckedArray<Accumulator, AnalogInputs::NUM_ANALOG_INPUTS> accumulators;
        Alarm alarm;
        AnalogInputs analog_inputs;
        AnalogOutputs analog_outputs;
        std::map<uint32_t,CANMotorController> can_motor_controllers;
        BoundsCheckedArray<Counter, Counter::MAX_COUNTER_COUNT> counters;
        DigitalSystem digital_system;
        std::vector<DSError> ds_errors;
        MatchInfo match_info;
        BoundsCheckedArray<FPGAEncoder, FPGAEncoder::NUM_ENCODERS> fpga_encoders;
        Global global;
        BoundsCheckedArray<Joystick, Joystick::MAX_JOYSTICK_COUNT> joysticks;
        NetComm net_comm;
        Power power;
        PWMSystem pwm_system;
        RelaySystem relay_system;
        RobotMode robot_mode;
        SPISystem spi_system;
        SysWatchdog watchdog;

        explicit RoboRIO() = default;

        friend class RoboRIOManager;
        RoboRIO(RoboRIO const&) = default;
    private:
        RoboRIO& operator=(const RoboRIO& r) = default;
    };
}
#endif
