using System;
using System.Runtime.InteropServices;

namespace GameEngine.UI.AvaloniaUI.Controllers
{
    internal static class XInputWindows
    {
        static IntPtr xInputStateDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XInputStateData)));

        public const double AnalogDifferenceThreshold = 0.02;
        public static double LeftStickDeadZone = 0.1;
        public static double RightStickDeadZone = 0.1;
        public static double LeftTriggerDeadZone = 0.1;
        public static double RightTriggerDeadZone = 0.1;

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        private static extern int XInputGetState4(int dwUserIndex, IntPtr xInputState);
        [DllImport("xinput9_1_0.dll", EntryPoint = "XInputGetState")]
        private static extern int XInputGetState9(int dwUserIndex, IntPtr xInputState);

        private static int XInputGetState(int controllerIndex, IntPtr xInputState)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (Environment.OSVersion.Version.Major == 10)
                {
                    return XInputGetState4(controllerIndex, xInputState);
                }
                else if (Environment.OSVersion.Version.Major == 8 || Environment.OSVersion.Version.Major == 7)
                {
                    return XInputGetState9(controllerIndex, xInputState);
                }
                else
                {
                    throw new Exception($"Major version not suppported: {Environment.OSVersion.Version.Major}");
                }
            }
            else
            {
                throw new Exception($"Platform not suppported: {Environment.OSVersion.Platform}");
            }
        }

        public static XInputState GetState(int controllerIndex)
        {
            int err = XInputGetState(controllerIndex, xInputStateDataPtr);

            if (err < 0)
            {
                Console.WriteLine($"{nameof(XInputGetState)} failed with error {err}");
                throw new Exception($"{nameof(XInputGetState)} failed with error {err}");
            }

            XInputStateData data = Marshal.PtrToStructure<XInputStateData>(xInputStateDataPtr);

            return new XInputState
            {
                DPad = new XInputDpad
                {
                    Up = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.DPAD_UP) != 0, Value = 0 },
                    Down = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.DPAD_DOWN) != 0, Value = 0 },
                    Left = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.DPAD_LEFT) != 0, Value = 0 },
                    Right = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.DPAD_RIGHT) != 0, Value = 0 },
                },
                Buttons = new XInputButtons
                {
                    A = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.A) != 0, Value = 0 },
                    B = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.B) != 0, Value = 0 },
                    X = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.X) != 0, Value = 0 },
                    Y = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.Y) != 0, Value = 0 },
                    Start = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.START) != 0, Value = 0 },
                    Select = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.BACK) != 0, Value = 0 },
                    BumperL = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.LEFT_SHOULDER) != 0, Value = 0 },
                    BumperR = new XInputValue { Pressed = (data.gamePad.wButtons & (ushort)ButtonBitMask.RIGHT_SHOULDER) != 0, Value = 0 },
                },
                Analog = new XInputAnalog
                {
                    LeftStickX = new XInputValue { Pressed = Math.Abs(data.gamePad.sThumbLX * 1.0 / short.MaxValue) > LeftStickDeadZone, Value = data.gamePad.sThumbLX * 1.0 / short.MaxValue },
                    LeftStickY = new XInputValue { Pressed = Math.Abs(data.gamePad.sThumbLY * 1.0 / short.MaxValue) > LeftStickDeadZone, Value = data.gamePad.sThumbLY * 1.0 / short.MaxValue },
                    RightStickX = new XInputValue { Pressed = Math.Abs(data.gamePad.sThumbRX * 1.0 / short.MaxValue) > RightStickDeadZone, Value = data.gamePad.sThumbRX * 1.0 / short.MaxValue },
                    RightStickY = new XInputValue { Pressed = Math.Abs(data.gamePad.sThumbRY * 1.0 / short.MaxValue) > RightStickDeadZone, Value = data.gamePad.sThumbRY * 1.0 / short.MaxValue },
                    LeftTrigger = new XInputValue { Pressed = Math.Abs(data.gamePad.bLeftTrigger * 1.0 / byte.MaxValue) > LeftTriggerDeadZone, Value = data.gamePad.bLeftTrigger * 1.0 / byte.MaxValue },
                    RightTrigger = new XInputValue { Pressed = Math.Abs(data.gamePad.bRightTrigger * 1.0 / byte.MaxValue) > RightTriggerDeadZone, Value = data.gamePad.bRightTrigger * 1.0 / byte.MaxValue },
                },
            };
        }

        internal class XInputState
        {
            public XInputState() { }

            public XInputState(XInputState state)
            {
                this.DPad = new XInputDpad
                {
                    Up = new XInputValue { Pressed = state.DPad.Up.Pressed, Value = state.DPad.Up.Value },
                    Down = new XInputValue { Pressed = state.DPad.Down.Pressed, Value = state.DPad.Down.Value },
                    Left = new XInputValue { Pressed = state.DPad.Left.Pressed, Value = state.DPad.Left.Value },
                    Right = new XInputValue { Pressed = state.DPad.Right.Pressed, Value = state.DPad.Right.Value },
                };
                this.Buttons = new XInputButtons
                {
                    A = new XInputValue { Pressed = state.Buttons.A.Pressed, Value = state.Buttons.A.Value },
                    B = new XInputValue { Pressed = state.Buttons.B.Pressed, Value = state.Buttons.B.Value },
                    X = new XInputValue { Pressed = state.Buttons.X.Pressed, Value = state.Buttons.X.Value },
                    Y = new XInputValue { Pressed = state.Buttons.Y.Pressed, Value = state.Buttons.Y.Value },
                    Start = new XInputValue { Pressed = state.Buttons.Start.Pressed, Value = state.Buttons.Start.Value },
                    Select = new XInputValue { Pressed = state.Buttons.Select.Pressed, Value = state.Buttons.Select.Value },
                    BumperL = new XInputValue { Pressed = state.Buttons.BumperL.Pressed, Value = state.Buttons.BumperL.Value },
                    BumperR = new XInputValue { Pressed = state.Buttons.BumperR.Pressed, Value = state.Buttons.BumperR.Value },
                };
                this.Analog = new XInputAnalog
                {
                    LeftStickX = new XInputValue { Pressed = state.Analog.LeftStickX.Pressed, Value = state.Analog.LeftStickX.Value },
                    LeftStickY = new XInputValue { Pressed = state.Analog.LeftStickY.Pressed, Value = state.Analog.LeftStickY.Value },
                    RightStickX = new XInputValue { Pressed = state.Analog.RightStickX.Pressed, Value = state.Analog.RightStickX.Value },
                    RightStickY = new XInputValue { Pressed = state.Analog.RightStickY.Pressed, Value = state.Analog.RightStickY.Value },
                    LeftTrigger = new XInputValue { Pressed = state.Analog.LeftTrigger.Pressed, Value = state.Analog.LeftTrigger.Value },
                    RightTrigger = new XInputValue { Pressed = state.Analog.RightTrigger.Pressed, Value = state.Analog.RightTrigger.Value },
                };
            }

            public XInputDpad DPad { get; set; } = new XInputDpad();
            public XInputButtons Buttons { get; set; } = new XInputButtons();
            public XInputAnalog Analog { get; set; } = new XInputAnalog();

            public static bool operator ==(XInputState left, XInputState right)
            {
                return (left == (object)null && right == (object)null)
                    || (left.DPad == right.DPad && left.Buttons == right.Buttons && left.Analog == right.Analog);
            }

            public static bool operator !=(XInputState left, XInputState right)
            {
                return (left == (object)null && right != (object)null)
                    || (left != (object)null && right == (object)null)
                    || !(left == right);
            }
        }

        internal class XInputDpad
        {
            public XInputValue Up { get; set; } = new XInputValue();
            public XInputValue Down { get; set; } = new XInputValue();
            public XInputValue Left { get; set; } = new XInputValue();
            public XInputValue Right { get; set; } = new XInputValue();

            public static bool operator ==(XInputDpad left, XInputDpad right)
            {
                return (left == (object)null && right == (object)null)
                    || (left.Up == right.Up && left.Down == right.Down && left.Left == right.Left && left.Right == right.Right);
            }

            public static bool operator !=(XInputDpad left, XInputDpad right)
            {
                return (left == (object)null && right != (object)null)
                    || (left != (object)null && right == (object)null)
                    || !(left == right);
            }
        }

        internal class XInputButtons
        {
            public XInputValue A { get; set; } = new XInputValue();
            public XInputValue B { get; set; } = new XInputValue();
            public XInputValue X { get; set; } = new XInputValue();
            public XInputValue Y { get; set; } = new XInputValue();
            public XInputValue Start { get; set; } = new XInputValue();
            public XInputValue Select { get; set; } = new XInputValue();
            public XInputValue BumperL { get; set; } = new XInputValue();
            public XInputValue BumperR { get; set; } = new XInputValue();
            public static bool operator ==(XInputButtons left, XInputButtons right)
            {
                return (left == (object)null && right == (object)null)
                    || (left.A == right.A
                        && left.B == right.B
                        && left.X == right.X
                        && left.Y == right.Y
                        && left.Start == right.Start
                        && left.Select == right.Select
                        && left.BumperL == right.BumperL
                        && left.BumperR == right.BumperR);
            }

            public static bool operator !=(XInputButtons left, XInputButtons right)
            {
                return (left == (object)null && right != (object)null)
                    || (left != (object)null && right == (object)null)
                    || !(left == right);
            }
        }

        internal class XInputAnalog
        {
            public XInputValue LeftStickX { get; set; } = new XInputValue();
            public XInputValue LeftStickY { get; set; } = new XInputValue();
            public XInputValue RightStickX { get; set; } = new XInputValue();
            public XInputValue RightStickY { get; set; } = new XInputValue();
            public XInputValue LeftTrigger { get; set; } = new XInputValue();
            public XInputValue RightTrigger { get; set; } = new XInputValue();
            public static bool operator ==(XInputAnalog left, XInputAnalog right)
            {
                return (left == (object)null && right == (object)null)
                    || (left.LeftStickX == right.LeftStickX
                        && left.LeftStickY == right.LeftStickY
                        && left.RightStickX == right.RightStickX
                        && left.RightStickY == right.RightStickY
                        && left.LeftTrigger == right.LeftTrigger
                        && left.RightTrigger == right.RightTrigger);
            }

            public static bool operator !=(XInputAnalog left, XInputAnalog right)
            {
                return (left == (object)null && right != (object)null)
                    || (left != (object)null && right == (object)null)
                    || !(left == right);
            }
        }

        internal class XInputValue
        {
            public bool Pressed { get; set; } = false;
            public double Value { get; set; } = 0;
            public static bool operator ==(XInputValue left, XInputValue right)
            {
                return (left == (object)null && right == (object)null) 
                    || (left.Pressed == right.Pressed && Math.Abs(left.Value - right.Value) < AnalogDifferenceThreshold);
            }

            public static bool operator !=(XInputValue left, XInputValue right)
            {
                return (left == (object)null && right != (object)null) 
                    || (left != (object)null && right == (object)null)
                    || !(left == right);
            }
        }

        struct XInputStateData
        {
            public uint dwPacketNumber;
            public XInputGamePad gamePad;
        }

        struct XInputGamePad
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        enum ButtonBitMask
        {
            DPAD_UP = 0x001,
            DPAD_DOWN = 0x002,
            DPAD_LEFT = 0x004,
            DPAD_RIGHT = 0x008,
            START = 0x010,
            BACK = 0x020,
            LEFT_THUMB = 0x040,
            RIGHT_THUMB = 0x080,
            LEFT_SHOULDER = 0x100,
            RIGHT_SHOULDER = 0x200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000,
        }
    }
}
