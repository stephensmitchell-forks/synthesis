﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public enum RobotSensorType : byte
{
    LIMIT,
    ENCODER,
    POTENTIOMETER
}

public class RobotSensor
{
    public short module, port;
    public readonly RobotSensorType type;
    public float[] polyCoeff;
    /// <summary>
    /// If this is true source the secondary angle from the joint.  (Rotational instead of linear for cylindrical)
    /// </summary>
    public bool useSecondarySource = false;

    public RobotSensor(RobotSensorType type)
    {
        this.type = type;
    }

    public void WriteData(BinaryWriter writer)
    {
        writer.Write((byte) type);
        writer.Write(module);
        writer.Write(port);
        writer.Write(polyCoeff.Length);
        for (int i = 0; i < polyCoeff.Length; i++)
        {
            writer.Write(polyCoeff[i]);
        }
        writer.Write(useSecondarySource);
    }

    public static RobotSensor ReadData(BinaryReader reader)
    {
        RobotSensor sensor = new RobotSensor((RobotSensorType) reader.ReadByte());
        sensor.module = reader.ReadInt16();
        sensor.port = reader.ReadInt16();
        sensor.polyCoeff = new float[reader.ReadInt32()];
        for (int i = 0; i < sensor.polyCoeff.Length; i++)
        {
            sensor.polyCoeff[i] = reader.ReadSingle();
        }
        sensor.useSecondarySource = reader.ReadBoolean();
        return sensor;
    }
    
    /// <summary>
    /// Compares two sensors, returns true if all fields are identical.
    /// </summary>
    /// <param name="otherSensor"></param>
    public bool Equals(RobotSensor otherSensor)
    {
        if (this.module != otherSensor.module || this.port != otherSensor.port || this.useSecondarySource != otherSensor.useSecondarySource)
            return false;

        for (int i = 0; i < this.polyCoeff.Length && i < otherSensor.polyCoeff.Length; i++)
        {
            if (this.polyCoeff[i] != otherSensor.polyCoeff[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Turns the sensors coefficients into a nice equation.
    /// </summary>
    /// <returns></returns>
    public string PolynomialToString()
    {
        string polynomial = "y=";

        for (int i = this.polyCoeff.Length - 1; i > 1; i--)
        {
            if (this.polyCoeff[i] != 0)
            {
                polynomial = polynomial + this.polyCoeff[i] + "x^" + i + "+";
            }
        }

        if (this.polyCoeff.Length > 2)
        {
            if (this.polyCoeff[1] != 0)
            {
                polynomial = polynomial + this.polyCoeff[1] + "x+";
            }
        }

        if (this.polyCoeff.Length > 1)
        {
            if (this.polyCoeff[0] != 0)
            {
                polynomial = polynomial + this.polyCoeff[0];
            }
        }

        if (polynomial[polynomial.Length - 1] == '+')
        {
            polynomial = polynomial.Remove(polynomial.Length - 1);
        }

        return polynomial;
    }
}
