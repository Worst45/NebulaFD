> ## About this fork
>
> This is a fork of [AITYunivers/NebulaFD](https://github.com/AITYunivers/NebulaFD), used by
> [ClickteamFusionKit](https://github.com/Worst45/ClickteamFusionKit) to read Clickteam Fusion
> projects (`.mfa`) and export their events as text. The branch `mfa-dump-enhanced` contains the
> changes below, on top of upstream commit `a24d4d1`. They mostly concern reading `.mfa` files; a few
> also affect compiled games. Tested on 162 MFAs (Fusion 2.5 builds 280-296, MMF2 builds 241-258):
> 45 could be read before, 162 now.
>
> **Object references in MFA events**
> - In MFAs, the object of a condition, action or object parameter is an EventObject handle. The lookup
>   is now centralized in `FrameEvents` (`GetMFAObject`, `GetMFAQualifierName`, `GetMFAObjectName`) and
>   used by `ACEventBase`, `ParameterObject`, `ParameterExpression`, `ParameterPosition`,
>   `ParameterCreate` and `ParameterShoot`. Before, qualifiers (object groups) all resolved to the first
>   frame item, and the Create / Shoot parameters read the wrong objects.
> - Object handles are specific to each frame: `Frame.MFAItems` holds each frame's objects and is used
>   first, so multi-frame projects no longer resolve references to an object of another frame.
> - Flag names (`MFAAltFlags`) are attached to their object instead of being lost.
>
> **Groups and comments**
> - Group start / end conditions (`-1/-10`, `-1/-11`) and the Skip action (`-1/0`) are rendered.
> - `Frame.GroupLookupTable` is filled when reading MFA events; `ParameterGroupPointer.ToString()` no
>   longer throws on a missing group.
> - Build 296: Activate / Deactivate Group shows the group's name instead of "Unknown Group".
> - New `ParameterComment` (parameter code 37): font, colors, alignment and text of the event editor's
>   comments, shown by the comment condition (`-1/-9`) and written back byte for byte (it was read as a
>   2-byte short).
>
> **Event rendering**
> - Zones defined by expressions (parameter code 72 followed by 4 expressions) are displayed, and zone
>   conditions use the right comparison parameter (MFAs and compiled games).
> - "Jump to frame": no more `InvalidCastException` in build 296 (the frame parameter is a 32-bit
>   integer), and in MFAs the frame is found by its handle.
> - Global values and strings used in an expression show their own name (the index was always 0).
> - Decimal constants: MFAs store a regular IEEE double, which `ByteReader.ReadDouble` read as the
>   fixed-point format of some non-Windows builds (1.25 showed as 1072955392). Decimals are rendered
>   in the invariant culture (`1.25`, not `1,25`) and keep a decimal point when round (`1.0`, not `1`).
>
> **Older formats and file structure**
> - Frame: the field after the frame password is read as an auto-Unicode string (older builds store an
>   empty string, which was read as a negative size).
> - Non-Unicode MMF2 MFAs: no language ID after the build number, and no "is Unicode" integer for the
>   extensions.
> - The block before the binary files (installer settings) has a size, which is now honored.
> - Menu bar: reading continues after the whole menu block.
> - Application icons: handles missing from the image bank are skipped, and the same icon is not added
>   twice.
> - An object whose properties cannot be converted is still registered; object properties are read with
>   `as ObjectCommon` instead of a cast.
>
> **Tooling**
> - `Parameter.Raw` keeps each parameter's raw bytes (code included), to decode unknown parameters.
> - `GetCommitHash()` no longer throws in a single-file publish (empty `Assembly.Location`).
>
> These changes are not submitted upstream. Nebula is © Yunivers and its contributors; the original
> repository has no license. The original README follows.

---

![Nebula Logo](https://raw.githubusercontent.com/AITYunivers/NebulaFD/master/.resources/NebulaLogo.png)
# 
**README is a Work In Progress.**

[Discord](https://discord.gg/aY2WFGPBBB)

Just another Clickteam Fusion 2.5 decompiler made by [Yunivers](https://github.com/AITYunivers).
|Table of Contents| Description |
|--|--|
| [Credits](https://github.com/AITYunivers/Nebula/tree/master#Credits)|A list of Nebula credits.|

## Credits
|User| Credit for..|
|--|--|
| [Yunivers](https://github.com/AITYunivers)| Creator of Nebula. |
| [Kostya](https://github.com/1987kostya1)| Developer of the Image Bank.|
