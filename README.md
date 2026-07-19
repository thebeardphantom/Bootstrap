# Bootstrap

[![openupm](https://img.shields.io/npm/v/com.beardphantom.bootstrap?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.beardphantom.bootstrap/)

An async-first bootstrapping framework for Unity projects. Bootstrap drives your app through a defined startup sequence, resolves and initializes services in priority order, and gives you a single place to hook in environment-specific setup for the editor, builds, and play mode.

## Features

- **Structured bootstrap sequence** — a defined set of phases (handler discovery, service discovery/binding/init, async task flush, ready) that every app instance passes through, so startup order is predictable and inspectable.
- **Service locator** — declare services via `ServiceList` assets, bind them to lookup types, and resolve them through `App` at runtime.
- **Environments** — map scenes and build profiles to `BootstrapEnvironmentAsset`s to control environment-specific configuration without branching code paths.
- **Pre/post bootstrap handlers** — plug custom logic before and after the core bootstrap sequence runs.
- **ZLogger integration** — optional structured logging with console and rotating file providers, gated behind the `BOOTSTRAP_ZLOGGER` scripting define.
- **Source generators** — attributes like `[GenerateLogger]` and `[GenerateSingleton]` remove boilerplate from service classes.

## Installation

Install via the Unity Package Manager using a git URL:

```
https://github.com/thebeardphantom/Bootstrap.git?path=Packages/com.beardphantom.bootstrap
```

Or through [OpenUPM](https://openupm.com/packages/com.beardphantom.bootstrap/):

```
openupm add com.beardphantom.bootstrap
```

## Documentation

See the [wiki](https://github.com/thebeardphantom/Bootstrap/wiki) for setup guides and API details.

## License

[MIT](LICENSE.md)
