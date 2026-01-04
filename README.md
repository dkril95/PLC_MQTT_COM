# SmartFactory PLC – MQTT Communication

## Overview
This project presents **MQTT communication between a Beckhoff PLC (TwinCAT 3)** and a **C# application** using an MQTT broker.

The PLC simulates an industrial process and publishes process data.
The C# application receives data and sends control commands.
All message formatting is handled on the **PLC side**.

---

## Components

### PLC (TwinCAT 3)
- Simulates an industrial process
- Controls process logic (START / STOP / RESET)
- Publishes process data via MQTT
- Subscribes to control commands from MQTT

### MQTT Broker
- Acts as a communication mediator
- Decouples PLC and C# application
- No logic implemented inside the broker

### C# Application
- Connects to the MQTT broker
- Receives process data from the PLC
- Sends control commands to the PLC
- Displays data in console

---

## Project Structure
PLC_MQTT_COM/
├── .gitignore
├── README.md
├── Beckhoff_PLC_MQTT/          # TwinCAT PLC project
│   ├── PLC_MQTT_COM.tsproj     # PLC project file
│   ├── MAIN/                   
│   ├── POUs/                   
│   ├── GVL/                    
│   └── other TwinCAT config files
│
├── SmartFactory_MQTT_App/      # C# client application
│   ├── Program.cs              # Main MQTT client logic
│   ├── SmartFactory_MQTT_App.csproj  # .NET project file
│   └── bin/obj/ folders (build outputs) 

---

## Communication Flow

1. PLC connects to MQTT broker
2. PLC subscribes to control topic
3. C# application sends commands via MQTT
4. PLC processes commands and updates internal logic
5. PLC publishes process data periodically
6. C# application receives and displays data

---

## MQTT Commands

| Command | Description |
|------|-------------|
| START | Start process and data publishing |
| STOP | Stop process |
| RESET | Reset counter |
| EXIT | Stop communication |

---

## Technologies Used
- Beckhoff TwinCAT 3
- IEC 61131-3 (Structured Text)
- TF6701 IoT MQTT
- Mosquitto MQTT Broker
- C# (.NET)

---

## Purpose
This project was created as a **learning and portfolio project** to demonstrate:
- PLC–IT communication
- MQTT-based industrial data exchange
- Modern Industry 4.0 concepts
