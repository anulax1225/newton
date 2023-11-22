using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newton;

/// <summary>
/// Simple callback closure
/// </summary>
public delegate void Callback();
/// <summary>
/// Closure that compute the data once it has been validated
/// The generic enables multiple types to be passed
/// </summary>
/// <typeparam name="T">Generic type param</typeparam>
/// <param name="input">User input</param>
public delegate void Validation<T>(T input);
/// <summary>
/// Closure to validate the input type, conversion etc.
/// The generic enables multiple types to be passed
/// </summary>
/// <typeparam name="T">Generic type param</typeparam>
/// <param name="input">User input</param>
/// <returns>Valide input</returns>
public delegate bool Verifier<T>(T input);

