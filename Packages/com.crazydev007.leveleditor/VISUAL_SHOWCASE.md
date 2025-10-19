# UI Toolkit Level Editor - Visual Showcase

## 🎨 Visual Design Overview

This document showcases the visual design and UI elements of the redesigned Tube Color Level Editor.

---

## Color Palette

### Primary Colors
```
┌─────────────────┬──────────┬─────────────────────────────┐
│ Color Name      │ Hex      │ Usage                       │
├─────────────────┼──────────┼─────────────────────────────┤
│ Primary Blue    │ #007acc  │ Main action buttons         │
│ Primary Hover   │ #005a9e  │ Button hover state          │
│ Success Green   │ #0e8a16  │ Positive actions            │
│ Danger Red      │ #d73a49  │ Destructive actions         │
│ Warning Orange  │ #e68900  │ Caution actions             │
└─────────────────┴──────────┴─────────────────────────────┘
```

### Background Colors
```
┌─────────────────┬──────────┬─────────────────────────────┐
│ Color Name      │ Hex      │ Usage                       │
├─────────────────┼──────────┼─────────────────────────────┤
│ Dark Base       │ #1e1e1e  │ Main background             │
│ Panel Dark      │ #252526  │ Panels, list view           │
│ Card Medium     │ #2d2d30  │ Cards, headers              │
│ Border/Accent   │ #3e3e42  │ Borders, separators         │
│ Input Dark      │ #3c3c3c  │ Text fields, inputs         │
│ Border Light    │ #555555  │ Input borders               │
└─────────────────┴──────────┴─────────────────────────────┘
```

### Text Colors
```
┌─────────────────┬──────────┬─────────────────────────────┐
│ Color Name      │ Hex      │ Usage                       │
├─────────────────┼──────────┼─────────────────────────────┤
│ Primary Text    │ #ffffff  │ Headers, titles             │
│ Secondary Text  │ #cccccc  │ Labels, body text           │
│ Tertiary Text   │ #a0a0a0  │ Hints, metadata             │
│ Disabled Text   │ #606060  │ Disabled elements           │
│ Link/Path       │ #87ceeb  │ File paths, links           │
└─────────────────┴──────────┴─────────────────────────────┘
```

---

## Layout Structure

### Window Layout (Text Representation)
```
╔════════════════════════════════════════════════════════════════╗
║  TUBE COLOR LEVEL EDITOR                                       ║
║  Design and manage game levels with ease                       ║
╠════════════════════════════════════════════════════════════════╣
║ [Load] [Save] [Browse...]    [New] [Duplicate] [Delete]       ║
╠════════════════════════════════════════════════════════════════╣
║ File Path: Packages/com.crazydev007/...                       ║
╠════════════════╦═══════════════════════════════════════════════╣
║ LEVELS    (42) ║ Level 1                                       ║
║ ┌────────────┐ ║                                               ║
║ │ Search...  │ ║ LEVEL PROPERTIES                              ║
║ └────────────┘ ║ Level Number    [      1]                     ║
║                ║ Empty Tubes     [      2]                     ║
║ Level 1 (7)    ║ Max Moves       [    100]                     ║
║ Level 2 (8)  ◄─║ Time Limit      [    180]                     ║
║ Level 3 (9)    ║ Available Swaps [      3]                     ║
║ Level 4 (7)    ║                                               ║
║ ...            ║ TUBES                         [+ Add Tube]    ║
║                ║ ┌──────┐ ┌──────┐ ┌──────┐                   ║
║                ║ │Tube 1│ │Tube 2│ │Tube 3│                   ║
║                ║ │ 3/4  │ │ 4/4  │ │ 2/4  │                   ║
║                ║ │ [1]  │ │ [2]  │ │ [3]  │                   ║
║                ║ │ [2]  │ │ [2]  │ │ [1]  │                   ║
║                ║ │ [3]  │ │ [3]  │ │Empty │                   ║
║                ║ │Empty │ │ [1]  │ │Empty │                   ║
║                ║ │+Clear│ │+Clear│ │+Clear│                   ║
║                ║ └──────┘ └──────┘ └──────┘                   ║
║                ║                                               ║
║                ║ LOCKED TUBES                                  ║
║                ║ Lock Index [0] [Lock] [Unlock]               ║
║                ║ Locked: 2, 5                                  ║
║                ║                                               ║
║                ║ VALIDATION                                    ║
║                ║ [Validate Level]                              ║
║                ║ ✓ Level validation passed                     ║
╠════════════════╩═══════════════════════════════════════════════╣
║ Status: Level 1 edited                                         ║
╚════════════════════════════════════════════════════════════════╝
```

---

## UI Components Showcase

