// The host is the composition root and Frameworks-&-Drivers layer: it implements the Application
// ports (Abstractions) and references the Application services + Infrastructure it wires together.
// These layers are used pervasively across the host's services/converters, so they are global usings.
global using PKHeX.Application.Abstractions;
global using PKHeX.Application.Services;
global using PKHeX.Infrastructure;
