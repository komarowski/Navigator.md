/**
 * Represents a tree node.
 * @typedef {Object} NodeType
 * @property {string} Path Node path (relative HTML path).
 * @property {string} Name Node name.
 * @property {string} Type Node type: "File"|"Folder".
 * @property {Array<NodeType> | null} Children List of children nodes.
 */

/**
 * Checks if element exist.
 * @param {HTMLElement} element Html element.
 * @param {string} name Element name.
 * @returns True if element exist.
 */
const isElementExist = (element, name) => {
    if (!element) {
        console.error(`'${name}' element not found!`);
        return false;
    }
    return true;
};

/**
 * Calculates the relative path between two node paths.
 * @param {string} nodePathFrom Source node path.
 * @param {string} nodePathTo Target node path.
 * @returns Path from the first node to the second.
 */
const getRelativePath = (nodePathFrom, nodePathTo) => {
    const pathPartsFrom = nodePathFrom.split("/").slice(0, -1);
    const pathPartsTo = nodePathTo.split("/");

    // Find the first index where the paths differ
    let commonBaseIndex = 0;
    while (
        commonBaseIndex < pathPartsFrom.length &&
        commonBaseIndex < pathPartsTo.length &&
        pathPartsFrom[commonBaseIndex] === pathPartsTo[commonBaseIndex]
    ) {
        commonBaseIndex++;
    }

    // Navigate up from the `from` path to the common base directory
    const stepsUp = "../".repeat(pathPartsFrom.length - commonBaseIndex);

    // Navigate down to the `to` path from the common base
    const stepsDown = pathPartsTo.slice(commonBaseIndex).join("/");

    return `${stepsUp}${stepsDown}`;
};

/**
 * Generates HTML from JSON data with a tree structure.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 * @param {string} currentNodePath Current node path.
 * @returns {string} HTML tree structure.
 */
const generateHtmlTree = (nodeList, currentNodePath) => {
    if (!nodeList || nodeList.length === 0) {
        return "";
    }

    let result = "";
    for (const node of nodeList) {
        if (
            node.Type === "Folder" &&
            node.Children &&
            node.Children.length !== 0
        ) {
            const folderLink = currentNodePath
                ? getRelativePath(currentNodePath, node.Path)
                : node.Path;

            result += `<div class="tree-folder" id="${node.Path}">`;
            result += `<div class="tree-folder-header"><span class="tree-folder-toggle"></span><a href="${folderLink}" class="tree-folder-link">${node.Name}</a></div>`;
            result += `<div class="tree-group" hidden>`;
            result += generateHtmlTree(node.Children, currentNodePath);
            result += "</div></div>";
        } else if (node.Type === "File") {
            const nodeLink = currentNodePath
                ? getRelativePath(currentNodePath, node.Path)
                : node.Path;

            result += `<a id="${node.Path}" href="${nodeLink}" class="tree-item">${node.Name}</a>`;
        }
    }
    return result;
};

/**
 * Expands a folder node.
 * @param {HTMLElement} folderNode Folder element.
 */
const expandFolder = (folderNode) => {
    const toggle = folderNode.querySelector(".tree-folder-toggle");
    const group = folderNode.querySelector(".tree-group");
    if (toggle && group) {
        toggle.classList.add("expanded");
        group.hidden = false;
    }
};

/**
 * Opens all subfolders in node's path and highlights the current node.
 * @param {HTMLElement} treeView Tree view element.
 * @param {string} currentNodePath Current node path.
 */
const openFolderNodes = (treeView, currentNodePath) => {
    if (currentNodePath) {
        const currentNode = document.getElementById(currentNodePath);
        if (currentNode) {
            // Check if it's a folder or file
            if (currentNode.classList.contains("tree-folder")) {
                expandFolder(currentNode);
                const link = currentNode.querySelector(".tree-folder-link");
                if (link) link.classList.add("tree-item-current");
            } else {
                currentNode.classList.add("tree-item-current");
            }

            // Open parent folders
            const indexes = [
                ...currentNodePath.matchAll(new RegExp("/", "gi")),
            ].map((a) => a.index);
            indexes.forEach((index) => {
                const folderPath = currentNodePath.slice(0, index + 1) + "index.html";
                const folderNode = document.getElementById(folderPath);
                if (folderNode) expandFolder(folderNode);
            });

            return;
        }
    }

    // Expand all folders if no current path (index page)
    treeView.querySelectorAll(".tree-folder").forEach(expandFolder);
};

/**
 * Sets up folder toggle click handlers.
 * @param {HTMLElement} treeView Tree view element.
 */
const setUpFolderToggles = (treeView) => {
    treeView.addEventListener("click", (e) => {
        const toggle = e.target.closest(".tree-folder-toggle");
        if (!toggle) return;

        const folder = toggle.closest(".tree-folder");
        const group = folder.querySelector(".tree-group");
        if (group) {
            const isExpanded = !group.hidden;
            group.hidden = isExpanded;
            toggle.classList.toggle("expanded", !isExpanded);
        }
    });
};

/**
 * Adds HTML headings (h1, h2) to the content table on the right for quick navigation.
 */
