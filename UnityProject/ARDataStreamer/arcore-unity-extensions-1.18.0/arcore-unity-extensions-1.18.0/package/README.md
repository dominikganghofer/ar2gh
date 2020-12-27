Google ARCore Extensions for AR Foundation
==========================================
Copyright 2019 Google LLC. All rights reserved.

This package provides extensions to Unity's AR Foundation cross platform
Augmented Reality API. It also provides native APIs for features provided by
Google ARCore. With these extensions you can build new AR experiences or
enhance existing apps with AR features.

This package does not yet provide access to all ARCore APIs and capabilities.
If one or more of those features are required by your project, consider using
the [ARCore SDK for Unity](//developers.google.com/ar/develop/downloads)
until those features have been added to this package.


## Upcoming breaking change affecting previously published 32-bit-only apps

In **August 2020**, _Google Play Services for AR_ (ARCore) will remove support
for 32-bit-only ARCore-enabled apps running on 64-bit devices. Support for
32-bit apps running on 32-bit devices is unaffected.

If you have published a 32-bit-only (`armeabi-v7a`) version of your
ARCore-enabled app without publishing a corresponding 64-bit (`arm64-v8a`)
version, you must update your app to include 64-bit native libraries before
August 2020. 32-bit-only ARCore-enabled apps that are not updated by this time
may crash when attempting to start an augmented reality (AR) session.

To learn more about this breaking change, and for instructions on how to update
your app, see https://developers.google.com/ar/64bit.


## Quick Start

See the [Getting Started with ARCore Extensions for Unity](//developers.google.com/ar/develop/unity-arf/quickstart)
developer guide.


## API Reference

See the [ARCore Extensions API Reference](//developers.google.com/ar/reference/unity-arf).


## Release Notes

The package release notes are available on the
[releases](//github.com/google-ar/arcore-unity-extensions/releases) page.


## Terms & Conditions

By downloading the ARCore Extensions for AR Foundation, you agree that the
[Google APIs Terms of Service](//developers.google.com/terms/) governs your use
thereof.


## User privacy requirements

You must disclose the use of Google Play Services for AR (ARCore) and how it
collects and processes data, prominently in your application, easily accessible
to users. You can do this by adding the following text on your main menu or
notice screen: "This application runs on
[Google Play Services for AR](//play.google.com/store/apps/details?id=com.google.ar.core)
(ARCore), which is provided by Google LLC and governed by the
[Google Privacy Policy](//policies.google.com/privacy)".
