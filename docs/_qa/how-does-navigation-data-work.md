---
question: How does navigation data work?
popularity: 1
---

# How does navigation data work?

The generator serializes wiki, task, and Q&A metadata into `data.js`.

`core.js` reads `rootNode`, `tasks`, and `qa` from that script and renders the sidebar dynamically in the browser.