const setUpContentTable = () => {
    const blog = document.getElementById("markdown");
    const tableOfContents = document.getElementById("content-table");

    if (
        !isElementExist(blog, "markdown") ||
        !isElementExist(tableOfContents, "content-table")
    ) {
        return;
    }

    const headers = blog.querySelectorAll("h1, h2");
    for (let i = 0; i < headers.length; i++) {
        const li = document.createElement("li");
        const a = document.createElement("a");
        a.innerHTML = headers[i].textContent;
        a.href = `#${headers[i].id}`;
        li.appendChild(a);
        tableOfContents.appendChild(li);
    }
};

/**
 * Detects the active tab based on the current node path.
 * @param {string} nodePath Current node path.
 * @returns {'wiki'|'tasks'|'qa'} Tab name.
 */
const detectTab = (nodePath) => {
    if (nodePath.startsWith("_tasks/")) return "tasks";
    if (nodePath.startsWith("_qa/")) return "qa";
    return "wiki";
};

/**
 * Generates HTML for task list in the sidebar.
 * @param {Array} taskList List of task objects from data.js.
 * @param {string} currentNodePath Current node path.
 * @returns {string} HTML string.
 */
const generateTaskTree = (taskList, currentNodePath) => {
    if (!taskList || taskList.length === 0) return "<p>No tasks</p>";

    return taskList.map(t => {
        const statusClass = t.status ? ` task-${t.status.toLowerCase()}` : "";
        const link = currentNodePath
            ? getRelativePath(currentNodePath, "_tasks/" + t.internalLink)
            : "_tasks/" + t.internalLink;
        const isCurrent = currentNodePath === "_tasks/" + t.internalLink;
        const currentClass = isCurrent ? " tree-item-current" : "";

        return `<a href="${link}" class="tree-item${currentClass}${statusClass}">${t.name}</a>`;
    }).join("");
};

/**
 * Generates HTML for Q&A list in the sidebar.
 * @param {Array} qaList List of Q&A objects from data.js.
 * @param {string} currentNodePath Current node path.
 * @returns {string} HTML string.
 */
const generateQaTree = (qaList, currentNodePath) => {
    if (!qaList || qaList.length === 0) return "<p>No Q&amp;A items</p>";

    return qaList.map(q => {
        const link = currentNodePath
            ? getRelativePath(currentNodePath, "_qa/" + q.path)
            : "_qa/" + q.path;
        const isCurrent = currentNodePath === "_qa/" + q.path;
        const currentClass = isCurrent ? " tree-item-current" : "";

        return `<a href="${link}" class="tree-item${currentClass}">${q.question}</a>`;
    }).join("");
};

/**
 * Sets up tab switching for sidebar navigation.
 * @param {NodeType} rootNode Root node of the wiki tree.
 * @param {string} currentNodePath Current node path.
 */
const setUpTabs = (rootNode, currentNodePath) => {
    const sidebar = document.getElementById("sidebar");
    const navTree = document.getElementById("nav-tree");
    const tabs = document.querySelectorAll(".header-tab");
    const isMobile = () => window.innerWidth <= 768;

    if (!isElementExist(sidebar, "sidebar") || !isElementExist(navTree, "nav-tree")) {
        return;
    }

    let activeTab = detectTab(currentNodePath);

    const renderTab = (tabName) => {
        navTree.innerHTML = "";

        switch (tabName) {
            case "wiki":
                if (rootNode && rootNode.Children) {
                    navTree.innerHTML = generateHtmlTree(rootNode.Children, currentNodePath);
                    setUpFolderToggles(navTree);
                    openFolderNodes(navTree, currentNodePath);
                }
                break;
            case "tasks":
                navTree.innerHTML = generateTaskTree(typeof tasks !== "undefined" ? tasks : [], currentNodePath);
                break;
            case "qa":
                navTree.innerHTML = generateQaTree(typeof qa !== "undefined" ? qa : [], currentNodePath);
                break;
        }

        // Update active button styles
        tabs.forEach(t => {
            t.classList.toggle("header-tab--active", t.dataset.tab === tabName);
        });

        activeTab = tabName;
    };

    // Tab click handlers
    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            const clickedTab = tab.dataset.tab;

            if (isMobile()) {
                if (clickedTab === activeTab && sidebar.classList.contains("sidebar--open")) {
                    sidebar.classList.remove("sidebar--open");
                    return;
                }
                sidebar.classList.add("sidebar--open");
            }

            if (clickedTab !== activeTab) {
                renderTab(clickedTab);
            }
        });
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener("click", (e) => {
        if (isMobile() && sidebar.classList.contains("sidebar--open")) {
            if (!sidebar.contains(e.target) && !e.target.closest(".header-tab")) {
                sidebar.classList.remove("sidebar--open");
            }
        }
    });

    // Render initial tab
    renderTab(activeTab);
};

/**
 * Entry point function.
 * @param {NodeType} rootNode Root node of the tree.
 */
const main = (rootNode) => {
    const navTree = document.getElementById("nav-tree");
    const currentNodePath = navTree ? navTree.dataset.node : "";

    setUpTabs(rootNode, currentNodePath);
    setUpContentTable();
};

// "rootNode", "tasks", "qa", "qaTags" are loaded from "data.js"
main(rootNode);