### 1. Header Section
```
╔════════════════════════════════════════════════════╗
║ ░░ TUBE COLOR LEVEL EDITOR ░░░░░░░░░░░░░░░░░░░░░░║
║ ░░ Design and manage game levels with ease ░░░░░░║
╚════════════════════════════════════════════════════╝

Background: #1e1e1e
Title: 20px, Bold, White (#ffffff)
Subtitle: 12px, Italic, Light Gray (#a0a0a0)
```

### 2. Toolbar Buttons

#### Primary Button (Load/Save)
```
┌──────────────┐
│ Load Levels  │  ← Background: #007acc, Color: White
└──────────────┘
     ↓ Hover
┌──────────────┐
│ Load Levels  │  ← Background: #005a9e
└──────────────┘
```

#### Success Button (New Level)
```
┌──────────────┐
│  New Level   │  ← Background: #0e8a16, Color: White
└──────────────┘
     ↓ Hover
┌──────────────┐
│  New Level   │  ← Background: #0a6b11
└──────────────┘
```

#### Danger Button (Delete)
```
┌──────────────┐
│ Delete Level │  ← Background: #d73a49, Color: White
└──────────────┘
     ↓ Hover
┌──────────────┐
│ Delete Level │  ← Background: #b02a37
└──────────────┘
```

### 3. File Path Display
```
╔═══════════════════════════════════════════════════╗
║ File Path: Packages/com.crazydev007/...          ║
╚═══════════════════════════════════════════════════╝

Background: #252526
Label: 11px, Light Gray (#cccccc)
Path: 11px, Sky Blue (#87ceeb)
```

### 4. Level List Panel
```
╔════════════════╗
║ LEVELS    (42) ║
║ ┌────────────┐ ║
║ │ Search...  │ ║
║ └────────────┘ ║
║                ║
║ Level 1 (7)    ║  ← Normal
║ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ║  ← Hover (#2d2d30)
║ ████ Level 3 ██ ║  ← Selected (#094771)
║ Level 4 (7)    ║
║ ...            ║
╚════════════════╝

Width: 280px
Background: #252526
Item Height: 32px
Item Padding: 10px 15px
Border: 1px #3e3e42
```

### 5. Property Fields
```
╔═══════════════════════════════════════════════════╗
║ Level Number                            [    1  ] ║
║ Empty Tubes                             [    2  ] ║
║ Max Moves                               [  100  ] ║
╚═══════════════════════════════════════════════════╝

Label: 12px, Light Gray (#cccccc), Min-width: 150px
Field: Background #3c3c3c, Border #555555
Focus: Border changes to #007acc
```

### 6. Tube Card
```
┌────────────────────┐
│ ▓ Tube 1      3/4  │  ← Header: #2d2d30
├────────────────────┤
│ #1    [  1  ]  ✕   │  ← Color Slot 1
│ #2    [  2  ]  ✕   │  ← Color Slot 2
│ #3    [  3  ]  ✕   │  ← Color Slot 3
│ #4    Empty        │  ← Color Slot 4 (empty)
├────────────────────┤
│ [+ Add] [Clear]    │  ← Actions
│       [Remove]     │
└────────────────────┘

Width: 150px
Background: #2d2d30
Border: 1px #3e3e42
Border-radius: 6px
Hover: Border #007acc, Background #323236
```

### 7. Validation Results

#### Success State
```
╔═══════════════════════════════════════════════════╗
║ ✓ Level validation passed                         ║
╚═══════════════════════════════════════════════════╝

Background: #0e441e (Dark Green)
Text: #4ade80 (Light Green)
Border: 1px #0e8a16
```

#### Error State
```
╔═══════════════════════════════════════════════════╗
║ ✗ Level has no tubes                              ║
╚═══════════════════════════════════════════════════╝

Background: #4a0e0e (Dark Red)
Text: #f87171 (Light Red)
Border: 1px #d73a49
```

### 8. Status Bar
```
╔═══════════════════════════════════════════════════╗
║ ▓▓▓ Ready ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓║
╚═══════════════════════════════════════════════════╝

Background: #007acc (Blue)
Text: 11px, White (#ffffff)
Height: 24px
```

---

## State Visualizations

### Button States
```
           ┌─────────┐
           │ Default │
           └─────────┘
                │
        ┌───────┴───────┐
        ▼               ▼
   ┌─────────┐     ┌─────────┐
   │  Hover  │     │ Pressed │
   └─────────┘     └─────────┘
        │               │
        └───────┬───────┘
                ▼
           ┌─────────┐
           │ Default │
           └─────────┘
```

### ListView Item States
```
Normal       → Background: #252526
             → Text: #ffffff

Hover        → Background: #2d2d30
             → Text: #ffffff

Selected     → Background: #094771
             → Text: #ffffff

Selected     → Background: #0e5a8a
+ Hover      → Text: #ffffff
```

### Field States
```
Normal       → Background: #3c3c3c
             → Border: #555555

Focus        → Background: #3c3c3c
             → Border: #007acc
             → Glow effect

Disabled     → Background: #2d2d30
             → Border: #3e3e42
             → Text: #606060
```

