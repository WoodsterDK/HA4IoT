﻿using System;
using System.Collections.Generic;
using Windows.Data.Json;
using HA4IoT.Contracts;
using HA4IoT.Contracts.Notifications;
using HA4IoT.Networking;
using HA4IoT.Notifications;

namespace HA4IoT.Actuators
{
    public class VirtualButtonGroup : ActuatorBase
    {
        private readonly Dictionary<string, VirtualButton> _buttons = new Dictionary<string, VirtualButton>(StringComparer.OrdinalIgnoreCase);

        public VirtualButtonGroup(string id, IHttpRequestController api, INotificationHandler log)
            : base(id, api, log)
        {
        }

        public VirtualButtonGroup WithButton(string id, Action<VirtualButton> initializer)
        {
            if (initializer == null) throw new ArgumentNullException(nameof(initializer));

            if (_buttons.ContainsKey(id))
            {
                throw new InvalidOperationException("Button with id " + id + " already part of the button group.");
            }

            var virtualButton = new VirtualButton(id, Api, Log);
            initializer(virtualButton);

            _buttons.Add(id, virtualButton);

            return this;
        }

        public override void HandleApiPost(ApiRequestContext context)
        {
            var button = context.Request.GetNamedString("button", string.Empty);

            if (string.IsNullOrEmpty(button))
            {
                throw new BadRequestException("Button is not set.");
            }

            VirtualButton virtualButton;
            if (!_buttons.TryGetValue(button, out virtualButton))
            {
                throw new BadRequestException("The specified button is unknown.");
            }

            virtualButton.HandleApiPost(context);
        }

        public override JsonObject GetConfiguration()
        {
            JsonObject configuration = base.GetConfiguration();

            JsonArray buttonIds = new JsonArray();
            foreach (var button in _buttons)
            {
                buttonIds.Add(JsonValue.CreateStringValue(button.Key));
            }
            
            configuration.SetNamedValue("buttons", buttonIds);

            return configuration;
        }
    }
}
