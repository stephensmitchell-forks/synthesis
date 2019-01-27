#include "roborio_manager.hpp"
#include "json_util.hpp"

namespace hel{
    BoundsCheckedArray<bool, PCM::NUM_SOLENOIDS> PCM::getSolenoids()const noexcept{
        return solenoids;
    }

    void PCM::setSolenoid(uint8_t index, bool value){
        solenoids[index] = value;
    }

    void PCM::setSolenoids(uint8_t values){
        for(unsigned i = 0; i < solenoids.size(); i++){
            solenoids[i] = checkBitHigh(values, i);
        }
    }

    void PCM::setSolenoids(const BoundsCheckedArray<bool,NUM_SOLENOIDS>& values){
        solenoids = values;
    }

    std::string PCM::serialize()const{
        std::string s = "{";
        s += serializeList(
            "\"solenoids\"",
            solenoids,
            std::function<std::string(bool)>(static_cast<std::string(*)(bool)>(asString))
            );
        s += "}";
        return s;
    }

    std::string PCM::toString()const{
        std::string s = "(";
        s += "solenoids:" + asString(
            solenoids,
            std::function<std::string(bool)>(static_cast<std::string(*)(bool)>(asString))
        );
        s += ")";
        return s;
    }

    PCM::PCM()noexcept:solenoids(false){}
    PCM::PCM(const PCM& source)noexcept:PCM(){
#define COPY(NAME) NAME = source.NAME
        COPY(solenoids);
#undef COPY
    }
}
