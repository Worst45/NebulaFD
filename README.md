# About this fork

This is a fork of [AITYunivers/NebulaFD](https://github.com/AITYunivers/NebulaFD), used by
[ClickteamFusionKit](https://github.com/Worst45/ClickteamFusionKit) to read Clickteam Fusion
projects (`.mfa`) and export their events as text. The branch `mfa-dump-enhanced` contains the
changes below, on top of upstream commit `a24d4d1`.

These changes are not submitted upstream. Nebula is © Yunivers and its contributors; the original
repository has no license. The original README follows.

# What this fork brings

<!-- Keep this section short: quick bullet points on the main features, for a human who only wants
to know what this fork adds to the original project. Never replace it with a block of text. New
changes go in "Detailed changes" below; only add or extend a bullet here for a new main feature. -->

- **Reads almost any `.mfa` project**: older Fusion 2.5 and Multimedia Fusion 2 projects, and
  projects with several frames.
- **Events shown as in the editor**: each condition, action and expression names the right object,
  object group, flag and global value.
- **The structure of the event list is kept**: groups (and whether they start disabled) and the
  author's comments.
- **Correct values**: decimal numbers, zones, directions, menu options, "Jump to frame".
- **Fewer crashes and freezes** on unusual projects: global events, installer settings, menus, icons,
  objects that can't be converted.
- **Easier to investigate unknown parameters**: their raw bytes are kept, for debugging.

# Detailed changes

They mostly concern reading `.mfa` files; a few also affect compiled games. Tested on 162 MFAs
(Fusion 2.5 builds 280-296, MMF2 builds 241-258): 45 could be read before, 162 now.

## Object references in MFA events
- In MFAs, the object of a condition, action or object parameter is an EventObject handle. The lookup
  is now centralized in `FrameEvents` (`GetMFAObject`, `GetMFAQualifierName`, `GetMFAObjectName`) and
  used by `ACEventBase`, `ParameterObject`, `ParameterExpression`, `ParameterPosition`,
  `ParameterCreate` and `ParameterShoot`. Before, qualifiers (object groups) all resolved to the first
  frame item, and the Create / Shoot parameters read the wrong objects.
- Object handles are specific to each frame: `Frame.MFAItems` holds each frame's objects and is used
  first, so multi-frame projects no longer resolve references to an object of another frame.
- Flag names (`MFAAltFlags`) are attached to their object instead of being lost.

## Groups and comments
- Group start / end conditions (`-1/-10`, `-1/-11`) and the Skip action (`-1/0`) are rendered.
- `Frame.GroupLookupTable` is filled when reading MFA events; `ParameterGroupPointer.ToString()` no
  longer throws on a missing group.
- Build 296: Activate / Deactivate Group shows the group's name instead of "Unknown Group".
- New `ParameterComment` (parameter code 37): font, colors, alignment and text of the event editor's
  comments, shown by the comment condition (`-1/-9`) and written back byte for byte (it was read as a
  2-byte short).

## Event rendering
- Zones defined by expressions (parameter code 72 followed by 4 expressions) are displayed, and zone
  conditions use the right comparison parameter (MFAs and compiled games).
- "Jump to frame": no more `InvalidCastException` in build 296 (the frame parameter is a 32-bit
  integer), and in MFAs the frame is found by its handle.
- Global values and strings used in an expression show their own name (the index was always 0).
- Menu options: an item in a sub-menu that is not the first sub-menu of its parent is found
  (`GetMenuItem` returned after searching the first sub-menu only); an unknown identifier shows
  `[menu id N]` instead of an empty name.
- Extension expressions: optional resolver `ParameterExpression.ExtensionExpressionInfo` (set by the
  host from a catalog of extensions) for the editor name of the expression, written in the editor's
  syntax `Name("object", parameters)`. Before, `Expression ID 83("Ini")"Game", "sound")` lacked the
  separator between the object and the parameters.
- Directions (direction parameter, Shoot parameter): the 32-bit mask (bit n = direction n of the
  editor's counter-clockwise wheel, 0 = right, 8 = up, 16 = left, 24 = down) is listed as
  `16 (left)` or `{0 (right), 16 (left)}`, instead of a row of dots.
- Alterable values / strings from index 26 show two letters (`BA`, `BB`…, as in expressions)
  instead of `[` (contributed by csando).
- Special expressions 67-71 numbered as in Fusion's runtime: `RuntimeName$`, `ReplaceString$(`,
  `Sign(`, `Lerp(`, `InvLerp(` (they were shifted by one, and 68 was unknown).
- Timer condition -3, the older "Timer equals" (it threw "Could not find ObjectType -4, Num -3").
- Sub-Application actions 88 "Show" and 89 "Hide" (they threw `IndexOutOfRangeException`;
  contributed by csando).
- Events of a behaviour: `FrameEvents.IsBehaviour` makes object names come from the behaviour's own
  object table, and `FrameEvents.LastIdentifier` tells which block a failed read was in
  (contributed by csando).
- Decimal constants: MFAs store a regular IEEE double, which `ByteReader.ReadDouble` read as the
  fixed-point format of some non-Windows builds (1.25 showed as 1072955392). Decimals are rendered
  in the invariant culture (`1.25`, not `1,25`) and keep a decimal point when round (`1.0`, not `1`).

## Older formats and file structure
- Event blocks: the end of the `Evts` / `STVE` block was computed 4 bytes too early, and `STVE` blocks
  end with 2 zero bytes that are not an event. The reader ended up out of step with the next
  identifiers: endless loop or read past the end of the file on projects with global events, lost
  comments on others. The reader now stops at the end of the block, and reaching the end of the data
  while looking for an identifier is an error instead of an endless loop.
- Frame: the field after the frame password is read as an auto-Unicode string (older builds store an
  empty string, which was read as a negative size).
- Non-Unicode MMF2 MFAs: no language ID after the build number, and no "is Unicode" integer for the
  extensions.
- The block before the binary files (installer settings) has a size, which is now honored.
- Menu bar: reading continues after the whole menu block.
- Application icons: handles missing from the image bank are skipped, and the same icon is not added
  twice.
- An object whose properties cannot be converted is still registered; object properties are read with
  `as ObjectCommon` instead of a cast.

- Included frame (MFA frame chunk 0x31, `Frame.FrameInclude`): handle of the frame included in
  this one; it was skipped as unknown.

## Tooling
- `Parameter.Raw` keeps each parameter's raw bytes (code included), to decode unknown parameters.
- `GetCommitHash()` no longer throws in a single-file publish (empty `Assembly.Location`).

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
