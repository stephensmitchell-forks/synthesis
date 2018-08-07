#pragma once

#include <map>
#include <vector>
#include <Fusion/Components/Joint.h>
#include "CustomJSONObject.h"
#include "Driver.h"
#include "JointSensor.h"

using namespace adsk;

namespace BXDJ
{
	class ConfigData : public CustomJSONObject
	{
	public:
		std::string robotName;

		ConfigData();
		ConfigData(const ConfigData & other);

		std::unique_ptr<Driver> getDriver(core::Ptr<fusion::Joint>) const;
		void setDriver(core::Ptr<fusion::Joint>, const Driver &);
		void setNoDriver(core::Ptr<fusion::Joint>);

		std::vector<std::shared_ptr<JointSensor>> getSensors(core::Ptr<fusion::Joint>) const;

		// Removes joint configurations that are not in a vector of joints, and adds empty configurations for those not present.
		void filterJoints(std::vector<core::Ptr<fusion::Joint>>);

		rapidjson::Value getJSONObject(rapidjson::MemoryPoolAllocator<>&) const;
		void loadJSONObject(const rapidjson::Value&);

	private:
		// Constants used for communicating joint motion type
		enum JointMotionType : int
		{
			ANGULAR = 1, LINEAR = 2, BOTH = 3, NEITHER = 0
		};

		struct JointConfig
		{
			std::string name;
			JointMotionType motion;
			std::unique_ptr<Driver> driver;
			std::vector<std::shared_ptr<JointSensor>> sensors;

			JointConfig() { name = ""; motion = NEITHER; driver = nullptr; }

			JointConfig(const JointConfig & other)
			{
				name = other.name;
				motion = other.motion;
				driver = (other.driver == nullptr) ? nullptr : std::make_unique<Driver>(*other.driver);
				for (std::shared_ptr<JointSensor> sensor : other.sensors)
					sensors.push_back(std::make_shared<JointSensor>(*sensor));
			}

			JointConfig& JointConfig::operator=(const JointConfig &other)
			{
				name = other.name;
				motion = other.motion;
				driver = (other.driver == nullptr) ? nullptr : std::make_unique<Driver>(*other.driver);
				sensors.clear();
				for (std::shared_ptr<JointSensor> sensor : other.sensors)
					sensors.push_back(std::make_shared<JointSensor>(*sensor));
				return *this;
			}
		};

		std::map<std::string, JointConfig> joints;

		JointMotionType internalJointMotion(fusion::JointTypes) const;

	};
}
