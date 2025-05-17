# 🧼 RESET.md — Project Baseline (Current State)

This document captures the known-good state of terminal rendering, input handling, CLI infrastructure, and narrative systems as of the latest stable reset. Use it as the source of truth for roles, system boundaries, and what’s working as of this baseline.

---

## 🖥 Terminal Rendering and Input Handling

### ✔ Core Behavior

- Terminal rendering is handled via Unity Canvas and TextMeshPro, with native Unity glow applied to the text for retro/CRT effect.
- Input is captured and routed through the `TerminalManager` script.
- Output is buffered and displayed, maintaining scrollback and cursor state.

---

## 🔐 Login Prompt Flow

- SSH command triggers a multi-step login prompt (username/password), managed by `TerminalManager`.
- There is **no initial system login**—login logic is only implemented for SSH at this stage.

---

## 📋 Menu/UI System

_(Not implemented)_

---

## 🚀 Boot/Init Sequence

_(No boot/init sequence yet)_

---

## 💻 Visual/Audio FX Layer

- Native Unity glow on terminal text only.
- No additional FX or audio layers implemented yet.

---

## 💬 CLI Command System

_(Section to be expanded)_

---

## 🚫 Deprecated / Removed

_(Section to be expanded)_

---

## 🧊 Summary

- Baseline: Core terminal rendering and input handling are in place, with SSH login flow functional. No initial login, boot sequence, advanced FX, or UI/menus implemented.

---

## 🗂 Supplemental Architecture Files

_(To be added as project expands)_
