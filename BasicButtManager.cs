using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;

namespace BasicButtManager
{
    public class BasicButtManager
    {
        private ButtplugClient client;
        private float previousRawLinear = 0f, previousLinear = 0f, oscillateSpeed = 0f, currentIntensity = 0f;
        private readonly CancellationTokenSource oscillateTokenSource = new CancellationTokenSource(), strokeTokenSource = new CancellationTokenSource();
        private readonly Queue<float> requestedLinearPositions = new Queue<float>();
        private CancellationToken oscillateToken, strokeToken;
        private bool isFading = false;
        private enum StrokeMode { AUTO, MANUAL };
        private StrokeMode strokeMode = StrokeMode.MANUAL;

        /// <summary>
        /// Establish server/device connections.
        /// </summary>
        /// 
        /// <param name="name">Name to identify the purpose of the manager, typically the game's name.</param>
        private async Task Init(String name)
        {
            Console.WriteLine("Initializing client...");
            client = new ButtplugClient(name);
            oscillateToken = oscillateTokenSource.Token;
            strokeToken = strokeTokenSource.Token;

            Console.WriteLine("Adding device listeners...");
            void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
                => Console.WriteLine($"Device connected: {aArgs.Device.Name}");

            client.DeviceAdded += HandleDeviceAdded;

            void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
                => Console.WriteLine($"Device disconnected: {aArgs.Device.Name}");

            client.DeviceRemoved += HandleDeviceRemoved;

            Console.WriteLine("Connecting to server...");
            await client.ConnectAsync(new ButtplugWebsocketConnector(uri: new Uri("ws://localhost:12345"))); //assume default

            Console.WriteLine("Scanning...");
            await client.StartScanningAsync(); //intentional, allows devices to be connected whenever
        }

        /// <summary>
        /// Create a new manager for the given game.
        /// </summary>
        /// 
        /// <param name="name">Name to identify the purpose of the manager, typically the game's name.</param>
        public BasicButtManager(String name = "BasicButtManager")
        {
            Console.WriteLine("ButtManager Constructor");
            Init(name).Wait();
            Console.WriteLine("Starting oscillation...");
            _ = Oscillate();
            Console.WriteLine("Starting strokes...");
            _ = Stroke();
        }

        /// <summary>
        /// Directly control your toys with the provided parameters.
        /// </summary>
        /// 
        /// <param name="intensity">Intensity for vibrators, oscillators, rotators, pressurizers, etc.</param>
        /// <param name="position">Position of linear actuators, if omitted will take the value of intensity.</param>
        /// <param name="oscillate">If true, let the linear actuators move automatically at a set speed (based on intensity). Only applicable if position not given.</param>
        /// 
        /// <remarks>It is recommended in most cases to use one of the provided convenient functions instead, most notably Set().</remarks>
        public async Task Control(float intensity, float position = -1f, bool oscillate = false)
        {
            if (client.Devices.Length < 1) return;

            intensity = Math.Max(0f, Math.Min(1f, intensity));
            position = Math.Min(1f, position);

            if (!isFading) currentIntensity = intensity;
            float location = position < 0f ? intensity : position;

            if (position < 0 && oscillate)
            {
                strokeMode = StrokeMode.AUTO;
                oscillateSpeed = intensity;
            }
            else
            {
                strokeMode = StrokeMode.MANUAL;
                if (Math.Abs(location - previousRawLinear) < 0.12f) return;
                requestedLinearPositions.Enqueue(location);
                previousRawLinear = location;
            }

            await Raw(intensity);
        }

        /// <summary>
        /// Vibrate, oscillate, stroke, etc. all connected devices according to the one given intensity value.
        /// </summary>
        /// 
        /// <param name="intensity">The intensity to be applied to all connected toys.</param>
        public async Task Set(float intensity)
        {
            await Control(intensity, -1f, true);
        }

        /// <summary>
        /// Set all toys to the same constant speed AND location.
        /// </summary>
        /// 
        /// <param name="actionValue">The intensity or location to be set to all connected toys.</param>
        public async Task Press(float actionValue)
        {
            await Control(actionValue, -1f);
        }

        /// <summary>
        /// Fade the intensity of all toys to a certain point.
        /// </summary>
        /// 
        /// <param name="targetIntensity">The target intensity, can be higher or lower than the current intensity.</param>
        /// <param name="smoothness">Between 0f and 1f, the higher the smoother the transition.</param>
        /// <remarks>Does not support multiple fades at one time.</remarks>
        public async Task Fade(float targetIntensity, float smoothness = 1f)
        {
            if (isFading) return;

            isFading = true;
            smoothness = Math.Max(0.1f, Math.Min(1f, smoothness));
            float delta = 500f / smoothness;

            if (targetIntensity > currentIntensity)
            {
                while (currentIntensity <= targetIntensity)
                {
                    currentIntensity += 0.1f;
                    await Press(currentIntensity);
                    await Task.Delay((int)delta);
                }
            }
            else if (targetIntensity < currentIntensity)
            {
                while (currentIntensity >= targetIntensity)
                {
                    currentIntensity -= 0.1f;
                    await Press(currentIntensity);
                    await Task.Delay((int)delta);
                }
            }
            await Press(targetIntensity);
            isFading = false;
        }

        /// <summary>
        /// Fade all toys in from zero intensity to max power.
        /// </summary>
        /// 
        /// <remarks>Does not support multiple fades at one time.</remarks>
        public async Task FadeIn()
        {
            await Fade(1);
        }

