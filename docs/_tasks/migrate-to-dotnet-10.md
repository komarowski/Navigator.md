---
name: Migrate to .NET 10
status: Open
---

# Migrate to .NET 10

## Goal

Update the solution from the current target framework to .NET 10.

## Areas to verify

- console project target framework
- core project target framework
- test project target framework
- package compatibility
- CI / local build behavior
- watcher and file system behavior after migration

## Done criteria

- solution builds successfully
- tests pass
- generated site still works the same
