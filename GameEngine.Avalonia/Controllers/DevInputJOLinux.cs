using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GameEngine.UI.AvaloniaUI.Controllers.XInputWindows;

namespace GameEngine.UI.AvaloniaUI.Controllers
{
    internal class DevInputJSLinux
    {
        private static bool evdev = true;
        static DevInputJSLinux()
        {
            Task.Run(() =>
            {
                IEnumerable<string> evdevJoySticks = Directory.EnumerateFiles("/dev/input/by-path", "*-event-joystick");
                IEnumerable<string> joysticks = Directory.EnumerateFiles("/dev/input", "js*");

                Stopwatch sw = Stopwatch.StartNew();
                while (sw.Elapsed.TotalSeconds < 5 && !evdevJoySticks.Any() && !joysticks.Any())
                {
                    evdevJoySticks = Directory.EnumerateFiles("/dev/input/by-path", "*-event-joystick");
                    joysticks = Directory.EnumerateFiles("/dev/input", "js*");
                }

                Console.WriteLine($"Controllers:{string.Join("\n\t", evdevJoySticks)}");
                FileStream stream;
                if (evdevJoySticks.Any())
                {
                    evdev = true;
                    stream = File.OpenRead(evdevJoySticks.First());
                    Console.WriteLine($"Using '{evdevJoySticks.First()}'");
                }
                else if (joysticks.Any())
                {
                    evdev = false;
                    stream = File.OpenRead(joysticks.First());
                    Console.WriteLine($"Using '{joysticks.First()}'");
                }
                else
                {
                    throw new Exception("No joysticks found");
                }

                Console.WriteLine($"stream: {stream}");
                new Thread(o => Polling((FileStream)o)).Start(stream);
            });
        }

        private static XInputState internalState = new XInputState();
        private static void Polling(FileStream stream)
        {
            if (evdev)
            {
                PollingEvdev(stream);
            }
            else
            {
                PollingJSX(stream);
            }
        }

        private static void PollingEvdev(FileStream stream)
        {
            byte[] buffer = new byte[sizeof(ulong) + sizeof(ulong) + sizeof(ushort) + sizeof(ushort) + sizeof(uint)];
            int bytes;
            while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytes < 0)
                {
                    break;
                }

                int index = 0;

                ulong sec = BitConverter.ToUInt64(buffer, index); index += sizeof(ulong);
                ulong usec = BitConverter.ToUInt64(buffer, index); index += sizeof(ulong);
                ushort type = BitConverter.ToUInt16(buffer, index); index += sizeof(ushort);
                ushort code = BitConverter.ToUInt16(buffer, index); index += sizeof(ushort);
                int value = BitConverter.ToInt32(buffer, index); index += sizeof(int);

                if (type == (int)evdevEventCodes.EV_SYN)
                {
                    continue;
                }

                jsEvent evt = new jsEvent
                {
                    timestamp = new time
                    {
                        sec = sec,
                        usec = usec,
                    },
                    type = type,
                    code = code,
                    value = value,
                };

                //Console.WriteLine($"{{ti{evt.timestamp.sec}.{evt.timestamp.usec}, ty{(EventCodes)evt.type}, c{evt.code:x}, v{evt.value}}}");

