# XamariNES.UI

What's an emulator without a UI to play it? Enter the Microsoft Xamarin Framework!

I actually first started using Xamarin several years ago and fell in love immediately. As a career .Net Developer, it was so refreshing to have a mobile framework that allowed me to use a language I'm very familiar with.

Things only got better when Xamarin.Forms was released, which allowed me to basically write-once-deploy-many for mobile and desktop applications. Obviously the name of this project is shared with Xamarin because the cross platform nature of XamariNES would not have been possible without Xamarin and .Net Standard.

# Supported Platforms

| Platform | Support | Details |
| ------ | ----------- |----------- |
| iOS | YES | Release mode using LLVM compiler runs at full speed |
| Android | YES | Release mode runs at full speed on MOST devices |
| macOS | YES-ish | Mouse support for touch events, need to add keyboard bindings |
| UWP | YES-ish | Mouse support for touch events, need to add keyboard bindings |
| WPF | NO | Waiting for .Net Core 3.0 Final |
| WASM | NO| ALTHOUGH, Completely possible using [Ooui](https://github.com/praeclarum/Ooui/wiki/Xamarin.Forms-on-the-Web) |

# Loading ROMS

Ok -- So obviously I couldn't post this to GitHub with commercial games.

Currently XamariNES supports one ROM at a time, loading it from an Embedded Resource. The path for the resource is a string constant in the **EmulatorHomeViewModel.cs** file.

Ideally in the future folks will be able to load their own ROMs using [Xamarin Essentials](https://docs.microsoft.com/en-us/xamarin/essentials/file-system-helpers?context=xamarin%2Fxamarin-forms&tabs=android), which exposes friendly cross-platform file system access. Just didn't have the time :)

# Playing

Just for fun, when you start XamariNES you'll get the classic "snow" screen, which for your youngsters out there is what you used to see when you turned on an old CRT television.

Press the POWER button on the NES console to start the emulator.

The controller on the bottom of the screen supports multi-touch pretty well, need to add support for A+B being pressed at the same time with one finger.

# Thanks

Huge shout-outs to folks at Microsoft on the Xamarin team, their conference sessions, videos, and blogs have been immensely  helpful as I continue learning the Xamarin Framework:

  - **James Montemagno** ([@JamesMontemagno](https://twitter.com/JamesMontemagno))
  - **David Ortinau** ([@davidortinau](https://twitter.com/davidortinau))
  - **Samantha Houts** ([@fwutterbys](https://twitter.com/fwutterbys)) - Unsung hero of my Xamarin education, immensely  helpful on GitHub!
  - **Miguel de Icaza** ([@migueldeicaza](https://twitter.com/migueldeicaza))