/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Provides values for the <see cref="DrumPatternQuarter.QuarterType"/> property.
    /// </summary>
    public enum DrumPatternQuarterType
    {
        /// <summary>
        /// No type has been assigned.
        /// </summary>
        None,
        /// <summary>
        /// The quarter is used in the drum pattern header.
        /// </summary>
        Header,
        /// <summary>
        /// The quarter is used in the drum pattern selector body.
        /// </summary>
        PatternSelector,
        /// <summary>
        /// The quarter is used in the drum pattern velocity selector section.
        /// </summary>
        VelocitySelector,
    }
}