                XInputValue xiv = null;
                switch (evt.code)
                {
                    default:
                        break;
                    case (int)evdevButtonCodes.BTN_A:
                        xiv = internalState.Buttons.A;
                        break;
                    case (int)evdevButtonCodes.BTN_B:
                        xiv = internalState.Buttons.B;
                        break;
                    case (int)evdevButtonCodes.BTN_X:
                        xiv = internalState.Buttons.X;
                        break;
                    case (int)evdevButtonCodes.BTN_Y:
                        xiv = internalState.Buttons.Y;
                        break;
                    case (int)evdevButtonCodes.BTN_TL:
                        xiv = internalState.Buttons.BumperL;
                        break;
                    case (int)evdevButtonCodes.BTN_TR:
                        xiv = internalState.Buttons.BumperR;
                        break;
                    //case (int)evdevButtonCodes.BTN_TL2:
                    //    xiv = internalState.Analog.LeftTrigger;
                    //    xiv.Value = evt.value * 1.0 / short.MaxValue;
                    //    break;
                    //case (int)evdevButtonCodes.BTN_TR2:
                    //    xiv = internalState.Analog.RightTrigger;
                    //    xiv.Value = evt.value * 1.0 / short.MaxValue;
                    //    break;
                    case (int)evdevButtonCodes.ABS_X:
                        internalState.Analog.LeftStickX.Value = evt.value * 1.0 / short.MaxValue;
                        internalState.Analog.LeftStickX.Pressed = Math.Abs(internalState.Analog.LeftStickX.Value) > LeftStickDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_Y:
                        internalState.Analog.LeftStickY.Value = evt.value * 1.0 / short.MaxValue;
                        internalState.Analog.LeftStickY.Pressed = Math.Abs(internalState.Analog.LeftStickY.Value) > LeftStickDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_Z:
                        internalState.Analog.LeftTrigger.Value = evt.value * 1.0 / byte.MaxValue;
                        internalState.Analog.LeftTrigger.Pressed = Math.Abs(internalState.Analog.LeftTrigger.Value) > LeftTriggerDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_RX:
                        internalState.Analog.RightStickX.Value = evt.value * 1.0 / short.MaxValue;
                        internalState.Analog.RightStickX.Pressed = Math.Abs(internalState.Analog.RightStickX.Value) > RightStickDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_RY:
                        internalState.Analog.RightStickY.Value = evt.value * 1.0 / short.MaxValue;
                        internalState.Analog.RightStickY.Pressed = Math.Abs(internalState.Analog.RightStickY.Value) > RightStickDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_RZ:
                        internalState.Analog.RightTrigger.Value = evt.value * 1.0 / byte.MaxValue;
                        internalState.Analog.RightTrigger.Pressed = Math.Abs(internalState.Analog.RightTrigger.Value) > RightTriggerDeadZone;
                        break;
                    case (int)evdevButtonCodes.ABS_HAT0X:
                        internalState.DPad.Left.Pressed = evt.value < 0;
                        internalState.DPad.Right.Pressed = evt.value > 0;
                        break;
                    case (int)evdevButtonCodes.ABS_HAT0Y:
                        internalState.DPad.Up.Pressed = evt.value < 0;
                        internalState.DPad.Down.Pressed = evt.value > 0;
                        break;
                }

                if (xiv != null)
                {
                    xiv.Pressed = evt.value != 0;
                    xiv.Value = evt.value;
                }
            }

