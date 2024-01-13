using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum AngleMode
    {
        Off = -90,
        Amper = 120,
        Resistance = -155,
        VoltageAC = 30,
        Voltage = -30,
        Diactivate
    }

    private float timeCount = 0.0f;
    [SerializeField] public AngleMode angleMode = AngleMode.Diactivate;
    public float SpeedSmooth = 0.1f;
    public GameObject Twister;
    public TMP_Text Value;
    public TMP_Text DataSet;
    
    Logic logic = new Logic();

    void Start()
    {
        DataSet.text = $"V 0\nA 0\n~ 0\nΩ 0";
        Value.text = "0";
        logic.Resistance = 1000.0f;
        logic.Power = 400.0f;
    }
    void Update()
    {
        if(angleMode != AngleMode.Diactivate)
        {
            Twister.transform.rotation = Quaternion.Lerp(Twister.transform.rotation, Quaternion.Euler((float)angleMode, 90, 90), timeCount);
            timeCount += (timeCount + Time.deltaTime) * SpeedSmooth;
            if(timeCount > 1.0f)
            {
                timeCount = 0.0f;
                angleMode = AngleMode.Diactivate;
            }
        }
    }

    public void SetAngleMode(string mode)
    {
        angleMode = Enum.Parse<AngleMode>(mode);
        switch(angleMode)
        {
            case AngleMode.Amper:
            {
                DataSet.text = $"V 0\nA {logic.GetAmper()}\n~ 0\nΩ 0";
                Value.text = logic.GetAmper().ToString();
                break;
            }
            case AngleMode.Voltage:
            {
                DataSet.text = $"V {logic.GetVoltage()}\nA 0\n~ 0\nΩ 0";
                Value.text = $"{logic.GetVoltage()}";
                break;
            }
            case AngleMode.Resistance:
            {
                DataSet.text = $"V 0\nA 0\n~ 0\nΩ {logic.Resistance}";
                Value.text = logic.Resistance.ToString();
                break;
            }
            case AngleMode.VoltageAC:
            {
                DataSet.text = $"V 0\nA 0\n~ {logic.GetVoltageAC()}\nΩ 0";
                Value.text = logic.GetVoltageAC().ToString();
                break;
            }
            case AngleMode.Off:
            {
                DataSet.text = $"V 0\nA 0\n~ 0\nΩ 0";
                Value.text = "0";
                break;
            }
        }
    }
}

public class Logic
{
    private float amper;
    private float voltage;
    private float resistance;
    private float power;
    public float Resistance 
    {   
        get => resistance;
        set
        {
            voltage = 0;
            amper = 0;
            resistance = value;
        }
    }
    public float Power
    {   
        get => power;
        set
        {
            voltage = 0;
            amper = 0;
            power = value;
        }
    }

    public float GetVoltage()
    {
        if(voltage == 0)
        {
            voltage = (float)Math.Round(Math.Sqrt(Power*Resistance), 2);
        }
        return voltage;
    }
    public float GetAmper()
    {
        if(amper == 0)
        {
            amper = (float)Math.Round(Math.Sqrt(Power/Resistance), 2);
        }
        return amper;
    }
    public float GetVoltageAC()
    {
        return 0.01f;
    }
}