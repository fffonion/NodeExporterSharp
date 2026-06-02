# Upsd Collector Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a NUT upsd collector that emits the same `node_ups_*` metrics as the existing apcups collector.

**Architecture:** Keep the existing UPS metric shape and add a separate upsd protocol reader. The reader connects to `127.0.0.1:3493` by default, discovers UPS names with `LIST UPS`, fetches each UPS with `LIST VAR <name>`, and maps NUT variables into `ApcUpsInfo`.

**Tech Stack:** C#/.NET 8, raw TCP sockets for the upsd text protocol, xUnit for tests.

---

### Task 1: Add Tests

**Files:**
- Create: `NodeExporterSharp.Tests/NodeExporterSharp.Tests.csproj`
- Create: `NodeExporterSharp.Tests/UpsdStatusReaderTests.cs`
- Modify: `NodeExporterSharp.sln`

- [ ] **Step 1: Create the test project**

Create an xUnit test project targeting `net8.0-windows` and reference `NodeExporterSharp.csproj`.

- [ ] **Step 2: Write failing tests**

Tests cover UPS discovery parsing, NUT variable parsing, metrics mapping, and the apcups/upsd mutual exclusion behavior.

- [ ] **Step 3: Run tests to verify RED**

Run: `dotnet test`
Expected: fail because `UpsdStatusReader`, `UpsdConfiguration`, and config validation do not exist yet.

### Task 2: Implement upsd Reader and Metrics

**Files:**
- Modify: `Exporter.cs`

- [ ] **Step 1: Add protocol parsing helpers**

Add public/static helpers that parse `LIST UPS` and `LIST VAR` responses so they can be tested without a live upsd server.

- [ ] **Step 2: Add TCP upsd protocol reader**

Connect to configured host/port, read greeting, send `LIST UPS`, then `LIST VAR <ups>` for each discovered UPS.

- [ ] **Step 3: Reuse existing UPS metric output**

Extract the existing `node_ups_*` rendering into a shared method used by apcups and upsd.

- [ ] **Step 4: Run tests to verify GREEN**

Run: `dotnet test`
Expected: all tests pass.

### Task 3: Wire Configuration and Docs

**Files:**
- Modify: `Exporter.cs`
- Modify: `README.md`

- [ ] **Step 1: Add `upsd` collector flag**

Default `upsd` to disabled. When `--collector.upsd` is passed, enable it and reject configurations where both `apcups` and `upsd` are enabled.

- [ ] **Step 2: Add environment variables**

Read `NODE_EXPORTER_UPSD_HOST` and `NODE_EXPORTER_UPSD_PORT`; default to `127.0.0.1` and `3493`.

- [ ] **Step 3: Update README**

Document `apcups`, `upsd`, the conflict rule, and upsd environment variables.

- [ ] **Step 4: Final verification**

Run: `dotnet test` and `dotnet build`
Expected: both exit 0.