---

## Typography Scale

```
┌──────────────┬──────────┬──────────┬─────────────────┐
│ Element      │ Size     │ Weight   │ Color           │
├──────────────┼──────────┼──────────┼─────────────────┤
│ Main Title   │ 20px     │ Bold     │ White (#ffffff) │
│ Level Title  │ 18px     │ Bold     │ White (#ffffff) │
│ Section Title│ 15px     │ Bold     │ White (#ffffff) │
│ Panel Title  │ 14px     │ Bold     │ White (#ffffff) │
│ Tube Title   │ 13px     │ Bold     │ White (#ffffff) │
│ Body Text    │ 12px     │ Regular  │ Gray (#cccccc)  │
│ Button Text  │ 12px     │ Regular  │ White           │
│ Small Text   │ 11px     │ Regular  │ Gray (#a0a0a0)  │
│ Micro Text   │ 10px     │ Regular  │ Gray (#a0a0a0)  │
└──────────────┴──────────┴──────────┴─────────────────┘
```

---

## Spacing System

### Padding
```
Extra Small (XS)    3px     Tight spacing
Small (S)           6px     Input padding
Medium (M)          10px    Default padding
Large (L)           15px    Panel padding
Extra Large (XL)    20px    Page padding
```

### Margins
```
None                0px     No margin
Small               5px     Between buttons
Medium              10px    Between sections
Large               20px    Between major sections
```

### Border Radius
```
Small               3px     Inputs, small buttons
Medium              4px     Standard buttons
Large               6px     Cards, panels
Extra Large         10px    Badges, pills
```

---

## Icon Usage (Text-based)

```
✓   Success / Validation passed
✗   Error / Validation failed
+   Add / Create new
✕   Remove / Delete
⚠   Warning
ℹ   Information
⚙   Settings
🔒  Locked
🔓  Unlocked
⬆   Move up
⬇   Move down
```

---

## Responsive Behavior

### Window Width: > 900px (Optimal)
```
┌──────────────────────────────────────────────────────┐
│ [280px List]    [Remaining space for Editor]         │
└──────────────────────────────────────────────────────┘
```

### Window Width: 600-900px
```
┌──────────────────────────────────────────────────────┐
│ [200px List]    [Remaining space for Editor]         │
└──────────────────────────────────────────────────────┘
```

### Minimum Width: 600px
```
Editor will maintain functionality but may have
horizontal scrolling for some elements.
```

---

## Animation Transitions (If Added in Future)

```
Button Hover        200ms ease-out
Color Changes       300ms ease-in-out
Panel Slide         250ms ease-in-out
Fade In/Out         200ms linear
```

---

## Accessibility Features

### Color Contrast
```
✓ All text meets WCAG AA standards
✓ White on Blue: 4.5:1 contrast
✓ White on Dark: 15:1 contrast
✓ Light Gray on Dark: 7:1 contrast
```

### Focus Indicators
```
All interactive elements have visible focus states:
- Border color changes to #007acc
- Border width increases
- Optional glow effect
```

---

## Best Practices Applied

1. ✅ **Consistent Spacing**: 5px increments (5, 10, 15, 20)
2. ✅ **Color Hierarchy**: Primary > Secondary > Tertiary
3. ✅ **Visual Weight**: Size + Color + Font weight
4. ✅ **Grouping**: Related items grouped with borders/backgrounds
5. ✅ **Feedback**: Every action has visual feedback
6. ✅ **State Indication**: Clear visual states (normal, hover, active, disabled)
7. ✅ **Scan-ability**: Important info stands out
8. ✅ **White Space**: Proper breathing room between elements

---

## Design Inspiration

This design draws inspiration from:
- **Unity Editor**: Dark theme, panel layouts
- **Visual Studio Code**: Color scheme, status bar
- **GitHub**: Button styles, state colors
- **Modern Web UI**: Card-based layouts, flexbox

---

## CSS Classes Quick Reference

```css
/* Containers */
.root-container
.header-container
.toolbar-container
.main-content-container
.left-panel
.right-panel

/* Buttons */
.toolbar-button
.primary-button
.secondary-button
.success-button
.danger-button

/* Lists */
.level-list
.level-list-scroll

/* Editor */
.section-container
.properties-grid
.property-row
.tubes-container

/* Tubes */
.tube-card
.tube-header
.tube-title
.tube-colors
.color-slot
.tube-actions

/* States */
.validation-success
.validation-error
```

---

This visual design provides:
- ✨ **Professional Appearance**: Matches Unity Editor aesthetics
- 🎨 **Consistent Design System**: Reusable colors and spacing
- 🔍 **Clear Hierarchy**: Visual weight guides the eye
- ♿ **Accessible**: Good contrast and focus indicators
- 🎯 **User-Friendly**: Clear feedback and intuitive layout

**The result is a modern, polished editor that's a pleasure to use!** 🚀
