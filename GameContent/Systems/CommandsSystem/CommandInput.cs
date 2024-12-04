﻿using System;

namespace TanksRebirth.GameContent.Systems.CommandsSystem;

/// <summary>A structure which represents the input of a command.</summary>
public readonly struct CommandInput : IEquatable<CommandInput> {
    /// <summary>The name of this <see cref="CommandInput"/>.</summary>
    public readonly string Name;
    /// <summary>The description of this <see cref="CommandInput"/>.</summary>
    public readonly string Description;

    /// <summary>Define a command.</summary>
    /// <param name="name">The name of this <see cref="CommandInput"/>.</param>
    /// <param name="description">The description of this <see cref="CommandInput"/>.</param>
    public CommandInput(string name, string description) {
        Name = name;
        Description = description;
    }

    /// <summary>Implicit casting to a tuple containing 2 <see cref="string"/>s.</summary>
    public static implicit operator CommandInput(ValueTuple<string, string> input) => new(input.Item1, input.Item2);

    public bool Equals(CommandInput other) {
        return Name == other.Name && Description == other.Description;
    }

    public override bool Equals(object? obj) {
        return obj is CommandInput other && Equals(other);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Name, Description);
    }

    public static bool operator ==(CommandInput left, CommandInput right) {
        return left.Equals(right);
    }

    public static bool operator !=(CommandInput left, CommandInput right) {
        return !left.Equals(right);
    }
}