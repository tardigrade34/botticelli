﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botticelli.Framework.Vk.API.Requests
{
    public class PollRequest
    {
        /// <summary>
        /// Session key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Number of the last event, from which we need to get
        /// subsequent events
        /// </summary>
        public long Ts { get; set; }

        /// <summary>
        ///  Time of waiting in seconds
        /// </summary>
        public int Wait { get; set; } = 100;

        /// <summary>
        /// Additional mode settings
        /// </summary>
        public int? Mode { get; set; }

        /// <summary>
        /// Long poll API ver
        /// </summary>
        public int Version => 3;
    }
}