            Console.WriteLine($"End reading from joystick input file with {bytes} bytes");
        }

        private static void PollingJSX(FileStream stream)
        {
            byte[] buffer = new byte[sizeof(uint) + sizeof(ushort) + sizeof(byte) + sizeof(byte)];
            int bytes;
            while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytes < 0)
                {
                    break;
                }

                int index = 0;

                uint time = BitConverter.ToUInt32(buffer, index); index += sizeof(uint);
                short value = BitConverter.ToInt16(buffer, index); index += sizeof(short);
                byte type = buffer[index]; index += sizeof(byte); // axis or button
                byte number = buffer[index]; index += sizeof(int); // number code

                Console.WriteLine($"{{ti{time}, tv{value}, ty{type}, n{number}}}");

                XInputValue xiv = null;
                switch (type)
                {
                    default:
                        break;
                    case (byte)jsxTypeCodes.JS_EVENT_BUTTON:
                        if (number == (byte)jsxButtonCodes.A)
                            xiv = internalState.Buttons.A;
                        if (number == (byte)jsxButtonCodes.B)
                            xiv = internalState.Buttons.B;
                        if (number == (byte)jsxButtonCodes.X)
                            xiv = internalState.Buttons.X;
                        if (number == (byte)jsxButtonCodes.Y)
                            xiv = internalState.Buttons.Y;
                        if (number == (byte)jsxButtonCodes.BL)
                            xiv = internalState.Buttons.BumperL;
                        if (number == (byte)jsxButtonCodes.BR)
                            xiv = internalState.Buttons.BumperR;
                        if (number == (byte)jsxButtonCodes.SELECT)
                            xiv = internalState.Buttons.Select;
                        if (number == (byte)jsxButtonCodes.START)
                            xiv = internalState.Buttons.Start;
                        break;
                    case (byte)jsxTypeCodes.JS_EVENT_AXIS:
                        if (number == (int)jsxButtonCodes.DLEFT)
                        {
                            internalState.DPad.Left.Pressed = value < 0;
                        }
                        if (number == (int)jsxButtonCodes.DRIGHT)
                        {
                            internalState.DPad.Right.Pressed = value > 0;
                        }
                        if (number == (int)jsxButtonCodes.DUP)
                        {
                            internalState.DPad.Up.Pressed = value < 0;
                        }
                        if (number == (int)jsxButtonCodes.DDOWN)
                        {
                            internalState.DPad.Down.Pressed = value > 0;
                        }
                        if (number == (int)jsxButtonCodes.X0)
                        {
                            internalState.Analog.LeftStickX.Value = value * 1.0 / short.MaxValue;
                            internalState.Analog.LeftStickX.Pressed = Math.Abs(internalState.Analog.LeftStickX.Value) > LeftStickDeadZone;
                        }
                        if (number == (int)jsxButtonCodes.Y0)
                        {
                            internalState.Analog.LeftStickY.Value = value * 1.0 / short.MaxValue;
                            internalState.Analog.LeftStickY.Pressed = Math.Abs(internalState.Analog.LeftStickY.Value) > LeftStickDeadZone;
                        }
                        // TODO: Figure out why trigger doesn't work
                        if (number == (int)jsxButtonCodes.Z0)
                        {
                            internalState.Analog.LeftTrigger.Value = value * 1.0 / byte.MaxValue;
                            internalState.Analog.LeftTrigger.Pressed = Math.Abs(internalState.Analog.LeftTrigger.Value) > LeftTriggerDeadZone;
                        }
                        if (number == (int)jsxButtonCodes.X1)
                        {
                            internalState.Analog.RightStickX.Value = value * 1.0 / short.MaxValue;
                            internalState.Analog.RightStickX.Pressed = Math.Abs(internalState.Analog.RightStickX.Value) > RightStickDeadZone;
                        }
                        if (number == (int)jsxButtonCodes.Y1)
                        {
                            internalState.Analog.RightStickY.Value = value * 1.0 / short.MaxValue;
                            internalState.Analog.RightStickY.Pressed = Math.Abs(internalState.Analog.RightStickY.Value) > RightStickDeadZone;
                        }
                        // TODO: Figure out why trigger doesn't work
                        if (number == (int)jsxButtonCodes.Z1)
                        {
                            internalState.Analog.RightTrigger.Value = value * 1.0 / byte.MaxValue;
                            internalState.Analog.RightTrigger.Pressed = Math.Abs(internalState.Analog.RightTrigger.Value) > RightTriggerDeadZone;
                        }
                        break;
                }
                if (xiv != null)
                {
                    xiv.Pressed = value != 0;
                    xiv.Value = value;
                }
            }

            Console.WriteLine($"End reading from joystick input file with {bytes} bytes");
        }

        struct jsEvent
        {
            internal time timestamp;
            internal ushort type;
            internal ushort code;
            internal int value;
        }

        struct time
        {
            internal ulong sec;
            internal ulong usec; 
        }

        public static XInputState GetState(int controllerIndex)
        {
            XInputState data = new XInputState(internalState);

            return data;
        }

        enum jsxTypeCodes
        {
            JS_EVENT_BUTTON = 0x01,
            JS_EVENT_AXIS = 0x02,
            JS_EVENT_INIT = 0x80,
        }

        enum jsxButtonCodes
        {
            X0 = 0x00,
            Y0 = 0x01,
            Z0 = 0x02, // left trigger // TODO: Figure out why trigger doesn't work
            X1 = 0x03,
            Y1 = 0x04,
            Z1 = 0x05, // right trigger // TODO: Figure out why trigger doesn't work
            DLEFT = 0x06,
            DRIGHT = 0x06,
            DUP = 0x07,
            DDOWN = 0x07,

            A = 0x00,
            B = 0x01,
            X = 0x02,
            Y = 0x03,
            BL = 0x04,
            BR = 0x05,
            SELECT = 0x06,
            START = 0x07,
            THUMBL_IN = 0x09,
            THUMBR_IN = 0x0A,
        }

        enum evdevEventCodes
        {
            EV_SYN = 0x00,
            EV_KEY = 0x01,
            EV_REL = 0x02,
            EV_ABS = 0x03,
        }

        enum evdevButtonCodes
        {
            ABS_X = 0x00,
            ABS_Y = 0x01,
            ABS_Z = 0x02, // left trigger
            ABS_RX = 0x03,
            ABS_RY = 0x04,
            ABS_RZ = 0x05, // right trigger
            ABS_HAT0X = 0x10, //dpad left-right
            ABS_HAT0Y = 0x11, //dpad up-down
            BTN_GAMEPAD = 0x130,
            BTN_A = 0x130,
            BTN_B = 0x131,
            BTN_X = 0x133,
            BTN_Y = 0x134,
            BTN_TL = 0x136,
            BTN_TR = 0x137,
            BTN_TL2 = 0x138,
            BTN_TR2 = 0x139,
            BTN_SELECT = 0x13A,
            BTN_START = 0x13B,
            BTN_THUMBL_IN = 0x13D,
            BTN_THUMBR_IN = 0x13E,
            BTN_DIGI = 0x140,
        }
    }
}
