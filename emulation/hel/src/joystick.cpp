#include "joystick.hpp"
#include "send_data.hpp"
#include "util.hpp"
#include "json_util.hpp"

namespace hel{
    bool Joystick::getIsXBox()const noexcept{
        return is_xbox;
    }

    void Joystick::setIsXBox(bool xbox)noexcept{
        is_xbox = xbox;
    }

    uint8_t Joystick::getType()const noexcept{
        return type;
    }

    void Joystick::setType(uint8_t t)noexcept{
        type = t;
    }

    std::string Joystick::getName()const noexcept{
        return name;
    }

    void Joystick::setName(std::string n)noexcept{
        name = n;
    }

    uint32_t Joystick::getButtons()const noexcept{
        return buttons;
    }

    void Joystick::setButtons(uint32_t b)noexcept{
        buttons = b;
    }

    uint8_t Joystick::getButtonCount()const noexcept{
        return button_count;
    }

    void Joystick::setButtonCount(uint8_t b_count)noexcept{
        button_count = b_count;
    }

    BoundsCheckedArray<int8_t, Joystick::MAX_AXIS_COUNT> Joystick::getAxes()const{
        return axes;
    }

    void Joystick::setAxes(BoundsCheckedArray<int8_t, Joystick::MAX_AXIS_COUNT> a){
        axes = a;
    }

    uint8_t Joystick::getAxisCount()const noexcept{
        return axis_count;
    }

    void Joystick::setAxisCount(uint8_t a_count)noexcept{
        axis_count = a_count;
    }

    BoundsCheckedArray<uint8_t, Joystick::MAX_AXIS_COUNT> Joystick::getAxisTypes()const{
        return axis_types;
    }

    void Joystick::setAxisTypes(BoundsCheckedArray<uint8_t, Joystick::MAX_AXIS_COUNT> a_types){
        axis_types = a_types;
    }

    BoundsCheckedArray<int16_t, Joystick::MAX_POV_COUNT> Joystick::getPOVs()const{
        return povs;
    }

    void Joystick::setPOVs(BoundsCheckedArray<int16_t, Joystick::MAX_POV_COUNT> p){
        povs = p;
    }

    uint8_t Joystick::getPOVCount()const noexcept{
        return pov_count;
    }

    void Joystick::setPOVCount(uint8_t p_count)noexcept{
        pov_count = p_count;
    }

    uint32_t Joystick::getOutputs()const noexcept{
        return outputs;
    }

    void Joystick::setOutputs(uint32_t out)noexcept{
        outputs = out;
    }

    uint16_t Joystick::getLeftRumble()const noexcept{
        return left_rumble;
    }

    void Joystick::setLeftRumble(uint16_t rumble)noexcept{
        left_rumble = rumble;
    }

    uint16_t Joystick::getRightRumble()const noexcept{
        return right_rumble;
    }

    void Joystick::setRightRumble(uint16_t rumble)noexcept{
        right_rumble = rumble;
    }