        /// <summary>
        /// Fade all toys out from max intensity to zero power.
        /// </summary>
        /// 
        /// <remarks>Does not support multiple fades at one time.</remarks>
        public async Task FadeOut()
        {
            await Fade(0);
        }

        /// <summary>
        /// Internal worker responsible for moving strokers back and forth.
        /// </summary>
        private async Task Oscillate()
        {
            int state = 0;

            while (true)
            {
                await Task.Delay(12);

                if (strokeMode != StrokeMode.AUTO) continue;
                if (oscillateSpeed <= 0f)
                {
                    if (previousLinear == 0f) continue;

                    foreach (var dev in client.Devices)
                    {
                        if (dev.LinearAttributes.Count > 0)
                        {
                            await dev.LinearAsync(250, 0f);
                            previousLinear = 0f;
                            Task.Delay(250).Wait();
                        }
                    }
                    continue;
                }

                float interval = 1000 * (1.5f - 0.5f * oscillateSpeed);
                float position = 0.35f + (0.65f * state);
                state = (state + 1) % 2;

                if (oscillateToken.IsCancellationRequested) break;
                await Task.Delay((int)(interval * 1.1), oscillateToken).ContinueWith(t =>
                {
                    previousLinear = position;

                    foreach (var dev in client.Devices)
                    {
                        if (dev.LinearAttributes.Count > 0)
                        {
                            dev.LinearAsync((uint)interval, position);
                        }
                    }
                }, oscillateToken);
            }

            foreach (var dev in client.Devices)
            {
                if (dev.LinearAttributes.Count > 0)
                {
                    await dev.LinearAsync(500, 0f);
                    previousLinear = 0f;
                    Task.Delay(1000).Wait();
                }
            }
        }

        /// <summary>
        /// Internal worker responsible for moving the stroker to the manually requested positions.
        /// </summary>
        private async Task Stroke()
        {
            while (true)
            {
                if (strokeMode != StrokeMode.MANUAL) continue;
                if (requestedLinearPositions.Count == 0)
                {
                    await Task.Delay(12);
                    continue;
                }

                float target = requestedLinearPositions.Dequeue();
                int duration = (int)(Math.Abs(target - previousLinear) * 1000);
                previousLinear = target;

                if (strokeToken.IsCancellationRequested) break;
                foreach (var dev in client.Devices)
                {
                    if (dev.LinearAttributes.Count > 0)
                    {
                        await dev.LinearAsync((uint)duration, target);
                    }
                }
                await Task.Delay(duration);
            }
        }

        /// <summary>
        /// Press for a single buzz/squeeze/stroke/spin.
        /// </summary>
        /// 
        /// <param name="reboundSpeed">How long the action is held before being released, from 0f to 1f.</param>
        /// <remarks>Client must make sure to not pulse too often too quickly (~ once per second is ideal).</remarks>
        public async Task Pulse(float reboundSpeed)
        {
            if (reboundSpeed < 0f || reboundSpeed > 1f)
            {
                Console.WriteLine($"Invalid rebound speed: {reboundSpeed}.");
                return;
            }

            await Raw(1.0f);
            await RawLinear(250, 1.0f);
            await Task.Delay((int)(1000f * (1.0f - reboundSpeed)));
            await RawLinear(250, 0f);
            await Raw(0f);
        }

        /// <summary>
        /// Push all strokers down, buzz vibrators for a bit, then push strokers back up.<br/>
        /// </summary>
        /// 
        /// <param name="reboundSpeed">How long the action is held before being released, from 0f to 1f.</param>
        /// <remarks>Basically the slow version of Pulse().</remarks>
        public async Task Hold(float reboundSpeed)
        {
            if (reboundSpeed < 0f || reboundSpeed > 1f)
            {
                Console.WriteLine($"Invalid rebound speed: {reboundSpeed}.");
                return;
            }
            await FadeIn();
            await Task.Delay((int)(1000f * (1.0f - reboundSpeed)));
            await FadeOut();
        }

        /// <summary>
        /// Private helper to set all vibrators, oscillators, and rotators to the same intensity.
        /// </summary>
        /// 
        /// <param name="intensity">The shared intensity.</param>
        private async Task Raw(float intensity)
        {

            foreach (var dev in client.Devices)
            {
                if (dev.VibrateAttributes.Count > 0)
                {
                    await dev.VibrateAsync(intensity);
                }
                if (dev.OscillateAttributes.Count > 0)
                {
                    await dev.OscillateAsync(intensity);
                }
                if (dev.RotateAttributes.Count > 0)
                {
                    await dev.RotateAsync(intensity, true);
                }
            }
        }

        /// <summary>
        /// Private helper to set all linear actuators perform the same action.
        /// </summary>
        /// 
        /// <param name="duration">The transition duration.</param>
        /// <param name="position">The target offset.</param>
        private async Task RawLinear(uint duration, float position)
        {
            foreach (var dev in client.Devices)
            {
                if (dev.LinearAttributes.Count > 0)
                {
                    await dev.LinearAsync(duration, position);
                }
            }
        }

        /// <summary>
        /// Stop all connected toys.
        /// </summary>
        public async Task Stop()
        {
            await Control(0f);
            oscillateSpeed = 0f;
        }

        /// <summary>
        /// Stop all connected toys and close server connection.
        /// </summary>
        public void Exit()
        {
            Stop().Wait();
            oscillateTokenSource.Cancel();
            strokeTokenSource.Cancel();
            client.StopScanningAsync().Wait();
            client.StopAllDevicesAsync().Wait();
            client.DisconnectAsync().Wait();
        }
    }
}
