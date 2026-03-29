---
name: Add more UTs and integration tests
status: Open
---

# Add more UTs and integration tests

## Goal

Improve confidence in refactoring and new feature work.

## Suggested areas

### Unit tests

- `PathManager`
- `MarkdownManager`
- task / Q&A metadata fallback logic
- path transformation helpers
- frontend helper logic where possible

### Integration tests

- `TreeBuilder`
- `SiteGenerator`
- full build output verification
- incremental rebuild scenarios
- generated navigation data

## Notes

Prioritize tests around the parts that change most often.