    std::string Joystick::toString()const{
        std::string s = "(";
        s += "is_xbox:" + asString(is_xbox) + ", ";
        s += "type:" + std::to_string(type) + ", ";
        s += "name:" + name + ", ";
        s += "buttons:" + std::to_string(buttons) + ", ";
        s += "button_count:" + std::to_string((int)button_count) + ", ";
        s += "axes:" + asString(axes, std::function<std::string(int8_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += "axis_count:" + std::to_string(axis_count) + ", ";
        s += "axis_types" + asString(axis_types, std::function<std::string(uint8_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += "povs:" + asString(povs, std::function<std::string(int16_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += "pov_count:" + std::to_string(pov_count) +", ";
        s += "outputs:" + std::to_string(outputs) + ", ";
        s += "left_rumble:" + std::to_string(left_rumble) + ", ";
        s += "right_rumble:" + std::to_string(right_rumble);
        s += ")";
        return s;
    }
    std::string Joystick::serialize()const{
        std::string s = "{";
        s += "\"is_xbox\":" + asString(is_xbox) + ", ";
        s += "\"type\":" + std::to_string(type) + ", ";
        s += "\"name\":" + quote(name) + ", ";
        s += "\"buttons\":" + std::to_string(buttons) + ", ";
        s += "\"button_count\":" + std::to_string((int)button_count) + ", ";
        s += serializeList("\"axes\"", axes, std::function<std::string(int8_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += "\"axis_count\":" + std::to_string(axis_count) + ", ";
        s += serializeList("\"axis_types\"", axis_types, std::function<std::string(uint8_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += serializeList("\"povs\"", povs, std::function<std::string(int16_t)>(static_cast<std::string(*)(int)>(std::to_string))) + ", ";
        s += "\"pov_count\":" + std::to_string(pov_count) +", ";
        s += "\"outputs\":" + std::to_string(outputs) + ", ";
        s += "\"left_rumble\":" + std::to_string(left_rumble) + ", ";
        s += "\"right_rumble\":" + std::to_string(right_rumble);
        s += "}";
        return s;
    }

    Joystick Joystick::deserialize(std::string input){
        Joystick joy;
        joy.is_xbox = stob(pullObject("\"is_xbox\"", input));
        joy.type = std::stoi(pullObject("\"type\"",input));
        joy.name = unquote(pullObject("\"name\"", input));
        joy.buttons = std::stoi(pullObject("\"buttons\"", input));
        joy.button_count = std::stoi(pullObject("\"button_count\"", input));
        std::vector<int8_t> axes_deserialized = deserializeList(pullObject("\"axes\"",input), std::function<int8_t(std::string)>([&](std::string input){ return std::stoi(input);}), true);
        if(axes_deserialized.size() == joy.axes.size()){
            joy.axes = axes_deserialized;
        } else {
            throw std::out_of_range("Exception: deserialization resulted in array of " + std::to_string(axes_deserialized.size()) + " axes, expected " + std::to_string(joy.axes.size()));
        }
        joy.axis_count = std::stoi(pullObject("\"axis_count\"", input));
        std::vector<uint8_t> axis_types_deserialized = deserializeList(pullObject("\"axis_types\"",input), std::function<uint8_t(std::string)>([&](std::string input){ return std::stoi(input);}), true);
        if(axis_types_deserialized.size() == joy.axis_types.size()){
            joy.axis_types = axis_types_deserialized;
        } else {
            throw std::out_of_range("Exception: deserialization resulted in array of " + std::to_string(axis_types_deserialized.size()) + " axis types, expected " + std::to_string(joy.axis_types.size()));
        }
        std::vector<int16_t> povs_deserialized = deserializeList(pullObject("\"povs\"",input), std::function<int16_t(std::string)>([&](std::string input){ return std::stoi(input);}), true);
        if(povs_deserialized.size() == joy.povs.size()){
            joy.povs = povs_deserialized;
        } else {
            throw std::out_of_range("Exception: deserialization resulted in array of " + std::to_string(povs_deserialized.size()) + " povs, expected " + std::to_string(joy.povs.size()));
        }
        joy.pov_count = std::stoi(pullObject("\"pov_count\"", input));
        joy.outputs = std::stoi(pullObject("\"outputs\"", input));
        joy.left_rumble = std::stoi(pullObject("\"left_rumble\"", input));
        joy.right_rumble = std::stoi(pullObject("\"right_rumble\"", input));

        return joy;
    }

    Joystick::Joystick()noexcept:is_xbox(false), type(0), name(""), buttons(0), button_count(0), axes(0), axis_count(0), axis_types(0), povs(-1), pov_count(0), outputs(0), left_rumble(0), right_rumble(0){}

    Joystick::Joystick(const Joystick& source)noexcept:Joystick(){
#define COPY(NAME) NAME = source.NAME
        COPY(is_xbox);
        COPY(type);
        COPY(name);
        COPY(buttons);
        COPY(button_count);
        COPY(axes);
        COPY(axis_count);
        COPY(axis_types);
        COPY(povs);
        COPY(pov_count);
        COPY(outputs);
        COPY(left_rumble);
        COPY(right_rumble);
#undef COPY
    }
}
