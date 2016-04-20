﻿using System;
using HA4IoT.Contracts.Actuators;
using HA4IoT.Contracts.Areas;
using HA4IoT.Contracts.Components;
using HA4IoT.Contracts.Hardware;

namespace HA4IoT.Actuators.Lamps
{
    public static class LampExtensions
    {
        public static IArea WithLamp(this IArea room, Enum id, IBinaryOutput output)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            if (output == null) throw new ArgumentNullException(nameof(output));

            var lamp = new Lamp(ComponentIdFactory.Create(room, id), new PortBasedBinaryStateEndpoint(output));
            room.AddComponent(lamp);

            return room;
        }

        public static ILamp GetLamp(this IArea room, Enum id)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));

            return room.GetComponent<ILamp>(ComponentIdFactory.Create(room, id));
        }
    }
}